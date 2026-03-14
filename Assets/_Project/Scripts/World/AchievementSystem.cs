using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.World
{
    public class AchievementSystem : MonoBehaviour
    {
        public static AchievementSystem Instance { get; private set; }

        [Header("Achievement UI")]
        [SerializeField] private GameObject m_achievementPanel;
        [SerializeField] private Text m_notificationText;
        [SerializeField] private Transform m_achievementList;

        [Header("Settings")]
        [SerializeField] private float m_notificationDuration = 3f;

        private List<Achievement> m_achievements = new List<Achievement>();
        private float m_notificationTimer;

        private float m_totalDriftTime;
        private float m_totalOffroadDistance;
        private float m_totalNightDrivingTime;
        private int m_carsOwned;
        private int m_upgradesPurchased;
        private int m_raceWins;
        private int m_presetsTried;
        private bool[] m_presetTriedFlags = new bool[4];
        private float m_currentSpeed;
        private bool m_isDrifting;
        private bool m_isOffroad;

        [System.Serializable]
        public class Achievement
        {
            public string id;
            public string title;
            public string description;
            public bool isUnlocked;
            public int rewardCredits;
            public System.Action unlockCondition;
            public float progress;
            public float target;
            public bool showProgress;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            m_achievements = new List<Achievement>
            {
                new Achievement { id = "first_drive", title = "First Drive", description = "Drive for the first time", rewardCredits = 100, target = 1 },
                new Achievement { id = "speed_demon", title = "Speed Demon", description = "Reach 150 km/h", rewardCredits = 250, target = 150 },
                new Achievement { id = "drift_master", title = "Drift Master", description = "Drift for 30 seconds total", rewardCredits = 500, target = 30, showProgress = true },
                new Achievement { id = "drift_king", title = "Drift King", description = "Drift for 2 minutes total", rewardCredits = 1000, target = 120, showProgress = true },
                new Achievement { id = "offroad_king", title = "Off-Road King", description = "Drive 10km off-road", rewardCredits = 300, target = 10000, showProgress = true },
                new Achievement { id = "offroad_expert", title = "Off-Road Expert", description = "Drive 50km off-road", rewardCredits = 750, target = 50000, showProgress = true },
                new Achievement { id = "night_driver", title = "Night Driver", description = "Drive at night for 5 minutes", rewardCredits = 200, target = 300, showProgress = true },
                new Achievement { id = "night_owl", title = "Night Owl", description = "Drive at night for 30 minutes", rewardCredits = 600, target = 1800, showProgress = true },
                new Achievement { id = "speedster", title = "Speedster", description = "Reach 200 km/h", rewardCredits = 500, target = 200 },
                new Achievement { id = "hyper_car", title = "Hyper Car", description = "Reach 250 km/h", rewardCredits = 1000, target = 250 },
                new Achievement { id = "collector", title = "Collector", description = "Own 3 cars", rewardCredits = 500, target = 3, showProgress = true },
                new Achievement { id = "car_connoisseur", title = "Car Connoisseur", description = "Own 7 cars", rewardCredits = 1500, target = 7, showProgress = true },
                new Achievement { id = "mechanic", title = "Mechanic", description = "Buy 10 upgrades", rewardCredits = 400, target = 10, showProgress = true },
                new Achievement { id = "master_mechanic", title = "Master Mechanic", description = "Buy 50 upgrades", rewardCredits = 1500, target = 50, showProgress = true },
                new Achievement { id = "survivor", title = "Survivor", description = "Win a race with 10% health", rewardCredits = 600 },
                new Achievement { id = "test_drive", title = "Test Driver", description = "Try all tuning presets", rewardCredits = 300, target = 4, showProgress = true },
                new Achievement { id = "racer", title = "Racer", description = "Win your first race", rewardCredits = 500 },
                new Achievement { id = "champion", title = "Champion", description = "Win 10 races", rewardCredits = 2000, target = 10, showProgress = true },
                new Achievement { id = "first_wheels", title = "First Wheels", description = "Buy your first wheel set", rewardCredits = 200 },
                new Achievement { id = "wheel_collector", title = "Wheel Collector", description = "Own 5 different wheel sets", rewardCredits = 750, target = 5, showProgress = true },
                new Achievement { id = "paint_lover", title = "Paint Lover", description = "Change your car color 10 times", rewardCredits = 300, target = 10, showProgress = true },
                new Achievement { id = "road_warrior", title = "Road Warrior", description = "Drive 100km total", rewardCredits = 800, target = 100000, showProgress = true },
                new Achievement { id = "long_trip", title = "Long Trip", description = "Drive 500km total", rewardCredits = 2500, target = 500000, showProgress = true }
            };

            LoadProgress();
            LoadAchievements();
        }

        public void AddOffroadDistance(float distance)
        {
            m_totalOffroadDistance += distance;
            UnlockAchievementIf("offroad_king", m_totalOffroadDistance >= 10000);
            UnlockAchievementIf("offroad_expert", m_totalOffroadDistance >= 50000);
            SaveProgress();
        }

        public void AddDriftTime(float time)
        {
            m_totalDriftTime += time;
            UnlockAchievementIf("drift_master", m_totalDriftTime >= 30);
            UnlockAchievementIf("drift_king", m_totalDriftTime >= 120);
            SaveProgress();
        }

        public void AddNightDrivingTime(float time)
        {
            m_totalNightDrivingTime += time;
            UnlockAchievementIf("night_driver", m_totalNightDrivingTime >= 300);
            UnlockAchievementIf("night_owl", m_totalNightDrivingTime >= 1800);
            SaveProgress();
        }

        public void AddUpgradePurchased()
        {
            m_upgradesPurchased++;
            UnlockAchievementIf("mechanic", m_upgradesPurchased >= 10);
            UnlockAchievementIf("master_mechanic", m_upgradesPurchased >= 50);
            SaveProgress();
        }

        public void AddCarOwned()
        {
            m_carsOwned++;
            UnlockAchievementIf("collector", m_carsOwned >= 3);
            UnlockAchievementIf("car_connoisseur", m_carsOwned >= 7);
            SaveProgress();
        }

        public void AddRaceWin()
        {
            m_raceWins++;
            UnlockAchievementIf("racer", m_raceWins >= 1);
            UnlockAchievementIf("champion", m_raceWins >= 10);
            SaveProgress();
        }

        public void RecordPresetUsed(int presetIndex)
        {
            if (presetIndex >= 0 && presetIndex < m_presetTriedFlags.Length)
            {
                if (!m_presetTriedFlags[presetIndex])
                {
                    m_presetTriedFlags[presetIndex] = true;
                    m_presetsTried++;
                    UnlockAchievementIf("test_drive", m_presetsTried >= 4);
                    SaveProgress();
                }
            }
        }

        public void AddWheelSet()
        {
            int wheels = PlayerPrefs.GetInt("WheelsOwned", 0) + 1;
            PlayerPrefs.SetInt("WheelsOwned", wheels);
            UnlockAchievementIf("first_wheels", wheels >= 1);
            UnlockAchievementIf("wheel_collector", wheels >= 5);
            SaveProgress();
        }

        public void AddPaintChange()
        {
            int paints = PlayerPrefs.GetInt("PaintChanges", 0) + 1;
            PlayerPrefs.SetInt("PaintChanges", paints);
            UnlockAchievementIf("paint_lover", paints >= 10);
            SaveProgress();
        }

        public void AddTotalDistance(float distance)
        {
            float total = PlayerPrefs.GetFloat("TotalDistanceDriven", 0f) + distance;
            PlayerPrefs.SetFloat("TotalDistanceDriven", total);
            UnlockAchievementIf("road_warrior", total >= 100000);
            UnlockAchievementIf("long_trip", total >= 500000);
            SaveProgress();
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetFloat("TotalDriftTime", m_totalDriftTime);
            PlayerPrefs.SetFloat("TotalOffroadDistance", m_totalOffroadDistance);
            PlayerPrefs.SetFloat("TotalNightDrivingTime", m_totalNightDrivingTime);
            PlayerPrefs.SetInt("CarsOwned", m_carsOwned);
            PlayerPrefs.SetInt("UpgradesPurchased", m_upgradesPurchased);
            PlayerPrefs.SetInt("RaceWins", m_raceWins);
            PlayerPrefs.SetInt("PresetsTried", m_presetsTried);
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            m_totalDriftTime = PlayerPrefs.GetFloat("TotalDriftTime", 0f);
            m_totalOffroadDistance = PlayerPrefs.GetFloat("TotalOffroadDistance", 0f);
            m_totalNightDrivingTime = PlayerPrefs.GetFloat("TotalNightDrivingTime", 0f);
            m_carsOwned = PlayerPrefs.GetInt("CarsOwned", 1);
            m_upgradesPurchased = PlayerPrefs.GetInt("UpgradesPurchased", 0);
            m_raceWins = PlayerPrefs.GetInt("RaceWins", 0);
            m_presetsTried = PlayerPrefs.GetInt("PresetsTried", 0);

            for (int i = 0; i < 4 && i < m_presetTriedFlags.Length; i++)
            {
                m_presetTriedFlags[i] = m_presetsTried > i;
            }
        }

        public float GetDriftProgress() => m_totalDriftTime;
        public float GetOffroadProgress() => m_totalOffroadDistance;
        public float GetNightDrivingProgress() => m_totalNightDrivingTime;
        public int GetCarsOwned() => m_carsOwned;
        public int GetUpgradesPurchased() => m_upgradesPurchased;
        public int GetRaceWins() => m_raceWins;

        private void Update()
        {
            CheckAchievements();
            UpdateNotification();
            TrackPlayerStats();
        }

        private void TrackPlayerStats()
        {
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle == null) return;

            var physics = spawner.CurrentVehicle.GetComponent<VehiclePhysics>();
            var input = spawner.CurrentVehicle.GetComponent<VehicleInput>();

            if (physics == null) return;

            m_currentSpeed = physics.CurrentSpeed;

            if (input != null)
            {
                m_isDrifting = input.IsDrifting;
            }

            float speedMps = m_currentSpeed / 3.6f;
            AddTotalDistance(speedMps * Time.deltaTime);

            bool isNight = IsNightTime();
            if (isNight)
            {
                AddNightDrivingTime(Time.deltaTime);
            }

            if (m_isDrifting)
            {
                AddDriftTime(Time.deltaTime);
            }

            bool isOffroad = CheckOffroad(physics);
            if (isOffroad)
            {
                AddOffroadDistance(speedMps * Time.deltaTime);
            }
        }

        private bool IsNightTime()
        {
            var timeOfDay = FindObjectOfType<TimeOfDay>();
            if (timeOfDay != null)
            {
                return timeOfDay.IsNight;
            }
            return false;
        }

        private bool CheckOffroad(VehiclePhysics physics)
        {
            if (physics == null) return false;
            RaycastHit hit;
            if (Physics.Raycast(physics.transform.position + Vector3.up, Vector3.down, out hit, 5f))
            {
                return hit.collider.CompareTag("Offroad") || hit.collider.CompareTag("Ground");
            }
            return false;
        }

        private void CheckAchievements()
        {
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                var physics = spawner.CurrentVehicle.GetComponent<VehiclePhysics>();
                if (physics != null)
                {
                    float speed = physics.CurrentSpeed;

                    UnlockAchievementIf("first_drive", true);
                    UnlockAchievementIf("speed_demon", speed >= 150);
                    UnlockAchievementIf("speedster", speed >= 200);
                    UnlockAchievementIf("hyper_car", speed >= 250);
                }
            }
        }

        private void UpdateNotification()
        {
            if (m_notificationTimer > 0)
            {
                m_notificationTimer -= Time.deltaTime;
                if (m_notificationTimer <= 0 && m_notificationText != null)
                {
                    m_notificationText.gameObject.SetActive(false);
                }
            }
        }

        public void UnlockAchievementIf(string id, bool condition)
        {
            if (!condition) return;

            var achievement = m_achievements.Find(a => a.id == id);
            if (achievement != null && !achievement.isUnlocked)
            {
                UnlockAchievement(achievement);
            }
        }

        private void UnlockAchievement(Achievement achievement)
        {
            achievement.isUnlocked = true;
            ShowNotification($"{achievement.title} Unlocked!");
            
            Debug.Log($"[Achievement] Unlocked: {achievement.title}");

            SaveAchievements();
        }

        private void ShowNotification(string message)
        {
            if (m_notificationText != null)
            {
                m_notificationText.text = message;
                m_notificationText.gameObject.SetActive(true);
                m_notificationTimer = m_notificationDuration;
            }
        }

        public void ShowAchievementPanel()
        {
            if (m_achievementPanel != null)
            {
                m_achievementPanel.SetActive(true);
                UpdateAchievementList();
            }
        }

        public void HideAchievementPanel()
        {
            if (m_achievementPanel != null)
            {
                m_achievementPanel.SetActive(false);
            }
        }

        private void UpdateAchievementList()
        {
            if (m_achievementList == null) return;

            foreach (Transform child in m_achievementList)
            {
                Destroy(child.gameObject);
            }

            foreach (var achievement in m_achievements)
            {
                GameObject item = new GameObject(achievement.title);
                item.transform.SetParent(m_achievementList);

                var layout = item.AddComponent<HorizontalLayoutGroup>();
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;

                var contentSizeFitter = item.AddComponent<ContentSizeFitter>();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                GameObject infoPanel = new GameObject("Info");
                infoPanel.transform.SetParent(item.transform);

                Text titleText = infoPanel.AddComponent<Text>();
                titleText.text = achievement.title;
                titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = achievement.isUnlocked ? Color.green : Color.white;

                Text descText = infoPanel.AddComponent<Text>();
                descText.text = achievement.description;
                descText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                descText.color = Color.gray;

                if (achievement.showProgress && !achievement.isUnlocked)
                {
                    float progress = GetAchievementProgress(achievement.id);
                    descText.text += $" ({progress:F0}/{achievement.target})";
                }

                if (achievement.isUnlocked)
                {
                    Text rewardText = new GameObject("Reward").AddComponent<Text>();
                    rewardText.transform.SetParent(item.transform);
                    rewardText.text = $"+{achievement.rewardCredits}";
                    rewardText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    rewardText.color = Color.yellow;
                }
            }
        }

        private float GetAchievementProgress(string id)
        {
            switch (id)
            {
                case "drift_master":
                case "drift_king":
                    return m_totalDriftTime;
                case "offroad_king":
                case "offroad_expert":
                    return m_totalOffroadDistance;
                case "night_driver":
                case "night_owl":
                    return m_totalNightDrivingTime;
                case "collector":
                case "car_connoisseur":
                    return m_carsOwned;
                case "mechanic":
                case "master_mechanic":
                    return m_upgradesPurchased;
                case "champion":
                    return m_raceWins;
                case "test_drive":
                    return m_presetsTried;
                case "wheel_collector":
                    return PlayerPrefs.GetInt("WheelsOwned", 0);
                case "paint_lover":
                    return PlayerPrefs.GetInt("PaintChanges", 0);
                case "road_warrior":
                case "long_trip":
                    return PlayerPrefs.GetFloat("TotalDistanceDriven", 0f);
                default:
                    return 0;
            }
        }

        public int GetUnlockedCount()
        {
            int count = 0;
            foreach (var a in m_achievements)
            {
                if (a.isUnlocked) count++;
            }
            return count;
        }

        public void SaveAchievements()
        {
            for (int i = 0; i < m_achievements.Count; i++)
            {
                PlayerPrefs.SetInt($"Achievement_{m_achievements[i].id}", m_achievements[i].isUnlocked ? 1 : 0);
            }
            SaveProgress();
            PlayerPrefs.Save();
        }

        private void LoadAchievements()
        {
            for (int i = 0; i < m_achievements.Count; i++)
            {
                if (PlayerPrefs.HasKey($"Achievement_{m_achievements[i].id}"))
                {
                    m_achievements[i].isUnlocked = PlayerPrefs.GetInt($"Achievement_{m_achievements[i].id}") == 1;
                }
            }
        }

        public List<Achievement> GetAllAchievements() => m_achievements;
    }
}
