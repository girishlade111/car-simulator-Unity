using UnityEngine;
using UnityEngine.Button;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_pausePanel;
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Button m_mainMenuButton;
    [SerializeField] private Button m_settingsButton;

    private bool m_isPaused;

    private void Start()
    {
        if (m_pausePanel != null)
            m_pausePanel.SetActive(false);

        if (m_resumeButton != null)
            m_resumeButton.onClick.AddListener(OnResume);

        if (m_mainMenuButton != null)
            m_mainMenuButton.onClick.AddListener(OnMainMenu);

        if (m_settingsButton != null)
            m_settingsButton.onClick.AddListener(OnSettings);
    }

    private void Update()
    {
        if (GameManager.Instance.IsPaused != m_isPaused)
        {
            m_isPaused = GameManager.Instance.IsPaused;
            if (m_pausePanel != null)
                m_pausePanel.SetActive(m_isPaused);
        }
    }

    private void OnResume()
    {
        GameManager.Instance.Resume();
    }

    private void OnMainMenu()
    {
        GameManager.Instance.ReturnToMenu();
        SceneNavigator.GoToMainMenu();
    }

    private void OnSettings()
    {
        Debug.Log("[PauseMenu] Settings not implemented yet");
    }
}
