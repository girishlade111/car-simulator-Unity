using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class EnhancedTrafficManager : MonoBehaviour
    {
        public static EnhancedTrafficManager Instance { get; private set; }

        [Header("Traffic Settings")]
        [SerializeField] private int m_maxTrafficCars = 20;
        [SerializeField] private float m_spawnRadius = 200f;
        [SerializeField] private float m_despawnDistance = 150f;
        [SerializeField] private float m_respawnDelay = 5f;

        [Header("Vehicle Prefabs")]
        [SerializeField] private GameObject m_sedanPrefab;
        [SerializeField] private GameObject m_suvPrefab;
        [SerializeField] private GameObject m_truckPrefab;
        [SerializeField] private GameObject m_sportsCarPrefab;
        [SerializeField] private GameObject m_taxiPrefab;
        [SerializeField] private GameObject m_policeCarPrefab;
        [SerializeField] private GameObject m_ambulancePrefab;
        [SerializeField] private GameObject m_busPrefab;
        [SerializeField] private GameObject m_motorcyclePrefab;

        [Header("Pathfinding")]
        [SerializeField] private bool m_useWaypoints = true;
        [SerializeField] private TrafficWaypoint[] m_waypoints;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] m_spawnPoints;
        [SerializeField] private bool m_spawnAtRandomPoints = true;

        [Header("Traffic Rules")]
        [SerializeField] private bool m_obeySpeedLimits = true;
        [SerializeField] private float m_citySpeedLimit = 50f;
        [SerializeField] private float m_highwaySpeedLimit = 100f;
        [SerializeField] private bool m_useLights = true;

        [Header("LOD")]
        [SerializeField] private float m LODDistance = 100f;
        [SerializeField] private bool m_enableLOD = true;

        private List<GameObject> m_activeCars = new List<GameObject>();
        private List<EnhancedTrafficCar> m_trafficScripts = new List<EnhancedTrafficCar>();
        private Transform m_playerTransform;
        private float m_spawnTimer;
        private bool m_isInitialized;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            FindPlayer();
            FindWaypoints();
            FindSpawnPoints();

            if (m_spawnAtRandomPoints)
            {
                InitializeTraffic();
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void FindWaypoints()
        {
            if (m_waypoints == null || m_waypoints.Length == 0)
            {
                m_waypoints = FindObjectsOfType<TrafficWaypoint>();
            }
        }

        private void FindSpawnPoints()
        {
            if (m_spawnPoints == null || m_spawnPoints.Length == 0)
            {
                var spawns = FindObjectsOfType<TrafficSpawnPoint>();
                m_spawnPoints = new Transform[spawns.Length];
                for (int i = 0; i < spawns.Length; i++)
                {
                    m_spawnPoints[i] = spawns[i].transform;
                }
            }
        }

        private void InitializeTraffic()
        {
            for (int i = 0; i < m_maxTrafficCars; i++)
            {
                SpawnTrafficCar();
            }
            m_isInitialized = true;
        }

        private void Update()
        {
            if (!m_isInitialized) return;

            UpdateTraffic();
            ManageTrafficSpawning();
            UpdateLOD();
        }

        private void UpdateTraffic()
        {
            for (int i = m_activeCars.Count - 1; i >= 0; i--)
            {
                if (m_activeCars[i] == null)
                {
                    m_activeCars.RemoveAt(i);
                    if (i < m_trafficScripts.Count)
                    {
                        m_trafficScripts.RemoveAt(i);
                    }
                    continue;
                }

                if (m_playerTransform != null)
                {
                    float dist = Vector3.Distance(m_activeCars[i].transform.position, m_playerTransform.position);
                    if (dist > m_despawnDistance)
                    {
                        DespawnCar(i);
                    }
                }
            }
        }

        private void ManageTrafficSpawning()
        {
            if (m_activeCars.Count >= m_maxTrafficCars) return;

            m_spawnTimer += Time.deltaTime;
            if (m_spawnTimer >= m_respawnDelay)
            {
                m_spawnTimer = 0f;
                SpawnTrafficCar();
            }
        }

        private void UpdateLOD()
        {
            if (!m_enableLOD || m_playerTransform == null) return;

            foreach (var car in m_activeCars)
            {
                if (car == null) continue;

                float dist = Vector3.Distance(car.transform.position, m_playerTransform.position);
                bool shouldEnable = dist < m_LODDistance;

                if (shouldEnable != car.activeSelf)
                {
                    car.SetActive(shouldEnable);
                }
            }
        }

        public GameObject SpawnTrafficCar()
        {
            GameObject prefab = GetRandomVehiclePrefab();
            if (prefab == null)
            {
                prefab = CreatePlaceholderCar();
            }

            Vector3 spawnPos = GetSpawnPosition();
            Quaternion spawnRot = GetSpawnRotation();

            GameObject car = Instantiate(prefab, spawnPos, spawnRot);
            EnhancedTrafficCar trafficAI = car.GetComponent<EnhancedTrafficCar>();

            if (trafficAI != null)
            {
                Transform[] path = GetRandomPath();
                if (path != null && path.Length > 0)
                {
                    trafficAI.SetWaypoints(path);
                }

                if (m_obeySpeedLimits)
                {
                    trafficAI.SetSpeed(m_citySpeedLimit);
                }
            }

            m_activeCars.Add(car);
            if (trafficAI != null)
            {
                m_trafficScripts.Add(trafficAI);
            }

            return car;
        }

        private GameObject GetRandomVehiclePrefab()
        {
            GameObject[] prefabs = { m_sedanPrefab, m_suvPrefab, m_truckPrefab, m_sportsCarPrefab, m_taxiPrefab, m_policeCarPrefab, m_ambulancePrefab, m_busPrefab, m_motorcyclePrefab };
            List<GameObject> validPrefabs = new List<GameObject>();

            foreach (var prefab in prefabs)
            {
                if (prefab != null)
                {
                    validPrefabs.Add(prefab);
                }
            }

            if (validPrefabs.Count > 0)
            {
                return validPrefabs[Random.Range(0, validPrefabs.Count)];
            }

            return null;
        }

        private GameObject CreatePlaceholderCar()
        {
            GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
            car.name = "TrafficCar";
            car.transform.localScale = new Vector3(1.8f, 1.2f, 4f);

            EnhancedTrafficCar ai = car.AddComponent<EnhancedTrafficCar>();

            GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hood.transform.SetParent(car.transform);
            hood.transform.localPosition = new Vector3(0, 0.3f, 1f);
            hood.transform.localScale = new Vector3(1.6f, 0.5f, 1.5f);

            return car;
        }

        private Vector3 GetSpawnPosition()
        {
            if (m_spawnPoints != null && m_spawnPoints.Length > 0)
            {
                Transform spawn = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
                return spawn.position;
            }

            if (m_playerTransform != null)
            {
                Vector2 circle = Random.insideUnitCircle.normalized * m_spawnRadius;
                return m_playerTransform.position + new Vector3(circle.x, 0, circle.y);
            }

            return new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
        }

        private Quaternion GetSpawnRotation()
        {
            float angle = Random.Range(0f, 360f);
            return Quaternion.Euler(0, angle, 0);
        }

        private Transform[] GetRandomPath()
        {
            if (m_waypoints == null || m_waypoints.Length == 0) return null;

            List<Transform> path = new List<Transform>();
            int startIndex = Random.Range(0, m_waypoints.Length);

            for (int i = 0; i < 5 && i < m_waypoints.Length; i++)
            {
                int idx = (startIndex + i) % m_waypoints.Length;
                if (m_waypoints[idx] != null)
                {
                    path.Add(m_waypoints[idx].transform);
                }
            }

            return path.ToArray();
        }

        private void DespawnCar(int index)
        {
            if (index < m_activeCars.Count && m_activeCars[index] != null)
            {
                Destroy(m_activeCars[index]);
            }
            m_activeCars.RemoveAt(index);
            if (index < m_trafficScripts.Count)
            {
                m_trafficScripts.RemoveAt(index);
            }
        }

        public void RemoveCar(GameObject car)
        {
            int index = m_activeCars.IndexOf(car);
            if (index >= 0)
            {
                DespawnCar(index);
            }
        }

        public void AddSpawnPoint(Transform point)
        {
            List<Transform> points = new List<Transform>(m_spawnPoints);
            points.Add(point);
            m_spawnPoints = points.ToArray();
        }

        public int GetActiveCarCount() => m_activeCars.Count;

        public void SetTrafficEnabled(bool enabled)
        {
            foreach (var car in m_activeCars)
            {
                if (car != null)
                {
                    car.SetActive(enabled);
                }
            }
        }
    }

    public class TrafficSpawnPoint : MonoBehaviour
    {
        [Header("Spawn Point Settings")]
        [SerializeField] private bool m_isEnabled = true;
        [SerializeField] private int m_spawnWeight = 1;
        [SerializeField] private VehicleType[] m_allowedVehicles;

        public bool IsEnabled => m_isEnabled;
        public int SpawnWeight => m_spawnWeight;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 1f);

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2f, new Vector3(3f, 4f, 6f));
        }
    }
}
