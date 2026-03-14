using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class CommercialBuildingGenerator : MonoBehaviour
    {
        public static CommercialBuildingGenerator Instance { get; private set; }

        [Header("Building Data")]
        [SerializeField] private CommercialBuildingData[] m_buildingTypes;

        [Header("Spawn Settings")]
        [SerializeField] private int m_maxBuildings = 15;
        [SerializeField] private Vector2 m_spawnArea = new Vector2(250f, 250f);
        [SerializeField] private float m_minDistanceBetween = 30f;
        [SerializeField] private float m_minDistanceFromCenter = 20f;

        [Header("Placement")]
        [SerializeField] private bool m_useRaycast = true;
        [SerializeField] private LayerMask m_groundLayer;
        [SerializeField] private bool m_avoidRoads = true;
        [SerializeField] private Collider[] m_roadColliders;

        [Header("Building Root")]
        [SerializeField] private Transform m_buildingRoot;

        private List<SpawnedCommercial> m_spawnedBuildings = new List<SpawnedCommercial>();

        [System.Serializable]
        public class SpawnedCommercial
        {
            public GameObject building;
            public CommercialBuilding script;
            public CommercialBuildingData data;
            public Vector3 position;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateBuildingRoot();
        }

        private void CreateBuildingRoot()
        {
            if (m_buildingRoot == null)
            {
                m_buildingRoot = new GameObject("CommercialBuildings").transform;
                m_buildingRoot.SetParent(transform);
            }
        }

        private void Start()
        {
            if (m_buildingTypes == null || m_buildingTypes.Length == 0)
            {
                CreateDefaultBuildingData();
            }

            SpawnBuildings();
        }

        private void CreateDefaultBuildingData()
        {
            m_buildingTypes = new CommercialBuildingData[6];

            for (int i = 0; i < 6; i++)
            {
                CommercialBuildingData data = ScriptableObject.CreateInstance<CommercialBuildingData>();
                data.name = $"CommercialData_{i}";

                switch (i)
                {
                    case 0:
                        data = CreateShopBuilding(data);
                        break;
                    case 1:
                        data = CreateRestaurantBuilding(data);
                        break;
                    case 2:
                        data = CreateOfficeBuilding(data);
                        break;
                    case 3:
                        data = CreateGasStation(data);
                        break;
                    case 4:
                        data = CreateSupermarket(data);
                        break;
                    case 5:
                        data = CreateStripMall(data);
                        break;
                }

                m_buildingTypes[i] = data;
            }
        }

        private CommercialBuildingData CreateShopBuilding(CommercialBuildingData data)
        {
            data.m_buildingName = "Retail Shop";
            data.m_buildingId = "retail_shop";
            data.m_commercialType = CommercialBuildingData.CommercialType.Shop;
            data.m_dimensions = new Vector3(10f, 4f, 8f);
            data.m_floors = 1;
            data.m_style = CommercialBuildingData.CommercialStyle.Modern;
            data.m_primaryColor = new Color(0.9f, 0.9f, 0.85f);
            data.m_storefrontCount = 2;
            data.m_hasAwnings = true;
            data.m_awningColor = new Color(0.8f, 0.2f, 0.2f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 6;
            return data;
        }

        private CommercialBuildingData CreateRestaurantBuilding(CommercialBuildingData data)
        {
            data.m_buildingName = "Restaurant";
            data.m_buildingId = "restaurant";
            data.m_commercialType = CommercialBuildingData.CommercialType.Restaurant;
            data.m_dimensions = new Vector3(12f, 4.5f, 10f);
            data.m_floors = 1;
            data.m_style = CommercialBuildingData.CommercialStyle.Traditional;
            data.m_primaryColor = new Color(0.85f, 0.75f, 0.65f);
            data.m_accentColor = new Color(0.6f, 0.3f, 0.2f);
            data.m_storefrontCount = 1;
            data.m_storefrontWidth = 6f;
            data.m_hasAwnings = true;
            data.m_awningColor = new Color(0.2f, 0.5f, 0.3f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 8;
            return data;
        }

        private CommercialBuildingData CreateOfficeBuilding(CommercialBuildingData data)
        {
            data.m_buildingName = "Office Building";
            data.m_buildingId = "office";
            data.m_commercialType = CommercialBuildingData.CommercialType.Office;
            data.m_dimensions = new Vector3(15f, 15f, 12f);
            data.m_floors = 3;
            data.m_floorHeight = 4f;
            data.m_style = CommercialBuildingData.CommercialStyle.Modern;
            data.m_primaryColor = new Color(0.7f, 0.75f, 0.8f);
            data.m_accentColor = new Color(0.3f, 0.5f, 0.7f);
            data.m_storefrontCount = 1;
            data.m_hasBuildingSign = true;
            data.m_hasParking = true;
            data.m_parkingSpaces = 15;
            return data;
        }

        private CommercialBuildingData CreateGasStation(CommercialBuildingData data)
        {
            data.m_buildingName = "Gas Station";
            data.m_buildingId = "gas_station";
            data.m_commercialType = CommercialBuildingData.CommercialType.GasStation;
            data.m_dimensions = new Vector3(12f, 4f, 10f);
            data.m_floors = 1;
            data.m_style = CommercialBuildingData.CommercialStyle.Modern;
            data.m_primaryColor = new Color(0.9f, 0.2f, 0.2f);
            data.m_storefrontCount = 1;
            data.m_storefrontWidth = 5f;
            data.m_hasAwnings = false;
            data.m_hasRoofSign = true;
            data.m_hasParking = true;
            data.m_parkingSpaces = 4;
            return data;
        }

        private CommercialBuildingData CreateSupermarket(CommercialBuildingData data)
        {
            data.m_buildingName = "Supermarket";
            data.m_buildingId = "supermarket";
            data.m_commercialType = CommercialBuildingData.CommercialType.Supermarket;
            data.m_dimensions = new Vector3(25f, 5f, 18f);
            data.m_floors = 1;
            data.m_style = CommercialBuildingData.CommercialStyle.Industrial;
            data.m_primaryColor = new Color(0.85f, 0.85f, 0.8f);
            data.m_storefrontCount = 1;
            data.m_storefrontWidth = 10f;
            data.m_hasLoadingDock = true;
            data.m_hasParking = true;
            data.m_parkingSpaces = 30;
            data.m_parkingAreaSize = new Vector2(30f, 20f);
            return data;
        }

        private CommercialBuildingData CreateStripMall(CommercialBuildingData data)
        {
            data.m_buildingName = "Strip Mall";
            data.m_buildingId = "strip_mall";
            data.m_commercialType = CommercialBuildingData.CommercialType.Mall;
            data.m_dimensions = new Vector3(30f, 4f, 12f);
            data.m_floors = 1;
            data.m_style = CommercialBuildingData.CommercialStyle.StripMall;
            data.m_primaryColor = new Color(0.9f, 0.88f, 0.82f);
            data.m_storefrontCount = 5;
            data.m_storefrontWidth = 5f;
            data.m_hasAwnings = true;
            data.m_awningColor = new Color(0.3f, 0.5f, 0.8f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 25;
            return data;
        }

        public void SpawnBuildings()
        {
            ClearBuildings();

            if (m_buildingTypes == null || m_buildingTypes.Length == 0) return;

            int attempts = 0;
            int maxAttempts = m_maxBuildings * 10;

            while (m_spawnedBuildings.Count < m_maxBuildings && attempts < maxAttempts)
            {
                attempts++;

                Vector3 position = GetRandomPosition();
                if (position.y < -100f) continue;

                if (!IsValidPosition(position)) continue;

                CommercialBuildingData data = GetRandomBuildingData();
                if (data == null) continue;

                GameObject building = CreateBuildingPrefab(data, position);
                if (building == null) continue;

                SpawnedCommercial spawned = new SpawnedCommercial
                {
                    building = building,
                    script = building.GetComponent<CommercialBuilding>(),
                    data = data,
                    position = position
                };

                m_spawnedBuildings.Add(spawned);
            }

            Debug.Log($"[CommercialBuildingGenerator] Spawned {m_spawnedBuildings.Count} commercial buildings");
        }

        private GameObject CreateBuildingPrefab(CommercialBuildingData data, Vector3 position)
        {
            GameObject building = new GameObject(data.BuildingName);
            building.transform.SetParent(m_buildingRoot);
            building.transform.position = position;

            float rotation = Random.Range(0, 4) * 90f;
            building.transform.rotation = Quaternion.Euler(0, rotation, 0);

            AddBuildingComponents(building, data);

            return building;
        }

        private void AddBuildingComponents(GameObject building, CommercialBuildingData data)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "BuildingBody";
            body.transform.SetParent(building.transform);
            body.transform.localPosition = new Vector3(0, data.Floors * data.FloorHeight / 2f, 0);
            body.transform.localScale = new Vector3(data.Dimensions.x, data.Floors * data.FloorHeight, data.Dimensions.z);

            Renderer bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material = new Material(Shader.Find("Standard"));
            bodyRenderer.material.color = data.PrimaryColor;

            if (data.StorefrontCount > 0)
            {
                AddStorefronts(building, data);
            }

            if (data.HasRoofSign)
            {
                AddRoofSign(building, data);
            }

            if (data.HasBuildingSign)
            {
                AddBuildingSign(building, data);
            }

            if (data.HasParking)
            {
                AddParkingArea(building, data);
            }

            CommercialBuilding commercialScript = building.AddComponent<CommercialBuilding>();
            commercialScript.Initialize(data);

            BoxCollider collider = building.AddComponent<BoxCollider>();
            collider.size = data.Dimensions;
            collider.center = new Vector3(0, data.Floors * data.FloorHeight / 2f, 0);

            building.layer = LayerMask.NameToLayer("Building");
        }

        private void AddStorefronts(GameObject building, CommercialBuildingData data)
        {
            float zPos = data.Dimensions.z / 2f + 0.1f;
            float height = data.FloorHeight * 0.7f;

            for (int i = 0; i < data.StorefrontCount; i++)
            {
                float xOffset = (i - (data.StorefrontCount - 1) / 2f) * data.StorefrontWidth;

                GameObject storefront = new GameObject($"Storefront_{i}");
                storefront.transform.SetParent(building.transform);
                storefront.transform.localPosition = new Vector3(xOffset, 0, zPos);

                GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
                window.transform.SetParent(storefront.transform);
                window.transform.localPosition = new Vector3(0, height / 2f, 0);
                window.transform.localScale = new Vector3(data.StorefrontWidth - 0.5f, height, 0.1f);

                Renderer windowRenderer = window.GetComponent<Renderer>();
                windowRenderer.material = new Material(Shader.Find("Standard"));
                windowRenderer.material.color = new Color(0.6f, 0.8f, 1f);
            }
        }

        private void AddRoofSign(GameObject building, CommercialBuildingData data)
        {
            GameObject sign = new GameObject("RoofSign");
            sign.transform.SetParent(building.transform);
            sign.transform.localPosition = new Vector3(0, data.Floors * data.FloorHeight + 1f, data.Dimensions.z / 2f);
            sign.transform.localScale = data.SignSize;

            TextMesh textMesh = sign.AddComponent<TextMesh>();
            textMesh.text = "STORE";
            textMesh.characterSize = 0.15f;
            textMesh.fontSize = 30;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.white;
        }

        private void AddBuildingSign(GameObject building, CommercialBuildingData data)
        {
            GameObject sign = new GameObject("BuildingSign");
            sign.transform.SetParent(building.transform);
            sign.transform.localPosition = new Vector3(0, data.Floors * data.FloorHeight - 1f, data.Dimensions.z / 2f + 0.1f);
            sign.transform.localScale = new Vector3(4f, 1.5f, 0.2f);

            TextMesh textMesh = sign.AddComponent<TextMesh>();
            textMesh.text = data.BuildingName;
            textMesh.characterSize = 0.2f;
            textMesh.fontSize = 24;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = data.AccentColor;
        }

        private void AddParkingArea(GameObject building, CommercialBuildingData data)
        {
            GameObject parking = new GameObject("ParkingArea");
            parking.transform.SetParent(building.transform);
            parking.transform.localPosition = new Vector3(0, 0, -data.Dimensions.z / 2f - data.ParkingAreaSize.y / 2f - 2f);
        }

        private Vector3 GetRandomPosition()
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = Random.Range(-m_spawnArea.x / 2f, m_spawnArea.x / 2f);
                float z = Random.Range(-m_spawnArea.y / 2f, m_spawnArea.y / 2f);

                if (Mathf.Abs(x) < m_minDistanceFromCenter && Mathf.Abs(z) < m_minDistanceFromCenter)
                {
                    continue;
                }

                if (m_avoidRoads && IsNearRoad(x, z))
                {
                    continue;
                }

                Vector3 position = new Vector3(x, 0, z);

                if (m_useRaycast)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hit, 100f, m_groundLayer))
                    {
                        position = hit.point;
                    }
                    else
                    {
                        continue;
                    }
                }

                return position;
            }

            return new Vector3(0, -1000, 0);
        }

        private bool IsNearRoad(float x, float z)
        {
            if (m_roadColliders == null || m_roadColliders.Length == 0) return false;

            foreach (var collider in m_roadColliders)
            {
                if (collider != null)
                {
                    Bounds bounds = collider.bounds;
                    bounds.Expand(8f);
                    if (bounds.Contains(new Vector3(x, 0, z)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsValidPosition(Vector3 position)
        {
            foreach (var building in m_spawnedBuildings)
            {
                float distance = Vector3.Distance(position, building.position);
                if (distance < m_minDistanceBetween)
                {
                    return false;
                }
            }
            return true;
        }

        private CommercialBuildingData GetRandomBuildingData()
        {
            if (m_buildingTypes.Length == 0) return null;

            float totalWeight = 0;
            foreach (var type in m_buildingTypes)
            {
                totalWeight += type.SpawnWeight;
            }

            float random = Random.Range(0, totalWeight);
            float current = 0;

            foreach (var type in m_buildingTypes)
            {
                current += type.SpawnWeight;
                if (random <= current)
                {
                    return type;
                }
            }

            return m_buildingTypes[0];
        }

        public void ClearBuildings()
        {
            foreach (var building in m_spawnedBuildings)
            {
                if (building.building != null)
                {
                    Destroy(building.building);
                }
            }
            m_spawnedBuildings.Clear();
        }

        public int GetBuildingCount() => m_spawnedBuildings.Count;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_spawnArea.x, 10f, m_spawnArea.y));
        }
    }
}
