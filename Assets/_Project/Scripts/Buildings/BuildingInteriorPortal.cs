using UnityEngine;

namespace CarSimulator.Buildings
{
    public class BuildingInteriorPortal : MonoBehaviour
    {
        [Header("Portal Info")]
        [SerializeField] private string m_portalId;
        [SerializeField] private string m_buildingId;
        [SerializeField] private string m_destinationScene;

        [Header("Portal Settings")]
        [SerializeField] private Vector3 m_portalSize = new Vector3(2.5f, 3f, 0.5f);
        [SerializeField] private bool m_isLocked;
        [SerializeField] private string m_unlockRequirement;

        [Header("Visual")]
        [SerializeField] private Color m_portalColor = new Color(0.2f, 0.4f, 1f, 0.6f);
        [SerializeField] private Color m_lockedColor = new Color(1f, 0.3f, 0.3f, 0.6f);
        [SerializeField] private float m_glowIntensity = 2f;

        [Header("Interaction")]
        [SerializeField] private float m_activationDistance = 5f;
        [SerializeField] private KeyCode m_interactKey = KeyCode.E;
        [SerializeField] private bool m_showPrompt = true;

        [Header("Interior Loading")]
        [SerializeField] private bool m_loadInteriorOnEnter = true;
        [SerializeField] private GameObject m_interiorPrefab;

        private Renderer m_portalRenderer;
        private Material m_portalMaterial;
        private bool m_isPlayerNearby;
        private Transform m_playerTransform;

        public string PortalId => m_portalId;
        public string BuildingId => m_buildingId;
        public bool IsLocked => m_isLocked;

        private void Start()
        {
            SetupPortal();
            FindPlayer();
        }

        private void SetupPortal()
        {
            GameObject portalFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            portalFrame.name = "PortalFrame";
            portalFrame.transform.SetParent(transform);
            portalFrame.transform.localPosition = Vector3.zero;
            portalFrame.transform.localScale = m_portalSize;

            m_portalRenderer = portalFrame.GetComponent<Renderer>();
            m_portalMaterial = new Material(Shader.Find("Standard"));
            m_portalMaterial.color = m_isLocked ? m_lockedColor : m_portalColor;
            m_portalMaterial.SetFloat("_Mode", 3);
            m_portalRenderer.material = m_portalMaterial;

            GameObject.Destroy(portalFrame.GetComponent<Collider>());

            CreatePortalGlow();
            CreatePrompt();
        }

        private void CreatePortalGlow()
        {
            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Quad);
            glow.name = "PortalGlow";
            glow.transform.SetParent(transform);
            glow.transform.localPosition = new Vector3(0, 0, m_portalSize.z / 2f + 0.01f);
            glow.transform.localScale = new Vector3(m_portalSize.x * 0.8f, m_portalSize.y * 0.8f, 1f);

            Renderer glowRenderer = glow.GetComponent<Renderer>();
            glowRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
            glowRenderer.material.color = new Color(m_portalColor.r, m_portalColor.g, m_portalColor.b, 0.5f);

            GameObject.Destroy(glow.GetComponent<Collider>());
        }

        private void CreatePrompt()
        {
            if (!m_showPrompt) return;

            GameObject prompt = new GameObject("InteractionPrompt");
            prompt.transform.SetParent(transform);
            prompt.transform.localPosition = new Vector3(0, m_portalSize.y / 2f + 0.5f, 0);

            TextMesh textMesh = prompt.AddComponent<TextMesh>();
            textMesh.text = "Press E to Enter";
            textMesh.characterSize = 0.3f;
            textMesh.fontSize = 40;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.white;

            prompt.SetActive(false);
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (m_playerTransform == null)
            {
                FindPlayer();
                return;
            }

            CheckPlayerProximity();
            HandleInput();
            UpdateVisuals();
        }

        private void CheckPlayerProximity()
        {
            float distance = Vector3.Distance(transform.position, m_playerTransform.position);
            m_isPlayerNearby = distance <= m_activationDistance;
        }

        private void HandleInput()
        {
            if (!m_isPlayerNearby) return;

            if (Input.GetKeyDown(m_interactKey))
            {
                TryEnter();
            }
        }

        private void TryEnter()
        {
            if (m_isLocked)
            {
                Debug.Log($"[BuildingInteriorPortal] Portal {m_portalId} is locked. Requirement: {m_unlockRequirement}");
                return;
            }

            Debug.Log($"[BuildingInteriorPortal] Entering building {m_buildingId} via portal {m_portalId}");

            if (m_loadInteriorOnEnter && m_interiorPrefab != null)
            {
                LoadInterior();
            }
            else if (!string.IsNullOrEmpty(m_destinationScene))
            {
                LoadScene();
            }

            OnPortalEntered?.Invoke(this);
        }

        private void LoadInterior()
        {
            if (m_interiorPrefab != null)
            {
                GameObject interior = Instantiate(m_interiorPrefab, transform.position, transform.rotation);
                Debug.Log($"[BuildingInteriorPortal] Loaded interior: {interior.name}");
            }
        }

        private void LoadScene()
        {
            Debug.Log($"[BuildingInteriorPortal] Would load scene: {m_destinationScene}");
        }

        private void UpdateVisuals()
        {
            if (m_portalMaterial == null) return;

            float pulse = Mathf.PingPong(Time.time * 2f, 1f) * m_glowIntensity;
            Color glowColor = m_isLocked ? m_lockedColor : m_portalColor;
            glowColor.a = 0.3f + pulse * 0.3f;

            if (m_portalRenderer != null)
            {
                m_portalRenderer.material.color = glowColor;
            }
        }

        public void Unlock(string requirement = "")
        {
            m_isLocked = false;
            m_unlockRequirement = requirement;
            Debug.Log($"[BuildingInteriorPortal] Portal {m_portalId} unlocked");
        }

        public void Lock(string reason = "")
        {
            m_isLocked = true;
            Debug.Log($"[BuildingInteriorPortal] Portal {m_portalId} locked: {reason}");
        }

        public event System.Action<BuildingInteriorPortal> OnPortalEntered;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = m_isLocked ? Color.red : new Color(0f, 0.5f, 1f, 0.5f);
            Gizmos.DrawWireCube(transform.position, m_portalSize);

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, m_activationDistance);
        }
    }
}
