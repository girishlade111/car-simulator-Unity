using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.World;

namespace CarSimulator.UI
{
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [Header("Race UI")]
        [SerializeField] private GameObject m_raceHUD;
        [SerializeField] private Text m_lapTimerText;
        [SerializeField] private Text m_currentLapText;
        [SerializeField] private Text m_lastLapText;
        [SerializeField] private Text m_bestLapText;
        [SerializeField] private Text m_positionText;
        [SerializeField] private Text m_countdownText;

        [Header("Race Settings")]
        [SerializeField] private int m_totalLaps = 3;
        [SerializeField] private bool m_countdownEnabled = true;
        [SerializeField] private float m_countdownTime = 3f;

        [Header("Checkpoints")]
        [SerializeField] private RaceCheckpoint[] m_checkpoints;

        private bool m_isRacing;
        private int m_currentLap;
        private float m_lapStartTime;
        private float m_currentLapTime;
        private float m_bestLapTime = float.MaxValue;
        private List<float> m_lapTimes = new List<float>();
        private int m_nextCheckpoint;
        private float m_raceStartTime;
        private List<RacerInfo> m_racers = new List<RacerInfo>();

        private class RacerInfo
        {
            public string name;
            public float distance;
            public int checkpointsHit;
        }

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
            FindCheckpoints();
            if (m_raceHUD != null) m_raceHUD.SetActive(false);
        }

        private void FindCheckpoints()
        {
            m_checkpoints = FindObjectsOfType<RaceCheckpoint>();
            System.Array.Sort(m_checkpoints, (a, b) => a.Order.CompareTo(b.Order));
        }

        public void StartRace()
        {
            if (m_countdownEnabled)
            {
                StartCoroutine(Countdown());
            }
            else
            {
                BeginRace();
            }
        }

        private System.Collections.IEnumerator Countdown()
        {
            m_isRacing = false;

            if (m_raceHUD != null) m_raceHUD.SetActive(true);

            for (int i = (int)m_countdownTime; i > 0; i--)
            {
                if (m_countdownText != null)
                {
                    m_countdownText.text = i.ToString();
                    m_countdownText.gameObject.SetActive(true);
                }
                yield return new WaitForSeconds(1f);
            }

            if (m_countdownText != null)
            {
                m_countdownText.text = "GO!";
                m_countdownText.color = Color.green;
                yield return new WaitForSeconds(0.5f);
                m_countdownText.gameObject.SetActive(false);
            }

            BeginRace();
        }

        private void BeginRace()
        {
            m_isRacing = true;
            m_currentLap = 1;
            m_lapStartTime = Time.time;
            m_raceStartTime = Time.time;
            m_nextCheckpoint = 0;
            m_lapTimes.Clear();

            if (m_raceHUD != null) m_raceHUD.SetActive(true);

            LeaderboardManager.Instance?.StartLap();

            Debug.Log($"[Race] Started! Laps: {m_totalLaps}");
        }

        public void EndRace()
        {
            if (!m_isRacing) return;

            m_isRacing = false;

            if (m_raceHUD != null) m_raceHUD.SetActive(false);

            float totalTime = Time.time - m_raceStartTime;
            Debug.Log($"[Race] Finished! Total time: {totalTime:F2}s");

            var achievements = FindObjectOfType<AchievementSystem>();
            if (achievements != null)
            {
                achievements.AddRaceWin();
            }
        }

        private void Update()
        {
            if (!m_isRacing) return;

            UpdateLapTimer();
            UpdateCheckpointProgress();
            UpdatePosition();
        }

        private void UpdateLapTimer()
        {
            m_currentLapTime = Time.time - m_lapStartTime;

            if (m_lapTimerText != null)
            {
                m_lapTimerText.text = FormatTime(m_currentLapTime);
            }

            if (m_currentLapText != null)
            {
                m_currentLapText.text = $"Lap {m_currentLap}/{m_totalLaps}";
            }
        }

        private void UpdateCheckpointProgress()
        {
            if (m_checkpoints == null || m_checkpoints.Length == 0) return;

            for (int i = 0; i < m_checkpoints.Length; i++)
            {
                if (m_checkpoints[i].IsPlayerAtCheckpoint())
                {
                    if (i == m_nextCheckpoint)
                    {
                        m_nextCheckpoint = (m_nextCheckpoint + 1) % m_checkpoints.Length;

                        if (m_nextCheckpoint == 0)
                        {
                            CompleteLap();
                        }
                    }
                }
            }
        }

        private void CompleteLap()
        {
            m_lapTimes.Add(m_currentLapTime);

            if (m_lastLapText != null)
            {
                m_lastLapText.text = $"Last: {FormatTime(m_currentLapTime)}";
            }

            if (m_currentLapTime < m_bestLapTime)
            {
                m_bestLapTime = m_currentLapTime;
                if (m_bestLapText != null)
                {
                    m_bestLapText.text = $"Best: {FormatTime(m_bestLapTime)}";
                }
            }

            LeaderboardManager.Instance?.FinishLap();

            m_currentLap++;
            m_lapStartTime = Time.time;

            if (m_currentLap > m_totalLaps)
            {
                EndRace();
            }
        }

        private void UpdatePosition()
        {
            if (m_positionText != null)
            {
                m_positionText.text = "1st";
            }
        }

        public void OnCheckpointHit(int order)
        {
            if (order == m_nextCheckpoint)
            {
                m_nextCheckpoint++;
                Debug.Log($"[Race] Checkpoint {order} hit! Next: {m_nextCheckpoint}");
            }
        }

        private string FormatTime(float seconds)
        {
            int mins = Mathf.FloorToInt(seconds / 60);
            int secs = Mathf.FloorToInt(seconds % 60);
            int ms = Mathf.FloorToInt((seconds * 100) % 100);
            return $"{mins:00}:{secs:00}.{ms:00}";
        }

        public bool IsRacing => m_isRacing;
        public int CurrentLap => m_currentLap;
        public int TotalLaps => m_totalLaps;
        public float CurrentLapTime => m_currentLapTime;
        public float BestLapTime => m_bestLapTime;
        public List<float> LapTimes => m_lapTimes;
    }
}
