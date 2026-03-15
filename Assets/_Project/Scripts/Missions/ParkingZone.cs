using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Missions
{
    public class ParkingZone : MonoBehaviour
    {
        [Header("Parking Zone Settings")]
        [SerializeField] private string m_zoneId;
        [SerializeField] private string m_zoneName = "Parking Zone";
        [SerializeField] private bool m_isActive = true;

        [Header("Zone Parameters")]
        [SerializeField] private Vector3 m_zoneCenter;
        [SerializeField] private Vector2 m_zoneSize = new Vector2(5f, 10f);
        [SerializeField] private float m_requiredParkingTime = 2f;

        [Header("Validation")]
        [SerializeField] private bool m_requireVehicleStopped = true;
        [SerializeField] private bool m_requireEngineOff;
        [SerializeField] private float m_maxSpeedToPark = 2f;

        [Header("State")]
        [SerializeField] private bool m_isOccupied;
        [SerializeField] private GameObject m_parkedVehicle;
        [SerializeField] private float m_parkingTimer;

        [Header("Visual")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_zoneColor = new Color(0f, 1f, 0f, 0.3f);

        private VehicleController m_vehicleController;

        private void Start()
        {
            m_zoneCenter = transform.position;
        }

        private void Update()
        {
            if (!m_isActive || !m_isOccupied) return;

            CheckParkingState();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!m_isActive || m_isOccupied) return;

            if (other.CompareTag("Player"))
            {
                m_parkedVehicle = other.gameObject;
                m_vehicleController = other.GetComponent<VehicleController>();
                m_isOccupied = true;
                m_parkingTimer = 0f;

                Debug.Log($"[ParkingZone] Vehicle entered: {m_zoneName}");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.gameObject == m_parkedVehicle)
            {
                m_isOccupied = false;
                m_parkedVehicle = null;
                m_vehicleController = null;
                m_parkingTimer = 0f;

                Debug.Log($"[ParkingZone] Vehicle exited: {m_zoneName}");
            }
        }

        private void CheckParkingState()
        {
            if (m_vehicleController == null) return;

            float currentSpeed = m_vehicleController.CurrentSpeed;

            bool speedCheck = !m_requireVehicleStopped || currentSpeed <= m_maxSpeedToPark;

            if (speedCheck)
            {
                m_parkingTimer += Time.deltaTime;
            }
            else
            {
                m_parkingTimer = 0f;
            }
        }

        public bool IsPlayerParked()
        {
            if (!m_isOccupied || m_parkedVehicle == null) return false;

            if (m_requireVehicleStopped && m_vehicleController != null)
            {
                if (m_vehicleController.CurrentSpeed > m_maxSpeedToPark) return false;
            }

            return m_parkingTimer >= m_requiredParkingTime;
        }

        public float GetParkingProgress()
        {
            if (m_requiredParkingTime <= 0) return 1f;
            return Mathf.Clamp01(m_parkingTimer / m_requiredParkingTime);
        }

        public bool IsOccupied() => m_isOccupied;
        public string GetZoneId() => m_zoneId;
        public Vector3 GetZoneCenter() => m_zoneCenter;

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_zoneColor;
            Gizmos.DrawCube(m_zoneCenter, new Vector3(m_zoneSize.x, 1f, m_zoneSize.y));

            Gizmos.color = IsOccupied() ? Color.green : Color.white;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
        }
    }

    public class ParkingManager : MonoBehaviour
    {
        public static ParkingManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_autoFindZones = true;
        [SerializeField] private List<ParkingZone> m_parkingZones = new List<ParkingZone>();
        [SerializeField] private float m_updateInterval = 0.5f;

        [Header("State")]
        [SerializeField] private ParkingZone m_nearestZone;
        [SerializeField] private float m_nearestDistance;
        [SerializeField] private float m_lastUpdateTime;

        private Transform m_playerTransform;

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
            if (m_autoFindZones)
            {
                var zones = FindObjectsOfType<ParkingZone>();
                m_parkingZones.AddRange(zones);
            }

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (Time.time - m_lastUpdateTime > m_updateInterval)
            {
                UpdateNearestZone();
                m_lastUpdateTime = Time.time;
            }
        }

        private void UpdateNearestZone()
        {
            if (m_playerTransform == null) return;

            m_nearestDistance = float.MaxValue;
            m_nearestZone = null;

            foreach (var zone in m_parkingZones)
            {
                if (zone == null || !zone.isActive) continue;

                float dist = Vector3.Distance(m_playerTransform.position, zone.GetZoneCenter());
                if (dist < m_nearestDistance)
                {
                    m_nearestDistance = dist;
                    m_nearestZone = zone;
                }
            }
        }

        public ParkingZone GetNearestZone()
        {
            return m_nearestZone;
        }

        public float GetDistanceToNearestZone()
        {
            return m_nearestDistance;
        }

        public bool IsPlayerInParkingZone()
        {
            return m_nearestZone != null && m_nearestZone.IsOccupied();
        }

        public bool IsPlayerParked()
        {
            if (m_nearestZone == null) return false;
            return m_nearestZone.IsPlayerParked();
        }

        public void RegisterZone(ParkingZone zone)
        {
            if (!m_parkingZones.Contains(zone))
            {
                m_parkingZones.Add(zone);
            }
        }
    }
}
