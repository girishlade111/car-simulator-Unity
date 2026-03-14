using UnityEngine;

namespace CarSimulator.Missions
{
    [CreateAssetMenu(fileName = "MissionData", menuName = "CarSimulator/Mission")]
    public class MissionData : ScriptableObject
    {
        [Header("Info")]
        public string missionId;
        public string missionName;
        [TextArea] public string description;

        [Header("Category")]
        public MissionCategory category = MissionCategory.Side;

        [Header("Type")]
        public MissionType type;

        [Header("Difficulty")]
        [Range(1, 5)] public int difficulty = 1;
        public string difficultyLabel = "Easy";

        [Header("Objectives")]
        public ObjectiveData[] objectives;

        [Header("Rewards")]
        public int currencyReward;
        public int experienceReward;
        public string[] unlockables;

        [Header("Settings")]
        public bool isRepeatable;
        public float timeLimit;
        public float recommendedSpeed;

        [Header("Visibility")]
        public bool isHidden;
        public string prerequisiteMissionId;

        public enum MissionType
        {
            Delivery,
            TimeTrial,
            Collection,
            Exploration,
            Race,
            Chase,
            Survival,
            Challenge
        }

        public enum MissionCategory
        {
            Main,
            Side,
            Daily,
            Weekly,
            Challenge
        }
    }

    [System.Serializable]
    public class ObjectiveData
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public Vector3 targetPosition;
        public float targetDistance = 5f;
        public int targetCount = 1;
        public bool isOptional;
        public GameObject targetObject;
        public string targetTag;

        public enum ObjectiveType
        {
            ReachLocation,
            CollectItems,
            DriveDistance,
            TimeTrial,
            Drift,
            Speed,
            FollowTarget,
            DestroyTargets,
            Survive
        }
    }

    public class MissionInstance
    {
        public MissionData Data { get; }
        public float ElapsedTime { get; set; }
        public bool IsComplete { get; private set; }
        public ObjectiveInstance[] Objectives { get; }

        public MissionInstance(MissionData data)
        {
            Data = data;
            ElapsedTime = 0;
            IsComplete = false;

            if (data.objectives != null)
            {
                Objectives = new ObjectiveInstance[data.objectives.Length];
                for (int i = 0; i < data.objectives.Length; i++)
                {
                    Objectives[i] = new ObjectiveInstance(data.objectives[i]);
                }
            }
        }

        public float GetProgress()
        {
            if (Objectives == null || Objectives.Length == 0) return 0;

            float total = 0;
            foreach (var obj in Objectives)
            {
                total += obj.GetProgress();
            }
            return total / Objectives.Length;
        }

        public string GetStatusText()
        {
            if (Data.timeLimit > 0)
            {
                float remaining = Data.timeLimit - ElapsedTime;
                int mins = Mathf.FloorToInt(remaining / 60);
                int secs = Mathf.FloorToInt(remaining % 60);
                return $"{mins:00}:{secs:00}";
            }
            return "";
        }
    }

    [System.Serializable]
    public class ObjectiveInstance
    {
        public string ObjectiveId { get; }
        public string Description { get; }
        public ObjectiveData.ObjectiveType Type { get; }
        public Vector3 TargetPosition { get; }
        public float TargetDistance { get; }
        public int TargetCount { get; }
        public bool IsOptional { get; }
        public bool IsComplete { get; set; }
        public float CurrentProgress { get; set; }

        public ObjectiveInstance(ObjectiveData data)
        {
            ObjectiveId = data.objectiveId;
            Description = data.description;
            Type = data.type;
            TargetPosition = data.targetPosition;
            TargetDistance = data.targetDistance;
            TargetCount = data.targetCount;
            IsOptional = data.isOptional;
            IsComplete = false;
            CurrentProgress = 0;
        }

        public float GetProgress()
        {
            if (IsComplete) return 1f;
            if (TargetCount <= 0) return 0;
            return Mathf.Clamp01(CurrentProgress / TargetCount);
        }
    }
}
