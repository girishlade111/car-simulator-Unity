using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    [ExecuteInEditMode]
    public class PropGroupContainer : MonoBehaviour
    {
        [Header("Group Info")]
        [SerializeField] private string m_groupName = "Prop Group";
        [SerializeField] private PropGroupType m_groupType = PropGroupType.Street;

        [Header("Optimization")]
        [SerializeField] private bool m_autoLOD = true;
        [SerializeField] private float m_lodStartDistance = 30f;
        [SerializeField] private float m_lodEndDistance = 60f;
        [SerializeField] private bool m_initiallyEnabled = true;

        [Header("Props")]
        [SerializeField] private PropSpawnAnchor[] m_propAnchors;
        [SerializeField] private List<GameObject> m_spawnedProps = new List<GameObject>();

        [Header("Performance Stats")]
        [SerializeField] private int m_propCount;
        [SerializeField] private int m_activeCount;

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_gizmoColor = Color.cyan;

        public enum PropGroupType
        {
            Street,
            Park,
            Building,
            Parking,
            Plaza,
            Nature,
            Detailed
        }

        private bool m_isEnabled;
        private Transform m_playerTransform;

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                RefreshAnchors();
            }
            else
            {
                if (m_initiallyEnabled)
                {
                    EnableGroup();
                }
                FindPlayer();
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                ClearProps();
            }
        }

        private void Start()
        {
            if (Application.isPlaying && m_initiallyEnabled)
            {
                SpawnAllProps();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying || !m_autoLOD) return;

            UpdateLOD();
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

        private void RefreshAnchors()
        {
            m_propAnchors = GetComponentsInChildren<PropSpawnAnchor>();
            m_propCount = m_propAnchors.Length;
        }

        public void SpawnAllProps()
        {
            ClearProps();

            RefreshAnchors();

            foreach (var anchor in m_propAnchors)
            {
                if (anchor == null || !anchor.IsActive) continue;

                GameObject prop = SpawnFromAnchor(anchor);
                if (prop != null)
                {
                    m_spawnedProps.Add(prop);
                }
            }

            m_activeCount = m_spawnedProps.Count;
            Debug.Log($"[PropGroupContainer] Spawned {m_activeCount} props in {m_groupName}");
        }

        private GameObject SpawnFromAnchor(PropSpawnAnchor anchor)
        {
            if (PropManager.Instance == null)
            {
                return CreateFallbackProp(anchor);
            }

            var category = PropManager.Instance.GetCategory(anchor.Category.ToString());
            if (category == null || category.prefabs == null || category.prefabs.Length == 0)
            {
                return CreateFallbackProp(anchor);
            }

            GameObject prefab = category.prefabs[Random.Range(0, category.prefabs.Length)];
            if (prefab == null)
            {
                return CreateFallbackProp(anchor);
            }

            GameObject prop = Instantiate(prefab, anchor.GetSpawnPosition(), anchor.GetSpawnRotation());
            prop.transform.localScale = anchor.GetSpawnScale();
            prop.transform.SetParent(transform);

            return prop;
        }

        private GameObject CreateFallbackProp(PropSpawnAnchor anchor)
        {
            GameObject prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prop.name = $"FallbackProp_{m_spawnedProps.Count}";
            prop.transform.position = anchor.GetSpawnPosition();
            prop.transform.rotation = anchor.GetSpawnRotation();
            prop.transform.localScale = anchor.GetSpawnScale();
            prop.transform.SetParent(transform);

            Renderer renderer = prop.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = GetGroupColor();
            }

            return prop;
        }

        private Color GetGroupColor()
        {
            switch (m_groupType)
            {
                case PropGroupType.Street: return Color.yellow;
                case PropGroupType.Park: return Color.green;
                case PropGroupType.Building: return Color.magenta;
                case PropGroupType.Parking: return Color.cyan;
                case PropGroupType.Plaza: return Color.white;
                case PropGroupType.Nature: return new Color(0.2f, 0.6f, 0.2f);
                case PropGroupType.Detailed: return new Color(1f, 0.5f, 0f);
                default: return Color.gray;
            }
        }

        public void EnableGroup()
        {
            m_isEnabled = true;

            foreach (var prop in m_spawnedProps)
            {
                if (prop != null)
                {
                    prop.SetActive(true);
                }
            }

            m_activeCount = m_spawnedProps.Count;
        }

        public void DisableGroup()
        {
            m_isEnabled = false;

            foreach (var prop in m_spawnedProps)
            {
                if (prop != null)
                {
                    prop.SetActive(false);
                }
            }

            m_activeCount = 0;
        }

        public void ToggleGroup()
        {
            if (m_isEnabled)
            {
                DisableGroup();
            }
            else
            {
                EnableGroup();
            }
        }

        private void UpdateLOD()
        {
            if (m_playerTransform == null || !m_autoLOD) return;

            float dist = Vector3.Distance(transform.position, m_playerTransform.position);

            if (dist > m_lodEndDistance)
            {
                DisableGroup();
            }
            else if (dist < m_lodStartDistance)
            {
                EnableGroup();
            }
        }

        public void ClearProps()
        {
            foreach (var prop in m_spawnedProps)
            {
                if (prop != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(prop);
                    }
                    else
                    {
                        DestroyImmediate(prop);
                    }
                }
            }
            m_spawnedProps.Clear();
            m_activeCount = 0;
        }

        public void Refresh()
        {
            SpawnAllProps();
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_gizmoColor;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);

            Gizmos.color = m_gizmoColor * 0.5f;
            Bounds bounds = new Bounds(transform.position, Vector3.one * 10f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public bool IsEnabled() => m_isEnabled;
        public int GetPropCount() => m_propCount;
        public int GetActiveCount() => m_activeCount;
    }

    public class PropOptimizationManager : MonoBehaviour
    {
        public static PropOptimizationManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_updateInterval = 0.5f;

        [Header("Groups")]
        [SerializeField] private List<PropGroupContainer> m_propGroups = new List<PropGroupContainer>();

        [Header("Player Reference")]
        [SerializeField] private Transform m_playerTransform;

        private float m_updateTimer;

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
            FindAllGroups();
        }

        private void Update()
        {
            if (!m_enabled) return;

            m_updateTimer += Time.deltaTime;
            if (m_updateTimer >= m_updateInterval)
            {
                m_updateTimer = 0;
                UpdatePropGroups();
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void FindAllGroups()
        {
            var groups = FindObjectsOfType<PropGroupContainer>();
            m_propGroups.AddRange(groups);

            Debug.Log($"[PropOptimizationManager] Found {m_propGroups.Count} prop groups");
        }

        private void UpdatePropGroups()
        {
            if (m_playerTransform == null) return;

            foreach (var group in m_propGroups)
            {
                if (group == null) continue;

                float dist = Vector3.Distance(m_playerTransform.position, group.transform.position);

                if (dist > 100f)
                {
                    group.DisableGroup();
                }
                else if (dist < 50f)
                {
                    group.EnableGroup();
                }
            }
        }

        public void RegisterGroup(PropGroupContainer group)
        {
            if (!m_propGroups.Contains(group))
            {
                m_propGroups.Add(group);
            }
        }

        public void UnregisterGroup(PropGroupContainer group)
        {
            m_propGroups.Remove(group);
        }

        public void EnableAllGroups()
        {
            foreach (var group in m_propGroups)
            {
                group?.EnableGroup();
            }
        }

        public void DisableAllGroups()
        {
            foreach (var group in m_propGroups)
            {
                group?.DisableGroup();
            }
        }

        public int GetTotalPropCount()
        {
            int count = 0;
            foreach (var group in m_propGroups)
            {
                if (group != null)
                {
                    count += group.GetPropCount();
                }
            }
            return count;
        }

        public int GetActivePropCount()
        {
            int count = 0;
            foreach (var group in m_propGroups)
            {
                if (group != null)
                {
                    count += group.GetActiveCount();
                }
            }
            return count;
        }
    }
}
