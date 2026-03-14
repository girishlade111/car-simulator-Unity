using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class CommercialBuilding : MonoBehaviour
    {
        [Header("Building Info")]
        [SerializeField] private string m_buildingName;
        [SerializeField] private CommercialBuildingData.CommercialType m_buildingType;
        [SerializeField] private int m_floorCount = 1;

        [Header("Components")]
        [SerializeField] private Renderer[] m_wallRenderers;
        [SerializeField] private Renderer[] m_signRenderers;
        [SerializeField] private Renderer[] m_awningRenderers;

        [Header("Storefronts")]
        [SerializeField] private List<Storefront> m_storefronts = new List<Storefront>();

        [Header("Parking")]
        [SerializeField] private Transform m_parkingArea;
        [SerializeField] private List<Transform> m_parkingSpots = new List<Transform>();

        [Header("Loading Dock")]
        [SerializeField] private Transform m_loadingDock;
        [SerializeField] private bool m_hasLoadingDock;

        [Header("Signage")]
        [SerializeField] private Transform m_roofSign;
        [SerializeField] private Transform m_buildingSign;

        [Header("Variation")]
        [SerializeField] private int m_variantIndex;
        [SerializeField] private Color m_buildingColor;

        private CommercialBuildingData m_buildingData;

        [System.Serializable]
        public class Storefront
        {
            public Transform transform;
            public string storeName;
            public bool hasAwning;
            public int windowCount;
        }

        public string BuildingName => m_buildingName;
        public CommercialBuildingData.CommercialType BuildingType => m_buildingType;
        public int FloorCount => m_floorCount;
        public List<Storefront> Storefronts => m_storefronts;
        public List<Transform> ParkingSpots => m_parkingSpots;
        public bool HasLoadingDock => m_hasLoadingDock;

        public void Initialize(CommercialBuildingData data)
        {
            m_buildingData = data;
            m_buildingName = data.BuildingName;
            m_buildingType = data.Type;
            m_floorCount = data.Floors;
            m_hasLoadingDock = data.HasLoadingDock;

            ApplyColors();
            SetupStorefronts();
            GenerateParking();
        }

        private void ApplyColors()
        {
            if (m_buildingData == null) return;

            m_buildingColor = m_buildingData.PrimaryColor;

            if (m_wallRenderers != null)
            {
                foreach (var renderer in m_wallRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_buildingColor;
                    }
                }
            }

            if (m_awningRenderers != null)
            {
                foreach (var renderer in m_awningRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_buildingData.AwningColor;
                    }
                }
            }
        }

        private void SetupStorefronts()
        {
            m_storefronts.Clear();

            if (m_buildingData == null || m_buildingData.StorefrontCount == 0) return;

            for (int i = 0; i < m_buildingData.StorefrontCount; i++)
            {
                Storefront storefront = new Storefront
                {
                    transform = CreateStorefrontTransform(i),
                    storeName = GetStoreName(i),
                    hasAwning = m_buildingData.HasAwnings,
                    windowCount = Random.Range(1, 3)
                };

                m_storefronts.Add(storefront);
            }
        }

        private Transform CreateStorefrontTransform(int index)
        {
            GameObject storefrontObj = new GameObject($"Storefront_{index}");
            storefrontObj.transform.SetParent(transform);

            float xOffset = (index - (m_buildingData.StorefrontCount - 1) / 2f) * m_buildingData.StorefrontWidth;
            storefrontObj.transform.localPosition = new Vector3(xOffset, 0, m_buildingData.Dimensions.z / 2f + 0.1f);

            CreateStorefrontVisuals(storefrontObj);

            return storefrontObj.transform;
        }

        private void CreateStorefrontVisuals(GameObject storefront)
        {
            float width = m_buildingData.StorefrontWidth - 0.5f;
            float height = m_buildingData.FloorHeight * 0.8f;

            GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
            window.name = "StoreWindow";
            window.transform.SetParent(storefront.transform);
            window.transform.localPosition = new Vector3(0, height / 2f, 0);
            window.transform.localScale = new Vector3(width, height, 0.1f);

            Renderer windowRenderer = window.GetComponent<Renderer>();
            if (windowRenderer != null)
            {
                windowRenderer.material = new Material(Shader.Find("Standard"));
                windowRenderer.material.color = new Color(0.6f, 0.8f, 1f);
                windowRenderer.material.SetFloat("_Glossiness", 0.9f);
            }

            if (m_buildingData.HasAwnings)
            {
                CreateAwning(storefront, width);
            }
        }

        private void CreateAwning(GameObject storefront, float width)
        {
            GameObject awning = GameObject.CreatePrimitive(PrimitiveType.Cube);
            awning.name = "Awning";
            awning.transform.SetParent(storefront.transform);
            awning.transform.localPosition = new Vector3(0, m_buildingData.FloorHeight * 0.7f, 0.3f);
            awning.transform.localScale = new Vector3(width, 0.1f, 1f);
            awning.transform.rotation = Quaternion.Euler(-20, 0, 0);

            Renderer awningRenderer = awning.GetComponent<Renderer>();
            if (awningRenderer != null)
            {
                awningRenderer.material = new Material(Shader.Find("Standard"));
                awningRenderer.material.color = m_buildingData.AwningColor;
            }

            if (m_awningRenderers == null) m_awningRenderers = new Renderer[1];
        }

        private void GenerateParking()
        {
            if (!m_buildingData.HasParking || m_parkingArea == null) return;

            m_parkingSpots.Clear();

            Vector2 areaSize = m_buildingData.ParkingAreaSize;
            int spotsX = Mathf.FloorToInt(areaSize.x / 3f);
            int spotsZ = Mathf.FloorToInt(areaSize.y / 5f);

            for (int x = 0; x < spotsX; x++)
            {
                for (int z = 0; z < spotsZ; z++)
                {
                    if (m_parkingSpots.Count >= m_buildingData.ParkingSpaces) break;

                    GameObject spot = new GameObject($"ParkingSpot_{x}_{z}");
                    spot.transform.SetParent(m_parkingArea);

                    float xPos = (x - spotsX / 2f) * 3f + 1.5f;
                    float zPos = (z - spotsZ / 2f) * 5f + 2.5f;
                    spot.transform.localPosition = new Vector3(xPos, 0, zPos);

                    CreateParkingSpaceMarker(spot);
                    m_parkingSpots.Add(spot.transform);
                }
            }
        }

        private void CreateParkingSpaceMarker(GameObject spot)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Quad);
            line.transform.SetParent(spot.transform);
            line.transform.localPosition = Vector3.zero;
            line.transform.localRotation = Quaternion.Euler(90, 0, 0);
            line.transform.localScale = new Vector3(2.5f, 0.1f, 4.5f);

            Renderer renderer = line.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = Color.white;
            }

            GameObject.Destroy(line.GetComponent<Collider>());
        }

        private string GetStoreName(int index)
        {
            string[] shopNames = { "General Store", "Cafe", "Bakery", "Clothing", "Electronics", "Pharmacy", "Bank", "Restaurant" };
            string[] prefixes = { "", "Downtown ", "Main St. ", "City " };

            return prefixes[Random.Range(0, prefixes.Length)] + shopNames[Random.Range(0, shopNames.Length)];
        }

        public void SetBuildingSign(string text)
        {
            if (m_buildingSign == null) return;

            var textMesh = m_buildingSign.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }

        public Transform GetFreeParkingSpot()
        {
            if (m_parkingSpots.Count == 0) return null;
            return m_parkingSpots[Random.Range(0, m_parkingSpots.Count)];
        }

        public void SetColor(Color color)
        {
            m_buildingColor = color;

            if (m_wallRenderers != null)
            {
                foreach (var renderer in m_wallRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = color;
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);

            foreach (var spot in m_parkingSpots)
            {
                if (spot != null)
                {
                    Gizmos.DrawWireCube(spot.position, new Vector3(2.5f, 0.1f, 4.5f));
                }
            }
        }
    }
}
