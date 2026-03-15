using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Missions
{
    public enum MissionObjectiveType
    {
        DriveToLocation,
        ParkVehicle,
        EnterBuilding,
        ExitBuilding,
        CollectItem,
        TalkToNPC,
        DefeatTarget,
        SurviveTime,
        DriveDistance
    }

    [System.Serializable]
    public class MissionObjective
    {
        public string objectiveId;
        public string description;
        public MissionObjectiveType type;
        
        [Header("Target")]
        public Vector3 targetPosition;
        public float targetRadius = 5f;
        public GameObject targetObject;
        public string targetTag;
        
        [Header("Requirements")]
        public int requiredCount = 1;
        public float timeLimit;
        public bool isOptional;
        
        [Header("Progress")]
        public int currentCount;
        public bool isCompleted;
        public bool isFailed;

        public MissionObjective(string id, string desc, MissionObjectiveType objType)
        {
            objectiveId = id;
            description = desc;
            type = objType;
            currentCount = 0;
            isCompleted = false;
            isFailed = false;
        }

        public float GetProgress()
        {
            if (requiredCount <= 0) return 1f;
            return (float)currentCount / requiredCount;
        }

        public bool CheckCompletion()
        {
            if (isCompleted || isFailed) return false;
            
            if (currentCount >= requiredCount)
            {
                isCompleted = true;
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class MissionStage
    {
        public string stageId;
        public string stageName;
        public List<MissionObjective> objectives = new List<MissionObjective>();
        public bool isCompleted;
    }

    [CreateAssetMenu(fileName = "DriveMission", menuName = "CarSimulator/Missions/Drive Mission")]
    public class DriveMissionData : ScriptableObject
    {
        [Header("Info")]
        public string missionId;
        public string missionName;
        [TextArea] public string description;

        [Header("Category")]
        public MissionCategory category = MissionCategory.Main;

        [Header("Difficulty")]
        [Range(1, 5)] public int difficulty = 1;

        [Header("Rewards")]
        public int creditsReward = 500;
        public int experienceReward = 100;

        [Header("Stages")]
        public List<MissionStage> stages = new List<MissionStage>();

        [Header("Settings")]
        public bool isRepeatable;
        public float timeLimit;

        public static DriveMissionData CreateDefault()
        {
            DriveMissionData mission = CreateInstance<DriveMissionData>();
            mission.missionId = "drive_mission_1";
            mission.missionName = "Apartment Hunt";
            mission.description = "Drive to the apartment, park, and explore the building.";
            mission.category = MissionCategory.Main;
            mission.difficulty = 1;
            mission.creditsReward = 500;
            mission.experienceReward = 100;

            mission.stages = new List<MissionStage>
            {
                CreateDriveToApartmentStage(),
                CreateParkStage(),
                CreateEnterApartmentStage(),
                CreateExitApartmentStage(),
                CreateReturnStage()
            };

            return mission;
        }

        private static MissionStage CreateDriveToApartmentStage()
        {
            MissionStage stage = new MissionStage
            {
                stageId = "drive_to_apartment",
                stageName = "Drive to Apartment"
            };

            stage.objectives.Add(new MissionObjective(
                "drive_1",
                "Drive to the apartment complex",
                MissionObjectiveType.DriveToLocation
            ));

            return stage;
        }

        private static MissionStage CreateParkStage()
        {
            MissionStage stage = new MissionStage
            {
                stageId = "park_vehicle",
                stageName = "Park Vehicle"
            };

            stage.objectives.Add(new MissionObjective(
                "park_1",
                "Park in the designated parking zone",
                MissionObjectiveType.ParkVehicle
            ));

            return stage;
        }

        private static MissionStage CreateEnterApartmentStage()
        {
            MissionStage stage = new MissionStage
            {
                stageId = "enter_apartment",
                stageName = "Enter Apartment"
            };

            stage.objectives.Add(new MissionObjective(
                "enter_1",
                "Enter the apartment building",
                MissionObjectiveType.EnterBuilding
            ));

            return stage;
        }

        private static MissionStage CreateExitApartmentStage()
        {
            MissionStage stage = new MissionStage
            {
                stageId = "exit_apartment",
                stageName = "Exit Apartment"
            };

            stage.objectives.Add(new MissionObjective(
                "exit_1",
                "Exit the apartment building",
                MissionObjectiveType.ExitBuilding
            ));

            return stage;
        }

        private static MissionStage CreateReturnStage()
        {
            MissionStage stage = new MissionStage
            {
                stageId = "return_home",
                stageName = "Return Home"
            };

            stage.objectives.Add(new MissionObjective(
                "return_1",
                "Drive back to the starting point",
                MissionObjectiveType.DriveToLocation
            ));

            return stage;
        }
    }
}
