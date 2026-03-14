using UnityEngine;

namespace CarSimulator.Core
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            None,
            Loading,
            Menu,
            Playing,
            Paused
        }

        private static GameManager s_instance;
        public static GameManager Instance => s_instance;

        private GameState m_state = GameState.None;
        private float m_timeScale = 1f;

        public GameState State => m_state;
        public bool IsPaused => m_state == GameState.Paused;
        public bool IsPlaying => m_state == GameState.Playing;

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else if (s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            SetState(GameState.Menu);
        }

        public void SetState(GameState newState)
        {
            if (m_state == newState) return;

            m_state = newState;
            OnStateChanged(newState);
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    m_timeScale = GameConstants.PAUSED_TIME_SCALE;
                    break;
                default:
                    m_timeScale = GameConstants.DEFAULT_TIME_SCALE;
                    break;
            }

            Time.timeScale = m_timeScale;
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
            if (IsPaused) Resume();
            else Pause();
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
}
