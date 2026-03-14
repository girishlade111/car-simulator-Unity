using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        Loading
    }

    [SerializeField] private GameState m_currentState = GameState.Menu;

    public GameState CurrentState => m_currentState;
    public bool IsPaused => m_currentState == GameState.Paused;
    public bool IsPlaying => m_currentState == GameState.Playing;

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

    public void SetState(GameState newState)
    {
        if (m_currentState == newState) return;

        m_currentState = newState;
        OnStateChanged(newState);
    }

    private void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
            case GameState.Menu:
            case GameState.Loading:
                Time.timeScale = 1f;
                break;
        }

        Debug.Log($"[GameManager] State changed to: {state}");
    }

    public void Pause()
    {
        SetState(GameState.Paused);
    }

    public void Resume()
    {
        SetState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void ReturnToMenu()
    {
        SetState(GameState.Menu);
    }
}
