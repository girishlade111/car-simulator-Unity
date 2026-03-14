using UnityEngine;
using UnityEngine.UI;
using CarSimulator.SaveSystem;
using CarSimulator.Runtime;

namespace CarSimulator.UI
{
    public class SaveLoadController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform m_slotContainer;
        [SerializeField] private GameObject m_slotPrefab;
        [SerializeField] private Button m_backButton;

        [Header("Mode")]
        [SerializeField] private bool m_isLoadMode = true;

        private SaveSlotInfo[] m_slots;

        private void Start()
        {
            if (m_backButton) m_backButton.onClick.AddListener(OnBack);
            RefreshSlots();
        }

        public void RefreshSlots()
        {
            if (SaveManager.Instance == null || m_slotContainer == null) return;

            foreach (Transform child in m_slotContainer)
            {
                Destroy(child.gameObject);
            }

            m_slots = SaveManager.Instance.GetAllSaveSlots();

            for (int i = 0; i < m_slots.Length; i++)
            {
                GameObject slotObj = Instantiate(m_slotPrefab, m_slotContainer);
                SaveSlotUI slotUI = slotObj.GetComponent<SaveSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Setup(m_slots[i], i, this);
                }
            }
        }

        public void OnSlotClick(int slotIndex)
        {
            if (m_isLoadMode)
            {
                LoadSlot(slotIndex);
            }
            else
            {
                SaveSlot(slotIndex);
            }
        }

        private void LoadSlot(int slotIndex)
        {
            if (SaveManager.Instance == null) return;

            var saveData = SaveManager.Instance.LoadGame(slotIndex);
            if (saveData != null)
            {
                SceneNavigator.GoToOpenWorld();
            }
        }

        private void SaveSlot(int slotIndex)
        {
            if (SaveManager.Instance == null) return;

            var saveData = new GameSaveData
            {
                saveName = $"Save {slotIndex + 1}",
                currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            };

            SaveManager.Instance.SaveGame(saveData, slotIndex);
            RefreshSlots();
        }

        public void DeleteSlot(int slotIndex)
        {
            SaveManager.Instance?.DeleteSave(slotIndex);
            RefreshSlots();
        }

        public void OnBack()
        {
            gameObject.SetActive(false);
        }
    }

    public class SaveSlotUI : MonoBehaviour
    {
        [SerializeField] private Text m_slotText;
        [SerializeField] private Text m_infoText;
        [SerializeField] private Button m_selectButton;
        [SerializeField] private Button m_deleteButton;

        private SaveLoadController m_parent;
        private int m_slotIndex;

        public void Setup(SaveSlotInfo slot, int index, SaveLoadController parent)
        {
            m_parent = parent;
            m_slotIndex = index;

            if (m_slotText) m_slotText.text = $"Slot {index + 1}";

            if (m_infoText)
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

            if (m_selectButton)
            {
                m_selectButton.onClick.AddListener(() => m_parent.OnSlotClick(m_slotIndex));
            }

            if (m_deleteButton)
            {
                m_deleteButton.onClick.AddListener(() => m_parent.DeleteSlot(m_slotIndex));
                m_deleteButton.gameObject.SetActive(slot.exists);
            }
        }
    }
}
