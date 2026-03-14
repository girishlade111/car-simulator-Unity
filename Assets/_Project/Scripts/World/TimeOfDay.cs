using UnityEngine;

namespace CarSimulator.World
{
    public class TimeOfDay : MonoBehaviour
    {
        public static TimeOfDay Instance { get; private set; }

        [Header("Time Settings")]
        [SerializeField] private float m_dayDuration = 600f;
        [SerializeField] private float m_startTime = 6f;
        [SerializeField] private float m_currentTime;
        [SerializeField] private bool m_paused = true;

        [Header("Time of Day")]
        [Range(0f, 24f)] public float m_sunriseTime = 6f;
        [Range(0f, 24f)] public float m_sunsetTime = 18f;
        [Range(0f, 24f)] public float m_noonTime = 12f;

        [Header("References")]
        [SerializeField] private Light m_sunLight;
        [SerializeField] private Light m_moonLight;

        [Header("Colors")]
        [SerializeField] private Color m_dayAmbient = new Color(0.5f, 0.5f, 0.5f);
        [SerializeField] private Color m_nightAmbient = new Color(0.1f, 0.1f, 0.2f);
        [SerializeField] private Color m_sunriseColor = new Color(1f, 0.5f, 0.3f);
        [SerializeField] private Color m_sunsetColor = new Color(1f, 0.4f, 0.2f);

        public float CurrentTime => m_currentTime;
        public float DayProgress => m_currentTime / 24f;
        public bool IsDay => m_currentTime > m_sunriseTime && m_currentTime < m_sunsetTime;
        public bool IsNight => !IsDay;

        private float m_sunIntensity;
        private float m_moonIntensity;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            m_currentTime = m_startTime;
            SetupLights();
        }

        private void SetupLights()
        {
            if (m_sunLight == null)
            {
                var lights = FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type == LightType.Directional && light.intensity > 0)
                    {
                        m_sunLight = light;
                        break;
                    }
                }
            }

            if (m_sunLight != null)
            {
                m_sunIntensity = m_sunLight.intensity;
            }
        }

        private void Start()
        {
            UpdateTimeOfDay();
        }

        private void Update()
        {
            if (!m_paused)
            {
                m_currentTime += Time.deltaTime * 24f / m_dayDuration;
                
                if (m_currentTime >= 24f)
                {
                    m_currentTime = 0f;
                }
            }

            UpdateTimeOfDay();
        }

        private void UpdateTimeOfDay()
        {
            float sunAngle = GetSunAngle();
            
            if (m_sunLight != null)
            {
                m_sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90f, 30f, 0f);
                UpdateSunColor();
            }

            UpdateAmbientLight();
            UpdateMoonLight();
        }

        private float GetSunAngle()
        {
            float t = m_currentTime / 24f;
            return t * 360f;
        }

        private void UpdateSunColor()
        {
            if (m_sunLight == null) return;

            Color targetColor;
            float intensityMultiplier = 1f;

            if (m_currentTime >= m_sunriseTime && m_currentTime < m_noonTime)
            {
                float t = (m_currentTime - m_sunriseTime) / (m_noonTime - m_sunriseTime);
                targetColor = Color.Lerp(m_sunriseColor, Color.white, t);
                intensityMultiplier = Mathf.Lerp(0.3f, 1f, t);
            }
            else if (m_currentTime >= m_noonTime && m_currentTime < m_sunsetTime)
            {
                float t = (m_currentTime - m_noonTime) / (m_sunsetTime - m_noonTime);
                targetColor = Color.Lerp(Color.white, m_sunsetColor, t);
                intensityMultiplier = Mathf.Lerp(1f, 0.3f, t);
            }
            else if (m_currentTime >= m_sunsetTime || m_currentTime < m_sunriseTime)
            {
                targetColor = new Color(0.2f, 0.2f, 0.4f);
                intensityMultiplier = 0.1f;
            }
            else
            {
                targetColor = Color.white;
            }

            m_sunLight.color = targetColor;
            m_sunLight.intensity = m_sunIntensity * intensityMultiplier;
        }

        private void UpdateAmbientLight()
        {
            float dayFactor = IsDay ? 1f : 0f;
            Color ambient = Color.Lerp(m_nightAmbient, m_dayAmbient, dayFactor);
            RenderSettings.ambientLight = ambient;
        }

        private void UpdateMoonLight()
        {
            if (m_moonLight == null) return;

            m_moonLight.enabled = IsNight;
            
            if (IsNight)
            {
                float moonAngle = (m_currentTime + 12f) / 24f * 360f;
                m_moonLight.transform.rotation = Quaternion.Euler(moonAngle - 90f, 30f, 0f);
            }
        }

        public void SetTime(float time)
        {
            m_currentTime = Mathf.Clamp(time, 0f, 24f);
            UpdateTimeOfDay();
        }

        public void SetPaused(bool paused)
        {
            m_paused = paused;
        }

        public void TogglePause()
        {
            m_paused = !m_paused;
        }

        public void SetDayDuration(float duration)
        {
            m_dayDuration = Mathf.Max(60f, duration);
        }

        public string GetTimeString()
        {
            int hours = Mathf.FloorToInt(m_currentTime);
            int minutes = Mathf.FloorToInt((m_currentTime - hours) * 60f);
            return $"{hours:00}:{minutes:00}";
        }
    }
}
