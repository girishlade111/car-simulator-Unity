using UnityEngine;
using System.Collections;

namespace CarSimulator.World
{
    public class ApartmentEntrance : WorldInteractable
    {
        [Header("Apartment Settings")]
        [SerializeField] private string m_apartmentName = "Apartment";
        [SerializeField] private string m_apartmentId;
        [SerializeField] private ApartmentType m_apartmentType = ApartmentType.Residential;

        [Header("Entry/Exit")]
        [SerializeField] private Transform m_exteriorSpawn;
        [SerializeField] private Transform m_interiorSpawn;
        [SerializeField] private InteriorScene m_interiorScene;

        [Header("Ownership")]
        [SerializeField] private bool m_isOwned;
        [SerializeField] private string m_ownerId;

        [Header("Transition")]
        [SerializeField] private TransitionType m_transitionType = TransitionType.SameScene;
        [SerializeField] private float m_transitionDuration = 0.5f;

        [Header("Interaction")]
        [SerializeField] private float m_interactionRange = 4f;
        [SerializeField] private KeyCode m_interactionKey = KeyCode.E;

        [Header("State")]
        [SerializeField] private bool m_isPlayerInside;
        [SerializeField] private bool m_isDoorOpen;

        private bool m_isPlayerNear;
        private GameObject m_player;
        private GameObject m_interiorInstance;
        private bool m_isTransitioning;

        public enum ApartmentType
        {
            Residential,
            Commercial,
            Industrial,
            Public
        }

        public enum TransitionType
        {
            SameScene,
            AdditiveScene,
            Teleport
        }

        protected override void Start()
        {
            base.Start();

            m_interactionPrompt = $"Enter {m_apartmentName}";
            
            if (m_apartmentId == null)
            {
                m_apartmentId = System.Guid.NewGuid().ToString();
            }

            SetupTriggers();
        }

        private void SetupTriggers()
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = m_interactionRange;
            trigger.center = Vector3.zero;
        }

        private void Update()
        {
            if (m_isPlayerNear && Input.GetKeyDown(m_interactionKey) && !m_isTransitioning)
            {
                ToggleEntry();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_isPlayerNear = true;
                m_player = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_isPlayerNear = false;
                m_player = null;
            }
        }

        public override void OnInteract()
        {
            if (!CanInteract()) return;

            if (m_isPlayerInside)
            {
                ExitInterior();
            }
            else
            {
                EnterInterior();
            }
        }

        public override bool CanInteract()
        {
            return base.CanInteract() && !m_isTransitioning;
        }

        public override string GetInteractionPrompt()
        {
            if (m_isTransitioning) return "Loading...";
            if (m_isLocked) return $"{m_apartmentName} (Locked)";
            return m_isPlayerInside ? $"Exit {m_apartmentName}" : $"Enter {m_apartmentName}";
        }

        private void ToggleEntry()
        {
            if (m_isPlayerInside)
            {
                ExitInterior();
            }
            else
            {
                EnterInterior();
            }
        }

        private void EnterInterior()
        {
            if (m_isTransitioning) return;

            Debug.Log($"[ApartmentEntrance] Entering {m_apartmentName}");

            StartCoroutine(TransitionToInterior());
        }

        private void ExitInterior()
        {
            if (m_isTransitioning) return;

            Debug.Log($"[ApartmentEntrance] Exiting {m_apartmentName}");

            StartCoroutine(TransitionToExterior());
        }

        private IEnumerator TransitionToInterior()
        {
            m_isTransitioning = true;

            yield return StartCoroutine(FadeScreen());

            switch (m_transitionType)
            {
                case TransitionType.SameScene:
                    LoadInteriorSameScene();
                    break;
                case TransitionType.AdditiveScene:
                    yield return StartCoroutine(LoadInteriorAdditive());
                    break;
                case TransitionType.Teleport:
                    TeleportToInterior();
                    break;
            }

            m_isPlayerInside = true;
            m_interactionPrompt = $"Exit {m_apartmentName}";

            yield return StartCoroutine(FadeIn());

            m_isTransitioning = false;
        }

        private IEnumerator TransitionToExterior()
        {
            m_isTransitioning = true;

            yield return StartCoroutine(FadeScreen());

            switch (m_transitionType)
            {
                case TransitionType.SameScene:
                    UnloadInteriorSameScene();
                    break;
                case TransitionType.AdditiveScene:
                    yield return StartCoroutine(UnloadInteriorAdditive());
                    break;
                case TransitionType.Teleport:
                    TeleportToExterior();
                    break;
            }

            m_isPlayerInside = false;
            m_interactionPrompt = $"Enter {m_apartmentName}";

            yield return StartCoroutine(FadeIn());

            m_isTransitioning = false;
        }

        private void LoadInteriorSameScene()
        {
            if (m_interiorScene != null)
            {
                m_interiorInstance = m_interiorScene.CreateInterior(transform.position);
            }
            else
            {
                m_interiorInstance = CreatePlaceholderInterior();
            }

            if (m_player != null && m_interiorSpawn != null)
            {
                m_player.transform.position = m_interiorSpawn.position;
                m_player.transform.rotation = m_interiorSpawn.rotation;
            }

            DisableExteriorVisibility();
        }

        private void UnloadInteriorSameScene()
        {
            if (m_interiorInstance != null)
            {
                Destroy(m_interiorInstance);
                m_interiorInstance = null;
            }

            if (m_player != null && m_exteriorSpawn != null)
            {
                m_player.transform.position = m_exteriorSpawn.position;
                m_player.transform.rotation = m_exteriorSpawn.rotation;
            }

            EnableExteriorVisibility();
        }

        private IEnumerator LoadInteriorAdditive()
        {
            if (m_interiorScene != null)
            {
                yield return m_interiorScene.LoadAdditive();

                if (m_player != null && m_interiorSpawn != null)
                {
                    m_player.transform.position = m_interiorSpawn.position;
                    m_player.transform.rotation = m_interiorSpawn.rotation;
                }
            }
        }

        private IEnumerator UnloadInteriorAdditive()
        {
            if (m_interiorScene != null)
            {
                yield return m_interiorScene.UnloadAdditive();
            }

            if (m_player != null && m_exteriorSpawn != null)
            {
                m_player.transform.position = m_exteriorSpawn.position;
                m_player.transform.rotation = m_exteriorSpawn.rotation;
            }
        }

        private void TeleportToInterior()
        {
            if (m_player != null && m_interiorSpawn != null)
            {
                m_player.transform.position = m_interiorSpawn.position;
                m_player.transform.rotation = m_interiorSpawn.rotation;
            }
        }

        private void TeleportToExterior()
        {
            if (m_player != null && m_exteriorSpawn != null)
            {
                m_player.transform.position = m_exteriorSpawn.position;
                m_player.transform.rotation = m_exteriorSpawn.rotation;
            }
        }

        private GameObject CreatePlaceholderInterior()
        {
            GameObject interior = new GameObject($"Interior_{m_apartmentName}");
            interior.transform.position = transform.position + Vector3.up * 100f;

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(interior.transform);
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = new Vector3(3f, 1f, 3f);

            GameObject walls = GameObject.CreatePrimitive(PrimitiveType.Cube);
            walls.name = "Walls";
            walls.transform.SetParent(interior.transform);
            walls.transform.localPosition = new Vector3(0, 2f, 0);
            walls.transform.localScale = new Vector3(15f, 4f, 0.2f);

            GameObject textObj = new GameObject("Sign");
            textObj.transform.SetParent(interior.transform);
            textObj.transform.localPosition = new Vector3(0, 2.5f, 0);

            return interior;
        }

        private void DisableExteriorVisibility()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
        }

        private void EnableExteriorVisibility()
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }
        }

        private IEnumerator FadeScreen()
        {
            var ui = UI.NotificationSystem.Instance;
            if (ui != null)
            {
                ui.ShowInfo("Loading...");
            }
            yield return new WaitForSeconds(m_transitionDuration * 0.5f);
        }

        private IEnumerator FadeIn()
        {
            yield return new WaitForSeconds(m_transitionDuration * 0.5f);
        }

        public void SetOwned(bool owned, string ownerId = "")
        {
            m_isOwned = owned;
            m_ownerId = ownerId;
        }

        public bool IsPlayerInside() => m_isPlayerInside;
        public string GetApartmentId() => m_apartmentId;
        public ApartmentType GetApartmentType() => m_apartmentType;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = m_isPlayerInside ? Color.cyan : (IsLocked() ? Color.red : Color.green);
            Gizmos.DrawWireSphere(transform.position, m_interactionRange);

            if (m_exteriorSpawn != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, m_exteriorSpawn.position);
            }

            if (m_interiorSpawn != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, m_interiorSpawn.position);
            }
        }
    }
}
