using UnityEngine;

namespace CarSimulator.Buildings
{
    [CreateAssetMenu(fileName = "SO_CommercialData", menuName = "CarSimulator/Commercial Building Data")]
    public class CommercialBuildingData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string m_buildingName;
        [SerializeField] private string m_buildingId;
        [SerializeField] private CommercialType m_commercialType;

        [Header("Dimensions")]
        [SerializeField] private Vector3 m_dimensions = new Vector3(15f, 10f, 12f);
        [SerializeField] private Vector2 m_footprint = new Vector3(15f, 12f);

        [Header("Configuration")]
        [SerializeField] private int m_floors = 1;
        [SerializeField] private float m_floorHeight = 4f;

        [Header("Style")]
        [SerializeField] private CommercialStyle m_style = CommercialStyle.Modern;
        [SerializeField] private Color m_primaryColor = new Color(0.9f, 0.9f, 0.85f);
        [SerializeField] private Color m_accentColor = new Color(0.3f, 0.5f, 0.7f);

        [Header("Storefront")]
        [SerializeField] private int m_storefrontCount = 2;
        [SerializeField] private float m_storefrontWidth = 4f;
        [SerializeField] private bool m_hasAwnings;
        [SerializeField] private Color m_awningColor = new Color(0.8f, 0.2f, 0.2f);

        [Header("Signage")]
        [SerializeField] private bool m_hasRoofSign;
        [SerializeField] private bool m_hasBuildingSign;
        [SerializeField] private Vector3 m_signSize = new Vector3(3f, 1f, 0.2f);

        [Header("Parking")]
        [SerializeField] private bool m_hasParking;
        [SerializeField] private int m_parkingSpaces = 10;
        [SerializeField] private Vector2 m_parkingAreaSize = new Vector2(20f, 15f);

        [Header("Loading Dock")]
        [SerializeField] private bool m_hasLoadingDock;
        [SerializeField] private Vector3 m_dockSize = new Vector3(4f, 3.5f, 6f);

        [Header("Interior")]
        [SerializeField] private bool m_hasInterior;
        [SerializeField] private GameObject m_interiorPrefab;

        [Header("Spawn")]
        [SerializeField] private float m_spawnWeight = 1f;

        public enum CommercialType
        {
            Shop,
            Restaurant,
            Office,
            Mall,
            Hotel,
            GasStation,
            CarDealership,
            Supermarket,
            Bank,
            Pharmacy
        }

        public enum CommercialStyle
        {
            Modern,
            Traditional,
            Industrial,
            StripMall
        }

        public string BuildingName => m_buildingName;
        public string BuildingId => m_buildingId;
        public CommercialType Type => m_commercialType;
        public Vector3 Dimensions => m_dimensions;
        public Vector2 Footprint => m_footprint;
        public int Floors => m_floors;
        public float FloorHeight => m_floorHeight;
        public CommercialStyle Style => m_style;
        public Color PrimaryColor => m_primaryColor;
        public Color AccentColor => m_accentColor;
        public int StorefrontCount => m_storefrontCount;
        public float StorefrontWidth => m_storefrontWidth;
        public bool HasAwnings => m_hasAwnings;
        public Color AwningColor => m_awningColor;
        public bool HasRoofSign => m_hasRoofSign;
        public bool HasBuildingSign => m_hasBuildingSign;
        public Vector3 SignSize => m_signSize;
        public bool HasParking => m_hasParking;
        public int ParkingSpaces => m_parkingSpaces;
        public Vector2 ParkingAreaSize => m_parkingAreaSize;
        public bool HasLoadingDock => m_hasLoadingDock;
        public Vector3 DockSize => m_dockSize;
        public bool HasInterior => m_hasInterior;
        public GameObject InteriorPrefab => m_interiorPrefab;
        public float SpawnWeight => m_spawnWeight;
    }
}
