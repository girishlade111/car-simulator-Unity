using UnityEngine;

namespace CarSimulator.World
{
    public class RaceStartFinish : MonoBehaviour
    {
        [Header("Race Settings")]
        [SerializeField] private int m_totalLaps = 3;
        [SerializeField] private bool m_countdownEnabled = true;
        [SerializeField] private float m_countdownTime = 3f;

        [Header("References")]
        [SerializeField] private LapTimer m_lapTimer;
        [SerializeField] private RaceCheckpoint m_finishLine;

        [Header("UI")]
        [SerializeField] private UnityEngine.UI.Text m_countdownText;

        private bool m_raceStarted;
        private float m_countdownTimer;

        public bool RaceStarted => m_raceStarted;

        private void Start()
        {
            FindComponents();
            
            if (m_countdownEnabled)
            {
                StartCountdown();
            }
        }

        private void FindComponents()
        {
            if (m_lapTimer == null)
                m_lapTimer = FindObjectOfType<LapTimer>();
            if (m_finishLine == null)
                m_finishLine = GetComponent<RaceCheckpoint>();
        }

        private void StartCountdown()
        {
            m_countdownTimer = m_countdownTime;
            m_raceStarted = false;
        }

        private void Update()
        {
            if (!m_countdownEnabled || m_raceStarted) return;

            m_countdownTimer -= Time.deltaTime;

            if (m_countdownText != null)
            {
                if (m_countdownTimer > 0)
                {
                    m_countdownText.text = Mathf.CeilToInt(m_countdownTimer).ToString();
                    m_countdownText.fontSize = 100;
                }
                else
                {
                    m_countdownText.text = "GO!";
                    m_countdownText.fontSize = 120;
                }
            }

            if (m_countdownTimer <= 0)
            {
                StartRace();
            }
        }

        private void StartRace()
        {
            m_raceStarted = true;
            m_countdownEnabled = false;

            if (m_lapTimer != null)
            {
                m_lapTimer.m_totalLaps = m_totalLaps;
                m_lapTimer.StartTimer();
            }

            if (m_countdownText != null)
            {
                m_countdownText.text = "";
            }
        }

        public void RestartRace()
        {
            m_countdownEnabled = true;
            m_raceStarted = false;
            StartCountdown();

            if (m_lapTimer != null)
            {
                m_lapTimer.ResetTimer();
            }
        }
    }
}
