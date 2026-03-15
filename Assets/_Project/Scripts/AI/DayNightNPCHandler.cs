using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class DayNightNPCHandler : MonoBehaviour
    {
        public static DayNightNPCHandler Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableDayNightBehavior = true;
        [SerializeField] private bool m_spawnDayNPCs = true;
        [SerializeField] private bool m_spawnNightNPCs = true;

        [Header("Day Settings")]
        [SerializeField] private int m_dayNPCSpawnCount = 30;
        [SerializeField] private float m_daySpawnRadius = 100f;
        [SerializeField] private NPCBehavior m_dayBehavior = NPCBehavior.Patrol;

        [Header("Night Settings")]
        [SerializeField] private int m_nightNPCSpawnCount = 15;
        [SerializeField] private float m_nightSpawnRadius = 50f;
        [SerializeField] private NPCBehavior m_nightBehavior = NPCBehavior.Wander;
        [SerializeField] private bool m_nightNPCsStayIndoors = true;

        [Header("Transition")]
        [SerializeField] private float m_transitionTime = 30f;
        [SerializeField] private bool m_gradualTransition = true;

        [Header("NPC Templates")]
        [SerializeField] private GameObject m_dayNPCTemplate;
        [SerializeField] private GameObject m_nightNPCTemplate;

        private List<GameObject> m_activeNPCs = new List<GameObject>();
        private bool m_isDay = true;
        private float m_transitionTimer;
        private float m_currentSpawnCount;

        public bool IsDay => m_isDay;

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
            FindTimeOfDay();
            SpawnNPCsForCurrentTime();
        }

        private void FindTimeOfDay()
        {
            var timeOfDay = FindObjectOfType<World.EnhancedTimeOfDay>();
            if (timeOfDay != null)
            {
                m_isDay = timeOfDay.IsDay;
                timeOfDay.OnDayStart += OnDayStart;
                timeOfDay.OnNightStart += OnNightStart;
            }
        }

        private void Update()
        {
            if (!m_enableDayNightBehavior) return;

            UpdateTransition();
        }

        private void UpdateTransition()
        {
            var timeOfDay = FindObjectOfType<World.EnhancedTimeOfDay>();
            if (timeOfDay == null) return;

            bool wasDay = m_isDay;
            m_isDay = timeOfDay.IsDay;

            if (wasDay != m_isDay)
            {
                OnTimeOfDayChanged();
            }
        }

        private void OnDayStart()
        {
            m_isDay = true;
            OnTimeOfDayChanged();
        }

        private void OnNightStart()
        {
            m_isDay = false;
            OnTimeOfDayChanged();
        }

        private void OnTimeOfDayChanged()
        {
            if (m_gradualTransition)
            {
                StartCoroutine(GradualTransition());
            }
            else
            {
                ImmediateTransition();
            }
        }

        private System.Collections.IEnumerator GradualTransition()
        {
            float t = 0;

            while (t < 1f)
            {
                t += Time.deltaTime / m_transitionTime;

                if (m_isDay)
                {
                    SpawnDayNPCs(Mathf.FloorToInt(t * m_dayNPCSpawnCount / m_transitionTime));
                }
                else
                {
                    RemoveNightNPCs(Mathf.FloorToInt(t * m_nightNPCSpawnCount / m_transitionTime));
                }

                yield return null;
            }

            FinalizeTransition();
        }

        private void ImmediateTransition()
        {
            ClearAllNPCs();
            SpawnNPCsForCurrentTime();
        }

        private void FinalizeTransition()
        {
            if (m_isDay)
            {
                SpawnDayNPCs(m_dayNPCSpawnCount);
            }
            else
            {
                SpawnNightNPCs(m_nightNPCSpawnCount);
            }
        }

        private void SpawnNPCsForCurrentTime()
        {
            if (m_isDay && m_spawnDayNPCs)
            {
                SpawnDayNPCs(m_dayNPCSpawnCount);
            }
            else if (!m_isDay && m_spawnNightNPCs)
            {
                SpawnNightNPCs(m_nightNPCSpawnCount);
            }
        }

        private void SpawnDayNPCs(int count)
        {
            ClearAllNPCs();

            for (int i = 0; i < count; i++)
            {
                GameObject npc = CreateNPC(m_dayNPCTemplate, "DayNPC");
                if (npc != null)
                {
                    SetNPCBehavior(npc, m_dayBehavior);
                    m_activeNPCs.Add(npc);
                }
            }

            Debug.Log($"[DayNightNPC] Spawned {count} day NPCs");
        }

        private void SpawnNightNPCs(int count)
        {
            ClearAllNPCs();

            for (int i = 0; i < count; i++)
            {
                GameObject npc = CreateNPC(m_nightNPCTemplate, "NightNPC");
                if (npc != null)
                {
                    SetNPCBehavior(npc, m_nightBehavior);

                    if (m_nightNPCsStayIndoors)
                    {
                        StayNearBuildings(npc);
                    }

                    SetNPCAppearance(npc, true);

                    m_activeNPCs.Add(npc);
                }
            }

            Debug.Log($"[DayNightNPC] Spawned {count} night NPCs");
        }

        private void RemoveNightNPCs(int count)
        {
            int removed = 0;
            for (int i = m_activeNPCs.Count - 1; i >= 0 && removed < count; i--)
            {
                if (m_activeNPCs[i] != null)
                {
                    Destroy(m_activeNPCs[i]);
                    m_activeNPCs.RemoveAt(i);
                    removed++;
                }
            }
        }

        private GameObject CreateNPC(GameObject template, string name)
        {
            GameObject npc;

            if (template != null)
            {
                npc = Instantiate(template, GetRandomSpawnPosition(), Quaternion.identity);
            }
            else
            {
                npc = CreatePlaceholderNPC();
            }

            npc.name = name;
            return npc;
        }

        private GameObject CreatePlaceholderNPC()
        {
            GameObject npc = new GameObject("NPC");
            npc.transform.position = GetRandomSpawnPosition();

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(npc.transform);
            body.transform.localPosition = new Vector3(0, 1f, 0);

            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = GetRandomNPCColor();
            }

            npc.tag = "NPC";
            return npc;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            Vector2 circle = Random.insideUnitCircle * (m_isDay ? m_daySpawnRadius : m_nightSpawnRadius);
            Vector3 spawnPos = new Vector3(circle.x, 0, circle.y);

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                spawnPos += player.transform.position;
            }

            return spawnPos;
        }

        private void SetNPCBehavior(GameObject npc, NPCBehavior behavior)
        {
            var buildingNPC = npc.GetComponent<BuildingNPC>();
            if (buildingNPC != null)
            {
                // Set behavior based on type
            }
        }

        private void StayNearBuildings(GameObject npc)
        {
            var buildings = FindObjectsOfType<Buildings.ApartmentBuilding>();
            if (buildings.Length > 0)
            {
                var building = buildings[Random.Range(0, buildings.Length)];
                npc.transform.position = building.transform.position + Random.insideUnitSphere * 10f;
            }
        }

        private void SetNPCAppearance(GameObject npc, bool isNight)
        {
            Renderer[] renderers = npc.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    if (isNight)
                    {
                        renderer.material.color = new Color(0.2f, 0.2f, 0.25f);
                    }
                    else
                    {
                        renderer.material.color = GetRandomNPCColor();
                    }
                }
            }
        }

        private Color GetRandomNPCColor()
        {
            Color[] colors = {
                new Color(0.8f, 0.6f, 0.5f),
                new Color(0.6f, 0.5f, 0.4f),
                new Color(0.5f, 0.4f, 0.3f),
                new Color(0.7f, 0.6f, 0.5f),
                new Color(0.9f, 0.7f, 0.6f)
            };

            return colors[Random.Range(0, colors.Length)];
        }

        private void ClearAllNPCs()
        {
            foreach (var npc in m_activeNPCs)
            {
                if (npc != null)
                {
                    Destroy(npc);
                }
            }
            m_activeNPCs.Clear();
        }

        public int GetActiveNPCCount() => m_activeNPCs.Count;
        public void SetDayBehavior(NPCBehavior behavior) => m_dayBehavior = behavior;
        public void SetNightBehavior(NPCBehavior behavior) => m_nightBehavior = behavior;

        private void OnDestroy()
        {
            var timeOfDay = FindObjectOfType<World.EnhancedTimeOfDay>();
            if (timeOfDay != null)
            {
                timeOfDay.OnDayStart -= OnDayStart;
                timeOfDay.OnNightStart -= OnNightStart;
            }
        }
    }
}
