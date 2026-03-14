using UnityEngine;

namespace CarSimulator.Optimization
{
    public class MobileHelper : MonoBehaviour
    {
        public static MobileHelper Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_autoDetect = true;
        [SerializeField] private bool m_isMobile;

        [Header("Mobile Optimizations")]
        [SerializeField] private int m_targetFrameRate = 30;
        [SerializeField] private bool m_reduceParticles = true;
        [SerializeField] private bool m_simplifyShadows = true;

        public bool IsMobile => m_isMobile;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (m_autoDetect)
            {
                DetectMobile();
            }

            if (m_isMobile)
            {
                ApplyMobileSettings();
            }
        }

        private void DetectMobile()
        {
            m_isMobile = Application.isMobilePlatform;
            Debug.Log($"[MobileHelper] Mobile detected: {m_isMobile}");
        }

        private void ApplyMobileSettings()
        {
            Application.targetFrameRate = m_targetFrameRate;

            if (m_reduceParticles)
            {
                var particles = FindObjectsOfType<ParticleSystem>();
                foreach (var ps in particles)
                {
                    var main = ps.main;
                    main.maxParticles = Mathf.Min(main.maxParticles, 50);
                }
            }

            if (m_simplifyShadows)
            {
                QualitySettings.shadowQuality = ShadowQuality.HardOnly;
                QualitySettings.shadowResolution = ShadowResolution.Low;
                QualitySettings.shadowDistance = 30f;
            }

            Debug.Log("[MobileHelper] Mobile settings applied");
        }

        public void SetTargetFrameRate(int fps)
        {
            m_targetFrameRate = fps;
            Application.targetFrameRate = fps;
        }

        public void OptimizeForMobile()
        {
            m_isMobile = true;
            ApplyMobileSettings();
        }

        public void ResetToDefaults()
        {
            Application.targetFrameRate = 60;
            QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
        }
    }

    public class PerformanceMonitor : MonoBehaviour
    {
        public static PerformanceMonitor Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_logPerformance;
        [SerializeField] private float m_logInterval = 5f;

        private float m_timer;
        private int m_frameCount;
        private float m_fps;

        public float FPS => m_fps;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            m_frameCount++;
            m_timer += Time.deltaTime;

            if (m_timer >= m_logInterval)
            {
                m_fps = m_frameCount / m_timer;
                m_frameCount = 0;
                m_timer = 0;

                if (m_logPerformance)
                {
                    Debug.Log($"[PerformanceMonitor] FPS: {m_fps:F1}");
                }
            }
        }
    }
}
