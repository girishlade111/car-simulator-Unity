using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class NPCInteractionManager : MonoBehaviour
    {
        public static NPCInteractionManager Instance { get; private set; }

        [Header("Interaction Settings")]
        [SerializeField] private float m_interactionRadius = 3f;
        [SerializeField] private KeyCode m_interactKey = KeyCode.E;
        [SerializeField] private bool m_showPrompts = true;

        [Header("UI")]
        [SerializeField] private GameObject m_interactionPanel;
        [SerializeField] private Text m_promptText;
        [SerializeField] private Button[] m_optionButtons;

        [Header("Interaction Types")]
        [SerializeField] private bool m_enableDialogue = true;
        [SerializeField] private bool m_enableTrading = true;
        [SerializeField] private bool m_enableQuests = true;
        [SerializeField] private bool m_enableInformation = true;

        private NPCInteraction m_currentInteraction;
        private bool m_isInteracting;
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
            FindPlayer();
            HideInteractionPanel();
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
            if (m_isInteracting)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    EndInteraction();
                }
                return;
            }

            CheckForNearbyNPC();
        }

        private void CheckForNearbyNPC()
        {
            if (m_playerTransform == null) return;

            var nearbyNPCs = Physics.OverlapSphere(m_playerTransform.position, m_interactionRadius);

            foreach (var collider in nearbyNPCs)
            {
                var npc = collider.GetComponent<InteriorNPC>();
                if (npc != null)
                {
                    ShowInteractionPrompt(npc);
                    return;
                }

                var pedestrian = collider.GetComponent<Pedestrian>();
                if (pedestrian != null)
                {
                    ShowPedestrianPrompt(pedestrian);
                    return;
                }
            }

            HideInteractionPanel();
        }

        private void ShowInteractionPrompt(InteriorNPC npc)
        {
            if (!m_showPrompts) return;

            ShowInteractionPanel();
            if (m_promptText != null)
            {
                m_promptText.text = $"Talk to {npc.GetName()}\nPress E";
            }
        }

        private void ShowPedestrianPrompt(Pedestrian npc)
        {
            if (!m_showPrompts) return;

            ShowInteractionPanel();
            if (m_promptText != null)
            {
                m_promptText.text = $"Talk to Pedestrian\nPress E";
            }
        }

        private void ShowInteractionPanel()
        {
            if (m_interactionPanel != null && !m_interactionPanel.activeSelf)
            {
                m_interactionPanel.SetActive(true);
            }
        }

        private void HideInteractionPanel()
        {
            if (m_interactionPanel != null)
            {
                m_interactionPanel.SetActive(false);
            }
        }

        public void StartInteraction(NPCInteraction interaction)
        {
            m_currentInteraction = interaction;
            m_isInteracting = true;
            Time.timeScale = 0;

            ShowInteractionUI();
        }

        private void ShowInteractionUI()
        {
            if (m_interactionPanel != null)
            {
                m_interactionPanel.SetActive(true);
            }
        }

        public void EndInteraction()
        {
            m_currentInteraction = null;
            m_isInteracting = false;
            Time.timeScale = 1;

            HideInteractionPanel();
        }

        public void RequestDialogue()
        {
            if (m_currentInteraction != null)
            {
                m_currentInteraction.StartDialogue();
            }
        }

        public void RequestTrade()
        {
            if (m_currentInteraction != null && m_enableTrading)
            {
                m_currentInteraction.OpenShop();
            }
        }

        public void RequestQuest()
        {
            if (m_currentInteraction != null && m_enableQuests)
            {
                m_currentInteraction.ShowQuests();
            }
        }

        public void RequestInformation()
        {
            if (m_currentInteraction != null && m_enableInformation)
            {
                m_currentInteraction.ProvideInformation();
            }
        }
    }

    public abstract class NPCInteraction : MonoBehaviour
    {
        [Header("Interaction Data")]
        [SerializeField] protected string m_npcName;
        [SerializeField] protected InteractionType[] m_availableInteractions;

        protected enum InteractionType
        {
            Dialogue,
            Trade,
            Quest,
            Information,
            Follow,
            Service
        }

        public virtual void StartDialogue() { }
        public virtual void OpenShop() { }
        public virtual void ShowQuests() { }
        public virtual void ProvideInformation() { }
        public virtual void RequestFollow() { }
        public virtual void RequestService() { }
    }

    public class CitizenNPC : NPCInteraction
    {
        [Header("Citizen Data")]
        [SerializeField] private string[] m_greetings;
        [SerializeField] private string[] m_farewells;
        [SerializeField] private bool m_hasQuest;
        [SerializeField] private string m_questId;
        [SerializeField] private ShopItem[] m_shopItems;

        [System.Serializable]
        public class ShopItem
        {
            public string itemName;
            public int price;
            public string description;
        }

        protected override void Start()
        {
            base.Start();
            m_npcName = GetRandomName();
        }

        public override void StartDialogue()
        {
            string greeting = m_greetings[Random.Range(0, m_greetings.Length)];
            Debug.Log($"[CitizenNPC] {m_npcName}: \"{greeting}\"");
        }

        public override void OpenShop()
        {
            if (m_shopItems == null || m_shopItems.Length == 0)
            {
                Debug.Log($"[CitizenNPC] {m_npcName}: Sorry, I don't have anything to sell.");
                return;
            }

            Debug.Log($"[CitizenNPC] {m_npcName}'s Shop:");
            foreach (var item in m_shopItems)
            {
                Debug.Log($"  - {item.itemName}: ${item.price}");
            }
        }

        public override void ShowQuests()
        {
            if (m_hasQuest)
            {
                Debug.Log($"[CitizenNPC] {m_npcName}: Can you help me? (Quest: {m_questId})");
            }
            else
            {
                Debug.Log($"[CitizenNPC} {m_npcName}: I don't have any tasks for you right now.");
            }
        }

        public override void ProvideInformation()
        {
            string[] locations = {
                "The hospital is to the north.",
                "There's a gas station downtown.",
                "The police station is nearby.",
                "Be careful at night in this area."
            };

            Debug.Log($"[CitizenNPC] {m_npcName}: {locations[Random.Range(0, locations.Length)]}");
        }

        private string GetRandomName()
        {
            string[] names = {
                "John", "Sarah", "Mike", "Emma", "David",
                "Lisa", "Tom", "Amy", "Bob", "Carol"
            };
            return names[Random.Range(0, names.Length)];
        }
    }

    public class ServiceNPC : NPCInteraction
    {
        [Header("Service Data")]
        [SerializeField] private ServiceType m_serviceType;
        [SerializeField] private int m_serviceCost;
        [SerializeField] private float m_serviceDuration = 5f;

        public enum ServiceType
        {
            Mechanic,
            Fuel,
            Repair,
            Heal,
            Taxi
        }

        public override void RequestService()
        {
            switch (m_serviceType)
            {
                case ServiceType.Mechanic:
                    Debug.Log($"[ServiceNPC] Mechanic: I can fix your car for ${m_serviceCost}");
                    break;
                case ServiceType.Fuel:
                    Debug.Log($"[ServiceNPC] Gas Station: Fuel available - ${m_serviceCost/10}/L");
                    break;
                case ServiceType.Repair:
                    Debug.Log($"[ServiceNPC] Repair Shop: Repairs starting - ${m_serviceCost}");
                    break;
                case ServiceType.Heal:
                    Debug.Log($"[ServiceNPC] Medic: Health services - ${m_serviceCost}");
                    break;
                case ServiceType.Taxi:
                    Debug.Log($"[ServiceNPC] Taxi: Where would you like to go? - ${m_serviceCost}");
                    break;
            }
        }

        public void PerformService(GameObject target)
        {
            Debug.Log($"[ServiceNPC] Performing {m_serviceType} service...");
        }
    }

    public class FollowerNPC : NPCInteraction
    {
        [Header("Follower Settings")]
        [SerializeField] private float m_followDistance = 3f;
        [SerializeField] private float m_followSpeed = 4f;
        [SerializeField] private Transform m_target;

        private bool m_isFollowing;

        public override void RequestFollow()
        {
            m_isFollowing = !m_isFollowing;
            Debug.Log($"[FollowerNPC] Following: {m_isFollowing}");
        }

        private void Update()
        {
            if (m_isFollowing && m_target != null)
            {
                FollowTarget();
            }
        }

        private void FollowTarget()
        {
            float distance = Vector3.Distance(transform.position, m_target.position);

            if (distance > m_followDistance)
            {
                Vector3 direction = (m_target.position - transform.position).normalized;
                transform.position += direction * m_followSpeed * Time.deltaTime;
                transform.LookAt(m_target);
            }
        }
    }
}
