using System;
using System.Collections.Generic;

namespace CarSimulator.SaveSystem
{

[Serializable]
public class GameSaveData
{
    public string saveId;
    public string saveName;
    public DateTime createdAt;
    public DateTime lastPlayedAt;
    public float playTime;
    public string currentScene;

    public PlayerData player;
    public MissionProgressData missions;
    public WorldStateData world;

    public GameSaveData()
    {
        saveId = Guid.NewGuid().ToString();
        createdAt = DateTime.Now;
        lastPlayedAt = DateTime.Now;
        playTime = 0f;
        currentScene = "OpenWorld_TestDistrict";

        player = new PlayerData();
        missions = new MissionProgressData();
        world = new WorldStateData();
    }
}

[Serializable]
public class PlayerData
{
    public string currentVehicleId;
    public Vector3Data lastPosition;
    public QuaternionData lastRotation;
    public int currency;
    public List<string> unlockedVehicles;
    public List<string> unlockedCustomizations;

    public PlayerData()
    {
        currentVehicleId = "default";
        lastPosition = new Vector3Data(0, 1, 0);
        lastRotation = new QuaternionData(0, 0, 0, 1);
        currency = 0;
        unlockedVehicles = new List<string> { "default" };
        unlockedCustomizations = new List<string>();
    }
}

[Serializable]
public class MissionProgressData
{
    public List<string> completedMissions;
    public List<string> activeMissions;
    public Dictionary<string, float> missionTimers;

    public MissionProgressData()
    {
        completedMissions = new List<string>();
        activeMissions = new List<string>();
        missionTimers = new Dictionary<string, float>();
    }
}

[Serializable]
public class WorldStateData
{
    public string currentDistrict;
    public List<string> unlockedDistricts;
    public Dictionary<string, bool> destroyedObjects;
    public Dictionary<string, Vector3Data> customPositions;

    public WorldStateData()
    {
        currentDistrict = "TestDistrict";
        unlockedDistricts = new List<string> { "TestDistrict" };
        destroyedObjects = new Dictionary<string, bool>();
        customPositions = new Dictionary<string, Vector3Data>();
    }
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
