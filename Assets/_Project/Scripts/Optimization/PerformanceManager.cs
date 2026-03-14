using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Optimization
{
    public class PerformanceManager : MonoBehaviour
    {
        public static PerformanceManager Instance { get; private set; }

        [Header("Quality Settings")]
        [SerializeField] private QualityLevel m_qualityLevel = QualityLevel.Balanced;
        [SerializeField] private bool m_applyOnStart = true;

        [Header("LOD Settings")]
        [SerializeField] private float m_LODDistance = 100f;
        [SerializeField] private float m_LODDistanceFar = 200f;

        [Header("Culling")]
        [SerializeField] private bool m_enableFrustumCulling = true;
        [SerializeField] private bool m_enableOcclusionCulling = false;
        [SerializeField] private float m_cullingDistance = 150f;

        [Header("Object Pooling")]
        [SerializeField] private bool m_useObjectPooling = true;
        [SerializeField] private int m_poolInitialSize = 50;

        [Header("Graphics")]
        [SerializeField] private bool m_vsyncEnabled = true;
        [SerializeField] private int m_targetFrameRate = 60;
        [SerializeField] private bool m_shadowsEnabled = true;
        [SerializeField] private ShadowQuality m_shadowQuality = ShadowQuality.Medium;

        [Header("Particles")]
        [SerializeField] private int m_maxParticles = 500;
        [SerializeField] private bool m_limitParticleDistance = true;
        [SerializeField] private float m_particleDrawDistance = 50f;

        [Header("Audio")]
        [SerializeField] private int m_maxAudioSources = 32;
        [SerializeField] private bool m_virtualizeUnusedAudio = true;

        [Header("Mobile Optimization")]
        [SerializeField] private bool m_mobileOptimizations = false;
        [SerializeField] private bool m_reduceDrawCalls = true;
        [SerializeField] private float m_textureQualityReduction = 0.5f;

        private Transform m_playerTransform;
        private Dictionary<string, ObjectPool> m_pools = new Dictionary<string, ObjectPool>();

        public enum QualityLevel
        {
            Performance,
            Balanced,
            Quality,
            Ultra
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (m_applyOnStart)
            {
                ApplySettings();
            }
        }

        private void Start()
        {
            FindPlayer();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        public void ApplySettings()
        {
            ApplyQualitySettings();
            ApplyGraphicsSettings();
            ApplyAudioSettings();
        }

        private void ApplyQualitySettings()
        {
            switch (m_qualityLevel)
            {
                case QualityLevel.Performance:
                    SetLODDistances(50f, 100f);
                    m_cullingDistance = 80f;
                    m_maxParticles = 200;
                    break;

                case QualityLevel.Balanced:
                    SetLODDistances(100f, 200f);
                    m_cullingDistance = 150f;
                    m_maxParticles = 500;
                    break;

                case QualityLevel.Quality:
                    SetLODDistances(150f, 300f);
                    m_cullingDistance = 200f;
                    m_maxParticles = 1000;
                    break;

                case QualityLevel.Ultra:
                    SetLODDistances(200f, 400f);
                    m_cullingDistance = 300f;
                    m_maxParticles = 2000;
                    break;
            }
        }

        private void ApplyGraphicsSettings()
        {
            Application.targetFrameRate = m_targetFrameRate;
            QualitySettings.vSyncCount = m_vsyncEnabled ? 1 : 0;

            QualitySettings.shadowCascades = (int)m_shadowQuality;
            QualitySettings.shadows = m_shadowsEnabled ? ShadowQuality.All : ShadowQuality.Disable;

            if (m_mobileOptimizations)
            {
                QualitySettings.SetQualityLevel(0, false);
            }
        }

        private void ApplyAudioSettings()
        {
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                if (i >= m_maxAudioSources)
                {
                    sources[i].ignoreListenerVolume = true;
                    sources[i].ignoreListenerPause = true;
                }
            }
        }

        private void Update()
        {
            if (m_playerTransform == null)
            {
                FindPlayer();
                return;
            }

            if (m_enableFrustumCulling)
            {
                UpdateFrustumCulling();
            }

            if (m_limitParticleDistance)
            {
                UpdateParticleCulling();
            }
        }

        private void UpdateFrustumCulling()
        {
            if (Camera.main == null) return;

            Vector3 playerPos = m_playerTransform.position;

            Renderer[] renderers = FindObjectsOfType<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                float dist = Vector3.Distance(playerPos, renderer.transform.position);
                bool shouldBeVisible = dist < m_cullingDistance;

                if (renderer.enabled != shouldBeVisible)
                {
                    renderer.enabled = shouldBeVisible;
                }
            }
        }

        private void UpdateParticleCulling()
        {
            if (Camera.main == null) return;

            ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
            foreach (var ps in particles)
            {
                float dist = Vector3.Distance(m_playerTransform.position, ps.transform.position);
                var main = ps.main;

                if (dist > m_particleDrawDistance)
                {
                    if (main.simulationSpeed > 0)
                    {
                        main.simulationSpeed = 0;
                    }
                }
                else
                {
                    if (main.simulationSpeed == 0)
                    {
                        main.simulationSpeed = 1;
                    }
                }
            }
        }

        public void SetLODDistances(float near, float far)
        {
            m_LODDistance = near;
            m_LODDistanceFar = far;
        }

        public void SetQualityLevel(QualityLevel level)
        {
            m_qualityLevel = level;
            ApplySettings();
        }

        public void SetTargetFrameRate(int fps)
        {
            m_targetFrameRate = Mathf.Clamp(fps, 30, 240);
            Application.targetFrameRate = m_targetFrameRate;
        }

        public void ToggleShadows(bool enabled)
        {
            m_shadowsEnabled = enabled;
            QualitySettings.shadows = enabled ? ShadowQuality.All : ShadowQuality.Disable;
        }

        public ObjectPool GetOrCreatePool(string name, GameObject prefab)
        {
            if (m_pools.ContainsKey(name))
            {
                return m_pools[name];
            }

            GameObject poolObj = new GameObject($"Pool_{name}");
            poolObj.transform.SetParent(transform);

            ObjectPool pool = poolObj.AddComponent<ObjectPool>();
            pool.Initialize(prefab, m_poolInitialSize);

            m_pools[name] = pool;
            return pool;
        }

        public void ReturnToPool(string name, GameObject obj)
        {
            if (m_pools.ContainsKey(name))
            {
                m_pools[name].ReturnObject(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        public int GetActiveObjectCount()
        {
            int count = 0;
            foreach (var pool in m_pools.Values)
            {
                count += pool.ActiveCount;
            }
            return count;
        }

        public int GetPoolCount() => m_pools.Count;

        public QualityLevel GetCurrentQuality() => m_qualityLevel;
    }

    public class ObjectPool : MonoBehaviour
    {
        private GameObject m_prefab;
        private Queue<GameObject> m_available = new Queue<GameObject>();
        private List<GameObject> m_active = new List<GameObject>();

        public int ActiveCount => m_active.Count;

        public void Initialize(GameObject prefab, int initialSize)
        {
            m_prefab = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                CreateObject();
            }
        }

        private void CreateObject()
        {
            GameObject obj = Instantiate(m_prefab, transform);
            obj.SetActive(false);
            m_available.Enqueue(obj);
        }

        public GameObject GetObject()
        {
            if (m_available.Count == 0)
            {
                CreateObject();
            }

            GameObject obj = m_available.Dequeue();
            obj.SetActive(true);
            m_active.Add(obj);

            return obj;
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            m_active.Remove(obj);
            m_available.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (var obj in m_active)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            m_active.Clear();

            while (m_available.Count > 0)
            {
                GameObject obj = m_available.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
