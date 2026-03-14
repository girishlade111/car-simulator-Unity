using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.World
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float m_respawnHeight = -20f;
        [SerializeField] private float m_respawnDelay = 2f;
        [SerializeField] private float m_boundaryBuffer = 10f;

        [Header("References")]
        [SerializeField] private GameObject m_playerVehiclePrefab;
        [SerializeField] private Transform m_playerSpawnPoint;
        [SerializeField] private DistrictManager m_districtManager;

        private GameObject m_currentPlayer;
        private SpawnPoint m_defaultSpawn;
        private float m_respawnTimer;
        private bool m_isRespawning;

        public GameObject CurrentPlayer => m_currentPlayer;
        public Transform PlayerTransform => m_currentPlayer != null ? m_currentPlayer.transform : null;

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
            SpawnPlayer();
        }

        private void Update()
        {
            if (m_currentPlayer == null || m_isRespawning) return;

            CheckForRespawn();
            CheckBoundary();
        }

        public void RegisterDefaultSpawn(SpawnPoint spawn)
        {
            m_defaultSpawn = spawn;
        }

        public void SpawnPlayer()
        {
            if (m_playerVehiclePrefab == null)
            {
                Debug.LogError("[SpawnManager] No player vehicle prefab assigned!");
                return;
            }

            Vector3 spawnPos;
            Quaternion spawnRot;

            if (m_playerSpawnPoint != null)
            {
                spawnPos = m_playerSpawnPoint.position;
                spawnRot = m_playerSpawnPoint.rotation;
            }
            else if (m_defaultSpawn != null)
            {
                spawnPos = m_defaultSpawn.Position;
                spawnRot = m_defaultSpawn.Rotation;
            }
            else
            {
                spawnPos = Vector3.zero + Vector3.up * 2f;
                spawnRot = Quaternion.identity;
            }

            m_currentPlayer = Instantiate(m_playerVehiclePrefab, spawnPos, spawnRot);
            m_currentPlayer.name = "PlayerVehicle";
            
            m_isRespawning = false;
            m_respawnTimer = 0f;
        }

        public void RespawnPlayer()
        {
            if (m_currentPlayer == null) return;

            Vector3 spawnPos;
            Quaternion spawnRot;

            if (m_defaultSpawn != null)
            {
                spawnPos = m_defaultSpawn.Position;
                spawnRot = m_defaultSpawn.Rotation;
            }
            else
            {
                spawnPos = Vector3.zero + Vector3.up * 2f;
                spawnRot = Quaternion.identity;
            }

            var physics = m_currentPlayer.GetComponent<VehiclePhysics>();
            if (physics != null)
            {
                physics.ResetVehicle();
            }

            m_currentPlayer.transform.position = spawnPos + Vector3.up * 2f;
            m_currentPlayer.transform.rotation = spawnRot;
            
            m_isRespawning = false;
            m_respawnTimer = 0f;
        }

        private void CheckForRespawn()
        {
            if (m_currentPlayer.transform.position.y < m_respawnHeight)
            {
                m_respawnTimer += Time.deltaTime;
                if (m_respawnTimer >= m_respawnDelay)
                {
                    m_isRespawning = true;
                    RespawnPlayer();
                }
            }
            else
            {
                m_respawnTimer = 0f;
            }
        }

        private void CheckBoundary()
        {
            if (m_districtManager == null) return;

            Vector2 playerPos = new Vector2(m_currentPlayer.transform.position.x, m_currentPlayer.transform.position.z);
            Vector2 boundarySize = m_districtManager.DistrictSize / 2f - new Vector2(m_boundaryBuffer, m_boundaryBuffer);

            if (!m_districtManager.IsPositionInDistrict(playerPos))
            {
                m_respawnTimer += Time.deltaTime;
                if (m_respawnTimer >= m_respawnDelay)
                {
                    m_isRespawning = true;
                    RespawnPlayer();
                }
            }
            else
            {
                m_respawnTimer = 0f;
            }
        }

        public void SetPlayerSpawnPoint(Transform spawnPoint)
        {
            m_playerSpawnPoint = spawnPoint;
        }

        public void SetPlayerSpawnPoint(SpawnPoint spawnPoint)
        {
            if (spawnPoint != null)
            {
                m_playerSpawnPoint = spawnPoint.transform;
            }
        }
    }
}
