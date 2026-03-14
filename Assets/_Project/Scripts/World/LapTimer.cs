using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class LapTimer : MonoBehaviour
    {
        [Header("Lap Settings")]
        [SerializeField] private int m_totalLaps = 3;
        [SerializeField] private bool m_startTimerOnStart = false;

        [Header("UI References")]
        [SerializeField] private Text m_currentLapText;
        [SerializeField] private Text m_bestLapText;
        [SerializeField] private Text m_lastLapText;
        [SerializeField] private Text m_timerText;
        [SerializeField] private Text m_finishText;

        [Header("Checkpoint System")]
        [SerializeField] private bool m_requireCheckpoints = true;
        [SerializeField] private int m_checkpointCount = 3;

        private List<Checkpoint> m_checkpoints = new List<Checkpoint>();
        private int m_currentCheckpoint;
        private int m_currentLap;
        private float m_lapStartTime;
        private float m_currentLapTime;
        private float m_bestLapTime = float.MaxValue;
        private float m_lastLapTime;
        private float m_totalTime;
        private bool m_isRunning;
        private bool m_isFinished;

        public float CurrentLapTime => m_currentLapTime;
        public float BestLapTime => m_bestLapTime;
        public int CurrentLap => m_currentLap;
        public int TotalLaps => m_totalLaps;
        public bool IsRunning => m_isRunning;
        public bool IsFinished => m_isFinished;

        private void Start()
        {
            if (m_startTimerOnStart)
            {
                StartTimer();
            }
        }

        private void Update()
        {
            if (!m_isRunning || m_isFinished) return;

            m_currentLapTime = Time.time - m_lapStartTime;
            m_totalTime += Time.deltaTime;

            UpdateUI();
        }

        public void StartTimer()
        {
            m_isRunning = true;
            m_currentLap = 1;
            m_currentCheckpoint = 0;
            m_lapStartTime = Time.time;
            m_currentLapTime = 0f;
            m_totalTime = 0f;

            Debug.Log($"[LapTimer] Race started! Lap {m_currentLap}/{m_totalLaps}");
        }

        public void StopTimer()
        {
            m_isRunning = false;
        }

        public void ResetTimer()
        {
            m_currentLap = 0;
            m_currentCheckpoint = 0;
            m_bestLapTime = float.MaxValue;
            m_lastLapTime = 0f;
            m_totalTime = 0f;
            m_currentLapTime = 0f;
            m_isRunning = false;
            m_isFinished = false;
            UpdateUI();
        }

        public void RegisterCheckpoint(Checkpoint checkpoint)
        {
            if (!m_checkpoints.Contains(checkpoint))
            {
                m_checkpoints.Add(checkpoint);
                m_checkpoints.Sort((a, b) => a.Order.CompareTo(b.Order));
            }
        }

        public void OnCheckpointReached(int checkpointIndex)
        {
            if (!m_isRunning || m_isFinished) return;

            if (m_requireCheckpoints)
            {
                if (checkpointIndex == m_currentCheckpoint)
                {
                    m_currentCheckpoint++;
                    
                    if (m_currentCheckpoint >= m_checkpoints.Count)
                    {
                        CompleteLap();
                    }
                }
            }
            else
            {
                CompleteLap();
            }
        }

        private void CompleteLap()
        {
            m_lastLapTime = m_currentLapTime;

            if (m_currentLapTime < m_bestLapTime)
            {
                m_bestLapTime = m_currentLapTime;
            }

            if (m_currentLap >= m_totalLaps)
            {
                FinishRace();
            }
            else
            {
                m_currentLap++;
                m_currentCheckpoint = 0;
                m_lapStartTime = Time.time;
                
                Debug.Log($"[LapTimer] Lap {m_currentLap - 1} completed: {FormatTime(m_lastLapTime)} | Best: {FormatTime(m_bestLapTime)}");
            }
        }

        private void FinishRace()
        {
            m_isRunning = false;
            m_isFinished = true;

            Debug.Log($"[LapTimer] Race finished! Total time: {FormatTime(m_totalTime)} | Best lap: {FormatTime(m_bestLapTime)}");

            if (m_finishText != null)
            {
                m_finishText.text = $"FINISHED!\nTotal: {FormatTime(m_totalTime)}\nBest: {FormatTime(m_bestLapTime)}";
                m_finishText.gameObject.SetActive(true);
            }
        }

        private void UpdateUI()
        {
            if (m_timerText != null)
            {
                m_timerText.text = FormatTime(m_currentLapTime);
            }

            if (m_currentLapText != null)
            {
                m_currentLapText.text = $"LAP {m_currentLap}/{m_totalLaps}";
            }

            if (m_bestLapText != null && m_bestLapTime < float.MaxValue)
            {
                m_bestLapText.text = $"BEST: {FormatTime(m_bestLapTime)}";
            }

            if (m_lastLapText != null && m_lastLapTime > 0)
            {
                m_lastLapText.text = $"LAST: {FormatTime(m_lastLapTime)}";
            }
        }

        public string FormatTime(float time)
        {
            int minutes = (int)(time / 60f);
            int seconds = (int)(time % 60f);
            int milliseconds = (int)((time * 100f) % 100f);
            return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        }

        public class Checkpoint : MonoBehaviour
        {
            public int Order;
            public LapTimer Timer;

            private void OnTriggerEnter(Collider other)
            {
                if (other.CompareTag("Player") && Timer != null)
                {
                    Timer.OnCheckpointReached(Order);
                }
            }
        }
    }
}
