using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CarSimulator.Missions
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        [Header("Mission Data")]
        [SerializeField] private DriveMissionData m_currentMission;
        [SerializeField] private int m_currentStageIndex;
        [SerializeField] private int m_currentObjectiveIndex;

        [Header("Mission State")]
        [SerializeField] private bool m_isMissionActive;
        [SerializeField] private bool m_isMissionComplete;
        [SerializeField] private float m_missionTimer;
        [SerializeField] private float m_timeRemaining;

        [Header("Player Reference")]
        [SerializeField] private Transform m_playerTransform;
        [SerializeField] private VehicleController m_vehicle;

        [Header("UI")]
        [SerializeField] private bool m_showObjectives = true;
        [SerializeField] private ObjectiveSystem m_objectiveUI;

        [Header("Triggers")]
        [SerializeField] private List<MissionTrigger> m_registeredTriggers = new List<MissionTrigger>();

        [Header("Cached References")]
        [SerializeField] private World.ApartmentEntrance[] m_cachedApartments;
        [SerializeField] private float m_apartmentCacheRefreshInterval = 30f;
        [SerializeField] private float m_lastApartmentCacheTime;

        public event System.Action<DriveMissionData> OnMissionStarted;
        public event System.Action<DriveMissionData> OnMissionCompleted;
        public event System.Action<string> OnMissionFailed;
        public event System.Action<string> OnObjectiveCompleted;

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
            FindPlayer();
            InitializeObjectiveUI();
            RefreshApartmentCache();
        }

        private void Update()
        {
            if (!m_isMissionActive) return;

            if (Time.time - m_lastApartmentCacheTime > m_apartmentCacheRefreshInterval)
            {
                RefreshApartmentCache();
            }

            UpdateMission();
            CheckTriggers();
        }

        private void RefreshApartmentCache()
        {
            m_cachedApartments = FindObjectsOfType<World.ApartmentEntrance>();
            m_lastApartmentCacheTime = Time.time;
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
                m_vehicle = player.GetComponent<VehicleController>();
            }
        }

        private void InitializeObjectiveUI()
        {
            if (m_objectiveUI == null)
            {
                m_objectiveUI = ObjectiveSystem.Instance;
            }
        }

        public void StartMission(DriveMissionData mission)
        {
            if (mission == null)
            {
                Debug.LogWarning("[MissionManager] No mission data provided");
                return;
            }

            m_currentMission = mission;
            m_currentStageIndex = 0;
            m_currentObjectiveIndex = 0;
            m_isMissionActive = true;
            m_isMissionComplete = false;
            m_missionTimer = 0f;
            m_timeRemaining = mission.timeLimit;

            Debug.Log($"[MissionManager] Started mission: {mission.missionName}");
            OnMissionStarted?.Invoke(mission);

            UpdateObjectiveUI();
            ShowMissionNotification($"Mission Started: {mission.missionName}");
        }

        public void StartDefaultMission()
        {
            DriveMissionData defaultMission = DriveMissionData.CreateDefault();
            StartMission(defaultMission);
        }

        private void UpdateMission()
        {
            m_missionTimer += Time.deltaTime;

            if (m_currentMission != null && m_currentMission.timeLimit > 0)
            {
                m_timeRemaining -= Time.deltaTime;
                if (m_timeRemaining <= 0)
                {
                    FailMission("Time limit exceeded!");
                    return;
                }
            }

            CheckCurrentObjective();
        }

        private void CheckCurrentObjective()
        {
            if (m_currentMission == null || !m_isMissionActive) return;

            var stages = m_currentMission.stages;
            if (m_currentStageIndex >= stages.Count) return;

            var stage = stages[m_currentStageIndex];
            if (m_currentObjectiveIndex >= stage.objectives.Count) return;

            var objective = stage.objectives[m_currentObjectiveIndex];

            switch (objective.type)
            {
                case MissionObjectiveType.DriveToLocation:
                    CheckDriveToLocation(objective);
                    break;
                case MissionObjectiveType.ParkVehicle:
                    CheckParking(objective);
                    break;
                case MissionObjectiveType.EnterBuilding:
                    CheckEnterBuilding(objective);
                    break;
                case MissionObjectiveType.ExitBuilding:
                    CheckExitBuilding(objective);
                    break;
            }
        }

        private void CheckDriveToLocation(MissionObjective objective)
        {
            if (m_playerTransform == null) return;

            float distance = Vector3.Distance(m_playerTransform.position, objective.targetPosition);
            if (distance <= objective.targetRadius)
            {
                CompleteObjective(objective);
            }
        }

        private void CheckParking(MissionObjective objective)
        {
            if (ParkingManager.Instance != null)
            {
                if (ParkingManager.Instance.IsPlayerParked())
                {
                    CompleteObjective(objective);
                }
            }
        }

        private void CheckEnterBuilding(MissionObjective objective)
        {
            var apartment = GetApartmentAtPosition();
            if (apartment != null && apartment.IsPlayerInside())
            {
                CompleteObjective(objective);
            }
        }

        private void CheckExitBuilding(MissionObjective objective)
        {
            var apartment = GetApartmentAtPosition();
            if (apartment != null && !apartment.IsPlayerInside())
            {
                CompleteObjective(objective);
            }
        }

        private World.ApartmentEntrance GetApartmentAtPosition()
        {
            if (m_cachedApartments == null || m_cachedApartments.Length == 0)
            {
                RefreshApartmentCache();
            }

            foreach (var apt in m_cachedApartments)
            {
                if (Vector3.Distance(m_playerTransform.position, apt.transform.position) < 10f)
                {
                    return apt;
                }
            }
            return null;
        }

        public void CompleteObjective(string missionId, string objectiveId)
        {
            if (m_currentMission == null || m_currentMission.missionId != missionId) return;

            var stages = m_currentMission.stages;
            foreach (var stage in stages)
            {
                foreach (var objective in stage.objectives)
                {
                    if (objective.objectiveId == objectiveId && !objective.isCompleted)
                    {
                        CompleteObjective(objective);
                        return;
                    }
                }
            }
        }

        private void CompleteObjective(MissionObjective objective)
        {
            if (objective.isCompleted) return;

            objective.isCompleted = true;
            Debug.Log($"[MissionManager] Objective completed: {objective.description}");

            ShowMissionNotification($"Objective Complete: {objective.description}");
            OnObjectiveCompleted?.Invoke(objective.objectiveId);

            m_currentObjectiveIndex++;

            if (m_currentObjectiveIndex >= m_currentMission.stages[m_currentStageIndex].objectives.Count)
            {
                CompleteStage();
            }
            else
            {
                UpdateObjectiveUI();
            }
        }

        private void CompleteStage()
        {
            var stage = m_currentMission.stages[m_currentStageIndex];
            stage.isCompleted = true;

            Debug.Log($"[MissionManager] Stage completed: {stage.stageName}");

            m_currentStageIndex++;
            m_currentObjectiveIndex = 0;

            if (m_currentStageIndex >= m_currentMission.stages.Count)
            {
                CompleteMission();
            }
            else
            {
                UpdateObjectiveUI();
            }
        }

        private void CompleteMission()
        {
            m_isMissionActive = false;
            m_isMissionComplete = true;

            int credits = PlayerPrefs.GetInt("PlayerCredits", 0);
            PlayerPrefs.SetInt("PlayerCredits", credits + m_currentMission.creditsReward);
            PlayerPrefs.Save();

            Debug.Log($"[MissionManager] Mission completed! Reward: ${m_currentMission.creditsReward}");

            ShowMissionNotification($"Mission Complete! +${m_currentMission.creditsReward}");
            OnMissionCompleted?.Invoke(m_currentMission);
        }

        private void FailMission(string reason)
        {
            m_isMissionActive = false;

            Debug.Log($"[MissionManager] Mission failed: {reason}");
            ShowMissionNotification($"Mission Failed: {reason}");
            OnMissionFailed?.Invoke(reason);
        }

        public void ResetMission()
        {
            if (m_currentMission != null)
            {
                StartMission(m_currentMission);
            }
        }

        private void UpdateObjectiveUI()
        {
            if (m_objectiveUI == null || !m_showObjectives) return;

            if (m_currentMission == null) return;

            var stages = m_currentMission.stages;
            if (m_currentStageIndex >= stages.Count) return;

            var stage = stages[m_currentStageIndex];
            string mainText = stage.stageName;

            if (m_currentObjectiveIndex < stage.objectives.Count)
            {
                var objective = stage.objectives[m_currentObjectiveIndex];
                m_objectiveUI.SetObjective(mainText, objective.description);

                if (objective.type == MissionObjectiveType.DriveToLocation)
                {
                    float progress = 1f - (Vector3.Distance(m_playerTransform.position, objective.targetPosition) / 100f);
                    m_objectiveUI.SetProgress(Mathf.Clamp01(progress));
                }
            }
        }

        private void ShowMissionNotification(string message)
        {
            var notification = UI.NotificationSystem.Instance;
            if (notification != null)
            {
                notification.ShowSuccess(message);
            }
        }

        public void RegisterTrigger(MissionTrigger trigger)
        {
            if (!m_registeredTriggers.Contains(trigger))
            {
                m_registeredTriggers.Add(trigger);
            }
        }

        public void UnregisterTrigger(MissionTrigger trigger)
        {
            m_registeredTriggers.Remove(trigger);
        }

        private void CheckTriggers()
        {
            if (m_playerTransform == null) return;

            foreach (var trigger in m_registeredTriggers)
            {
                if (trigger != null && trigger.CheckTrigger(m_playerTransform.gameObject))
                {
                    trigger.Trigger();
                }
            }
        }

        public bool IsMissionActive() => m_isMissionActive;
        public bool IsMissionComplete() => m_isMissionComplete;
        public DriveMissionData GetCurrentMission() => m_currentMission;
    }
}
