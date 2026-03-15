using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class EmergencyResponseSystem : MonoBehaviour
    {
        public static EmergencyResponseSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableResponses = true;
        [SerializeField] private float m_responseDelay = 10f;

        [Header("Vehicle Prefabs")]
        [SerializeField] private GameObject m_fireTruckPrefab;
        [SerializeField] private GameObject m_ambulancePrefab;
        [SerializeField] private GameObject m_policeCarPrefab;

        [Header("Response Settings")]
        [SerializeField] private int m_maxFireTrucks = 3;
        [SerializeField] private int m_maxAmbulances = 3;
        [SerializeField] private int m_maxPoliceCars = 5;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] m_emergencySpawnPoints;

        private List<EmergencyVehicle> m_activeFireTrucks = new List<EmergencyVehicle>();
        private List<EmergencyVehicle> m_activeAmbulances = new List<EmergencyVehicle>();
        private List<EmergencyVehicle> m_activePoliceCars = new List<EmergencyVehicle>();

        private Dictionary<string, EmergencyCall> m_pendingCalls = new Dictionary<string, EmergencyCall>();

        public enum EmergencyType
        {
            Fire,
            Medical,
            Crime,
            TrafficAccident
        }

        [System.Serializable]
        public class EmergencyCall
        {
            public string callId;
            public EmergencyType type;
            public Vector3 location;
            public float timeReceived;
            public float priority;
            public bool isResponded;
        }

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
            FindEmergencySpawnPoints();
        }

        private void FindEmergencySpawnPoints()
        {
            if (m_emergencySpawnPoints == null || m_emergencySpawnPoints.Length == 0)
            {
                var spawns = FindObjectsOfType<EmergencySpawnPoint>();
                m_emergencySpawnPoints = new Transform[spawns.Length];
                for (int i = 0; i < spawns.Length; i++)
                {
                    m_emergencySpawnPoints[i] = spawns[i].transform;
                }
            }
        }

        public void ReceiveEmergencyCall(EmergencyType type, Vector3 location, float priority = 1f)
        {
            if (!m_enableResponses) return;

            string callId = System.Guid.NewGuid().ToString();
            EmergencyCall call = new EmergencyCall
            {
                callId = callId,
                type = type,
                location = location,
                timeReceived = Time.time,
                priority = priority,
                isResponded = false
            };

            m_pendingCalls[callId] = call;
            Debug.Log($"[Emergency] Received {type} call at {location}");

            ProcessCall(call);
        }

        private void ProcessCall(EmergencyCall call)
        {
            switch (call.type)
            {
                case EmergencyType.Fire:
                    RespondToFire(call);
                    break;
                case EmergencyType.Medical:
                    RespondToMedical(call);
                    break;
                case EmergencyType.Crime:
                case EmergencyType.TrafficAccident:
                    RespondToPolice(call);
                    break;
            }
        }

        private void RespondToFire(EmergencyCall call)
        {
            if (m_activeFireTrucks.Count >= m_maxFireTrucks)
            {
                Debug.Log("[Emergency] All fire trucks busy");
                return;
            }

            GameObject vehicle = SpawnEmergencyVehicle(m_fireTruckPrefab, "FireTruck");
            if (vehicle != null)
            {
                EmergencyVehicle emergency = vehicle.AddComponent<EmergencyVehicle>();
                emergency.Initialize(call.location, EmergencyVehicle.VehicleType.FireTruck);
                m_activeFireTrucks.Add(emergency);
                call.isResponded = true;
            }
        }

        private void RespondToMedical(EmergencyCall call)
        {
            if (m_activeAmbulances.Count >= m_maxAmbulances)
            {
                Debug.Log("[Emergency] All ambulances busy");
                return;
            }

            GameObject vehicle = SpawnEmergencyVehicle(m_ambulancePrefab, "Ambulance");
            if (vehicle != null)
            {
                EmergencyVehicle emergency = vehicle.AddComponent<EmergencyVehicle>();
                emergency.Initialize(call.location, EmergencyVehicle.VehicleType.Ambulance);
                m_activeAmbulances.Add(emergency);
                call.isResponded = true;
            }
        }

        private void RespondToPolice(EmergencyCall call)
        {
            if (m_activePoliceCars.Count >= m_maxPoliceCars)
            {
                Debug.Log("[Emergency] All police cars busy");
                return;
            }

            GameObject vehicle = SpawnEmergencyVehicle(m_policeCarPrefab, "PoliceCar");
            if (vehicle != null)
            {
                EmergencyVehicle emergency = vehicle.AddComponent<EmergencyVehicle>();
                emergency.Initialize(call.location, EmergencyVehicle.VehicleType.PoliceCar);
                m_activePoliceCars.Add(emergency);
                call.isResponded = true;
            }
        }

        private GameObject SpawnEmergencyVehicle(GameObject prefab, string name)
        {
            Vector3 spawnPos = GetRandomSpawnPoint();
            spawnPos.y = 0.5f;

            GameObject vehicle;
            if (prefab != null)
            {
                vehicle = Instantiate(prefab, spawnPos, Quaternion.identity);
            }
            else
            {
                vehicle = CreatePlaceholderVehicle(name);
            }

            vehicle.name = name;
            return vehicle;
        }

        private GameObject CreatePlaceholderVehicle(string name)
        {
            GameObject vehicle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vehicle.transform.localScale = new Vector3(2f, 1.5f, 4f);

            Renderer renderer = vehicle.GetComponent<Renderer>();
            if (renderer != null)
            {
                switch (name)
                {
                    case "FireTruck":
                        renderer.material.color = Color.red;
                        break;
                    case "Ambulance":
                        renderer.material.color = Color.white;
                        break;
                    case "PoliceCar":
                        renderer.material.color = Color.blue;
                        break;
                }
            }

            return vehicle;
        }

        private Vector3 GetRandomSpawnPoint()
        {
            if (m_emergencySpawnPoints != null && m_emergencySpawnPoints.Length > 0)
            {
                return m_emergencySpawnPoints[Random.Range(0, m_emergencySpawnPoints.Length)].position;
            }

            return new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        }

        public void RegisterFireAt(Vector3 location)
        {
            ReceiveEmergencyCall(EmergencyType.Fire, location, 2f);
        }

        public void RegisterMedicalEmergency(Vector3 location)
        {
            ReceiveEmergencyCall(EmergencyType.Medical, location, 1.5f);
        }

        public void RegisterCrime(Vector3 location)
        {
            ReceiveEmergencyCall(EmergencyType.Crime, location, 1f);
        }

        public void RegisterAccident(Vector3 location)
        {
            ReceiveEmergencyCall(EmergencyType.TrafficAccident, location, 1f);
        }
    }

    public class EmergencyVehicle : MonoBehaviour
    {
        public enum VehicleType
        {
            FireTruck,
            Ambulance,
            PoliceCar
        }

        [Header("Settings")]
        [SerializeField] private VehicleType m_vehicleType;
        [SerializeField] private float m_speed = 60f;
        [SerializeField] private float m_destinationReachedDistance = 10f;

        [Header("State")]
        [SerializeField] private Vector3 m_destination;
        [SerializeField] private bool m_isResponding;
        [SerializeField] private bool m_isAtDestination;
        [SerializeField] private float m_serviceDuration = 30f;
        [SerializeField] private float m_serviceTimer;

        private Rigidbody m_rb;
        private bool m_initialized;

        public void Initialize(Vector3 destination, VehicleType type)
        {
            m_vehicleType = type;
            m_destination = destination;
            m_isResponding = true;

            m_rb = GetComponent<Rigidbody>();
            if (m_rb == null)
            {
                m_rb = gameObject.AddComponent<Rigidbody>();
            }
            m_rb.mass = 3000f;
            m_rb.useGravity = true;
            m_rb.isKinematic = false;

            m_initialized = true;
        }

        private void Update()
        {
            if (!m_initialized) return;

            if (m_isResponding && !m_isAtDestination)
            {
                MoveToDestination();
            }
            else if (m_isAtDestination)
            {
                PerformService();
            }
        }

        private void MoveToDestination()
        {
            float distance = Vector3.Distance(transform.position, m_destination);

            if (distance <= m_destinationReachedDistance)
            {
                m_isAtDestination = true;
                m_isResponding = false;
                ArrivedAtScene();
                return;
            }

            Vector3 direction = (m_destination - transform.position).normalized;
            direction.y = 0;

            transform.position += direction * m_speed * Time.deltaTime;
            transform.LookAt(m_destination);
        }

        private void ArrivedAtScene()
        {
            switch (m_vehicleType)
            {
                case VehicleType.FireTruck:
                    ArrivedAtFire();
                    break;
                case VehicleType.Ambulance:
                    ArrivedAtMedical();
                    break;
                case VehicleType.PoliceCar:
                    ArrivedAtCrime();
                    break;
            }
        }

        private void ArrivedAtFire()
        {
            Debug.Log("[Emergency] Fire truck arrived");

            var fireSystem = FindObjectOfType<World.BuildingFireSystem>();
            if (fireSystem != null)
            {
                fireSystem.ExtinguishAt(transform.position, 15f);
            }

            m_serviceTimer = m_serviceDuration;
        }

        private void ArrivedAtMedical()
        {
            Debug.Log("[Emergency] Ambulance arrived");
            m_serviceTimer = m_serviceDuration;
        }

        private void ArrivedAtCrime()
        {
            Debug.Log("[Emergency] Police car arrived");
            m_serviceTimer = m_serviceDuration * 0.5f;
        }

        private void PerformService()
        {
            m_serviceTimer -= Time.deltaTime;

            if (m_serviceTimer <= 0)
            {
                ReturnToStation();
            }
        }

        private void ReturnToStation()
        {
            m_isAtDestination = false;
            m_isResponding = true;
            m_destination = GetRandomStationLocation();

            Debug.Log("[Emergency] Returning to station");
        }

        private Vector3 GetRandomStationLocation()
        {
            return new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
        }
    }

    public class EmergencySpawnPoint : MonoBehaviour
    {
        [Header("Station Info")]
        [SerializeField] private string m_stationName;
        [SerializeField] private EmergencySpawnPoint.StationType m_stationType;

        public enum StationType
        {
            FireStation,
            Hospital,
            PoliceStation
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
}
