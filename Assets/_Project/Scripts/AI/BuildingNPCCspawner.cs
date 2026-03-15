using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class BuildingNPCCpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private int m_maxNPCs = 10;
        [SerializeField] private float m_spawnInterval = 5f;
        [SerializeField] private bool m_autoSpawn = true;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] m_spawnPoints;

        [Header("NPC Types")]
        [SerializeField] private NPCTemplate[] m_npcTemplates;

        [Header("Behavior")]
        [SerializeField] private NPCBehavior m_defaultBehavior = NPCBehavior.Patrol;
        [SerializeField] private float m_patrolRadius = 20f;
        [SerializeField] private bool m_wander = true;

        [Header("References")]
        [SerializeField] private Transform m_buildingTransform;

        [Header("Optimization")]
        [SerializeField] private float m_updateInterval = 0.5f;

        private List<GameObject> m_spawnedNPCs = new List<GameObject>();
        private float m_spawnTimer;
        private float m_lastUpdateTime;
        private bool m_isBuildingActive = true;

        public enum NPCBehavior
        {
            Stationary,
            Patrol,
            Wander,
            FollowPath
        }

        [System.Serializable]
        public class NPCTemplate
        {
            public string npcId;
            public GameObject prefab;
            public float spawnWeight = 1f;
            public NPCBehavior behavior;
            public bool canEnterBuildings;
        }

        private void Start()
        {
            if (m_autoSpawn)
            {
                StartSpawning();
            }
        }

        public void StartSpawning()
        {
            for (int i = 0; i < m_maxNPCs / 2; i++)
            {
                SpawnNPC();
            }
        }

        public void StopSpawning()
        {
            m_autoSpawn = false;
        }

        private void Update()
        {
            if (!m_autoSpawn || !m_isBuildingActive) return;
            if (Time.time - m_lastUpdateTime < m_updateInterval) return;
            m_lastUpdateTime = Time.time;

            m_spawnTimer += m_updateInterval;

            if (m_spawnTimer >= m_spawnInterval)
            {
                m_spawnTimer = 0f;

                if (m_spawnedNPCs.Count < m_maxNPCs)
                {
                    SpawnNPC();
                }
            }

            CleanupNPCs();
        }

        private void SpawnNPC()
        {
            if (m_spawnPoints == null || m_spawnPoints.Length == 0) return;

            Transform spawnPoint = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
            if (spawnPoint == null) return;

            GameObject npcPrefab = GetRandomNPCTemplate();
            if (npcPrefab == null)
            {
                npcPrefab = CreatePlaceholderNPC();
            }

            GameObject npc = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
            npc.transform.SetParent(transform);

            Pedestrian pedestrian = npc.GetComponent<Pedestrian>();
            if (pedestrian != null)
            {
                pedestrian.SetBuildingOrigin(m_buildingTransform);
            }

            BuildingNPC npcScript = npc.AddComponent<BuildingNPC>();
            npcScript.Initialize(m_defaultBehavior, m_patrolRadius, m_wander);

            m_spawnedNPCs.Add(npc);
        }

        private GameObject GetRandomNPCTemplate()
        {
            if (m_npcTemplates == null || m_npcTemplates.Length == 0) return null;

            float totalWeight = 0;
            foreach (var template in m_npcTemplates)
            {
                totalWeight += template.spawnWeight;
            }

            float random = Random.Range(0, totalWeight);
            float current = 0;

            foreach (var template in m_npcTemplates)
            {
                current += template.spawnWeight;
                if (random <= current)
                {
                    return template.prefab;
                }
            }

            return m_npcTemplates[0]?.prefab;
        }

        private GameObject CreatePlaceholderNPC()
        {
            GameObject npc = new GameObject("NPC");
            npc.transform.position = Vector3.zero;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(npc.transform);
            body.transform.localPosition = new Vector3(0, 1f, 0);
            body.transform.localScale = new Vector3(0.5f, 1f, 0.5f);

            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = GetRandomSkinTone();
            }

            npc.tag = "NPC";
            npc.layer = LayerMask.NameToLayer("Prop");

            Rigidbody rb = npc.AddComponent<Rigidbody>();
            rb.mass = 70f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            SphereCollider collider = npc.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
            collider.center = new Vector3(0, 1f, 0);

            return npc;
        }

        private Color GetRandomSkinTone()
        {
            Color[] tones = {
                new Color(0.9f, 0.75f, 0.6f),
                new Color(0.8f, 0.65f, 0.5f),
                new Color(0.6f, 0.45f, 0.35f),
                new Color(0.5f, 0.35f, 0.25f),
                new Color(0.4f, 0.25f, 0.2f)
            };

            return tones[Random.Range(0, tones.Length)];
        }

        private void CleanupNPCs()
        {
            for (int i = m_spawnedNPCs.Count - 1; i >= 0; i--)
            {
                if (m_spawnedNPCs[i] == null)
                {
                    m_spawnedNPCs.RemoveAt(i);
                    continue;
                }

                float distance = Vector3.Distance(m_spawnedNPCs[i].transform.position, transform.position);
                if (distance > m_patrolRadius * 3)
                {
                    Destroy(m_spawnedNPCs[i]);
                    m_spawnedNPCs.RemoveAt(i);
                }
            }
        }

        public void SetBuildingActive(bool active)
        {
            m_isBuildingActive = active;

            foreach (var npc in m_spawnedNPCs)
            {
                if (npc != null)
                {
                    npc.SetActive(active);
                }
            }
        }

        public int GetNPCCount() => m_spawnedNPCs.Count;

        public void AddSpawnPoint(Transform point)
        {
            List<Transform> points = new List<Transform>(m_spawnPoints);
            points.Add(point);
            m_spawnPoints = points.ToArray();
        }
    }

    public class BuildingNPC : MonoBehaviour
    {
        [SerializeField] private BuildingNPCCpawner.NPCBehavior m_behavior;
        [SerializeField] private float m_patrolRadius = 20f;
        [SerializeField] private bool m_wander = true;
        [SerializeField] private float m_moveSpeed = 2f;

        private Rigidbody m_rb;
        private Vector3 m_originPoint;
        private Vector3 m_targetPoint;
        private bool m_isMoving;

        public void Initialize(BuildingNPCCpawner.NPCBehavior behavior, float radius, bool wander)
        {
            m_behavior = behavior;
            m_patrolRadius = radius;
            m_wander = wander;
            m_originPoint = transform.position;
            m_targetPoint = m_originPoint;

            m_rb = GetComponent<Rigidbody>();
            if (m_rb == null)
            {
                m_rb = gameObject.AddComponent<Rigidbody>();
            }
            m_rb.isKinematic = true;

            SetNewTarget();
        }

        private void Update()
        {
            if (!m_isMoving) return;

            MoveToTarget();
        }

        private void MoveToTarget()
        {
            Vector3 direction = (m_targetPoint - transform.position).normalized;
            direction.y = 0;

            transform.position += direction * m_moveSpeed * Time.deltaTime;
            transform.LookAt(m_targetPoint);

            float distance = Vector3.Distance(transform.position, m_targetPoint);
            if (distance < 0.5f)
            {
                SetNewTarget();
            }
        }

        private void SetNewTarget()
        {
            switch (m_behavior)
            {
                case BuildingNPCCpawner.NPCBehavior.Patrol:
                    m_targetPoint = GetRandomPointInRadius(m_originPoint, m_patrolRadius);
                    break;

                case BuildingNPCCpawner.NPCBehavior.Wander:
                    if (m_wander)
                    {
                        m_targetPoint = GetRandomPointInRadius(m_originPoint, m_patrolRadius);
                    }
                    else
                    {
                        m_targetPoint = m_originPoint;
                    }
                    break;

                case BuildingNPCCpawner.NPCBehavior.Stationary:
                    m_isMoving = false;
                    return;
            }

            m_isMoving = true;
        }

        private Vector3 GetRandomPointInRadius(Vector3 center, float radius)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            return center + new Vector3(circle.x, 0, circle.y);
        }

        public void SetBuildingOrigin(Transform origin)
        {
            m_originPoint = origin != null ? origin.position : transform.position;
        }
    }
}
