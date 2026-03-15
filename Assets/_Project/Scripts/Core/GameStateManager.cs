using UnityEngine;
using System;

namespace CarSimulator.Core
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        [Header("State Events")]
        public Action<GameState> OnStateChanged;
        public Action<GameState, GameState> OnStateTransition;

        [Header("Current State")]
        [SerializeField] private GameState m_currentState = GameState.MainMenu;
        [SerializeField] private GameState m_previousState;

        [Header("State Settings")]
        [SerializeField] private bool m_canPause = true;
        [SerializeField] private bool m_pauseWithEscape = true;

        public enum GameState
        {
            MainMenu,
            Loading,
            Playing,
            Paused,
            InMenu,
            InDialogue,
            InCutscene,
            InGarage,
            InInterior
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        private void Update()
        {
            if (m_pauseWithEscape && Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscape();
            }
        }

        private void HandleEscape()
        {
            switch (m_currentState)
            {
                case GameState.Playing:
                    Pause();
                    break;
                case GameState.Paused:
                    Resume();
                    break;
                case GameState.InGarage:
                case GameState.InMenu:
                    Resume();
                    break;
                default:
                    break;
            }
        }

        public void SetState(GameState newState)
        {
            if (m_currentState == newState) return;

            GameState oldState = m_currentState;
            m_previousState = oldState;
            m_currentState = newState;

            OnStateTransition?.Invoke(oldState, newState);
            OnStateChanged?.Invoke(newState);

            ApplyStateSettings();
        }

        private void ApplyStateSettings()
        {
            switch (m_currentState)
            {
                case GameState.Paused:
                case GameState.InMenu:
                case GameState.InDialogue:
                case GameState.InCutscene:
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;

                case GameState.Playing:
                case GameState.InGarage:
                case GameState.InInterior:
                    Time.timeScale = 1f;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    break;

                case GameState.MainMenu:
                case GameState.Loading:
                    Time.timeScale = 1f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }
        }

        public void StartGame()
        {
            SetState(GameState.Loading);
        }

        public void BeginPlaying()
        {
            SetState(GameState.Playing);
        }

        public void Pause()
        {
            if (!m_canPause) return;
            if (m_currentState != GameState.Playing) return;

            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (m_currentState == GameState.Paused)
            {
                SetState(m_previousState == GameState.MainMenu ? GameState.MainMenu : GameState.Playing);
            }
            else if (m_currentState == GameState.InGarage || m_currentState == GameState.InMenu)
            {
                SetState(GameState.Playing);
            }
        }

        public void OpenGarage()
        {
            SetState(GameState.InGarage);
        }

        public void CloseGarage()
        {
            SetState(GameState.Playing);
        }

        public void EnterInterior()
        {
            SetState(GameState.InInterior);
        }

        public void ExitInterior()
        {
            SetState(GameState.Playing);
        }

        public void StartDialogue()
        {
            SetState(GameState.InDialogue);
        }

        public void EndDialogue()
        {
            SetState(GameState.Playing);
        }

        public void ReturnToMainMenu()
        {
            SetState(GameState.MainMenu);
        }

        public bool CanMove() => m_currentState == GameState.Playing;
        public bool CanPause() => m_canPause && m_currentState == GameState.Playing;
        public bool IsPaused() => m_currentState == GameState.Paused;
        public bool IsPlaying() => m_currentState == GameState.Playing;

        public GameState GetCurrentState() => m_currentState;
        public GameState GetPreviousState() => m_previousState;

        public void SetCanPause(bool canPause)
        {
            m_canPause = canPause;
        }
    }

    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject m_mainMenuPanel;
        [SerializeField] private GameObject m_hudPanel;
        [SerializeField] private GameObject m_pauseMenuPanel;
        [SerializeField] private GameObject m_settingsPanel;
        [SerializeField] private GameObject m_minimapPanel;

        [Header("State References")]
        [SerializeField] private GameStateManager m_stateManager;

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
            if (m_stateManager == null)
            {
                m_stateManager = GameStateManager.Instance;
            }

            if (m_stateManager != null)
            {
                m_stateManager.OnStateChanged += OnGameStateChanged;
            }

            ShowMainMenu();
        }

        private void OnDestroy()
        {
            if (m_stateManager != null)
            {
                m_stateManager.OnStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case GameState.Playing:
                    ShowHUD();
                    break;
                case GameState.Paused:
                    ShowPauseMenu();
                    break;
                case GameState.InGarage:
                    ShowGarage();
                    break;
                default:
                    break;
            }
        }

        public void ShowMainMenu()
        {
            if (m_mainMenuPanel != null) m_mainMenuPanel.SetActive(true);
            if (m_hudPanel != null) m_hudPanel.SetActive(false);
            if (m_pauseMenuPanel != null) m_pauseMenuPanel.SetActive(false);
            if (m_settingsPanel != null) m_settingsPanel.SetActive(false);
        }

        public void ShowHUD()
        {
            if (m_mainMenuPanel != null) m_mainMenuPanel.SetActive(false);
            if (m_hudPanel != null) m_hudPanel.SetActive(true);
            if (m_pauseMenuPanel != null) m_pauseMenuPanel.SetActive(false);
        }

        public void ShowPauseMenu()
        {
            if (m_pauseMenuPanel != null) m_pauseMenuPanel.SetActive(true);
        }

        public void HidePauseMenu()
        {
            if (m_pauseMenuPanel != null) m_pauseMenuPanel.SetActive(false);
        }

        public void ShowGarage()
        {
            if (m_hudPanel != null) m_hudPanel.SetActive(false);
        }

        public void ToggleMinimap()
        {
            if (m_minimapPanel != null)
            {
                m_minimapPanel.SetActive(!m_minimapPanel.activeSelf);
            }
        }
    }
}
