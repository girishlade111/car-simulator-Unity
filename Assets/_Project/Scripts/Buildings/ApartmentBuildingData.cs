using UnityEngine;

namespace CarSimulator.Buildings
{
    [CreateAssetMenu(fileName = "SO_ApartmentData", menuName = "CarSimulator/Apartment Building Data")]
    public class ApartmentBuildingData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string m_buildingName;
        [SerializeField] private string m_buildingId;
        [SerializeField] private BuildingStyle m_style = BuildingStyle.Modern;

        [Header("Dimensions")]
        [SerializeField] private Vector3 m_dimensions = new Vector3(12f, 15f, 12f);
        [SerializeField] private Vector2 m_footprint = new Vector2(12f, 12f);

        [Header("Configuration")]
        [SerializeField] private int m_minFloors = 3;
        [SerializeField] private int m_maxFloors = 8;
        [SerializeField] private float m_floorHeight = 3f;

        [Header("Facade")]
        [SerializeField] private FacadeType m_facadeType = FacadeType.Brick;
        [SerializeField] private Color m_primaryColor = new Color(0.8f, 0.75f, 0.7f);
        [SerializeField] private Color m_accentColor = new Color(0.3f, 0.3f, 0.35f);

        [Header("Roof")]
        [SerializeField] private RoofType m_roofType = RoofType.Flat;
        [SerializeField] private Color m_roofColor = new Color(0.2f, 0.2f, 0.25f);

        [Header("Windows")]
        [SerializeField] private int m_windowsPerFloor = 4;
        [SerializeField] private WindowStyle m_windowStyle = WindowStyle.Standard;
        [SerializeField] private Color m_windowFrameColor = new Color(0.2f, 0.2f, 0.2f);
        [SerializeField] private bool m_hasBalconies;

        [Header("Entry")]
        [SerializeField] private int m_entryCount = 1;
        [SerializeField] private Vector2 m_entrySpacing = new Vector2(6f, 0f);

        [Header("Parking")]
        [SerializeField] private bool m_hasParking = true;
        [SerializeField] private int m_parkingSpaces = 6;
        [SerializeField] private Vector2 m_parkingAreaSize = new Vector2(15f, 8f);

        [Header("Interior")]
        [SerializeField] private bool m_hasInterior;
        [SerializeField] private GameObject m_interiorPrefab;
        [SerializeField] private InteriorPortal[] m_portals;

        [Header("Spawn")]
        [SerializeField] private float m_spawnWeight = 1f;
        [SerializeField] private bool m_requireRoadAccess = true;

        [Header("LOD")]
        [SerializeField] private GameObject m_lodLevel1;
        [SerializeField] private GameObject m_lodLevel2;

        public enum BuildingStyle
        {
            Modern,
            Classic,
            Brutalist,
            ArtDeco
        }

        public enum FacadeType
        {
            Brick,
            Concrete,
            Stucco,
            Metal,
            Glass
        }

        public enum RoofType
        {
            Flat,
            Pitched,
            Terraced,
            Penthouse
        }

        public enum WindowStyle
        {
            Standard,
            FloorToCeiling,
            Arched,
            Circular
        }

        [System.Serializable]
        public class InteriorPortal
        {
            public string portalId;
            public Vector3 position;
            public Vector3 size = new Vector3(2f, 2.5f, 0.5f);
            public bool isLocked;
        }

        public string BuildingName => m_buildingName;
        public string BuildingId => m_buildingId;
        public BuildingStyle Style => m_style;
        public Vector3 Dimensions => m_dimensions;
        public Vector2 Footprint => m_footprint;
        public int MinFloors => m_minFloors;
        public int MaxFloors => m_maxFloors;
        public float FloorHeight => m_floorHeight;
        public FacadeType Facade => m_facadeType;
        public Color PrimaryColor => m_primaryColor;
        public Color AccentColor => m_accentColor;
        public RoofType Roof => m_roofType;
        public Color RoofColor => m_roofColor;
        public int WindowsPerFloor => m_windowsPerFloor;
        public WindowStyle Window => m_windowStyle;
        public Color WindowFrameColor => m_windowFrameColor;
        public bool HasBalconies => m_hasBalconies;
        public int EntryCount => m_entryCount;
        public Vector2 EntrySpacing => m_entrySpacing;
        public bool HasParking => m_hasParking;
        public int ParkingSpaces => m_parkingSpaces;
        public Vector2 ParkingAreaSize => m_parkingAreaSize;
        public bool HasInterior => m_hasInterior;
        public GameObject InteriorPrefab => m_interiorPrefab;
        public InteriorPortal[] Portals => m_portals;
        public float SpawnWeight => m_spawnWeight;
        public bool RequireRoadAccess => m_requireRoadAccess;
        public GameObject LodLevel1 => m_lodLevel1;
        public GameObject LodLevel2 => m_lodLevel2;
    }
}
