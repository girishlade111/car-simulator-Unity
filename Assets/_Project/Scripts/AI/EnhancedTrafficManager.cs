using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class EnhancedTrafficManager : MonoBehaviour
    {
        public static EnhancedTrafficManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private int m_maxVehicles = 20;
        [SerializeField] private float m_spawnRadius = 100f;
        [SerializeField] private float m_despawnDistance = 150f;

        [Header("Traffic Vehicles")]
        [SerializeField] private GameObject[] m_vehiclePrefabs;
        [SerializeField] private EnhancedTrafficCar.VehicleType[] m_allowedTypes;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] m_spawnPoints;
        [SerializeField] private Transform[] m_despawnPoints;

        [Header("Paths")]
        [SerializeField] private bool m_usePaths = true;
        [SerializeField] private TrafficPath[] m_trafficPaths;

        [Header("Performance")]
        [SerializeField] private bool m_lodEnabled = true;
        [SerializeField] private float m_lodDistance = 80f;
        [SerializeField] private bool m_physicsOnDemand = true;

        [Header("References")]
        [SerializeField] private Transform m_playerTransform;

        private List<EnhancedTrafficCar> m_activeVehicles = new List<EnhancedTrafficCar>();
        private List<TrafficPath> m_availablePaths = new List<TrafficPath>();

        private float m_spawnTimer;
        private float m_spawnInterval = 3f;

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
            InitializePaths();
            SpawnInitialTraffic();
        }

        private void Update()
        {
            if (!m_enabled) return;

            UpdateSpawning();
            UpdateVehicleLOD();
            RemoveDistantVehicles();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void InitializePaths()
        {
            if (m_usePaths)
            {
                var paths = FindObjectsOfType<TrafficPath>();
                m_availablePaths.AddRange(paths);
            }
        }

        private void SpawnInitialTraffic()
        {
            int initialSpawn = Mathf.Min(m_maxVehicles / 2, 10);

            for (int i = 0; i < initialSpawn; i++)
            {
                SpawnVehicle();
            }
        }

        private void UpdateSpawning()
        {
            if (m_playerTransform == null) return;

            float distToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);

            if (distToPlayer > m_spawnRadius * 2)
            {
                return;
            }

            m_spawnTimer += Time.deltaTime;

            float adjustedInterval = m_spawnInterval;
            if (distToPlayer < 50f)
            {
                adjustedInterval = m_spawnInterval * 0.5f;
            }

            if (m_spawnTimer >= adjustedInterval && m_activeVehicles.Count < m_maxVehicles)
            {
                m_spawnTimer = 0;
                SpawnVehicle();
            }
        }

        private void SpawnVehicle()
        {
            if (m_vehiclePrefabs == null || m_vehiclePrefabs.Length == 0) return;

            Vector3 spawnPos = GetSpawnPosition();
            if (spawnPos == Vector3.zero) return;

            GameObject prefab = m_vehiclePrefabs[Random.Range(0, m_vehiclePrefabs.Length)];
            GameObject vehicle = Instantiate(prefab, spawnPos, GetSpawnRotation());

            var enhancedCar = vehicle.GetComponent<EnhancedTrafficCar>();
            if (enhancedCar != null)
            {
                enhancedCar.SetWaypoints(GetRandomWaypoints());
                enhancedCar.SetSpeed(GetRandomSpeed());
            }

            var basicCar = vehicle.GetComponent<TrafficCar>();
            if (basicCar != null)
            {
                basicCar.SetWaypoints(GetRandomTransformWaypoints());
                basicCar.SetSpeed(GetRandomSpeed());
            }

            m_activeVehicles.Add(enhancedCar ?? basicCar);
        }

        private Vector3 GetSpawnPosition()
        {
            if (m_spawnPoints != null && m_spawnPoints.Length > 0)
            {
                Transform spawn = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
                return spawn.position + Random.insideUnitSphere * 5f;
            }

            if (m_playerTransform == null) return Vector3.zero;

            Vector2 randomPoint = Random.insideUnitCircle * m_spawnRadius;
            Vector3 spawnPos = new Vector3(randomPoint.x, 0, randomPoint.y) + m_playerTransform.position;

            if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
            {
                spawnPos.y = hit.point.y;
            }

            return spawnPos;
        }

        private Quaternion GetSpawnRotation()
        {
            if (m_availablePaths.Count > 0)
            {
                TrafficPath path = m_availablePaths[Random.Range(0, m_availablePaths.Count)];
                return Quaternion.LookRotation(path.GetDirection());
            }

            return Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        private Transform[] GetRandomWaypoints()
        {
            if (m_availablePaths.Count == 0) return null;

            List<Transform> waypoints = new List<Transform>();
            TrafficPath path = m_availablePaths[Random.Range(0, m_availablePaths.Count)];

            for (int i = 0; i < path.GetWaypointCount(); i++)
            {
                Transform wp = path.GetWaypoint(i);
                if (wp != null)
                {
                    waypoints.Add(wp);
                }
            }

            return waypoints.ToArray();
        }

        private Transform[] GetRandomTransformWaypoints()
        {
            var allWaypoints = FindObjectsOfType<TrafficWaypoint>();
            if (allWaypoints.Length == 0) return null;

            List<Transform> waypoints = new List<Transform>();
            int count = Mathf.Min(5, allWaypoints.Length);

            for (int i = 0; i < count; i++)
            {
                waypoints.Add(allWaypoints[Random.Range(0, allWaypoints.Length)].transform);
            }

            return waypoints.ToArray();
        }

        private float GetRandomSpeed()
        {
            return Random.Range(40f, 80f);
        }

        private void UpdateVehicleLOD()
        {
            if (!m_lodEnabled || m_playerTransform == null) return;

            foreach (var vehicle in m_activeVehicles)
            {
                if (vehicle == null) continue;

                float dist = Vector3.Distance(m_playerTransform.position, vehicle.transform.position);

                if (dist > m_lodDistance)
                {
                    SetVehicleLOD(vehicle, true);
                }
                else
                {
                    SetVehicleLOD(vehicle, false);
                }
            }
        }

        private void SetVehicleLOD(EnhancedTrafficCar vehicle, bool highLOD)
        {
            if (vehicle == null) return;

            Renderer[] renderers = vehicle.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = highLOD;
            }
        }

        private void RemoveDistantVehicles()
        {
            if (m_playerTransform == null) return;

            for (int i = m_activeVehicles.Count - 1; i >= 0; i--)
            {
                if (m_activeVehicles[i] == null)
                {
                    m_activeVehicles.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(m_playerTransform.position, m_activeVehicles[i].transform.position);

                if (dist > m_despawnDistance)
                {
                    Destroy(m_activeVehicles[i].gameObject);
                    m_activeVehicles.RemoveAt(i);
                }
            }
        }

        public void AddSpawnPoint(Transform spawnPoint)
        {
            if (m_spawnPoints == null)
            {
                m_spawnPoints = new Transform[] { spawnPoint };
            }
            else
            {
                System.Array.Resize(ref m_spawnPoints, m_spawnPoints.Length + 1);
                m_spawnPoints[m_spawnPoints.Length - 1] = spawnPoint;
            }
        }

        public void AddPath(TrafficPath path)
        {
            if (!m_availablePaths.Contains(path))
            {
                m_availablePaths.Add(path);
            }
        }

        public int GetActiveVehicleCount() => m_activeVehicles.Count;
    }
}
