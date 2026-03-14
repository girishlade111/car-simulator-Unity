using UnityEngine;

[CreateAssetMenu(fileName = "SO_Mission", menuName = "CarSimulator/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Mission Info")]
    [SerializeField] private string m_missionId;
    [SerializeField] private string m_missionName;
    [SerializeField] [TextArea] private string m_description;
    [SerializeField] private MissionType m_missionType;

    [Header("Objectives")]
    [SerializeField] private ObjectiveData[] m_objectives;

    [Header("Rewards")]
    [SerializeField] private int m_currencyReward;
    [SerializeField] private string[] m_unlockables;

    [Header("Settings")]
    [SerializeField] private bool m_isRepeatable;
    [SerializeField] private float m_timeLimit;

    public enum MissionType
    {
        Delivery,
        TimeTrial,
        Collection,
        Exploration,
        Race
    }

    public string MissionId => m_missionId;
    public string MissionName => m_missionName;
    public string Description => m_description;
    public MissionType Type => m_missionType;
    public ObjectiveData[] Objectives => m_objectives;
    public int CurrencyReward => m_currencyReward;
    public string[] Unlockables => m_unlockables;
    public bool IsRepeatable => m_isRepeatable;
    public float TimeLimit => m_timeLimit;
}

[System.Serializable]
public class ObjectiveData
{
    public string objectiveId;
    public string description;
    public ObjectiveType type;
    public Vector3 targetPosition;
    public string targetTag;
    public float targetDistance = 5f;
    public int targetCount = 1;
    public bool isOptional;

    public enum ObjectiveType
    {
        ReachLocation,
        CollectItems,
        DestroyTargets,
        DriveDistance,
        TimeTrial
    }
}
