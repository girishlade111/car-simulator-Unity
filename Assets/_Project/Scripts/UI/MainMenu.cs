using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button m_newGameButton;
    [SerializeField] private Button m_continueButton;
    [SerializeField] private Button m_settingsButton;
    [SerializeField] private Button m_quitButton;

    [SerializeField] private GameObject m_settingsPanel;

    private void Start()
    {
        if (m_newGameButton != null)
            m_newGameButton.onClick.AddListener(OnNewGame);

        if (m_continueButton != null)
            m_continueButton.onClick.AddListener(OnContinue);

        if (m_settingsButton != null)
            m_settingsButton.onClick.AddListener(OnSettings);

        if (m_quitButton != null)
            m_quitButton.onClick.AddListener(OnQuit);
    }

    private void OnNewGame()
    {
        GameManager.Instance.StartGame();
        SceneNavigator.GoToOpenWorld();
    }

    private void OnContinue()
    {
        GameManager.Instance.StartGame();
        SceneNavigator.GoToOpenWorld();
    }

    private void OnSettings()
    {
        if (m_settingsPanel != null)
        {
            m_settingsPanel.SetActive(!m_settingsPanel.activeSelf);
        }
    }

    private void OnQuit()
    {
        Debug.Log("[MainMenu] Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
