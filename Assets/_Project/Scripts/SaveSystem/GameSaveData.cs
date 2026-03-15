using UnityEngine;
using System;

namespace CarSimulator.SaveSystem
{
    [Serializable]
    public class GameSaveData
    {
        public const int CURRENT_VERSION = 1;

        [Header("Save Info")]
        public int version = CURRENT_VERSION;
        public string saveId;
        public string saveName;
        public DateTime createdAt;
        public DateTime lastPlayedAt;
        public float playTime;
        public string currentScene;

        [Header("Player State")]
        public PlayerData player;

        [Header("Mission Progress")]
        public MissionProgressData missions;

        [Header("World State")]
        public WorldStateData world;

        [Header("Settings")]
        public GameSettingsData settings;

        public GameSaveData()
        {
            version = CURRENT_VERSION;
            saveId = Guid.NewGuid().ToString();
            saveName = "New Save";
            createdAt = DateTime.Now;
            lastPlayedAt = DateTime.Now;
            playTime = 0f;
            currentScene = "OpenWorld_TestDistrict";

            player = new PlayerData();
            missions = new MissionProgressData();
            world = new WorldStateData();
            settings = new GameSettingsData();
        }

        public static GameSaveData CreateDefault()
        {
            return new GameSaveData();
        }

        public bool Validate()
        {
            if (version > CURRENT_VERSION)
            {
                Debug.LogWarning($"[GameSaveData] Save version {version} is newer than current {CURRENT_VERSION}");
                return false;
            }

            if (string.IsNullOrEmpty(saveId))
            {
                Debug.LogWarning("[GameSaveData] Invalid save ID");
                return false;
            }

            return true;
        }

        public void MigrateToCurrentVersion()
        {
            if (version < CURRENT_VERSION)
            {
                Debug.Log($"[GameSaveData] Migrating save from version {version} to {CURRENT_VERSION}");

                if (player == null) player = new PlayerData();
                if (missions == null) missions = new MissionProgressData();
                if (world == null) world = new WorldStateData();
                if (settings == null) settings = new GameSettingsData();

                version = CURRENT_VERSION;
            }
        }
    }

    [Serializable]
    public class PlayerData
    {
        public string currentVehicleId;
        public Vector3Data lastPosition;
        public QuaternionData lastRotation;
        public int currency;
        public int experience;
        public int level;
        public string lastCheckpointId;
        public string lastDistrictId;
        public Vector3Data spawnPosition;
        public QuaternionData spawnRotation;
        public List<string> unlockedVehicles;
        public List<string> unlockedCustomizations;
        public Dictionary<string, int> vehicleMileage;

        public PlayerData()
        {
            currentVehicleId = "default";
            lastPosition = new Vector3Data(0, 1, 0);
            lastRotation = new QuaternionData(0, 0, 0, 1);
            currency = 1000;
            experience = 0;
            level = 1;
            lastCheckpointId = "";
            lastDistrictId = "TestDistrict";
            spawnPosition = new Vector3Data(0, 1, 0);
            spawnRotation = new QuaternionData(0, 0, 0, 1);
            unlockedVehicles = new List<string> { "default" };
            unlockedCustomizations = new List<string>();
            vehicleMileage = new Dictionary<string, int>();
        }

        public void SetSpawnPoint(Vector3 position, Quaternion rotation)
        {
            spawnPosition = position;
            spawnRotation = rotation;
            lastPosition = position;
            lastRotation = rotation;
        }
    }

    [Serializable]
    public class MissionProgressData
    {
        public List<string> completedMissionIds;
        public List<string> activeMissionIds;
        public Dictionary<string, int> missionAttempts;
        public Dictionary<string, string> missionStageProgress;
        public Dictionary<string, bool> missionRewardsClaimed;
        public int totalMissionsCompleted;
        public int totalMissionsFailed;

        public MissionProgressData()
        {
            completedMissionIds = new List<string>();
            activeMissionIds = new List<string>();
            missionAttempts = new Dictionary<string, int>();
            missionStageProgress = new Dictionary<string, string>();
            missionRewardsClaimed = new Dictionary<string, bool>();
            totalMissionsCompleted = 0;
            totalMissionsFailed = 0;
        }

        public bool IsMissionCompleted(string missionId)
        {
            return completedMissionIds.Contains(missionId);
        }

        public bool IsMissionActive(string missionId)
        {
            return activeMissionIds.Contains(missionId);
        }

        public void CompleteMission(string missionId)
        {
            if (!completedMissionIds.Contains(missionId))
            {
                completedMissionIds.Add(missionId);
                totalMissionsCompleted++;
            }
            activeMissionIds.Remove(missionId);
        }

        public void StartMission(string missionId)
        {
            if (!activeMissionIds.Contains(missionId))
            {
                activeMissionIds.Add(missionId);
            }

            if (!missionAttempts.ContainsKey(missionId))
            {
                missionAttempts[missionId] = 0;
            }
            missionAttempts[missionId]++;
        }

        public void FailMission(string missionId)
        {
            activeMissionIds.Remove(missionId);
            totalMissionsFailed++;
        }
    }

    [Serializable]
    public class WorldStateData
    {
        public string currentDistrict;
        public List<string> unlockedDistricts;
        public List<string> visitedDistricts;
        public Dictionary<string, bool> destroyedObjects;
        public Dictionary<string, Vector3Data> customPositions;
        public Dictionary<string, string> buildingStates;
        public long lastSavedTime;

        public WorldStateData()
        {
            currentDistrict = "TestDistrict";
            unlockedDistricts = new List<string> { "TestDistrict" };
            visitedDistricts = new List<string> { "TestDistrict" };
            destroyedObjects = new Dictionary<string, bool>();
            customPositions = new Dictionary<string, Vector3Data>();
            buildingStates = new Dictionary<string, string>();
            lastSavedTime = System.DateTime.Now.Ticks;
        }

        public void MarkDistrictVisited(string districtId)
        {
            if (!visitedDistricts.Contains(districtId))
            {
                visitedDistricts.Add(districtId);
            }
        }

        public void UnlockDistrict(string districtId)
        {
            if (!unlockedDistricts.Contains(districtId))
            {
                unlockedDistricts.Add(districtId);
            }
        }
    }

    [Serializable]
    public class GameSettingsData
    {
        public GraphicsSettingsData graphics;
        public AudioSettingsData audio;
        public InputSettingsData input;
        public GameplaySettingsData gameplay;

        public GameSettingsData()
        {
            graphics = new GraphicsSettingsData();
            audio = new AudioSettingsData();
            input = new InputSettingsData();
            gameplay = new GameplaySettingsData();
        }

        public static GameSettingsData CreateFromCurrent()
        {
            GameSettingsData data = new GameSettingsData();

            data.graphics.qualityLevel = QualitySettings.GetQualityLevel();
            data.graphics.fullscreen = Screen.fullScreen;
            data.graphics.fov = PlayerPrefs.GetFloat("FOV", 60f);

            data.audio.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            data.audio.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            data.audio.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

            data.input.invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
            data.input.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);

            return data;
        }

        public void ApplyToCurrent()
        {
            QualitySettings.SetQualityLevel(graphics.qualityLevel, true);
            Screen.fullScreen = graphics.fullscreen;

            PlayerPrefs.SetFloat("FOV", graphics.fov);
            PlayerPrefs.SetFloat("MasterVolume", audio.masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", audio.musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", audio.sfxVolume);
            PlayerPrefs.SetInt("InvertY", input.invertY ? 1 : 0);
            PlayerPrefs.SetFloat("Sensitivity", input.sensitivity);
            PlayerPrefs.Save();
        }
    }

    [Serializable]
    public class GraphicsSettingsData
    {
        public int qualityLevel = 2;
        public int resolutionWidth = 1920;
        public int resolutionHeight = 1080;
        public bool fullscreen = true;
        public bool vsync = true;
        public float fov = 60f;
        public float viewDistance = 500f;
        public bool shadows = true;
        public int antiAliasing = 2;

        public GraphicsSettingsData() { }
    }

    [Serializable]
    public class AudioSettingsData
    {
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public float dialogueVolume = 1f;
        public bool muteAudio = false;

        public AudioSettingsData() { }
    }

    [Serializable]
    public class InputSettingsData
    {
        public bool invertY = false;
        public float sensitivity = 1f;
        public bool controllerEnabled = false;
        public float controllerSensitivity = 1f;

        public InputSettingsData() { }
    }

    [Serializable]
    public class GameplaySettingsData
    {
        public bool tutorialsEnabled = true;
        public bool miniMapEnabled = true;
        public bool damageEnabled = true;
        public bool trafficEnabled = true;
        public float gameSpeed = 1f;

        public GameplaySettingsData() { }
    }

    [Serializable]
    public struct Vector3Data
    {
        public float x, y, z;

        public Vector3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator UnityEngine.Vector3(Vector3Data v) => new UnityEngine.Vector3(v.x, v.y, v.z);
        public static implicit operator Vector3Data(UnityEngine.Vector3 v) => new Vector3Data(v.x, v.y, v.z);
    }

    [Serializable]
    public struct QuaternionData
    {
        public float x, y, z, w;

        public QuaternionData(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator UnityEngine.Quaternion(QuaternionData q) => new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        public static implicit operator QuaternionData(UnityEngine.Quaternion q) => new QuaternionData(q.x, q.y, q.z, q.w);
    }
}
