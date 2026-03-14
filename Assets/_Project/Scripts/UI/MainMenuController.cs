using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Core;
using CarSimulator.Runtime;
using CarSimulator.SaveSystem;

namespace CarSimulator.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button m_newGameButton;
        [SerializeField] private Button m_continueButton;
        [SerializeField] private Button m_loadGameButton;
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_quitButton;

        [Header("Panels")]
        [SerializeField] private GameObject m_settingsPanel;
        [SerializeField] private GameObject m_loadPanel;

        [Header("Settings")]
        [SerializeField] private SettingsController m_settings;

        private void Start()
        {
            SetupButtons();
            UpdateButtonStates();
        }

        private void SetupButtons()
        {
            if (m_newGameButton) m_newGameButton.onClick.AddListener(OnNewGame);
            if (m_continueButton) m_continueButton.onClick.AddListener(OnContinue);
            if (m_loadGameButton) m_loadGameButton.onClick.AddListener(OnLoadGame);
            if (m_settingsButton) m_settingsButton.onClick.AddListener(OnSettings);
            if (m_quitButton) m_quitButton.onClick.AddListener(OnQuit);
        }

        private void UpdateButtonStates()
        {
            bool hasSaves = false;
            if (SaveManager.Instance != null)
            {
                var slots = SaveManager.Instance.GetAllSaveSlots();
                foreach (var slot in slots)
                {
                    if (slot.exists)
                    {
                        hasSaves = true;
                        break;
                    }
                }
            }

            if (m_continueButton) m_continueButton.interactable = hasSaves;
            if (m_loadGameButton) m_loadGameButton.interactable = hasSaves;
        }

        public void OnNewGame()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.StartGame();

            SceneNavigator.GoToOpenWorld();
        }

        public void OnContinue()
        {
            OnLoadGame();
        }

        public void OnLoadGame()
        {
            if (m_loadPanel != null)
            {
                m_loadPanel.SetActive(true);
            }
            else
            {
                SaveManager.Instance?.LoadGame(0);
                SceneNavigator.GoToOpenWorld();
            }
        }

        public void OnSettings()
        {
            if (m_settingsPanel != null)
            {
                m_settingsPanel.SetActive(!m_settingsPanel.activeSelf);
            }
        }

        public void OnQuit()
        {
            Bootstrap.Quit();
        }
    }
}
