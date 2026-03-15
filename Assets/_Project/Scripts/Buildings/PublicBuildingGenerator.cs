using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class PublicBuildingGenerator : MonoBehaviour
    {
        public static PublicBuildingGenerator Instance { get; private set; }

        [Header("Building Data")]
        [SerializeField] private PublicBuildingData[] m_buildingTypes;

        [Header("Spawn Settings")]
        [SerializeField] private int m_maxBuildings = 10;
        [SerializeField] private Vector2 m_spawnArea = new Vector2(300f, 300f);
        [SerializeField] private float m_minDistanceBetween = 50f;

        [Header("Placement")]
        [SerializeField] private bool m_useRaycast = true;
        [SerializeField] private LayerMask m_groundLayer;

        [Header("Building Root")]
        [SerializeField] private Transform m_buildingRoot;

        private List<SpawnedPublic> m_spawnedBuildings = new List<SpawnedPublic>();

        [System.Serializable]
        public class SpawnedPublic
        {
            public GameObject building;
            PublicBuilding script;
            PublicBuildingData data;
            Vector3 position;
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
                m_buildingRoot = new GameObject("PublicBuildings").transform;
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
            m_buildingTypes = new PublicBuildingData[8];

            for (int i = 0; i < 8; i++)
            {
                PublicBuildingData data = ScriptableObject.CreateInstance<PublicBuildingData>();
                data.name = $"PublicBuilding_{i}";

                switch (i)
                {
                    case 0:
                        data = CreateSchool(data);
                        break;
                    case 1:
                        data = CreateHospital(data);
                        break;
                    case 2:
                        data = CreatePoliceStation(data);
                        break;
                    case 3:
                        data = CreateFireStation(data);
                        break;
                    case 4:
                        data = CreateLibrary(data);
                        break;
                    case 5:
                        data = CreateCommunityCenter(data);
                        break;
                    case 6:
                        data = CreateStadium(data);
                        break;
                    case 7:
                        data = CreatePark(data);
                        break;
                }

                m_buildingTypes[i] = data;
            }
        }

        private PublicBuildingData CreateSchool(PublicBuildingData data)
        {
            data.m_buildingName = "Elementary School";
            data.m_buildingId = "school";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.School;
            data.m_dimensions = new Vector3(25f, 8f, 20f);
            data.m_floors = 2;
            data.m_style = PublicBuildingData.PublicStyle.Institutional;
            data.m_primaryColor = new Color(0.9f, 0.85f, 0.8f);
            data.m_accentColor = new Color(0.3f, 0.5f, 0.7f);
            data.m_hasPlayground = true;
            data.m_hasParking = true;
            data.m_capacity = 300;
            data.m_staffCount = 20;
            return data;
        }

        private PublicBuildingData CreateHospital(PublicBuildingData data)
        {
            data.m_buildingName = "City Hospital";
            data.m_buildingId = "hospital";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.Hospital;
            data.m_dimensions = new Vector3(30f, 18f, 25f);
            data.m_floors = 4;
            data.m_style = PublicBuildingData.PublicStyle.Modern;
            data.m_primaryColor = new Color(0.9f, 0.9f, 0.95f);
            data.m_accentColor = new Color(0.3f, 0.7f, 0.5f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 40;
            data.m_capacity = 200;
            data.m_staffCount = 100;
            return data;
        }

        private PublicBuildingData CreatePoliceStation(PublicBuildingData data)
        {
            data.m_buildingName = "Police Station";
            data.m_buildingId = "police";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.PoliceStation;
            data.m_dimensions = new Vector3(20f, 8f, 15f);
            data.m_floors = 2;
            data.m_style = PublicBuildingData.PublicStyle.Modern;
            data.m_primaryColor = new Color(0.7f, 0.75f, 0.8f);
            data.m_accentColor = new Color(0.2f, 0.3f, 0.5f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 15;
            data.m_capacity = 50;
            data.m_staffCount = 30;
            return data;
        }

        private PublicBuildingData CreateFireStation(PublicBuildingData data)
        {
            data.m_buildingName = "Fire Station";
            data.m_buildingId = "fire";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.FireStation;
            data.m_dimensions = new Vector3(18f, 7f, 15f);
            data.m_floors = 1;
            data.m_style = PublicBuildingData.PublicStyle.Modern;
            data.m_primaryColor = new Color(0.8f, 0.2f, 0.2f);
            data.m_accentColor = new Color(0.9f, 0.9f, 0.9f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 8;
            data.m_capacity = 20;
            data.m_staffCount = 15;
            return data;
        }

        private PublicBuildingData CreateLibrary(PublicBuildingData data)
        {
            data.m_buildingName = "Public Library";
            data.m_buildingId = "library";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.Library;
            data.m_dimensions = new Vector3(18f, 10f, 15f);
            data.m_floors = 2;
            data.m_style = PublicBuildingData.PublicStyle.Classical;
            data.m_primaryColor = new Color(0.85f, 0.8f, 0.75f);
            data.m_accentColor = new Color(0.5f, 0.4f, 0.3f);
            data.m_hasParking = true;
            data.m_capacity = 100;
            data.m_staffCount = 15;
            return data;
        }

        private PublicBuildingData CreateCommunityCenter(PublicBuildingData data)
        {
            data.m_buildingName = "Community Center";
            data.m_buildingId = "community";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.CommunityCenter;
            data.m_dimensions = new Vector3(15f, 6f, 12f);
            data.m_floors = 1;
            data.m_style = PublicBuildingData.PublicStyle.Modern;
            data.m_primaryColor = new Color(0.85f, 0.85f, 0.8f);
            data.m_accentColor = new Color(0.6f, 0.5f, 0.4f);
            data.m_hasParking = true;
            data.m_capacity = 150;
            data.m_staffCount = 8;
            return data;
        }

        private PublicBuildingData CreateStadium(PublicBuildingData data)
        {
            data.m_buildingName = "Stadium";
            data.m_buildingId = "stadium";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.Stadium;
            data.m_dimensions = new Vector3(60f, 15f, 40f);
            data.m_floors = 1;
            data.m_style = PublicBuildingData.PublicStyle.Modern;
            data.m_primaryColor = new Color(0.8f, 0.8f, 0.85f);
            data.m_accentColor = new Color(0.9f, 0.3f, 0.2f);
            data.m_hasParking = true;
            data.m_parkingSpaces = 100;
            data.m_capacity = 5000;
            data.m_staffCount = 50;
            return data;
        }

        private PublicBuildingData CreatePark(PublicBuildingData data)
        {
            data.m_buildingName = "City Park";
            data.m_buildingId = "park";
            data.m_buildingType = PublicBuildingData.PublicBuildingType.Park;
            data.m_dimensions = new Vector3(50f, 2f, 50f);
            data.m_floors = 0;
            data.m_hasGarden = true;
            data.m_hasPlayground = true;
            data.m_hasParking = true;
            data.m_parkingSpaces = 30;
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

                PublicBuildingData data = m_buildingTypes[Random.Range(0, m_buildingTypes.Length)];
                if (data == null) continue;

                GameObject building = CreateBuildingPrefab(data, position);
                if (building == null) continue;

                m_spawnedBuildings.Add(new SpawnedPublic
                {
                    building = building,
                    script = building.GetComponent<PublicBuilding>(),
                    data = data,
                    position = position
                });
            }

            Debug.Log($"[PublicBuildingGenerator] Spawned {m_spawnedBuildings.Count} public buildings");
        }

        private GameObject CreateBuildingPrefab(PublicBuildingData data, Vector3 position)
        {
            GameObject building = new GameObject(data.BuildingName);
            building.transform.SetParent(m_buildingRoot);
            building.transform.position = position;

            float rotation = Random.Range(0, 4) * 90f;
            building.transform.rotation = Quaternion.Euler(0, rotation, 0);

            if (data.Floors > 0)
            {
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
                body.name = "BuildingBody";
                body.transform.SetParent(building.transform);
                body.transform.localPosition = new Vector3(0, data.Floors * data.FloorHeight / 2f, 0);
                body.transform.localScale = data.Dimensions;

                Renderer bodyRenderer = body.GetComponent<Renderer>();
                bodyRenderer.material = new Material(Shader.Find("Standard"));
                bodyRenderer.material.color = data.PrimaryColor;

                GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roof.name = "Roof";
                roof.transform.SetParent(building.transform);
                roof.transform.localPosition = new Vector3(0, data.Floors * data.FloorHeight + 0.5f, 0);
                roof.transform.localScale = new Vector3(data.Dimensions.x * 0.9f, 1f, data.Dimensions.z * 0.9f);

                Renderer roofRenderer = roof.GetComponent<Renderer>();
                roofRenderer.material = new Material(Shader.Find("Standard"));
                roofRenderer.material.color = new Color(0.3f, 0.3f, 0.35f);
            }

            if (data.HasParking)
            {
                GameObject parking = new GameObject("ParkingArea");
                parking.transform.SetParent(building.transform);
                parking.transform.localPosition = new Vector3(0, 0, -data.Dimensions.z / 2f - 8f);
            }

            PublicBuilding publicScript = building.AddComponent<PublicBuilding>();
            publicScript.Initialize(data);

            BoxCollider collider = building.AddComponent<BoxCollider>();
            collider.size = data.Dimensions;
            collider.center = new Vector3(0, data.Floors * data.FloorHeight / 2f, 0);

            building.layer = LayerMask.NameToLayer("Building");

            return building;
        }

        private Vector3 GetRandomPosition()
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = Random.Range(-m_spawnArea.x / 2f, m_spawnArea.x / 2f);
                float z = Random.Range(-m_spawnArea.y / 2f, m_spawnArea.y / 2f);

                if (Mathf.Abs(x) < 40f && Mathf.Abs(z) < 40f) continue;

                Vector3 position = new Vector3(x, 0, z);

                if (m_useRaycast)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hit, 100f, m_groundLayer))
                    {
                        position = hit.point;
                    }
                    else continue;
                }

                return position;
            }

            return new Vector3(0, -1000, 0);
        }

        private bool IsValidPosition(Vector3 position)
        {
            foreach (var building in m_spawnedBuildings)
            {
                if (Vector3.Distance(position, building.position) < m_minDistanceBetween)
                    return false;
            }
            return true;
        }

        public void ClearBuildings()
        {
            foreach (var b in m_spawnedBuildings)
            {
                if (b.building != null) Destroy(b.building);
            }
            m_spawnedBuildings.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_spawnArea.x, 10f, m_spawnArea.y));
        }
    }
}
