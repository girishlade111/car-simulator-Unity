using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class WorldInteractionManager : MonoBehaviour
    {
        public static WorldInteractionManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_interactionRange = 3f;
        [SerializeField] private KeyCode m_interactionKey = KeyCode.E;
        [SerializeField] private LayerMask m_interactableLayer = -1;
        [SerializeField] private float m_updateInterval = 0.1f;

        [Header("UI")]
        [SerializeField] private bool m_showPrompts = true;

        [Header("References")]
        [SerializeField] private Transform m_playerTransform;

        private List<WorldInteractable> m_nearbyInteractables = new List<WorldInteractable>();
        private WorldInteractable m_closestInteractable;
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
        }

        private void Update()
        {
            if (!m_enabled) return;

            if (Time.time - m_lastUpdateTime > m_updateInterval)
            {
                UpdateNearbyInteractables();
                m_lastUpdateTime = Time.time;
            }

            HandleInput();
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

        private void UpdateNearbyInteractables()
        {
            if (m_playerTransform == null) return;

            m_nearbyInteractables.Clear();

            Collider[] colliders = Physics.OverlapSphere(m_playerTransform.position, m_interactionRange, m_interactableLayer);

            float closestDist = float.MaxValue;
            m_closestInteractable = null;

            foreach (var collider in colliders)
            {
                var interactable = collider.GetComponent<WorldInteractable>();
                if (interactable != null && interactable.CanInteract())
                {
                    m_nearbyInteractables.Add(interactable);

                    float dist = Vector3.Distance(m_playerTransform.position, collider.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        m_closestInteractable = interactable;
                    }
                }
            }

            if (m_showPrompts)
            {
                UpdateInteractionPrompts();
            }
        }

        private void UpdateInteractionPrompts()
        {
            if (m_closestInteractable != null)
            {
                var ui = UI.NotificationSystem.Instance;
                if (ui != null)
                {
                    ui.ShowInfo($"[E] {m_closestInteractable.GetInteractionPrompt()}");
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(m_interactionKey) && m_closestInteractable != null)
            {
                InteractWithClosest();
            }
        }

        public void InteractWithClosest()
        {
            if (m_closestInteractable != null && m_closestInteractable.CanInteract())
            {
                m_closestInteractable.OnInteract();
            }
        }

        public void RegisterInteractable(WorldInteractable interactable)
        {
            if (!m_nearbyInteractables.Contains(interactable))
            {
                m_nearbyInteractables.Add(interactable);
            }
        }

        public void UnregisterInteractable(WorldInteractable interactable)
        {
            m_nearbyInteractables.Remove(interactable);
        }

        public WorldInteractable GetClosestInteractable() => m_closestInteractable;
    }

    public class WorldInteractable : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private string m_interactionPrompt = "Interact";
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_interactionRange = 3f;

        [Header("Interaction Events")]
        [SerializeField] private UnityEngine.Events.UnityEvent m_onInteract;

        [Header("State")]
        [SerializeField] private bool m_isLocked;

        protected WorldInteractionManager m_interactionManager;

        protected virtual void Start()
        {
            m_interactionManager = WorldInteractionManager.Instance;
        }

        public virtual void OnInteract()
        {
            if (!CanInteract()) return;

            m_onInteract?.Invoke();
            Debug.Log($"[WorldInteractable] Interacted with {gameObject.name}");
        }

        public virtual bool CanInteract()
        {
            return m_enabled && !m_isLocked;
        }

        public virtual string GetInteractionPrompt()
        {
            if (m_isLocked) return $"{m_interactionPrompt} (Locked)";
            return m_interactionPrompt;
        }

        public void SetEnabled(bool enabled)
        {
            m_enabled = enabled;
        }

        public void SetLocked(bool locked)
        {
            m_isLocked = locked;
        }

        public bool IsLocked() => m_isLocked;
    }
}
