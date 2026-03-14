using UnityEngine;

namespace CarSimulator.World
{
    public class OpenWorldBuilder : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2 m_worldSize = new Vector2(500f, 500f);
        [SerializeField] private int m_mainRoads = 2;
        [SerializeField] private int m_crossRoads = 2;
        [SerializeField] private float m_roadWidth = 8f;
        [SerializeField] private int m_buildingCount = 8;
        [SerializeField] private int m_parkedCarCount = 10;
        [SerializeField] private int m_treeCount = 30;
        [SerializeField] private int m_rockCount = 20;

        [Header("References")]
        [SerializeField] private GameObject m_playerCarPrefab;
        [SerializeField] private PlaceholderFactory m_factory;

        private Transform m_root;

        [ContextMenu("Build World")]
        public void BuildWorld()
        {
            ClearWorld();
            CreateRoot();

            CreateGround();
            CreateRoadNetwork();
            CreateBuildings();
            CreateParkedCars();
            CreateEnvironment();
            SpawnPlayerCar();
            SetupLighting();
            SetupCamera();

            Debug.Log("[OpenWorldBuilder] World built successfully!");
        }

        [ContextMenu("Clear World")]
        public void ClearWorld()
        {
            if (m_root != null)
            {
                DestroyImmediate(m_root.gameObject);
            }
        }

        private void CreateRoot()
        {
            m_root = new GameObject("WorldRoot").transform;
        }

        private void CreateGround()
        {
            if (m_factory == null) m_factory = GetComponent<PlaceholderFactory>();
            if (m_factory == null) m_factory = gameObject.AddComponent<PlaceholderFactory>();

            GameObject ground = m_factory.CreateGround(Vector3.zero, m_worldSize);
            ground.transform.SetParent(m_root);
        }

        private void CreateRoadNetwork()
        {
            GameObject roadsRoot = new GameObject("Roads");
            roadsRoot.transform.SetParent(m_root);

            float length = m_worldSize.x;
            float spacing = m_worldSize.x / (m_mainRoads + 1);

            for (int i = 0; i < m_mainRoads; i++)
            {
                float z = (i + 1) * spacing - m_worldSize.x / 2f;
                GameObject road = m_factory.CreateRoad(new Vector3(0, 0, z), new Vector2(m_roadWidth, length), 0);
                road.transform.SetParent(roadsRoot.transform);
            }

            length = m_worldSize.y;
            spacing = m_worldSize.y / (m_crossRoads + 1);

            for (int i = 0; i < m_crossRoads; i++)
            {
                float x = (i + 1) * spacing - m_worldSize.y / 2f;
                GameObject road = m_factory.CreateRoad(new Vector3(x, 0, 0), new Vector2(length, m_roadWidth), 90);
                road.transform.SetParent(roadsRoot.transform);
            }
        }

        private void CreateBuildings()
        {
            GameObject buildingsRoot = new GameObject("Buildings");
            buildingsRoot.transform.SetParent(m_root);

            for (int i = 0; i < m_buildingCount; i++)
            {
                Vector2 pos = GetRandomBuildingPosition();
                if (pos == Vector2.zero) continue;

                float width = Random.Range(12f, 18f);
                float height = Random.Range(15f, 25f);
                float depth = Random.Range(12f, 18f);
                float rotation = Random.Range(0, 4) * 90f;

                GameObject building = m_factory.CreateBuilding(new Vector3(pos.x, 0, pos.y), width, height, depth);
                building.transform.rotation = Quaternion.Euler(0, rotation, 0);
                building.transform.SetParent(buildingsRoot.transform);
            }
        }

        private Vector2 GetRandomBuildingPosition()
        {
            for (int attempts = 0; attempts < 50; attempts++)
            {
                Vector2 candidate = new Vector2(
                    Random.Range(-m_worldSize.x / 2f + 30f, m_worldSize.x / 2f - 30f),
                    Random.Range(-m_worldSize.y / 2f + 30f, m_worldSize.y / 2f - 30f)
                );

                if (!IsNearRoad(candidate) && !IsNearIntersection(candidate))
                {
                    return candidate;
                }
            }
            return Vector2.zero;
        }

        private bool IsNearRoad(Vector2 pos)
        {
            float minDist = m_roadWidth + 10f;
            float spacing = m_worldSize.x / (m_mainRoads + 1);

            for (int i = 0; i < m_mainRoads; i++)
            {
                float z = (i + 1) * spacing - m_worldSize.x / 2f;
                if (Mathf.Abs(pos.y - z) < minDist) return true;
            }

            spacing = m_worldSize.y / (m_crossRoads + 1);
            for (int i = 0; i < m_crossRoads; i++)
            {
                float x = (i + 1) * spacing - m_worldSize.y / 2f;
                if (Mathf.Abs(pos.x - x) < minDist) return true;
            }

            return false;
        }

        private bool IsNearIntersection(Vector2 pos)
        {
            float minDist = m_roadWidth + 5f;
            float spacingX = m_worldSize.x / (m_mainRoads + 1);
            float spacingZ = m_worldSize.y / (m_crossRoads + 1);

            for (int i = 0; i < m_mainRoads; i++)
            {
                float z = (i + 1) * spacingX - m_worldSize.x / 2f;
                for (int j = 0; j < m_crossRoads; j++)
                {
                    float x = (j + 1) * spacingZ - m_worldSize.y / 2f;
                    if (Mathf.Abs(pos.x - x) < minDist && Mathf.Abs(pos.y - z) < minDist)
                        return true;
                }
            }
            return false;
        }

        private void CreateParkedCars()
        {
            GameObject vehiclesRoot = new GameObject("ParkedVehicles");
            vehiclesRoot.transform.SetParent(m_root);

            for (int i = 0; i < m_parkedCarCount; i++)
            {
                Vector2 pos = GetRandomCarPosition();
                if (pos == Vector2.zero) continue;

                float rotation = GetCarRotation(pos);
                GameObject car = m_factory.CreateParkedCar(new Vector3(pos.x, 0.4f, pos.y), rotation);
                car.transform.SetParent(vehiclesRoot.transform);
            }
        }

        private Vector2 GetRandomCarPosition()
        {
            for (int attempts = 0; attempts < 50; attempts++)
            {
                Vector2 candidate = new Vector2(
                    Random.Range(-m_worldSize.x / 2f + 15f, m_worldSize.x / 2f - 15f),
                    Random.Range(-m_worldSize.y / 2f + 15f, m_worldSize.y / 2f - 15f)
                );

                if (IsNearRoad(candidate) && !IsNearIntersection(candidate))
                {
                    return candidate;
                }
            }
            return Vector2.zero;
        }

        private float GetCarRotation(Vector2 pos)
        {
            float spacing = m_worldSize.x / (m_mainRoads + 1);
            for (int i = 0; i < m_mainRoads; i++)
            {
                float z = (i + 1) * spacing - m_worldSize.x / 2f;
                if (Mathf.Abs(pos.y - z) < m_roadWidth + 5f) return 90f;
            }
            return 0f;
        }

        private void CreateEnvironment()
        {
            GameObject envRoot = new GameObject("Environment");
            envRoot.transform.SetParent(m_root);

            for (int i = 0; i < m_treeCount; i++)
            {
                Vector3 pos = GetRandomEnvPosition();
                if (pos.y < -100f) continue;

                GameObject tree = m_factory.CreateTree(pos);
                tree.transform.SetParent(envRoot.transform);
            }

            for (int i = 0; i < m_rockCount; i++)
            {
                Vector3 pos = GetRandomEnvPosition();
                if (pos.y < -100f) continue;

                GameObject rock = m_factory.CreateRock(pos);
                rock.transform.SetParent(envRoot.transform);
            }
        }

        private Vector3 GetRandomEnvPosition()
        {
            for (int attempts = 0; attempts < 20; attempts++)
            {
                Vector3 candidate = new Vector3(
                    Random.Range(-m_worldSize.x / 2f, m_worldSize.x / 2f),
                    50f,
                    Random.Range(-m_worldSize.y / 2f, m_worldSize.y / 2f)
                );

                if (Physics.Raycast(candidate, Vector3.down, out RaycastHit hit, 100f))
                {
                    return hit.point;
                }
            }
            return new Vector3(0, -1000f, 0);
        }

        private void SpawnPlayerCar()
        {
            if (m_playerCarPrefab == null)
            {
                Debug.LogWarning("[OpenWorldBuilder] No player car prefab assigned!");
                return;
            }

            GameObject player = Instantiate(m_playerCarPrefab, Vector3.zero, Quaternion.identity);
            player.name = "PlayerCar";
            player.tag = "Player";
            player.transform.SetParent(m_root);
        }

        private void SetupLighting()
        {
            if (FindObjectOfType<Light>() == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(0, 10f, -15f);
                cam.transform.rotation = Quaternion.Euler(20f, 0f, 0f);

                var followCam = cam.GetComponent<CarSimulator.Camera.FollowCamera>();
                if (followCam == null)
                {
                    followCam = cam.gameObject.AddComponent<CarSimulator.Camera.FollowCamera>();
                }
            }
        }
    }
}
