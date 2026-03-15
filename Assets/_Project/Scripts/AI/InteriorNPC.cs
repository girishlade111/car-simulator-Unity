using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class InteriorNPC : MonoBehaviour
    {
        [Header("NPC Info")]
        [SerializeField] private string m_npcName;
        [SerializeField] private NPCRole m_role = NPCRole.Shopper;
        [SerializeField] private string m_dialogueId;
        [SerializeField] private PersonalityType m_personality = PersonalityType.Friendly;

        [Header("Personality Traits")]
        [SerializeField] private float m_talkativeness = 0.5f;
        [SerializeField] private float m_patience = 0.5f;
        [SerializeField] private float m_generosity = 0.5f;

        [Header("State")]
        [SerializeField] private InteriorNPCState m_currentState = InteriorNPCState.Idle;
        [SerializeField] private InteriorRoom m_currentRoom;

        [Header("Movement")]
        [SerializeField] private float m_moveSpeed = 1.5f;
        [SerializeField] private float m_idleTime = 3f;
        [SerializeField] private float m_currentIdleTime;

        [Header("Interaction")]
        [SerializeField] private bool m_canInteract = true;
        [SerializeField] private string[] m_interactionPrompts;
        [SerializeField] private DialogueData[] m_dialogues;

        [Header("Animation")]
        [SerializeField] private Animator m_animator;

        private Rigidbody m_rb;
        private Vector3 m_targetPosition;
        private bool m_isMoving;
        private Transform m_playerTransform;

        public enum NPCRole
        {
            Shopper,
            Employee,
            Customer,
            Resident,
            Visitor,
            Security,
            Janitor
        }

        public enum PersonalityType
        {
            Friendly,
            Shy,
            Aggressive,
            Professional,
            Flirty,
            Busy,
            Curious,
            Grumpy,
            Optimist
        }

        public enum InteriorNPCState
        {
            Idle,
            Walking,
            Working,
            Shopping,
            Talking,
            Leaving
        }

        [System.Serializable]
        public class DialogueData
        {
            public string prompt;
            public string response;
            public string[] options;
        }

        private void Start()
        {
            m_rb = GetComponent<Rigidbody>();
            if (m_rb == null)
            {
                m_rb = gameObject.AddComponent<Rigidbody>();
            }
            m_rb.isKinematic = true;

            ApplyPersonalityTraits();
            FindPlayer();
            SetRandomState();
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
            UpdateState();
            HandleMovement();
            CheckPlayerProximity();
            UpdateAnimation();
        }

        private void UpdateState()
        {
            switch (m_currentState)
            {
                case InteriorNPCState.Idle:
                    m_currentIdleTime += Time.deltaTime;
                    if (m_currentIdleTime >= m_idleTime)
                    {
                        PickNewActivity();
                    }
                    break;

                case InteriorNPCState.Walking:
                    if (!m_isMoving)
                    {
                        m_currentState = InteriorNPCState.Idle;
                    }
                    break;

                case InteriorNPCState.Talking:
                    break;
            }
        }

        private void HandleMovement()
        {
            if (!m_isMoving) return;

            Vector3 direction = (m_targetPosition - transform.position).normalized;
            direction.y = 0;

            transform.position += direction * m_moveSpeed * Time.deltaTime;
            transform.LookAt(m_targetPosition);

            float distance = Vector3.Distance(transform.position, m_targetPosition);
            if (distance < 0.5f)
            {
                m_isMoving = false;
                m_currentState = InteriorNPCState.Idle;
                m_currentIdleTime = 0;
            }
        }

        private void CheckPlayerProximity()
        {
            if (m_playerTransform == null) return;

            float distance = Vector3.Distance(transform.position, m_playerTransform.position);
            float interactionDist = GetInteractionDistance();

            if (distance < interactionDist && m_canInteract)
            {
                ShowInteractionPrompt();
            }
            else
            {
                HideInteractionPrompt();
            }
        }

        private void ShowInteractionPrompt()
        {
            // Show interaction UI - placeholder
        }

        private void HideInteractionPrompt()
        {
            // Hide interaction UI - placeholder
        }

        private void UpdateAnimation()
        {
            if (m_animator == null) return;

            m_animator.SetBool("IsMoving", m_isMoving);
        }

        private void PickNewActivity()
        {
            float rand = Random.value;

            if (rand < 0.4f)
            {
                WanderToRandomRoom();
            }
            else if (rand < 0.7f)
            {
                GoToWorkstation();
            }
            else
            {
                m_currentState = InteriorNPCState.Idle;
                m_currentIdleTime = 0;
            }
        }

        public void WanderToRandomRoom()
        {
            m_currentState = InteriorNPCState.Walking;
            m_targetPosition = GetRandomPositionInBuilding();
            m_isMoving = true;
        }

        public void GoToWorkstation()
        {
            m_currentState = InteriorNPCState.Working;
            m_targetPosition = GetWorkstationPosition();
            m_isMoving = true;
        }

        private Vector3 GetRandomPositionInBuilding()
        {
            var building = GetComponentInParent<BuildingInterior>();
            if (building != null)
            {
                var exit = building.GetRandomExit();
                if (exit != null)
                {
                    return exit.position + Random.insideUnitSphere * 2f;
                }
            }

            return transform.position + Random.insideUnitSphere * 5f;
        }

        private Vector3 GetWorkstationPosition()
        {
            return transform.position + transform.forward * 2f;
        }

        public void Interact()
        {
            if (!m_canInteract) return;

            if (m_dialogues != null && m_dialogues.Length > 0)
            {
                DialogueData dialogue = m_dialogues[Random.Range(0, m_dialogues.Length)];
                ShowDialogue(dialogue);
            }
            else
            {
                ShowDefaultDialogue();
            }
        }

        private void ShowDialogue(DialogueData dialogue)
        {
            string response = GetPersonalityBasedResponse(dialogue.response);
            Debug.Log($"[InteriorNPC] {m_npcName}: {response}");
            m_currentState = InteriorNPCState.Talking;
        }

        private string GetPersonalityBasedResponse(string baseResponse)
        {
            if (!WillEngageInConversation())
            {
                return GetPersonalizedFarewell();
            }
            return baseResponse;
        }

        private void ShowDefaultDialogue()
        {
            string greeting = GetPersonalizedGreeting();
            Debug.Log($"[InteriorNPC] {m_npcName}: {greeting}");
            m_currentState = InteriorNPCState.Talking;
        }

        public void SetRoom(InteriorRoom room)
        {
            m_currentRoom = room;
        }

        public void SetRole(NPCRole role)
        {
            m_role = role;

            switch (role)
            {
                case NPCRole.Employee:
                case NPCRole.Security:
                    m_canInteract = true;
                    break;
                case NPCRole.Shopper:
                case NPCRole.Customer:
                case NPCRole.Visitor:
                    m_canInteract = true;
                    break;
            }
        }

        public InteriorNPCState GetState() => m_currentState;
        public string GetName() => m_npcName;
        public PersonalityType GetPersonality() => m_personality;

        public void SetPersonality(PersonalityType personality)
        {
            m_personality = personality;
            ApplyPersonalityTraits();
        }

        private void ApplyPersonalityTraits()
        {
            switch (m_personality)
            {
                case PersonalityType.Friendly:
                    m_talkativeness = 0.8f;
                    m_patience = 0.7f;
                    m_generosity = 0.6f;
                    m_moveSpeed *= 0.9f;
                    break;
                case PersonalityType.Shy:
                    m_talkativeness = 0.2f;
                    m_patience = 0.8f;
                    m_generosity = 0.3f;
                    m_moveSpeed *= 0.8f;
                    m_canInteract = true;
                    break;
                case PersonalityType.Aggressive:
                    m_talkativeness = 0.4f;
                    m_patience = 0.2f;
                    m_generosity = 0.1f;
                    m_moveSpeed *= 1.1f;
                    break;
                case PersonalityType.Professional:
                    m_talkativeness = 0.5f;
                    m_patience = 0.9f;
                    m_generosity = 0.4f;
                    m_moveSpeed *= 1.0f;
                    break;
                case PersonalityType.Flirty:
                    m_talkativeness = 0.9f;
                    m_patience = 0.5f;
                    m_generosity = 0.7f;
                    m_moveSpeed *= 0.85f;
                    break;
                case PersonalityType.Busy:
                    m_talkativeness = 0.3f;
                    m_patience = 0.3f;
                    m_generosity = 0.2f;
                    m_moveSpeed *= 1.2f;
                    break;
                case PersonalityType.Curious:
                    m_talkativeness = 0.95f;
                    m_patience = 0.6f;
                    m_generosity = 0.5f;
                    m_moveSpeed *= 0.75f;
                    break;
                case PersonalityType.Grumpy:
                    m_talkativeness = 0.2f;
                    m_patience = 0.15f;
                    m_generosity = 0.1f;
                    m_moveSpeed *= 1.0f;
                    break;
                case PersonalityType.Optimist:
                    m_talkativeness = 0.85f;
                    m_patience = 0.85f;
                    m_generosity = 0.8f;
                    m_moveSpeed *= 0.95f;
                    break;
            }
        }

        public string GetPersonalizedGreeting()
        {
            switch (m_personality)
            {
                case PersonalityType.Friendly:
                    return "Hey there! Great to see you!";
                case PersonalityType.Shy:
                    return "Oh... hello. Nice to meet you.";
                case PersonalityType.Aggressive:
                    return "What do you want?";
                case PersonalityType.Professional:
                    return "Welcome. How may I assist you?";
                case PersonalityType.Flirty:
                    return "Well hello there, stranger~";
                case PersonalityType.Busy:
                    return "I'm in a rush, make it quick.";
                case PersonalityType.Curious:
                    return "Oh! You look interesting! What's your story?";
                case PersonalityType.Grumpy:
                    return "*grunt* What.";
                case PersonalityType.Optimist:
                    return "Hey! Great day isn't it? Awesome to see you!";
                default:
                    return "Hello!";
            }
        }

        public string GetPersonalizedResponse(string topic)
        {
            topic = topic.ToLower();

            switch (m_personality)
            {
                case PersonalityType.Friendly:
                    if (topic.Contains("help")) return "Of course! I'm happy to help with anything you need!";
                    if (topic.Contains("store")) return "I love this place! The staff is wonderful!";
                    return "That's really nice! Tell me more!";
                case PersonalityType.Shy:
                    if (topic.Contains("help")) return "I... I could try to help, if you need it...";
                    if (topic.Contains("store")) return "It's nice here... I come here sometimes.";
                    return "I suppose... if you say so...";
                case PersonalityType.Aggressive:
                    if (topic.Contains("help")) return "Help yourself. I'm busy.";
                    if (topic.Contains("store")) return "This place is overpriced. Change my mind.";
                    return "Whatever. I don't care.";
                case PersonalityType.Professional:
                    if (topic.Contains("help")) return "Certainly. What specifically do you need assistance with?";
                    if (topic.Contains("store")) return "We have excellent products and services here.";
                    return "I understand. Please let me know how I can help.";
                case PersonalityType.Flirty:
                    if (topic.Contains("help")) return "For you? Anything, honey~";
                    if (topic.Contains("store")) return "I'd show you around, if you want~";
                    return "You make me want to talk all day!";
                case PersonalityType.Busy:
                    if (topic.Contains("help")) return "I don't have time for this. Figure it out.";
                    if (topic.Contains("store")) return "Just buy what you need and go.";
                    return "Seriously, I don't have all day.";
                case PersonalityType.Curious:
                    if (topic.Contains("help")) return "Oh! Let me help! I want to know everything about it!";
                    if (topic.Contains("store")) return "Did you know they have hidden stuff here? Let's explore!";
                    return "Wait wait wait! Tell me EVERYTHING about that!";
                case PersonalityType.Grumpy:
                    if (topic.Contains("help")) return "Fine. What is it now?";
                    if (topic.Contains("store")) return "Prices are too high. Service is too slow.";
                    return "Bah. Fine. Whatever.";
                case PersonalityType.Optimist:
                    if (topic.Contains("help")) return "Absolutely! Helping others makes MY day better!";
                    if (topic.Contains("store")) return "This place has the BEST atmosphere! I love it!";
                    return "That's wonderful! I love hearing about stuff like that!";
                default:
                    return "I see.";
            }
        }

        public string GetPersonalizedFarewell()
        {
            switch (m_personality)
            {
                case PersonalityType.Friendly:
                    return "Bye! Come back soon!";
                case PersonalityType.Shy:
                    return "Goodbye... it was nice talking...";
                case PersonalityType.Aggressive:
                    return "Finally.";
                case PersonalityType.Professional:
                    return "Thank you for your visit. Have a productive day.";
                case PersonalityType.Flirty:
                    return "Leaving already? I'll be here if you need me~";
                case PersonalityType.Busy:
                    return "Okay, bye.";
                case PersonalityType.Curious:
                    return "Wait! Don't go yet! Tell me more!";
                case PersonalityType.Grumpy:
                    return "Whatever.";
                case PersonalityType.Optimist:
                    return "Bye! Have an AMAZING day!";
                default:
                    return "Goodbye.";
            }
        }

        public float GetInteractionDistance()
        {
            switch (m_personality)
            {
                case PersonalityType.Shy:
                    return 4f;
                case PersonalityType.Aggressive:
                    return 2f;
                case PersonalityType.Curious:
                    return 6f;
                default:
                    return 3f;
            }
        }

        public bool WillGiveItem()
        {
            float roll = Random.value;
            return roll < m_generosity;
        }

        public bool WillEngageInConversation()
        {
            float roll = Random.value;
            return roll < m_talkativeness;
        }

    public class InteriorNPCManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int m_maxNPCs = 15;
        [SerializeField] private float m_spawnInterval = 2f;

        [Header("NPC Templates")]
        [SerializeField] private InteriorNPC.NPCRole[] m_availableRoles;

        [Header("Building Reference")]
        [SerializeField] private BuildingInterior m_buildingInterior;

        private List<InteriorNPC> m_spawnedNPCs = new List<InteriorNPC>();
        private float m_spawnTimer;

        private void Start()
        {
            SpawnInitialNPCs();
        }

        private void Update()
        {
            m_spawnTimer += Time.deltaTime;

            if (m_spawnTimer >= m_spawnInterval && m_spawnedNPCs.Count < m_maxNPCs)
            {
                m_spawnTimer = 0f;
                SpawnNPC();
            }
        }

        private void SpawnInitialNPCs()
        {
            for (int i = 0; i < m_maxNPCs / 2; i++)
            {
                SpawnNPC();
            }
        }

        private void SpawnNPC()
        {
            GameObject npc = CreateNPC();
            InteriorNPC npcScript = npc.GetComponent<InteriorNPC>();

            if (m_availableRoles != null && m_availableRoles.Length > 0)
            {
                InteriorNPC.NPCRole role = m_availableRoles[Random.Range(0, m_availableRoles.Length)];
                npcScript.SetRole(role);
            }

            m_spawnedNPCs.Add(npcScript);
        }

        private GameObject CreateNPC()
        {
            GameObject npc = new GameObject($"InteriorNPC_{m_spawnedNPCs.Count}");
            npc.transform.SetParent(transform);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(npc.transform);
            body.transform.localPosition = new Vector3(0, 1f, 0);
            body.transform.localScale = new Vector3(0.5f, 1f, 0.5f);

            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = GetRandomClothingColor();
            }

            npc.tag = "NPC";
            npc.layer = LayerMask.NameToLayer("Prop");

            InteriorNPC npcScript = npc.AddComponent<InteriorNPC>();
            npcScript.SetRole(InteriorNPC.NPCRole.Shopper);
            npcScript.SetPersonality(GetRandomPersonality());

            return npc;
        }

        private Color GetRandomClothingColor()
        {
            Color[] colors = {
                new Color(0.2f, 0.3f, 0.8f),
                new Color(0.8f, 0.2f, 0.3f),
                new Color(0.3f, 0.7f, 0.4f),
                new Color(0.9f, 0.7f, 0.2f),
                new Color(0.6f, 0.3f, 0.7f),
                new Color(0.3f, 0.3f, 0.3f),
                new Color(0.9f, 0.9f, 0.9f)
            };

            return colors[Random.Range(0, colors.Length)];
        }

        private InteriorNPC.PersonalityType GetRandomPersonality()
        {
            InteriorNPC.PersonalityType[] personalities = {
                InteriorNPC.PersonalityType.Friendly,
                InteriorNPC.PersonalityType.Shy,
                InteriorNPC.PersonalityType.Aggressive,
                InteriorNPC.PersonalityType.Professional,
                InteriorNPC.PersonalityType.Flirty,
                InteriorNPC.PersonalityType.Busy,
                InteriorNPC.PersonalityType.Curious,
                InteriorNPC.PersonalityType.Grumpy,
                InteriorNPC.PersonalityType.Optimist
            };

            return personalities[Random.Range(0, personalities.Length)];
        }

        public void SetBuilding(BuildingInterior building)
        {
            m_buildingInterior = building;
        }

        public int GetNPCCount() => m_spawnedNPCs.Count;
    }
}
