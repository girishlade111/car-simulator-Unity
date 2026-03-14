using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using CarSimulator.Vehicle;
using CarSimulator.World;

namespace CarSimulator.UI
{
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        [Header("Leaderboard UI")]
        [SerializeField] private GameObject m_leaderboardPanel;
        [SerializeField] private Text m_categoryTitle;
        [SerializeField] private Transform m_entryContainer;
        [SerializeField] private GameObject m_entryPrefab;

        [Header("Category Tabs")]
        [SerializeField] private Button m_speedTab;
        [SerializeField] private Button m_lapTimeTab;
        [SerializeField] private Button m_driftTab;
        [SerializeField] private Button m_distanceTab;

        [Header("Current Session")]
        [SerializeField] private Text m_currentSpeedRecord;
        [SerializeField] private Text m_currentLapTime;
        [SerializeField] private Text m_currentDriftScore;
        [SerializeField] private Text m_currentDistance;

        private LeaderboardCategory m_currentCategory = LeaderboardCategory.Speed;
        private List<LeaderboardEntry> m_speedLeaderboard = new List<LeaderboardEntry>();
        private List<LeaderboardEntry> m_lapTimeLeaderboard = new List<LeaderboardEntry>();
        private List<LeaderboardEntry> m_driftLeaderboard = new List<LeaderboardEntry>();
        private List<LeaderboardEntry> m_distanceLeaderboard = new List<LeaderboardEntry>();

        private float m_currentLapStartTime;
        private float m_currentDriftScore;
        private float m_currentDriftTime;
        private bool m_isDrifting;
        private bool m_isInLap;

        public enum LeaderboardCategory
        {
            Speed,
            LapTime,
            Drift,
            Distance
        }

        [System.Serializable]
        public class LeaderboardEntry
        {
            public string playerName;
            public float value;
            public int rank;
            public long timestamp;

            public string FormattedValue(LeaderboardCategory category)
            {
                switch (category)
                {
                    case LeaderboardCategory.Speed:
                        return $"{value:F1} km/h";
                    case LeaderboardCategory.LapTime:
                        return FormatTime(value);
                    case LeaderboardCategory.Drift:
                        return $"{value:F0} pts";
                    case LeaderboardCategory.Distance:
                        return FormatDistance(value);
                    default:
                        return value.ToString();
                }
            }

            private string FormatTime(float seconds)
            {
                int mins = Mathf.FloorToInt(seconds / 60);
                int secs = Mathf.FloorToInt(seconds % 60);
                int ms = Mathf.FloorToInt((seconds * 100) % 100);
                return $"{mins:00}:{secs:00}.{ms:00}";
            }

            private string FormatDistance(float meters)
            {
                if (meters < 1000)
                    return $"{meters:F0}m";
                return $"{meters / 1000:F1}km";
            }
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

            LoadLeaderboards();
            SetupTabs();
        }

        private void SetupTabs()
        {
            if (m_speedTab != null) m_speedTab.onClick.AddListener(() => SetCategory(LeaderboardCategory.Speed));
            if (m_lapTimeTab != null) m_lapTimeTab.onClick.AddListener(() => SetCategory(LeaderboardCategory.LapTime));
            if (m_driftTab != null) m_driftTab.onClick.AddListener(() => SetCategory(LeaderboardCategory.Drift));
            if (m_distanceTab != null) m_distanceTab.onClick.AddListener(() => SetCategory(LeaderboardCategory.Distance));
        }

        private void LoadLeaderboards()
        {
            LoadCategory(LeaderboardCategory.Speed, m_speedLeaderboard, "Leaderboard_Speed");
            LoadCategory(LeaderboardCategory.LapTime, m_lapTimeLeaderboard, "Leaderboard_LapTime");
            LoadCategory(LeaderboardCategory.Drift, m_driftLeaderboard, "Leaderboard_Drift");
            LoadCategory(LeaderboardCategory.Distance, m_distanceLeaderboard, "Leaderboard_Distance");
        }

        private void LoadCategory(LeaderboardCategory category, List<LeaderboardEntry> list, string key)
        {
            list.Clear();
            int count = PlayerPrefs.GetInt($"{key}_Count", 0);

            for (int i = 0; i < count; i++)
            {
                string json = PlayerPrefs.GetString($"{key}_{i}", "");
                if (!string.IsNullOrEmpty(json))
                {
                    LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                    entry.rank = i + 1;
                    list.Add(entry);
                }
            }
        }

        private void SaveCategory(LeaderboardCategory category, List<LeaderboardEntry> list, string key)
        {
            PlayerPrefs.SetInt($"{key}_Count", list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                string json = JsonUtility.ToJson(list[i]);
                PlayerPrefs.SetString($"{key}_{i}", json);
            }

            PlayerPrefs.Save();
        }

        public void SetCategory(LeaderboardCategory category)
        {
            m_currentCategory = category;

            if (m_categoryTitle != null)
            {
                m_categoryTitle.text = category.ToString() + " Leaderboard";
            }

            UpdateLeaderboardDisplay();
        }

        public void UpdateLeaderboardDisplay()
        {
            if (m_entryContainer == null) return;

            foreach (Transform child in m_entryContainer)
            {
                Destroy(child.gameObject);
            }

            List<LeaderboardEntry> currentList = GetCurrentList();

            int displayCount = Mathf.Min(currentList.Count, 10);

            for (int i = 0; i < displayCount; i++)
            {
                CreateEntryRow(currentList[i], i + 1);
            }

            if (displayCount == 0)
            {
                GameObject emptyText = new GameObject("EmptyText");
                emptyText.transform.SetParent(m_entryContainer);

                Text text = emptyText.AddComponent<Text>();
                text.text = "No records yet. Start driving!";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.color = Color.gray;
                text.alignment = TextAnchor.MiddleCenter;
            }
        }

        private List<LeaderboardEntry> GetCurrentList()
        {
            switch (m_currentCategory)
            {
                case LeaderboardCategory.Speed:
                    return m_speedLeaderboard;
                case LeaderboardCategory.LapTime:
                    return m_lapTimeLeaderboard;
                case LeaderboardCategory.Drift:
                    return m_driftLeaderboard;
                case LeaderboardCategory.Distance:
                    return m_distanceLeaderboard;
                default:
                    return m_speedLeaderboard;
            }
        }

        private void CreateEntryRow(LeaderboardEntry entry, int rank)
        {
            GameObject entryObj = Instantiate(m_entryPrefab, m_entryContainer);
            entryObj.transform.SetParent(m_entryContainer);

            var texts = entryObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = rank.ToString();
                texts[1].text = entry.playerName;
                texts[2].text = entry.FormattedValue(m_currentCategory);
            }
        }

        private void Update()
        {
            UpdateCurrentStats();
        }

        private void UpdateCurrentStats()
        {
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle == null) return;

            var vehicle = spawner.CurrentVehicle;
            var physics = vehicle.GetComponent<VehiclePhysics>();
            var input = vehicle.GetComponent<VehicleInput>();

            if (physics != null)
            {
                float speed = physics.CurrentSpeed;
                if (m_currentSpeedRecord != null)
                {
                    m_currentSpeedRecord.text = $"{speed:F1} km/h";
                }

                CheckSpeedRecord(speed);
            }

            if (input != null)
            {
                bool currentlyDrifting = input.IsDrifting;

                if (currentlyDrifting && !m_isDrifting)
                {
                    m_isDrifting = true;
                    m_currentDriftScore = 0;
                    m_currentDriftTime = 0;
                }
                else if (!currentlyDrifting && m_isDrifting)
                {
                    m_isDrifting = false;
                    SubmitDriftScore(m_currentDriftScore);
                }

                if (m_isDrifting)
                {
                    m_currentDriftTime += Time.deltaTime;
                    float multiplier = 1f + (m_currentDriftTime * 0.5f);
                    float driftRate = physics != null ? physics.CurrentSpeed * 10f : 100f;
                    m_currentDriftScore += driftRate * multiplier * Time.deltaTime;

                    if (m_currentDriftScore != null)
                    {
                        m_currentDriftScore.text = $"{m_currentDriftScore:F0} pts";
                    }
                }
            }
        }

        public void StartLap()
        {
            m_isInLap = true;
            m_currentLapStartTime = Time.time;
        }

        public void FinishLap()
        {
            if (!m_isInLap) return;

            float lapTime = Time.time - m_currentLapStartTime;
            m_isInLap = false;

            if (m_currentLapTime != null)
            {
                m_currentLapTime.text = FormatTime(lapTime);
            }

            SubmitLapTime(lapTime);
        }

        private void CheckSpeedRecord(float speed)
        {
            float currentRecord = m_speedLeaderboard.Count > 0 ? m_speedLeaderboard[0].value : 0;

            if (speed > currentRecord && speed > 50)
            {
                SubmitSpeedRecord(speed);
            }
        }

        public void SubmitSpeedRecord(float speed)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            AddEntry(m_speedLeaderboard, new LeaderboardEntry
            {
                playerName = playerName,
                value = speed,
                timestamp = System.DateTime.Now.Ticks
            }, true);

            SaveCategory(LeaderboardCategory.Speed, m_speedLeaderboard, "Leaderboard_Speed");
            UpdateLeaderboardDisplay();
        }

        public void SubmitLapTime(float time)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            AddEntry(m_lapTimeLeaderboard, new LeaderboardEntry
            {
                playerName = playerName,
                value = time,
                timestamp = System.DateTime.Now.Ticks
            }, false);

            SaveCategory(LeaderboardCategory.LapTime, m_lapTimeLeaderboard, "Leaderboard_LapTime");
            UpdateLeaderboardDisplay();
        }

        public void SubmitDriftScore(float score)
        {
            if (score < 100) return;

            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            AddEntry(m_driftLeaderboard, new LeaderboardEntry
            {
                playerName = playerName,
                value = score,
                timestamp = System.DateTime.Now.Ticks
            }, true);

            SaveCategory(LeaderboardCategory.Drift, m_driftLeaderboard, "Leaderboard_Drift");
            UpdateLeaderboardDisplay();
        }

        public void SubmitDistance(float distance)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            AddEntry(m_distanceLeaderboard, new LeaderboardEntry
            {
                playerName = playerName,
                value = distance,
                timestamp = System.DateTime.Now.Ticks
            }, true);

            SaveCategory(LeaderboardCategory.Distance, m_distanceLeaderboard, "Leaderboard_Distance");
            UpdateLeaderboardDisplay();
        }

        private void AddEntry(List<LeaderboardEntry> list, LeaderboardEntry entry, bool higherIsBetter)
        {
            list.Add(entry);

            if (higherIsBetter)
            {
                list.Sort((a, b) => b.value.CompareTo(a.value));
            }
            else
            {
                list.Sort((a, b) => a.value.CompareTo(b.value));
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i].rank = i + 1;
            }

            while (list.Count > 10)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        private string FormatTime(float seconds)
        {
            int mins = Mathf.FloorToInt(seconds / 60);
            int secs = Mathf.FloorToInt(seconds % 60);
            int ms = Mathf.FloorToInt((seconds * 100) % 100);
            return $"{mins:00}:{secs:00}.{ms:00}";
        }

        public void ShowLeaderboard()
        {
            if (m_leaderboardPanel != null)
            {
                m_leaderboardPanel.SetActive(true);
                UpdateLeaderboardDisplay();
            }
        }

        public void HideLeaderboard()
        {
            if (m_leaderboardPanel != null)
            {
                m_leaderboardPanel.SetActive(false);
            }
        }

        public void ToggleLeaderboard()
        {
            if (m_leaderboardPanel != null)
            {
                if (m_leaderboardPanel.activeSelf)
                    HideLeaderboard();
                else
                    ShowLeaderboard();
            }
        }

        public void ClearLeaderboard(LeaderboardCategory category)
        {
            switch (category)
            {
                case LeaderboardCategory.Speed:
                    m_speedLeaderboard.Clear();
                    SaveCategory(LeaderboardCategory.Speed, m_speedLeaderboard, "Leaderboard_Speed");
                    break;
                case LeaderboardCategory.LapTime:
                    m_lapTimeLeaderboard.Clear();
                    SaveCategory(LeaderboardCategory.LapTime, m_lapTimeLeaderboard, "Leaderboard_LapTime");
                    break;
                case LeaderboardCategory.Drift:
                    m_driftLeaderboard.Clear();
                    SaveCategory(LeaderboardCategory.Drift, m_driftLeaderboard, "Leaderboard_Drift");
                    break;
                case LeaderboardCategory.Distance:
                    m_distanceLeaderboard.Clear();
                    SaveCategory(LeaderboardCategory.Distance, m_distanceLeaderboard, "Leaderboard_Distance");
                    break;
            }
            UpdateLeaderboardDisplay();
        }

        public List<LeaderboardEntry> GetSpeedLeaderboard() => m_speedLeaderboard;
        public List<LeaderboardEntry> GetLapTimeLeaderboard() => m_lapTimeLeaderboard;
        public List<LeaderboardEntry> GetDriftLeaderboard() => m_driftLeaderboard;
        public List<LeaderboardEntry> GetDistanceLeaderboard() => m_distanceLeaderboard;
    }
}
