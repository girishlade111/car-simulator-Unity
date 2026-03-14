using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Mission Database")]
    [SerializeField] private MissionData[] m_availableMissions;

    [Header("Active Mission")]
    [SerializeField] private MissionInstance m_activeMission;

    [Header("Progress")]
    [SerializeField] private List<string> m_completedMissions;
    [SerializeField] private List<string> m_failedMissions;

    private bool m_isMissionActive;
    private float m_missionTimer;

    public bool IsMissionActive => m_isMissionActive;
    public MissionInstance ActiveMission => m_activeMission;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        m_completedMissions = new List<string>();
        m_failedMissions = new List<string>();
    }

    private void Update()
    {
        if (m_isMissionActive && m_activeMission != null)
        {
            UpdateMission();
        }
    }

    public void StartMission(MissionData missionData)
    {
        if (m_isMissionActive)
        {
            Debug.LogWarning("[MissionManager] Mission already active!");
            return;
        }

        m_activeMission = new MissionInstance(missionData);
        m_isMissionActive = true;
        m_missionTimer = 0f;

        Debug.Log($"[MissionManager] Started mission: {missionData.MissionName}");
        MissionEvents.OnMissionStarted?.Invoke(missionData);
    }

    public void StartMission(string missionId)
    {
        MissionData mission = GetMissionById(missionId);
        if (mission != null)
        {
            StartMission(mission);
        }
        else
        {
            Debug.LogWarning($"[MissionManager] Mission not found: {missionId}");
        }
    }

    private void UpdateMission()
    {
        if (m_activeMission == null) return;

        m_missionTimer += Time.deltaTime;

        if (m_activeMission.Data.TimeLimit > 0 && m_missionTimer >= m_activeMission.Data.TimeLimit)
        {
            FailMission("Time limit exceeded");
            return;
        }

        CheckObjectives();

        if (m_activeMission.IsComplete)
        {
            CompleteMission();
        }
    }

    private void CheckObjectives()
    {
        if (m_activeMission == null || m_activeMission.Data.Objectives == null) return;

        for (int i = 0; i < m_activeMission.Data.Objectives.Length; i++)
        {
            var objective = m_activeMission.Data.Objectives[i];
            var progress = m_activeMission.GetOrCreateProgress(objective.objectiveId);

            switch (objective.type)
            {
                case ObjectiveData.ObjectiveType.ReachLocation:
                    CheckReachLocation(objective, progress);
                    break;
                case ObjectiveData.ObjectiveType.DriveDistance:
                    CheckDriveDistance(objective, progress);
                    break;
                case ObjectiveData.ObjectiveType.Collection:
                case ObjectiveData.ObjectiveType.DestroyTargets:
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
            Debug.Log($"[MissionManager] Objective complete: {objective.description}");
            MissionEvents.OnObjectiveCompleted?.Invoke(objective);
        }
    }

    private void CheckDriveDistance(ObjectiveData objective, MissionProgress progress)
    {
        if (progress.IsComplete) return;

        var vehicle = Object.FindObjectOfType<VehicleController>();
        if (vehicle != null)
        {
            float distanceDriven = vehicle.CurrentSpeed * Time.deltaTime / 3600f;
            progress.CurrentCount += distanceDriven;

            if (progress.CurrentCount >= objective.targetCount)
            {
                progress.IsComplete = true;
                MissionEvents.OnObjectiveCompleted?.Invoke(objective);
            }
        }
    }

    public void CompleteMission()
    {
        if (m_activeMission == null) return;

        string missionId = m_activeMission.Data.MissionId;
        m_completedMissions.Add(missionId);
        m_isMissionActive = false;

        Debug.Log($"[MissionManager] Mission completed: {m_activeMission.Data.MissionName}");
        MissionEvents.OnMissionCompleted?.Invoke(m_activeMission.Data);

        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.AddCurrency(m_activeMission.Data.CurrencyReward);
        }

        m_activeMission = null;
    }

    public void FailMission(string reason)
    {
        if (m_activeMission == null) return;

        string missionId = m_activeMission.Data.MissionId;
        m_failedMissions.Add(missionId);
        m_isMissionActive = false;

        Debug.Log($"[MissionManager] Mission failed: {m_activeMission.Data.MissionName} - {reason}");
        MissionEvents.OnMissionFailed?.Invoke(m_activeMission.Data, reason);

        m_activeMission = null;
    }

    public void AbortMission()
    {
        if (m_activeMission != null)
        {
            FailMission("Aborted by player");
        }
    }

    public MissionData[] GetAvailableMissions()
    {
        return m_availableMissions;
    }

    public MissionData GetMissionById(string missionId)
    {
        if (m_availableMissions == null) return null;

        for (int i = 0; i < m_availableMissions.Length; i++)
        {
            if (m_availableMissions[i].MissionId == missionId)
            {
                return m_availableMissions[i];
            }
        }
        return null;
    }

    public bool IsMissionCompleted(string missionId)
    {
        return m_completedMissions.Contains(missionId);
    }

    public bool CanStartMission(string missionId)
    {
        if (m_isMissionActive) return false;
        if (IsMissionCompleted(missionId))
        {
            var mission = GetMissionById(missionId);
            return mission != null && mission.IsRepeatable;
        }
        return true;
    }
}

public class MissionInstance
{
    public MissionData Data { get; private set; }
    public Dictionary<string, MissionProgress> Progress { get; private set; }
    public bool IsComplete { get; private set; }

    public MissionInstance(MissionData data)
    {
        Data = data;
        Progress = new Dictionary<string, MissionProgress>();
        IsComplete = false;

        if (Data.Objectives != null)
        {
            bool allComplete = true;
            for (int i = 0; i < Data.Objectives.Length; i++)
            {
                if (!Data.Objectives[i].isOptional)
                {
                    allComplete = false;
                    break;
                }
            }
            IsComplete = allComplete;
        }
    }

    public MissionProgress GetOrCreateProgress(string objectiveId)
    {
        if (!Progress.ContainsKey(objectiveId))
        {
            Progress[objectiveId] = new MissionProgress();
        }
        return Progress[objectiveId];
    }

    public void UpdateProgress(string objectiveId, int count)
    {
        var progress = GetOrCreateProgress(objectiveId);
        progress.CurrentCount = count;

        if (Data.Objectives != null)
        {
            for (int i = 0; i < Data.Objectives.Length; i++)
            {
                if (Data.Objectives[i].objectiveId == objectiveId)
                {
                    if (progress.CurrentCount >= Data.Objectives[i].targetCount)
                    {
                        progress.IsComplete = true;
                    }
                    break;
                }
            }
        }

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (Data.Objectives == null) return;

        bool allComplete = true;
        for (int i = 0; i < Data.Objectives.Length; i++)
        {
            var objective = Data.Objectives[i];
            if (!objective.isOptional)
            {
                var progress = GetOrCreateProgress(objective.objectiveId);
                if (!progress.IsComplete)
                {
                    allComplete = false;
                    break;
                }
            }
        }
        IsComplete = allComplete;
    }
}

public class MissionProgress
{
    public int CurrentCount { get; set; }
    public bool IsComplete { get; set; }
}

public static class MissionEvents
{
    public static System.Action<MissionData> OnMissionStarted;
    public static System.Action<MissionData> OnMissionCompleted;
    public static System.Action<MissionData, string> OnMissionFailed;
    public static System.Action<ObjectiveData> OnObjectiveCompleted;
}
