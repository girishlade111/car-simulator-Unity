using UnityEngine;
using System;

namespace CarSimulator.SaveSystem
{
    public class CheckpointSystem : MonoBehaviour
    {
        public static CheckpointSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_autoActivateOnReach = true;
        [SerializeField] private float m_activationRadius = 8f;
        [SerializeField] private bool m_saveOnCheckpoint = true;

        [Header("Current Checkpoint")]
        [SerializeField] private string m_currentCheckpointId;
        [SerializeField] private Vector3 m_currentSpawnPosition;
        [SerializeField] private Quaternion m_currentSpawnRotation;

        [Header("Debug")]
        [SerializeField] private bool m_showDebugInfo;

        private Transform m_playerTransform;
        private SaveManager m_saveManager;

        public event System.Action<Checkpoint> OnCheckpointActivated;

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
            m_saveManager = SaveManager.Instance;
            FindPlayer();
            LoadSavedCheckpoint();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void LoadSavedCheckpoint()
        {
            if (m_saveManager?.CurrentSave?.player != null)
            {
                var playerData = m_saveManager.CurrentSave.player;

                if (!string.IsNullOrEmpty(playerData.lastCheckpointId))
                {
                    m_currentCheckpointId = playerData.lastCheckpointId;
                    m_currentSpawnPosition = playerData.spawnPosition;
                    m_currentSpawnRotation = playerData.spawnRotation;

                    Debug.Log($"[CheckpointSystem] Loaded checkpoint: {m_currentCheckpointId}");
                }
                else
                {
                    SetDefaultSpawn();
                }
            }
        }

        private void SetDefaultSpawn()
        {
            m_currentCheckpointId = "start";
            m_currentSpawnPosition = new Vector3(0, 1, 0);
            m_currentSpawnRotation = Quaternion.identity;
        }

        public void ActivateCheckpoint(Checkpoint checkpoint)
        {
            if (checkpoint == null) return;

            m_currentCheckpointId = checkpoint.GetCheckpointId();
            m_currentSpawnPosition = checkpoint.GetSpawnPosition();
            m_currentSpawnRotation = checkpoint.GetSpawnRotation();

            if (m_saveManager != null)
            {
                m_saveManager.UpdateCheckpoint(m_currentCheckpointId);
                m_saveManager.UpdateSpawnPoint(m_currentSpawnPosition, m_currentSpawnRotation);

                if (m_saveOnCheckpoint)
                {
                    m_saveManager.QuickSave();
                }
            }

            Debug.Log($"[CheckpointSystem] Checkpoint activated: {m_currentCheckpointId}");
            OnCheckpointActivated?.Invoke(checkpoint);

            var notification = UI.NotificationSystem.Instance;
            if (notification != null)
            {
                notification.ShowSuccess("Checkpoint Reached!");
            }
        }

        public void RespawnAtCheckpoint()
        {
            if (m_playerTransform == null)
            {
                FindPlayer();
            }

            if (m_playerTransform == null)
            {
                Debug.LogWarning("[CheckpointSystem] No player found for respawn");
                return;
            }

            var vehicle = m_playerTransform.GetComponent<Vehicle.VehicleController>();
            if (vehicle != null)
            {
                vehicle.TeleportTo(m_currentSpawnPosition, m_currentSpawnRotation);
            }
            else
            {
                m_playerTransform.position = m_currentSpawnPosition;
                m_playerTransform.rotation = m_currentSpawnRotation;
            }

            Debug.Log($"[CheckpointSystem] Respawned at checkpoint: {m_currentCheckpointId}");
        }

        public void ResetToStart(Vector3 startPosition, Quaternion startRotation)
        {
            m_currentCheckpointId = "start";
            m_currentSpawnPosition = startPosition;
            m_currentSpawnRotation = startRotation;

            if (m_saveManager?.CurrentSave?.player != null)
            {
                m_saveManager.UpdateCheckpoint("start");
                m_saveManager.UpdateSpawnPoint(startPosition, startRotation);
            }
        }

        public Vector3 GetCurrentSpawnPosition() => m_currentSpawnPosition;
        public Quaternion GetCurrentSpawnRotation() => m_currentSpawnRotation;
        public string GetCurrentCheckpointId() => m_currentCheckpointId;
    }

    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint Settings")]
        [SerializeField] private string m_checkpointId;
        [SerializeField] private string m_displayName = "Checkpoint";
        [SerializeField] private bool m_isOneTime = false;

        [Header("Spawn Point")]
        [SerializeField] private Vector3 m_spawnPosition;
        [SerializeField] private Quaternion m_spawnRotation;

        [Header("Visual")]
        [SerializeField] private GameObject m_visualMarker;
        [SerializeField] private Color m_activeColor = Color.green;
        [SerializeField] private Color m_inactiveColor = Color.gray;
        [SerializeField] private bool m_showGizmo = true;

        [Header("State")]
        [SerializeField] private bool m_isActivated;

        private Renderer m_markerRenderer;

        private void Start()
        {
            if (string.IsNullOrEmpty(m_checkpointId))
            {
                m_checkpointId = System.Guid.NewGuid().ToString();
            }

            m_spawnPosition = transform.position;
            m_spawnRotation = transform.rotation;

            m_markerRenderer = m_visualMarker?.GetComponent<Renderer>();
            UpdateVisual();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!m_isActivated && other.CompareTag("Player"))
            {
                Activate();
            }
        }

        public void Activate()
        {
            if (m_isActivated && m_isOneTime) return;

            m_isActivated = true;

            if (CheckpointSystem.Instance != null)
            {
                CheckpointSystem.Instance.ActivateCheckpoint(this);
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (m_markerRenderer != null)
            {
                m_markerRenderer.material.color = m_isActivated ? m_activeColor : m_inactiveColor;
            }
        }

        public string GetCheckpointId() => m_checkpointId;
        public string GetDisplayName() => m_displayName;
        public Vector3 GetSpawnPosition() => m_spawnPosition;
        public Quaternion GetSpawnRotation() => m_spawnRotation;
        public bool IsActivated() => m_isActivated;

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_isActivated ? m_activeColor : m_inactiveColor;
            Gizmos.DrawWireSphere(transform.position, 2f);

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
