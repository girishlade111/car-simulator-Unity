using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class VehicleSpawner : MonoBehaviour
    {
        public static VehicleSpawner Instance { get; private set; }

        [Header("Vehicle Prefab")]
        [SerializeField] private GameObject m_vehiclePrefab;
        [SerializeField] private VehicleTuning m_defaultTuning;

        [Header("Spawn Settings")]
        [SerializeField] private bool m_spawnOnStart = true;
        [SerializeField] private bool m_buildIfNoPrefab = true;

        [Header("References")]
        [SerializeField] private Transform m_spawnPoint;

        private GameObject m_currentVehicle;

        public GameObject CurrentVehicle => m_currentVehicle;
        public Transform VehicleTransform => m_currentVehicle != null ? m_currentVehicle.transform : null;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (m_defaultTuning == null)
            {
                m_defaultTuning = CreateDefaultTuning();
            }
        }

        private VehicleTuning CreateDefaultTuning()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            tuning.name = "DefaultTuning";
            return tuning;
        }

        private void Start()
        {
            if (m_spawnOnStart)
            {
                SpawnVehicle();
            }
        }

        public void SpawnVehicle()
        {
            if (m_currentVehicle != null)
            {
                Destroy(m_currentVehicle);
            }

            Vector3 spawnPos = GetSpawnPosition();
            Quaternion spawnRot = GetSpawnRotation();

            if (m_vehiclePrefab != null)
            {
                m_currentVehicle = Instantiate(m_vehiclePrefab, spawnPos, spawnRot);
            }
            else if (m_buildIfNoPrefab)
            {
                m_currentVehicle = BuildDefaultVehicle(spawnPos, spawnRot);
            }
            else
            {
                Debug.LogError("[VehicleSpawner] No vehicle prefab assigned and buildIfNoPrefab is false!");
                return;
            }

            m_currentVehicle.name = "PlayerVehicle";
            m_currentVehicle.tag = "Player";
            m_currentVehicle.layer = LayerMask.NameToLayer("Player");

            RegisterWithSpawnManager();
        }

        private GameObject BuildDefaultVehicle(Vector3 position, Quaternion rotation)
        {
            GameObject builderObj = new GameObject("TempBuilder");
            PlayerVehicleBuilder builder = builderObj.AddComponent<PlayerVehicleBuilder>();
            builder.SetTuning(m_defaultTuning);
            
            GameObject vehicle = builder.BuildVehicle();
            vehicle.transform.position = position;
            vehicle.transform.rotation = rotation;

            Destroy(builderObj);
            return vehicle;
        }

        private Vector3 GetSpawnPosition()
        {
            if (m_spawnPoint != null)
            {
                return m_spawnPoint.position;
            }

            var spawnPoint = FindObjectOfType<SpawnPoint>();
            if (spawnPoint != null)
            {
                return spawnPoint.Position + Vector3.up * 2f;
            }

            return Vector3.zero + Vector3.up * 2f;
        }

        private Quaternion GetSpawnRotation()
        {
            if (m_spawnPoint != null)
            {
                return m_spawnPoint.rotation;
            }

            var spawnPoint = FindObjectOfType<SpawnPoint>();
            if (spawnPoint != null)
            {
                return spawnPoint.Rotation;
            }

            return Quaternion.identity;
        }

        private void RegisterWithSpawnManager()
        {
            var spawnManager = FindObjectOfType<World.SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.SetPlayerSpawnPoint(m_currentVehicle.transform);
            }
        }

        public void SetSpawnPoint(Transform spawnPoint)
        {
            m_spawnPoint = spawnPoint;
        }

        public void RespawnVehicle()
        {
            SpawnVehicle();
        }
    }
}
