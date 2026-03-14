using UnityEngine;
using System.Collections.Generic;
using CarSimulator.World;

namespace CarSimulator.Buildings
{
    public class ApartmentBuildingGenerator : MonoBehaviour
    {
        public static ApartmentBuildingGenerator Instance { get; private set; }

        [Header("Building Data")]
        [SerializeField] private ApartmentBuildingData[] m_buildingVariants;

        [Header("Spawn Settings")]
        [SerializeField] private int m_maxBuildings = 30;
        [SerializeField] private Vector2 m_spawnArea = new Vector2(300f, 300f);
        [SerializeField] private float m_minDistanceBetween = 25f;
        [SerializeField] private float m_minDistanceFromCenter = 30f;

        [Header("Placement")]
        [SerializeField] private bool m_useRaycast = true;
        [SerializeField] private LayerMask m_groundLayer;
        [SerializeField] private bool m_avoidRoads = true;
        [SerializeField] private Collider[] m_roadColliders;

        [Header("Building Root")]
        [SerializeField] private Transform m_buildingRoot;

        [Header("Performance")]
        [SerializeField] private bool m_useLOD = true;
        [SerializeField] private bool m_asyncSpawn = true;

        private List<SpawnedBuilding> m_spawnedBuildings = new List<SpawnedBuilding>();
        private bool m_isInitialized;

        [System.Serializable]
        public class SpawnedBuilding
        {
            public GameObject building;
            public ApartmentBuilding script;
            public ApartmentBuildingData data;
            public Vector3 position;
            public int variantIndex;
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
                m_buildingRoot = new GameObject("ApartmentBuildings").transform;
                m_buildingRoot.SetParent(transform);
            }
        }

        private void Start()
        {
            if (m_buildingVariants == null || m_buildingVariants.Length == 0)
            {
                CreateDefaultBuildingData();
            }

            SpawnBuildings();
            m_isInitialized = true;
        }

        private void CreateDefaultBuildingData()
        {
            m_buildingVariants = new ApartmentBuildingData[4];

            for (int i = 0; i < 4; i++)
            {
                ApartmentBuildingData data = ScriptableObject.CreateInstance<ApartmentBuildingData>();
                data.name = $"BuildingData_{i}";

                switch (i)
                {
                    case 0:
                        data = CreateModernApartment(data);
                        break;
                    case 1:
                        data = CreateClassicApartment(data);
                        break;
                    case 2:
                        data = CreateBrutalistApartment(data);
                        break;
                    case 3:
                        data = CreateArtDecoApartment(data);
                        break;
                }

                m_buildingVariants[i] = data;
            }
        }

        private ApartmentBuildingData CreateModernApartment(ApartmentBuildingData data)
        {
            data.m_buildingName = "Modern Apartment";
            data.m_buildingId = "modern_apt";
            data.m_style = ApartmentBuildingData.BuildingStyle.Modern;
            data.m_dimensions = new Vector3(14f, 18f, 14f);
            data.m_minFloors = 4;
            data.m_maxFloors = 8;
            data.m_facadeType = ApartmentBuildingData.FacadeType.Glass;
            data.m_primaryColor = new Color(0.6f, 0.65f, 0.7f);
            data.m_roofType = ApartmentBuildingData.RoofType.Flat;
            data.m_windowsPerFloor = 5;
            return data;
        }

        private ApartmentBuildingData CreateClassicApartment(ApartmentBuildingData data)
        {
            data.m_buildingName = "Classic Apartment";
            data.m_buildingId = "classic_apt";
            data.m_style = ApartmentBuildingData.BuildingStyle.Classic;
            data.m_dimensions = new Vector3(12f, 15f, 12f);
            data.m_minFloors = 3;
            data.m_maxFloors = 6;
            data.m_facadeType = ApartmentBuildingData.FacadeType.Brick;
            data.m_primaryColor = new Color(0.8f, 0.7f, 0.6f);
            data.m_roofType = ApartmentBuildingData.RoofType.Pitched;
            data.m_windowsPerFloor = 4;
            return data;
        }

        private ApartmentBuildingData CreateBrutalistApartment(ApartmentBuildingData data)
        {
            data.m_buildingName = "Brutalist Tower";
            data.m_buildingId = "brutalist";
            data.m_style = ApartmentBuildingData.BuildingStyle.Brutalist;
            data.m_dimensions = new Vector3(16f, 24f, 16f);
            data.m_minFloors = 6;
            data.m_maxFloors = 10;
            data.m_facadeType = ApartmentBuildingData.FacadeType.Concrete;
            data.m_primaryColor = new Color(0.5f, 0.5f, 0.55f);
            data.m_roofType = ApartmentBuildingData.RoofType.Flat;
            data.m_windowsPerFloor = 6;
            return data;
        }

        private ApartmentBuildingData CreateArtDecoApartment(ApartmentBuildingData data)
        {
            data.m_buildingName = "Art Deco Residence";
            data.m_buildingId = "artdeco";
            data.m_style = ApartmentBuildingData.BuildingStyle.ArtDeco;
            data.m_dimensions = new Vector3(10f, 20f, 10f);
            data.m_minFloors = 5;
            data.m_maxFloors = 8;
            data.m_facadeType = ApartmentBuildingData.FacadeType.Stucco;
            data.m_primaryColor = new Color(0.9f, 0.85f, 0.75f);
            data.m_accentColor = new Color(0.6f, 0.5f, 0.3f);
            data.m_roofType = ApartmentBuildingData.RoofType.Terraced;
            data.m_windowsPerFloor = 3;
            data.m_hasBalconies = true;
            return data;
        }

        public void SpawnBuildings()
        {
            ClearBuildings();

            if (m_buildingVariants == null || m_buildingVariants.Length == 0)
            {
                Debug.LogWarning("[ApartmentBuildingGenerator] No building variants!");
                return;
            }

            int attempts = 0;
            int maxAttempts = m_maxBuildings * 10;

            while (m_spawnedBuildings.Count < m_maxBuildings && attempts < maxAttempts)
            {
                attempts++;

                Vector3 position = GetRandomPosition();
                if (position.y < -100f) continue;

                if (!IsValidPosition(position)) continue;

                ApartmentBuildingData data = GetRandomBuildingData();
                if (data == null) continue;

                GameObject building = CreateBuildingPrefab(data, position);
                if (building == null) continue;

                SpawnedBuilding spawned = new SpawnedBuilding
                {
                    building = building,
                    script = building.GetComponent<ApartmentBuilding>(),
                    data = data,
                    position = position,
                    variantIndex = Random.Range(0, m_buildingVariants.Length)
                };

                m_spawnedBuildings.Add(spawned);
            }

            Debug.Log($"[ApartmentBuildingGenerator] Spawned {m_spawnedBuildings.Count} apartment buildings");
        }

        private GameObject CreateBuildingPrefab(ApartmentBuildingData data, Vector3 position)
        {
            GameObject building = new GameObject(data.BuildingName);
            building.transform.SetParent(m_buildingRoot);
            building.transform.position = position;

            float rotation = Random.Range(0, 4) * 90f;
            building.transform.rotation = Quaternion.Euler(0, rotation, 0);

            AddBuildingComponents(building, data);

            return building;
        }

        private void AddBuildingComponents(GameObject building, ApartmentBuildingData data)
        {
            int floors = Random.Range(data.MinFloors, data.MaxFloors + 1);
            float floorHeight = data.FloorHeight;
            Vector3 footprint = data.Footprint;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "BuildingBody";
            body.transform.SetParent(building.transform);
            body.transform.localPosition = new Vector3(0, floors * floorHeight / 2f, 0);
            body.transform.localScale = new Vector3(footprint.x, floors * floorHeight, footprint.y);

            Renderer bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material = new Material(Shader.Find("Standard"));
            bodyRenderer.material.color = data.PrimaryColor;

            AddWindows(building, data, floors);
            AddRoof(building, data, floors);
            AddBalconies(building, data, floors);

            ApartmentBuilding apartmentScript = building.AddComponent<ApartmentBuilding>();
            apartmentScript.Initialize(data);

            BoxCollider collider = building.AddComponent<BoxCollider>();
            collider.size = new Vector3(footprint.x, floors * floorHeight, footprint.y);
            collider.center = new Vector3(0, floors * floorHeight / 2f, 0);

            building.layer = LayerMask.NameToLayer("Building");
        }

        private void AddWindows(GameObject building, ApartmentBuildingData data, int floors)
        {
            float floorHeight = data.FloorHeight;
            Vector3 footprint = data.Footprint;
            int windowsPerFloor = data.WindowsPerFloor;

            for (int floor = 0; floor < floors; floor++)
            {
                float yPos = floor * floorHeight + floorHeight * 0.6f;

                for (int w = 0; w < windowsPerFloor; w++)
                {
                    float xOffset = (w - (windowsPerFloor - 1) / 2f) * (footprint.x / (windowsPerFloor + 1));

                    GameObject window = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    window.name = $"Window_{floor}_{w}";
                    window.transform.SetParent(building.transform);
                    window.transform.localPosition = new Vector3(xOffset, yPos, footprint.y / 2f + 0.01f);
                    window.transform.localScale = new Vector3(1f, 1.5f, 1f);

                    Renderer windowRenderer = window.GetComponent<Renderer>();
                    windowRenderer.material = new Material(Shader.Find("Standard"));
                    windowRenderer.material.color = data.WindowFrameColor;
                    windowRenderer.material.SetFloat("_Glossiness", 0.9f);
                }
            }
        }

        private void AddRoof(GameObject building, ApartmentBuildingData data, int floors)
        {
            float floorHeight = data.FloorHeight;
            Vector3 footprint = data.Footprint;
            float roofHeight = floorHeight * 0.5f;

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Roof";
            roof.transform.SetParent(building.transform);
            roof.transform.localPosition = new Vector3(0, floors * floorHeight + roofHeight / 2f, 0);

            switch (data.Roof)
            {
                case ApartmentBuildingData.RoofType.Flat:
                    roof.transform.localScale = new Vector3(footprint.x * 0.9f, roofHeight, footprint.y * 0.9f);
                    break;
                case ApartmentBuildingData.RoofType.Pitched:
                    roof.transform.localScale = new Vector3(footprint.x * 0.8f, roofHeight * 2f, footprint.y * 0.5f);
                    roof.transform.rotation = Quaternion.Euler(0, 45, 0);
                    break;
                case ApartmentBuildingData.RoofType.Terraced:
                    roof.transform.localScale = new Vector3(footprint.x * 0.85f, roofHeight * 1.5f, footprint.y * 0.85f);
                    break;
            }

            Renderer roofRenderer = roof.GetComponent<Renderer>();
            roofRenderer.material = new Material(Shader.Find("Standard"));
            roofRenderer.material.color = data.RoofColor;
        }

        private void AddBalconies(GameObject building, ApartmentBuildingData data, int floors)
        {
            if (!data.HasBalconies) return;

            float floorHeight = data.FloorHeight;
            Vector3 footprint = data.Footprint;

            for (int floor = 1; floor < floors; floor += 2)
            {
                if (Random.value > 0.5f) continue;

                float yPos = floor * floorHeight + 0.5f;

                GameObject balcony = GameObject.CreatePrimitive(PrimitiveType.Cube);
                balcony.name = $"Balcony_{floor}";
                balcony.transform.SetParent(building.transform);
                balcony.transform.localPosition = new Vector3(footprint.x / 2f + 0.5f, yPos, 0);
                balcony.transform.localScale = new Vector3(1f, 1f, 2f);

                Renderer balconyRenderer = balcony.GetComponent<Renderer>();
                balconyRenderer.material = new Material(Shader.Find("Standard"));
                balconyRenderer.material.color = new Color(0.3f, 0.3f, 0.35f);
            }
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
                    bounds.Expand(10f);
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

        private ApartmentBuildingData GetRandomBuildingData()
        {
            if (m_buildingVariants.Length == 0) return null;

            float totalWeight = 0;
            foreach (var variant in m_buildingVariants)
            {
                totalWeight += variant.SpawnWeight;
            }

            float random = Random.Range(0, totalWeight);
            float current = 0;

            foreach (var variant in m_buildingVariants)
            {
                current += variant.SpawnWeight;
                if (random <= current)
                {
                    return variant;
                }
            }

            return m_buildingVariants[0];
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

        public void SetRoadColliders(Collider[] colliders)
        {
            m_roadColliders = colliders;
        }

        public List<SpawnedBuilding> GetSpawnedBuildings() => m_spawnedBuildings;
        public int GetBuildingCount() => m_spawnedBuildings.Count;

        public Transform GetNearestBuilding(Vector3 position)
        {
            if (m_spawnedBuildings.Count == 0) return null;

            SpawnedBuilding nearest = m_spawnedBuildings[0];
            float minDist = Vector3.Distance(position, nearest.position);

            foreach (var building in m_spawnedBuildings)
            {
                float dist = Vector3.Distance(position, building.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = building;
                }
            }

            return nearest.building.transform;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 1f, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_spawnArea.x, 10f, m_spawnArea.y));
        }
    }
}
