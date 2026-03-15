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

            if (distance < 3f && m_canInteract)
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
            Debug.Log($"[InteriorNPC] {m_npcName}: {dialogue.response}");
            m_currentState = InteriorNPCState.Talking;
        }

        private void ShowDefaultDialogue()
        {
            string[] greetings = {
                "Hello! Can I help you?",
                "Welcome to our store!",
                "Nice to see you!",
                "Looking for something?"
            };

            Debug.Log($"[InteriorNPC] {m_npcName}: {greetings[Random.Range(0, greetings.Length)]}");
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

        public void SetBuilding(BuildingInterior building)
        {
            m_buildingInterior = building;
        }

        public int GetNPCCount() => m_spawnedNPCs.Count;
    }
}
