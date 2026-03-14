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

        [Header("Type")]
        public MissionType type;

        [Header("Objectives")]
        public ObjectiveData[] objectives;

        [Header("Rewards")]
        public int currencyReward;
        public string[] unlockables;

        [Header("Settings")]
        public bool isRepeatable;
        public float timeLimit;

        public enum MissionType
        {
            Delivery,
            TimeTrial,
            Collection,
            Exploration,
            Race
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

        public enum ObjectiveType
        {
            ReachLocation,
            CollectItems,
            DriveDistance,
            TimeTrial
        }
    }
}
