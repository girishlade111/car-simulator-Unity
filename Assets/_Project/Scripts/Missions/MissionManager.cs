using System.Collections.Generic;
using UnityEngine;
using CarSimulator.SaveSystem;

namespace CarSimulator.Missions
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        [Header("Missions")]
        [SerializeField] private MissionData[] m_availableMissions;
        [SerializeField] private MissionInstance m_activeMission;

        [Header("Progress")]
        [SerializeField] private List<string> m_completedMissions = new List<string>();

        public bool IsMissionActive => m_activeMission != null;
        public MissionInstance ActiveMission => m_activeMission;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartMission(MissionData mission)
        {
            if (m_activeMission != null)
            {
                Debug.LogWarning("[MissionManager] Mission already active!");
                return;
            }

            m_activeMission = new MissionInstance(mission);
            Debug.Log($"[MissionManager] Started: {mission.missionName}");
        }

        public void StartMission(string missionId)
        {
            MissionData mission = GetMissionById(missionId);
            if (mission != null)
            {
                StartMission(mission);
            }
        }

        private void Update()
        {
            if (m_activeMission != null)
            {
                UpdateMission();
            }
        }

        private void UpdateMission()
        {
            if (m_activeMission.Data.timeLimit > 0)
            {
                m_activeMission.ElapsedTime += Time.deltaTime;
                if (m_activeMission.ElapsedTime >= m_activeMission.Data.timeLimit)
                {
                    FailMission("Time limit exceeded");
                    return;
                }
            }

            CheckObjectives();

            if (m_activeMission.IsComplete)
            {
                CompleteMission();
            }
        }

        private void CheckObjectives()
        {
            if (m_activeMission.Data.objectives == null) return;

            foreach (var objective in m_activeMission.Data.objectives)
            {
                var progress = m_activeMission.GetOrCreateProgress(objective.objectiveId);

                switch (objective.type)
                {
                    case ObjectiveData.ObjectiveType.ReachLocation:
                        CheckReachLocation(objective, progress);
                        break;
                    case ObjectiveData.ObjectiveType.DriveDistance:
                        CheckDriveDistance(objective, progress);
                        break;
                }
            }
        }

        private void CheckReachLocation(ObjectiveData objective, MissionProgress progress)
        {
            if (progress.IsComplete) return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            float distance = Vector3.Distance(player.transform.position, objective.targetPosition);
            if (distance <= objective.targetDistance)
            {
                progress.CurrentCount = 1;
                progress.IsComplete = true;
            }
        }

        private void CheckDriveDistance(ObjectiveData objective, MissionProgress progress)
        {
            if (progress.IsComplete) return;

            var vehicle = Object.FindObjectOfType<CarSimulator.Vehicle.VehiclePhysics>();
            if (vehicle != null)
            {
                float distance = vehicle.CurrentSpeed * Time.deltaTime / 3600f;
                progress.CurrentCount += distance;

                if (progress.CurrentCount >= objective.targetCount)
                {
                    progress.IsComplete = true;
                }
            }
        }

        public void CompleteMission()
        {
            if (m_activeMission == null) return;

            m_completedMissions.Add(m_activeMission.Data.missionId);
            Debug.Log($"[MissionManager] Completed: {m_activeMission.Data.missionName}");

            if (m_activeMission.Data.currencyReward > 0)
            {
                // Add currency to profile
            }

            m_activeMission = null;
        }

        public void FailMission(string reason)
        {
            if (m_activeMission == null) return;

            Debug.Log($"[MissionManager] Failed: {m_activeMission.Data.missionName} - {reason}");
            m_activeMission = null;
        }

        public void AbandonMission()
        {
            m_activeMission = null;
        }

        public MissionData GetMissionById(string missionId)
        {
            if (m_availableMissions == null) return null;

            foreach (var mission in m_availableMissions)
            {
                if (mission.missionId == missionId) return mission;
            }
            return null;
        }

        public bool IsMissionCompleted(string missionId)
        {
            return m_completedMissions.Contains(missionId);
        }

        public MissionData[] GetAvailableMissions()
        {
            return m_availableMissions;
        }
    }

    public class MissionInstance
    {
        public MissionData Data { get; }
        public Dictionary<string, MissionProgress> Progress { get; }
        public bool IsComplete { get; private set; }
        public float ElapsedTime { get; set; }

        public MissionInstance(MissionData data)
        {
            Data = data;
            Progress = new Dictionary<string, MissionProgress>();
            IsComplete = false;
            ElapsedTime = 0f;
        }

        public MissionProgress GetOrCreateProgress(string objectiveId)
        {
            if (!Progress.ContainsKey(objectiveId))
            {
                Progress[objectiveId] = new MissionProgress();
            }
            return Progress[objectiveId];
        }
    }

    public class MissionProgress
    {
        public float CurrentCount { get; set; }
        public bool IsComplete { get; set; }
    }
}
