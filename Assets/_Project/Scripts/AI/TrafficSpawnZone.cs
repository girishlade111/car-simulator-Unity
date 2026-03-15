using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    [ExecuteInEditMode]
    public class TrafficSpawnZone : MonoBehaviour
    {
        [Header("Zone Settings")]
        [SerializeField] private string m_zoneName = "Traffic Zone";
        [SerializeField] private ZoneType m_zoneType = ZoneType.Road;

        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] m_vehiclePrefabs;
        [SerializeField] private int m_maxVehicles = 5;
        [SerializeField] private float m_spawnInterval = 5f;
        [SerializeField] private float m_despawnDistance = 100f;

        [Header("Waypoints")]
        [SerializeField] private Transform[] m_entryWaypoints;
        [SerializeField] private Transform[] m_exitWaypoints;
        [SerializeField] private Lane[] m_lanes;

        [Header("Traffic Rules")]
        [SerializeField] private float m_speedLimit = 60f;
        [SerializeField] private bool m_allowPedestrians = true;
        [SerializeField] private bool m_allowParking = true;

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_gizmoColor = new Color(1f, 0.5f, 0f, 0.3f);

        [Header("Active Vehicles")]
        [SerializeField] private List<GameObject> m_activeVehicles = new List<GameObject>();

        private float m_spawnTimer;
        private Transform m_playerTransform;

        public enum ZoneType
        {
            Road,
            Highway,
            Residential,
            Commercial,
            Industrial,
            ParkingLot
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                ValidateWaypoints();
            }
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                FindPlayer();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying) return;

            m_spawnTimer += Time.deltaTime;

            if (m_spawnTimer >= m_spawnInterval && m_activeVehicles.Count < m_maxVehicles)
            {
                m_spawnTimer = 0;
                TrySpawnVehicle();
            }

            ManageVehicles();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void ValidateWaypoints()
        {
            if (m_lanes == null || m_lanes.Length == 0)
            {
                GenerateDefaultLanes();
            }
        }

        private void GenerateDefaultLanes()
        {
            m_lanes = new Lane[2];

            m_lanes[0] = new Lane
            {
                direction = transform.forward,
                isForward = true,
                speedLimit = m_speedLimit
            };

            m_lanes[1] = new Lane
            {
                direction = -transform.forward,
                isForward = false,
                speedLimit = m_speedLimit
            };
        }

        private void TrySpawnVehicle()
        {
            if (m_playerTransform != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
                if (distToPlayer > 150f)
                {
                    return;
                }
            }

            if (m_vehiclePrefabs == null || m_vehiclePrefabs.Length == 0)
            {
                return;
            }

            GameObject prefab = m_vehiclePrefabs[Random.Range(0, m_vehiclePrefabs.Length)];
            Vector3 spawnPos = GetSpawnPosition();
            Quaternion spawnRot = GetSpawnRotation();

            GameObject vehicle = Instantiate(prefab, spawnPos, spawnRot);
            vehicle.transform.SetParent(transform);

            var enhancedCar = vehicle.GetComponent<EnhancedTrafficCar>();
            if (enhancedCar != null)
            {
                enhancedCar.SetSpeed(m_speedLimit);
            }

            var basicCar = vehicle.GetComponent<TrafficCar>();
            if (basicCar != null)
            {
                basicCar.SetSpeed(m_speedLimit);
            }

            m_activeVehicles.Add(vehicle);
        }

        private Vector3 GetSpawnPosition()
        {
            if (m_entryWaypoints != null && m_entryWaypoints.Length > 0)
            {
                Transform entry = m_entryWaypoints[Random.Range(0, m_entryWaypoints.Length)];
                return entry.position + Random.insideUnitSphere * 2f;
            }

            return transform.position + transform.forward * 5f;
        }

        private Quaternion GetSpawnRotation()
        {
            return Quaternion.LookRotation(transform.forward);
        }

        private void ManageVehicles()
        {
            for (int i = m_activeVehicles.Count - 1; i >= 0; i--)
            {
                if (m_activeVehicles[i] == null)
                {
                    m_activeVehicles.RemoveAt(i);
                    continue;
                }

                if (m_playerTransform != null)
                {
                    float dist = Vector3.Distance(m_activeVehicles[i].transform.position, m_playerTransform.position);
                    if (dist > m_despawnDistance)
                    {
                        Destroy(m_activeVehicles[i]);
                        m_activeVehicles.RemoveAt(i);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_gizmoColor;
            Gizmos.DrawCube(transform.position, new Vector3(10f, 1f, 10f));

            Gizmos.color = Color.white;
            if (m_entryWaypoints != null)
            {
                foreach (var wp in m_entryWaypoints)
                {
                    if (wp != null)
                    {
                        Gizmos.DrawWireSphere(wp.position, 1f);
                    }
                }
            }

            if (m_exitWaypoints != null)
            {
                foreach (var wp in m_exitWaypoints)
                {
                    if (wp != null)
                    {
                        Gizmos.DrawWireSphere(wp.position, 0.5f);
                    }
                }
            }
        }

        [System.Serializable]
        public class Lane
        {
            public Vector3 direction;
            public bool isForward;
            public float speedLimit;
            public int vehicleCount;
            public Transform path;
        }

        public int GetVehicleCount() => m_activeVehicles.Count;
    }
}
