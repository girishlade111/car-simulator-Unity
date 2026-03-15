using UnityEngine;

namespace CarSimulator.Buildings
{
    [CreateAssetMenu(fileName = "SO_PublicBuildingData", menuName = "CarSimulator/Public Building Data")]
    public class PublicBuildingData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string m_buildingName;
        [SerializeField] private string m_buildingId;
        [SerializeField] private PublicBuildingType m_buildingType;

        [Header("Dimensions")]
        [SerializeField] private Vector3 m_dimensions = new Vector3(20f, 12f, 15f);
        [SerializeField] private Vector2 m_footprint = new Vector2(20f, 15f);

        [Header("Configuration")]
        [SerializeField] private int m_floors = 2;
        [SerializeField] private float m_floorHeight = 4f;

        [Header("Style")]
        [SerializeField] private PublicStyle m_style = PublicStyle.Modern;
        [SerializeField] private Color m_primaryColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color m_accentColor = new Color(0.3f, 0.5f, 0.7f);

        [Header("Features")]
        [SerializeField] private bool m_hasPlayground;
        [SerializeField] private bool m_hasParking = true;
        [SerializeField] private int m_parkingSpaces = 20;
        [SerializeField] private bool m_hasSportsField;
        [SerializeField] private bool m_hasGarden;

        [Header("Capacity")]
        [SerializeField] private int m_capacity = 100;
        [SerializeField] private int m_staffCount = 10;

        [Header("Interior")]
        [SerializeField] private bool m_hasInterior;
        [SerializeField] private GameObject m_interiorPrefab;

        [Header("Spawn")]
        [SerializeField] private float m_spawnWeight = 1f;

        public enum PublicBuildingType
        {
            School,
            Hospital,
            PoliceStation,
            FireStation,
            Library,
            CommunityCenter,
            Park,
            Stadium
        }

        public enum PublicStyle
        {
            Modern,
            Classical,
            Institutional
        }

        public string BuildingName => m_buildingName;
        public string BuildingId => m_buildingId;
        public PublicBuildingType Type => m_buildingType;
        public Vector3 Dimensions => m_dimensions;
        public Vector2 Footprint => m_footprint;
        public int Floors => m_floors;
        public float FloorHeight => m_floorHeight;
        public PublicStyle Style => m_style;
        public Color PrimaryColor => m_primaryColor;
        public Color AccentColor => m_accentColor;
        public bool HasPlayground => m_hasPlayground;
        public bool HasParking => m_hasParking;
        public int ParkingSpaces => m_parkingSpaces;
        public bool HasSportsField => m_hasSportsField;
        public bool HasGarden => m_hasGarden;
        public int Capacity => m_capacity;
        public int StaffCount => m_staffCount;
        public bool HasInterior => m_hasInterior;
        public GameObject InteriorPrefab => m_interiorPrefab;
        public float SpawnWeight => m_spawnWeight;
    }
}
