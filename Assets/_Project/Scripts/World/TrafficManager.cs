using UnityEngine;
using System.Collections.Generic;
using CarSimulator.AI;

namespace CarSimulator.World
{
    public class TrafficManager : MonoBehaviour
    {
        public static TrafficManager Instance { get; private set; }

        [Header("Traffic Settings")]
        [SerializeField] private int m_maxTrafficCars = 10;
        [SerializeField] private float m_spawnRadius = 100f;
        [SerializeField] private float m_despawnDistance = 150f;

        [Header("Traffic Cars")]
        [SerializeField] private GameObject[] m_trafficCarPrefabs;
        [SerializeField] private Transform[] m_spawnPoints;

        [Header("Optimization")]
        [SerializeField] private float m_updateInterval = 0.1f;

        [Header("References")]
        [SerializeField] private Transform m_playerTransform;

        private List<TrafficCar> m_activeCars = new List<TrafficCar>();
        private List<Transform> m_waypoints = new List<Transform>();
        private float m_lastUpdateTime;

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
            SpawnInitialTraffic();
        }

        private void Update()
        {
            if (Time.time - m_lastUpdateTime > m_updateInterval)
            {
                ManageTraffic();
                m_lastUpdateTime = Time.time;
            }
        }

        private void FindPlayer()
        {
            if (m_playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    m_playerTransform = player.transform;
                }
            }
        }

        private void FindWaypoints()
        {
            var waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
            foreach (var wp in waypointObjects)
            {
                m_waypoints.Add(wp.transform);
            }

            var roadSegments = FindObjectsOfType<RoadSegment>();
            foreach (var road in roadSegments)
            {
                GameObject wp = new GameObject($"RoadWaypoint_{m_waypoints.Count}");
                wp.transform.position = road.Center;
                wp.transform.SetParent(road.transform);
                m_waypoints.Add(wp.transform);
            }
        }

        private void SpawnInitialTraffic()
        {
            for (int i = 0; i < m_maxTrafficCars; i++)
            {
                SpawnTrafficCar();
            }
        }

        private void SpawnTrafficCar()
        {
            if (m_trafficCarPrefabs == null || m_trafficCarPrefabs.Length == 0) return;

            Vector3 spawnPos = GetRandomSpawnPosition();
            if (spawnPos == Vector3.zero) return;

            GameObject prefab = m_trafficCarPrefabs[Random.Range(0, m_trafficCarPrefabs.Length)];
            GameObject carObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            
            TrafficCar trafficCar = carObj.GetComponent<TrafficCar>();
            if (trafficCar != null && m_waypoints.Count > 0)
            {
                Transform[] waypoints = GetRandomWaypointSubset(5);
                trafficCar.SetWaypoints(waypoints);
            }

            m_activeCars.Add(trafficCar);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            if (m_spawnPoints != null && m_spawnPoints.Length > 0)
            {
                Transform spawn = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
                return spawn.position + Random.insideUnitSphere * 10f;
            }

            if (m_playerTransform == null) return Vector3.zero;

            Vector2 randomPoint = Random.insideUnitCircle * m_spawnRadius;
            return new Vector3(randomPoint.x, 0, randomPoint.y) + m_playerTransform.position;
        }

        private Transform[] GetRandomWaypointSubset(int count)
        {
            if (m_waypoints.Count == 0) return null;

            List<Transform> subset = new List<Transform>();
            List<Transform> available = new List<Transform>(m_waypoints);

            for (int i = 0; i < Mathf.Min(count, m_waypoints.Count); i++)
            {
                if (available.Count == 0) break;
                int index = Random.Range(0, available.Count);
                subset.Add(available[index]);
                available.RemoveAt(index);
            }

            return subset.ToArray();
        }

        private void ManageTraffic()
        {
            if (m_playerTransform == null)
            {
                FindPlayer();
                if (m_playerTransform == null) return;
            }

            for (int i = m_activeCars.Count - 1; i >= 0; i--)
            {
                if (m_activeCars[i] == null)
                {
                    m_activeCars.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(m_playerTransform.position, m_activeCars[i].transform.position);
                
                if (dist > m_despawnDistance)
                {
                    Destroy(m_activeCars[i].gameObject);
                    m_activeCars.RemoveAt(i);
                }
            }

            while (m_activeCars.Count < m_maxTrafficCars)
            {
                SpawnTrafficCar();
            }
        }

        public void AddSpawnPoint(Transform spawnPoint)
        {
            if (m_spawnPoints == null || m_spawnPoints.Length == 0)
            {
                m_spawnPoints = new Transform[] { spawnPoint };
            }
            else
            {
                System.Array.Resize(ref m_spawnPoints, m_spawnPoints.Length + 1);
                m_spawnPoints[m_spawnPoints.Length - 1] = spawnPoint;
            }
        }

        public void AddWaypoint(Transform waypoint)
        {
            m_waypoints.Add(waypoint);
        }
    }
}
