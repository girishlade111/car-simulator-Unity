using UnityEngine;

[CreateAssetMenu(fileName = "SO_BuildingData", menuName = "CarSimulator/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("Building Info")]
    [SerializeField] private string m_buildingName;
    [SerializeField] private string m_buildingId;
    [SerializeField] private BuildingType m_buildingType;

    [Header("Prefab")]
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private GameObject m_interiorPrefab;

    [Header("Dimensions")]
    [SerializeField] private Vector3 m_dimensions = new Vector3(15f, 20f, 15f);

    [Header("Spawn Settings")]
    [SerializeField] private float m_spawnWeight = 1f;
    [SerializeField] private bool m_hasParking = true;
    [SerializeField] private int m_parkingSpaces = 5;

    [Header("Variants")]
    [SerializeField] private Material[] m_exteriorMaterials;

    public enum BuildingType
    {
        Apartment,
        Commercial,
        Industrial,
        MixedUse
    }

    public string BuildingName => m_buildingName;
    public string BuildingId => m_buildingId;
    public BuildingType Type => m_buildingType;
    public GameObject Prefab => m_prefab;
    public GameObject InteriorPrefab => m_interiorPrefab;
    public Vector3 Dimensions => m_dimensions;
    public float SpawnWeight => m_spawnWeight;
    public bool HasParking => m_hasParking;
    public int ParkingSpaces => m_parkingSpaces;
    public Material[] ExteriorMaterials => m_exteriorMaterials;
}
