using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Missions
{
    public enum MissionCategory
    {
        Main,
        Side,
        Daily,
        Weekly,
        Challenge
    }

    public enum MissionStatus
    {
        Locked,
        Available,
        Active,
        Completed,
        Failed
    }

    [System.Serializable]
    public class MissionProgress
    {
        public string missionId;
        public MissionStatus status;
        public float progress;
        public int currentObjective;
        public long startTime;
        public int attempts;

        public MissionProgress(string id)
        {
            missionId = id;
            status = MissionStatus.Available;
            progress = 0;
            currentObjective = 0;
            startTime = 0;
            attempts = 0;
        }
    }

    public class EnhancedMissionManager : MonoBehaviour
    {
        public static EnhancedMissionManager Instance { get; private set; }

        [Header("Mission Database")]
        [SerializeField] private MissionDatabase m_missionDatabase;

        [Header("Mission Settings")]
        [SerializeField] private int m_maxActiveMissions = 3;
        [SerializeField] private bool m_showMissionNotifications = true;

        [Header("Progress")]
        [SerializeField] private List<MissionProgress> m_missionProgress = new List<MissionProgress>();
        [SerializeField] private List<MissionInstance> m_activeMissions = new List<MissionInstance>();

        [Header("Daily Missions")]
        [SerializeField] private bool m_generateDailyMissions = true;
        [SerializeField] private int m_dailyMissionCount = 3;

        private long m_lastDailyReset;

        public event System.Action<MissionInstance> OnMissionStarted;
        public event System.Action<MissionInstance> OnMissionCompleted;
        public event System.Action<MissionInstance, string> OnMissionFailed;
        public event System.Action<string> OnObjectiveCompleted;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadProgress();
            CheckDailyReset();
        }

        private void Start()
        {
            if (m_generateDailyMissions)
            {
                GenerateDailyMissions();
            }
        }

        private void LoadProgress()
        {
            int count = PlayerPrefs.GetInt("MissionProgressCount", 0);
            for (int i = 0; i < count; i++)
            {
                string json = PlayerPrefs.GetString($"MissionProgress_{i}", "");
                if (!string.IsNullOrEmpty(json))
                {
                    MissionProgress progress = JsonUtility.FromJson<MissionProgress>(json);
                    m_missionProgress.Add(progress);
                }
            }
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetInt("MissionProgressCount", m_missionProgress.Count);
            for (int i = 0; i < m_missionProgress.Count; i++)
            {
                string json = JsonUtility.ToJson(m_missionProgress[i]);
                PlayerPrefs.SetString($"MissionProgress_{i}", json);
            }
            PlayerPrefs.Save();
        }

        private void CheckDailyReset()
        {
            long now = System.DateTime.Now.Ticks;
            long dayTicks = System.TimeSpan.TicksPerDay;

            if (now - m_lastDailyReset > dayTicks)
            {
                ResetDailyMissions();
                m_lastDailyReset = now;
            }
        }

        private void ResetDailyMissions()
        {
            foreach (var progress in m_missionProgress)
            {
                var mission = GetMissionData(progress.missionId);
                if (mission != null && mission.category == MissionCategory.Daily)
                {
                    progress.status = MissionStatus.Available;
                    progress.progress = 0;
                    progress.currentObjective = 0;
                }
            }
            SaveProgress();
        }

        private void GenerateDailyMissions()
        {
            if (m_missionDatabase == null || m_missionDatabase.missions == null) return;

            List<MissionData> dailyMissions = new List<MissionData>();
            foreach (var mission in m_missionDatabase.missions)
            {
                if (mission.category == MissionCategory.Daily)
                {
                    dailyMissions.Add(mission);
                }
            }

            if (dailyMissions.Count == 0) return;

            for (int i = 0; i < m_dailyMissionCount && i < dailyMissions.Count; i++)
            {
                MissionData mission = dailyMissions[Random.Range(0, dailyMissions.Count)];
                if (!HasProgress(mission.missionId))
                {
                    AddMissionProgress(mission.missionId);
                }
            }
        }

        private bool HasProgress(string missionId)
        {
            return m_missionProgress.Exists(p => p.missionId == missionId);
        }

        private void AddMissionProgress(string missionId)
        {
            m_missionProgress.Add(new MissionProgress(missionId));
            SaveProgress();
        }

        public void StartMission(string missionId)
        {
            MissionData mission = GetMissionData(missionId);
            if (mission == null)
            {
                Debug.LogWarning($"[EnhancedMissionManager] Mission not found: {missionId}");
                return;
            }

            if (m_activeMissions.Count >= m_maxActiveMissions)
            {
                Debug.LogWarning("[EnhancedMissionManager] Max active missions reached");
                return;
            }

            MissionInstance instance = new MissionInstance(mission);
            m_activeMissions.Add(instance);

            MissionProgress progress = GetOrCreateProgress(missionId);
            progress.status = MissionStatus.Active;
            progress.startTime = System.DateTime.Now.Ticks;
            progress.attempts++;

            SaveProgress();

            OnMissionStarted?.Invoke(instance);

            if (m_showMissionNotifications)
            {
                ShowNotification($"Mission Started: {mission.missionName}");
            }

            Debug.Log($"[EnhancedMissionManager] Started: {mission.missionName}");
        }

        public void StartMission(MissionData mission)
        {
            StartMission(mission.missionId);
        }

        public void CompleteMission(string missionId)
        {
            MissionInstance mission = m_activeMissions.Find(m => m.Data.missionId == missionId);
            if (mission == null) return;

            MissionProgress progress = GetProgress(missionId);
            if (progress != null)
            {
                progress.status = MissionStatus.Completed;
            }

            OnMissionCompleted?.Invoke(mission);

            if (m_showMissionNotifications)
            {
                ShowNotification($"Mission Complete: {mission.Data.missionName}");
            }

            AwardRewards(mission.Data);

            m_activeMissions.Remove(mission);
            SaveProgress();

            Debug.Log($"[EnhancedMissionManager] Completed: {mission.Data.missionName}");
        }

        public void FailMission(string missionId, string reason = "")
        {
            MissionInstance mission = m_activeMissions.Find(m => m.Data.missionId == missionId);
            if (mission == null) return;

            MissionProgress progress = GetProgress(missionId);
            if (progress != null)
            {
                if (mission.Data.isRepeatable)
                {
                    progress.status = MissionStatus.Available;
                }
                else
                {
                    progress.status = MissionStatus.Failed;
                }
            }

            OnMissionFailed?.Invoke(mission, reason);

            if (m_showMissionNotifications)
            {
                ShowNotification($"Mission Failed: {mission.Data.missionName}");
            }

            m_activeMissions.Remove(mission);
            SaveProgress();

            Debug.Log($"[EnhancedMissionManager] Failed: {mission.Data.missionName} - {reason}");
        }

        private void AwardRewards(MissionData mission)
        {
            if (mission.currencyReward > 0)
            {
                int current = PlayerPrefs.GetInt("PlayerCredits", 0);
                PlayerPrefs.SetInt("PlayerCredits", current + mission.currencyReward);
            }
        }

        private void Update()
        {
            UpdateActiveMissions();
        }

        private void UpdateActiveMissions()
        {
            List<MissionInstance> toRemove = new List<MissionInstance>();

            foreach (var mission in m_activeMissions)
            {
                if (mission.Data.timeLimit > 0)
                {
                    mission.ElapsedTime += Time.deltaTime;
                    if (mission.ElapsedTime >= mission.Data.timeLimit)
                    {
                        FailMission(mission.Data.missionId, "Time limit exceeded");
                        continue;
                    }
                }

                UpdateMissionObjectives(mission);

                if (CheckMissionComplete(mission))
                {
                    CompleteMission(mission.Data.missionId);
                }
            }
        }

        private void UpdateMissionObjectives(MissionInstance mission)
        {
            foreach (var objective in mission.Objectives)
            {
                if (objective.IsComplete) continue;

                switch (objective.Type)
                {
                    case ObjectiveData.ObjectiveType.ReachLocation:
                        UpdateReachLocation(objective);
                        break;
                    case ObjectiveData.ObjectiveType.DriveDistance:
                        UpdateDriveDistance(objective);
                        break;
                    case ObjectiveData.ObjectiveType.CollectItems:
                        UpdateCollectItems(objective);
                        break;
                    case ObjectiveData.ObjectiveType.Drift:
                        UpdateDrift(objective);
                        break;
                    case ObjectiveData.ObjectiveType.Speed:
                        UpdateSpeed(objective);
                        break;
                }
            }
        }

        private void UpdateReachLocation(ObjectiveInstance objective)
        {
            var player = GetPlayerTransform();
            if (player == null) return;

            float dist = Vector3.Distance(player.position, objective.TargetPosition);
            if (dist <= objective.TargetDistance)
            {
                objective.IsComplete = true;
                OnObjectiveCompleted?.Invoke(objective.Description);
            }
        }

        private void UpdateDriveDistance(ObjectiveInstance objective)
        {
            var player = GetPlayerTransform();
            if (player == null) return;

            var physics = player.GetComponent<Vehicle.VehiclePhysics>();
            if (physics != null)
            {
                float speedMps = physics.CurrentSpeed / 3.6f;
                objective.CurrentProgress += speedMps * Time.deltaTime;

                if (objective.CurrentProgress >= objective.TargetCount)
                {
                    objective.IsComplete = true;
                    OnObjectiveCompleted?.Invoke(objective.Description);
                }
            }
        }

        private void UpdateCollectItems(ObjectiveInstance objective)
        {
            if (objective.CurrentProgress >= objective.TargetCount)
            {
                objective.IsComplete = true;
                OnObjectiveCompleted?.Invoke(objective.Description);
            }
        }

        private void UpdateDrift(ObjectiveInstance objective)
        {
            var player = GetPlayerTransform();
            if (player == null) return;

            var input = player.GetComponent<Vehicle.VehicleInput>();
            if (input != null && input.IsDrifting)
            {
                objective.CurrentProgress += Time.deltaTime;

                if (objective.CurrentProgress >= objective.TargetCount)
                {
                    objective.IsComplete = true;
                    OnObjectiveCompleted?.Invoke(objective.Description);
                }
            }
        }

        private void UpdateSpeed(ObjectiveInstance objective)
        {
            var player = GetPlayerTransform();
            if (player == null) return;

            var physics = player.GetComponent<Vehicle.VehiclePhysics>();
            if (physics != null && physics.CurrentSpeed >= objective.TargetCount)
            {
                objective.CurrentProgress += Time.deltaTime;

                if (objective.CurrentProgress >= 3f)
                {
                    objective.IsComplete = true;
                    OnObjectiveCompleted?.Invoke(objective.Description);
                }
            }
        }

        private bool CheckMissionComplete(MissionInstance mission)
        {
            foreach (var obj in mission.Objectives)
            {
                if (!obj.IsComplete && !obj.IsOptional)
                {
                    return false;
                }
            }
            return true;
        }

        private Transform GetPlayerTransform()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            return player?.transform;
        }

        private MissionData GetMissionData(string missionId)
        {
            if (m_missionDatabase == null || m_missionDatabase.missions == null) return null;

            foreach (var mission in m_missionDatabase.missions)
            {
                if (mission.missionId == missionId) return mission;
            }
            return null;
        }

        private MissionProgress GetOrCreateProgress(string missionId)
        {
            MissionProgress progress = GetProgress(missionId);
            if (progress == null)
            {
                progress = new MissionProgress(missionId);
                m_missionProgress.Add(progress);
            }
            return progress;
        }

        private MissionProgress GetProgress(string missionId)
        {
            return m_missionProgress.Find(p => p.missionId == missionId);
        }

        private void ShowNotification(string message)
        {
            Debug.Log($"[Notification] {message}");
        }

        public List<MissionInstance> GetAvailableMissions()
        {
            List<MissionInstance> available = new List<MissionInstance>();

            if (m_missionDatabase == null || m_missionDatabase.missions == null) return available;

            foreach (var mission in m_missionDatabase.missions)
            {
                MissionProgress progress = GetProgress(mission.missionId);
                if (progress == null || progress.status == MissionStatus.Available)
                {
                    available.Add(new MissionInstance(mission));
                }
            }

            return available;
        }

        public List<MissionInstance> GetActiveMissions() => m_activeMissions;
        public int GetCompletedCount() => m_missionProgress.FindAll(p => p.status == MissionStatus.Completed).Count;
    }

    [System.Serializable]
    public class MissionDatabase
    {
        public MissionData[] missions;
    }
}
