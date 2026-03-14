using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Core;
using CarSimulator.Runtime;

namespace CarSimulator.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_restartButton;
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_mainMenuButton;
        [SerializeField] private Button m_quitButton;

        [Header("Panels")]
        [SerializeField] private GameObject m_pausePanel;
        [SerializeField] private GameObject m_settingsPanel;

        private bool m_isPaused;

        private void Start()
        {
            SetupButtons();

            if (m_pausePanel)
                m_pausePanel.SetActive(false);
        }

        private void SetupButtons()
        {
            if (m_resumeButton) m_resumeButton.onClick.AddListener(OnResume);
            if (m_restartButton) m_restartButton.onClick.AddListener(OnRestart);
            if (m_settingsButton) m_settingsButton.onClick.AddListener(OnSettings);
            if (m_mainMenuButton) m_mainMenuButton.onClick.AddListener(OnMainMenu);
            if (m_quitButton) m_quitButton.onClick.AddListener(OnQuit);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            m_isPaused = !m_isPaused;

            if (m_pausePanel)
                m_pausePanel.SetActive(m_isPaused);

            if (GameManager.Instance != null)
            {
                if (m_isPaused)
                    GameManager.Instance.Pause();
                else
                    GameManager.Instance.Resume();
            }
        }

        public void OnResume()
        {
            TogglePause();
        }

        public void OnRestart()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Resume();

            SceneNavigator.Restart();
        }

        public void OnSettings()
        {
            if (m_settingsPanel)
            {
                m_settingsPanel.SetActive(!m_settingsPanel.activeSelf);
            }
        }

        public void OnMainMenu()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ReturnToMenu();

            SceneNavigator.GoToMainMenu();
        }

        public void OnQuit()
        {
            Bootstrap.Quit();
        }
    }
}
