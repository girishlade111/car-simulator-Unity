using UnityEngine;
using UnityEngine.UI;

namespace CarSimulator.SaveSystem
{
    public class SaveLoadUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject m_savePanel;
        [SerializeField] private Transform m_saveSlotsContainer;
        [SerializeField] private GameObject m_slotButtonPrefab;

        [Header("Info Display")]
        [SerializeField] private Text m_saveNameText;
        [SerializeField] private Text m_playTimeText;
        [SerializeField] private Text m_lastPlayedText;
        [SerializeField] private Text m_currencyText;
        [SerializeField] private Text m_missionsText;

        [Header("Buttons")]
        [SerializeField] private Button m_saveButton;
        [SerializeField] private Button m_loadButton;
        [SerializeField] private Button m_quickSaveButton;
        [SerializeField] private Button m_quickLoadButton;
        [SerializeField] private Button m_deleteButton;
        [SerializeField] private Button m_closeButton;

        [Header("Settings")]
        [SerializeField] private int m_selectedSlot = -1;

        private SaveManager m_saveManager;

        private void Start()
        {
            m_saveManager = SaveManager.Instance;
            SetupButtons();
            RefreshSaveSlots();
        }

        private void SetupButtons()
        {
            if (m_saveButton != null)
                m_saveButton.onClick.AddListener(OnSaveClicked);

            if (m_loadButton != null)
                m_loadButton.onClick.AddListener(OnLoadClicked);

            if (m_quickSaveButton != null)
                m_quickSaveButton.onClick.AddListener(OnQuickSaveClicked);

            if (m_quickLoadButton != null)
                m_quickLoadButton.onClick.AddListener(OnQuickLoadClicked);

            if (m_deleteButton != null)
                m_deleteButton.onClick.AddListener(OnDeleteClicked);

            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(ClosePanel);
        }

        public void OpenPanel()
        {
            if (m_savePanel != null)
            {
                m_savePanel.SetActive(true);
                RefreshSaveSlots();
            }
        }

        public void ClosePanel()
        {
            if (m_savePanel != null)
            {
                m_savePanel.SetActive(false);
            }
        }

        public void RefreshSaveSlots()
        {
            if (m_saveSlotsContainer == null || m_slotButtonPrefab == null) return;

            foreach (Transform child in m_saveSlotsContainer)
            {
                Destroy(child.gameObject);
            }

            var slots = m_saveManager.GetAllSaveSlots();

            for (int i = 0; i < slots.Length; i++)
            {
                CreateSlotButton(i, slots[i]);
            }
        }

        private void CreateSlotButton(int index, SaveSlotInfo slotInfo)
        {
            GameObject button = Instantiate(m_slotButtonPrefab, m_saveSlotsContainer);
            button.name = $"SaveSlot_{index}";

            var buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = slotInfo.exists ? $"{slotInfo.saveName}\n{slotInfo.FormattedPlayTime()}" : $"Empty Slot {index + 1}";
            }

            var buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => SelectSlot(index));
            }
        }

        private void SelectSlot(int slotIndex)
        {
            m_selectedSlot = slotIndex;
            var slotInfo = m_saveManager.GetSaveSlotInfo(slotIndex);

            if (m_saveNameText != null)
                m_saveNameText.text = slotInfo.exists ? slotInfo.saveName : $"Slot {slotIndex + 1}";

            if (m_playTimeText != null)
                m_playTimeText.text = slotInfo.exists ? $"Play Time: {slotInfo.FormattedPlayTime()}" : "";

            if (m_lastPlayedText != null)
                m_lastPlayedText.text = slotInfo.exists ? $"Last Played: {slotInfo.lastPlayedAt:g}" : "";

            if (m_currencyText != null)
                m_currencyText.text = slotInfo.exists ? $"Currency: ${slotInfo.currency}" : "";

            if (m_missionsText != null)
                m_missionsText.text = slotInfo.exists ? $"Missions: {slotInfo.missionsCompleted}" : "";
        }

        private void OnSaveClicked()
        {
            if (m_selectedSlot < 0)
            {
                Debug.LogWarning("[SaveLoadUI] No slot selected");
                return;
            }

            var saveData = m_saveManager.CurrentSave;
            if (saveData == null)
            {
                saveData = new GameSaveData();
            }

            saveData.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            saveData.saveName = $"Save {m_selectedSlot + 1}";

            m_saveManager.SaveGame(saveData, m_selectedSlot);
            RefreshSaveSlots();

            var notification = UI.NotificationSystem.Instance;
            notification?.ShowSuccess("Game Saved!");
        }

        private void OnLoadClicked()
        {
            if (m_selectedSlot < 0)
            {
                Debug.LogWarning("[SaveLoadUI] No slot selected");
                return;
            }

            var saveData = m_saveManager.LoadGame(m_selectedSlot);
            if (saveData != null)
            {
                LoadScene(saveData.currentScene);
            }
        }

        private void OnQuickSaveClicked()
        {
            var saveData = m_saveManager.QuickSave();
            if (saveData != null)
            {
                var notification = UI.NotificationSystem.Instance;
                notification?.ShowSuccess("Quick Saved!");
            }
        }

        private void OnQuickLoadClicked()
        {
            var saveData = m_saveManager.QuickLoad();
            if (saveData != null)
            {
                LoadScene(saveData.currentScene);
            }
            else
            {
                var notification = UI.NotificationSystem.Instance;
                notification?.ShowError("No Quick Save Found!");
            }
        }

        private void OnDeleteClicked()
        {
            if (m_selectedSlot < 0) return;

            m_saveManager.DeleteSave(m_selectedSlot);
            RefreshSaveSlots();
            m_selectedSlot = -1;
        }

        private void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = "OpenWorld_TestDistrict";
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

    public class AutoSaveManager : MonoBehaviour
    {
        public static AutoSaveManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_intervalSeconds = 300f;
        [SerializeField] private bool m_showNotification = true;

        [Header("State")]
        [SerializeField] private float m_timer;
        [SerializeField] private int m_saveCount;

        private SaveManager m_saveManager;
        private UI.NotificationSystem m_notification;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            m_saveManager = SaveManager.Instance;
            m_notification = UI.NotificationSystem.Instance;
        }

        private void Update()
        {
            if (!m_enabled || m_saveManager == null) return;

            m_timer += Time.deltaTime;

            if (m_timer >= m_intervalSeconds)
            {
                m_timer = 0;
                PerformAutoSave();
            }
        }

        private void PerformAutoSave()
        {
            if (m_saveManager.CurrentSave != null)
            {
                m_saveManager.CurrentSave.playTime += m_intervalSeconds;
                m_saveManager.QuickSave();
                m_saveCount++;

                if (m_showNotification && m_notification != null)
                {
                    m_notification.ShowInfo($"Auto Saved ({m_saveCount})");
                }

                Debug.Log($"[AutoSaveManager] Auto saved (count: {m_saveCount})");
            }
        }

        public void TriggerImmediateSave()
        {
            PerformAutoSave();
        }
    }
}
