using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private Button m_newGameButton;
    [SerializeField] private Button m_loadGameButton;
    [SerializeField] private Button m_garageButton;
    [SerializeField] private Button m_settingsButton;
    [SerializeField] private Button m_quitButton;

    [Header("Sub Panels")]
    [SerializeField] private GameObject m_settingsPanel;
    [SerializeField] private GameObject m_saveLoadPanel;
    [SerializeField] private SettingsMenu m_settingsMenu;
    [SerializeField] private SaveLoadUI m_saveLoadUI;

    [Header("Confirmation")]
    [SerializeField] private GameObject m_confirmPanel;
    [SerializeField] private Text m_confirmText;
    private System.Action m_confirmAction;

    private bool m_isLoadingGame;

    private void Start()
    {
        SetupButtons();
        CheckSaveFiles();
    }

    private void SetupButtons()
    {
        if (m_newGameButton != null)
            m_newGameButton.onClick.AddListener(OnNewGame);

        if (m_loadGameButton != null)
            m_loadGameButton.onClick.AddListener(OnLoadGame);

        if (m_garageButton != null)
            m_garageButton.onClick.AddListener(OnGarage);

        if (m_settingsButton != null)
            m_settingsButton.onClick.AddListener(OnSettings);

        if (m_quitButton != null)
            m_quitButton.onClick.AddListener(OnQuit);
    }

    private void CheckSaveFiles()
    {
        bool hasSaves = false;

        if (SaveManager.Instance != null)
        {
            var slots = SaveManager.Instance.GetAllSaveSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].exists)
                {
                    hasSaves = true;
                    break;
                }
            }
        }

        if (m_loadGameButton != null)
        {
            m_loadGameButton.interactable = hasSaves;
        }

        if (m_continueButton != null)
        {
            m_continueButton.interactable = hasSaves;
        }
    }

    public void OnNewGame()
    {
        if (HasActiveSave())
        {
            ShowConfirm("Start new game? Current progress will be lost.", () =>
            {
                Bootstrap.NewGame();
            });
        }
        else
        {
            Bootstrap.NewGame();
        }
    }

    private Button m_continueButton;

    public void OnContinue()
    {
        OnLoadGame();
    }

    public void OnLoadGame()
    {
        if (m_saveLoadUI != null)
        {
            m_saveLoadUI.ShowLoadPanel();
        }
        else
        {
            Bootstrap.LoadGame(0);
        }
    }

    public void OnGarage()
    {
        Bootstrap.GoToGarage();
    }

    public void OnSettings()
    {
        if (m_settingsPanel != null)
        {
            bool isActive = m_settingsPanel.activeSelf;
            CloseAllPanels();

            if (!isActive)
            {
                m_settingsPanel.SetActive(true);
                if (m_settingsMenu != null)
                {
                    m_settingsMenu.ShowSettings();
                }
            }
        }
    }

    public void OnSaveGame()
    {
        if (m_saveLoadUI != null)
        {
            m_saveLoadUI.ShowSavePanel();
        }
        else
        {
            Bootstrap.SaveGame(0, "Quick Save");
        }
    }

    public void OnQuit()
    {
        ShowConfirm("Quit game?", () =>
        {
            Bootstrap.QuitGame();
        });
    }

    private void CloseAllPanels()
    {
        if (m_settingsPanel != null) m_settingsPanel.SetActive(false);
        if (m_saveLoadPanel != null) m_saveLoadPanel.SetActive(false);
        if (m_confirmPanel != null) m_confirmPanel.SetActive(false);
    }

    private bool HasActiveSave()
    {
        if (SaveManager.Instance == null) return false;

        var slots = SaveManager.Instance.GetAllSaveSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].exists) return true;
        }
        return false;
    }

    private void ShowConfirm(string message, System.Action onConfirm)
    {
        m_confirmAction = onConfirm;

        if (m_confirmPanel != null)
        {
            m_confirmPanel.SetActive(true);
            if (m_confirmText != null)
            {
                m_confirmText.text = message;
            }
        }
    }

    public void OnConfirmYes()
    {
        if (m_confirmAction != null)
        {
            m_confirmAction.Invoke();
        }

        if (m_confirmPanel != null)
        {
            m_confirmPanel.SetActive(false);
        }
    }

    public void OnConfirmNo()
    {
        if (m_confirmPanel != null)
        {
            m_confirmPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllPanels();
        }
    }
}
