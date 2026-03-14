using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SaveLoadUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject m_savePanel;
    [SerializeField] private GameObject m_loadPanel;
    [SerializeField] private Transform m_saveSlotContainer;
    [SerializeField] private GameObject m_saveSlotPrefab;

    [Header("Save Info Display")]
    [SerializeField] private Text m_saveNameText;
    [SerializeField] private Text m_playTimeText;
    [SerializeField] private Text m_lastPlayedText;
    [SerializeField] private Text m_sceneText;

    [Header("Input")]
    [SerializeField] private InputField m_saveNameInput;
    [SerializeField] private int m_selectedSlot = -1;

    private SaveSlotInfo[] m_saveSlots;

    public void ShowSavePanel()
    {
        if (m_savePanel != null)
        {
            m_savePanel.SetActive(true);
            m_loadPanel?.SetActive(false);
            RefreshSaveSlots();
        }
    }

    public void ShowLoadPanel()
    {
        if (m_loadPanel != null)
        {
            m_loadPanel.SetActive(true);
            m_savePanel?.SetActive(false);
            RefreshSaveSlots();
        }
    }

    public void ClosePanel()
    {
        if (m_savePanel != null) m_savePanel.SetActive(false);
        if (m_loadPanel != null) m_loadPanel.SetActive(false);
    }

    private void RefreshSaveSlots()
    {
        if (SaveManager.Instance == null || m_saveSlotContainer == null) return;

        foreach (Transform child in m_saveSlotContainer)
        {
            Destroy(child.gameObject);
        }

        m_saveSlots = SaveManager.Instance.GetAllSaveSlots();

        for (int i = 0; i < m_saveSlots.Length; i++)
        {
            GameObject slotObj = Instantiate(m_saveSlotPrefab, m_saveSlotContainer);
            SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();

            if (slotUI != null)
            {
                slotUI.Setup(m_saveSlots[i], i, this);
            }
        }
    }

    public void SelectSlot(int index)
    {
        m_selectedSlot = index;

        if (m_saveSlots != null && index >= 0 && index < m_saveSlots.Length)
        {
            var slot = m_saveSlots[index];

            if (m_saveNameText != null)
                m_saveNameText.text = slot.exists ? slot.saveName : $"Slot {index + 1}";

            if (m_playTimeText != null)
                m_playTimeText.text = slot.exists ? FormatPlayTime(slot.playTime) : "-";

            if (m_lastPlayedText != null)
                m_lastPlayedText.text = slot.exists ? slot.lastPlayed.ToString("yyyy-MM-dd HH:mm") : "-";

            if (m_sceneText != null)
                m_sceneText.text = slot.exists ? slot.scene : "-";
        }
    }

    public void SaveToSelectedSlot()
    {
        if (m_selectedSlot < 0)
        {
            Debug.LogWarning("[SaveLoadUI] No slot selected");
            return;
        }

        string saveName = m_saveNameInput != null ? m_saveNameInput.text : $"Save {m_selectedSlot + 1}";
        if (string.IsNullOrEmpty(saveName))
        {
            saveName = $"Save {m_selectedSlot + 1}";
        }

        GameSaveData saveData = new GameSaveData
        {
            saveName = saveName,
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        };

        if (ProfileManager.Instance != null && ProfileManager.Instance.ActiveProfile != null)
        {
            saveData.player = ProfileManager.Instance.ActiveProfile.PlayerData;
        }

        bool success = SaveManager.Instance.SaveGame(saveData, m_selectedSlot);

        if (success)
        {
            Debug.Log($"[SaveLoadUI] Game saved to slot {m_selectedSlot}");
            RefreshSaveSlots();
        }
    }

    public void LoadSelectedSlot()
    {
        if (m_selectedSlot < 0)
        {
            Debug.LogWarning("[SaveLoadUI] No slot selected");
            return;
        }

        GameSaveData saveData = SaveManager.Instance.LoadGame(m_selectedSlot);

        if (saveData != null)
        {
            ApplySaveData(saveData);
            ClosePanel();
            SceneNavigator.GoToOpenWorld();
        }
        else
        {
            Debug.LogWarning("[SaveLoadUI] Failed to load save");
        }
    }

    private void ApplySaveData(GameSaveData saveData)
    {
        if (saveData.player != null && ProfileManager.Instance != null)
        {
            ProfileManager.Instance.UpdatePlayerProgress(saveData.player);
        }

        Debug.Log($"[SaveLoadUI] Loaded save: {saveData.saveName}");
    }

    public void DeleteSelectedSlot()
    {
        if (m_selectedSlot < 0) return;

        SaveManager.Instance.DeleteSave(m_selectedSlot);
        RefreshSaveSlots();
        SelectSlot(-1);
    }

    private string FormatPlayTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);

        if (hours > 0)
            return $"{hours}h {minutes}m";
        else
            return $"{minutes}m";
    }
}

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private Text m_slotText;
    [SerializeField] private Text m_infoText;
    [SerializeField] private Button m_selectButton;

    private SaveLoadUI m_parent;
    private int m_slotIndex;

    public void Setup(SaveSlotInfo slot, int index, SaveLoadUI parent)
    {
        m_parent = parent;
        m_slotIndex = index;

        if (m_slotText != null)
            m_slotText.text = $"Slot {index + 1}";

        if (m_infoText != null)
        {
            if (slot.exists)
            {
                m_infoText.text = $"{slot.saveName}\n{slot.lastPlayed:MM/dd HH:mm}";
            }
            else
            {
                m_infoText.text = "Empty";
            }
        }

        if (m_selectButton != null)
        {
            m_selectButton.onClick.AddListener(() => OnSelect());
        }
    }

    private void OnSelect()
    {
        if (m_parent != null)
        {
            m_parent.SelectSlot(m_slotIndex);
        }
    }
}
