using UnityEngine;

namespace CarSimulator.World
{
    public class WeatherFireInteraction : MonoBehaviour
    {
        public static WeatherFireInteraction Instance { get; private set; }

        [Header("Weather Impact Settings")]
        [SerializeField] private bool m_enableWeatherImpact = true;

        [Header("Rain Effects")]
        [SerializeField] private float m_rainFireReduction = 0.5f;
        [SerializeField] private float m_stormFireReduction = 0.8f;

        [Header("Wind Effects")]
        [SerializeField] private float m_windSpreadMultiplier = 1.5f;
        [SerializeField] private float m_windSpreadThreshold = 15f;

        [Header("Snow Effects")]
        [SerializeField] private float m_snowFireExtinguishChance = 0.1f;

        [Header("Temperature")]
        [SerializeField] private float m_freezingFireChance = 0.3f;
        [SerializeField] private float m_freezingThreshold = 0f;

        private WeatherSystem m_weatherSystem;
        private BuildingFireSystem m_fireSystem;

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
            FindSystems();
        }

        private void FindSystems()
        {
            m_weatherSystem = FindObjectOfType<WeatherSystem>();
            m_fireSystem = FindObjectOfType<BuildingFireSystem>();

            if (m_weatherSystem != null)
            {
                m_weatherSystem.OnWeatherChanged += OnWeatherChanged;
            }
        }

        private void OnWeatherChanged(WeatherSystem.WeatherType weather)
        {
            if (!m_enableWeatherImpact) return;

            ApplyWeatherToFire(weather);
        }

        private void Update()
        {
            if (!m_enableWeatherImpact) return;

            ContinuousWeatherEffects();
        }

        private void ApplyWeatherToFire(WeatherSystem.WeatherType weather)
        {
            switch (weather)
            {
                case WeatherSystem.WeatherType.Rain:
                    ApplyRainEffects(m_rainFireReduction);
                    break;

                case WeatherSystem.WeatherType.Storm:
                    ApplyRainEffects(m_stormFireReduction);
                    ApplyWindEffects();
                    break;

                case WeatherSystem.WeatherType.Snow:
                    ApplySnowEffects();
                    break;

                case WeatherSystem.WeatherType.Clear:
                case WeatherSystem.WeatherType.Sunny:
                    ResetWeatherEffects();
                    break;

                case WeatherSystem.WeatherType.Cloudy:
                    ApplyCloudEffects();
                    break;

                case WeatherSystem.WeatherType.Fog:
                    ApplyFogEffects();
                    break;
            }
        }

        private void ApplyRainEffects(float reduction)
        {
            if (m_fireSystem == null) return;

            int fireCount = m_fireSystem.GetActiveFireCount();
            if (fireCount > 0)
            {
                Debug.Log($"[WeatherFire] Rain reducing fire intensity by {reduction * 100}%");

                if (m_fireSystem.IsOnFire())
                {
                    int firesToReduce = Mathf.FloorToInt(fireCount * reduction);
                    for (int i = 0; i < firesToReduce && i < fireCount; i++)
                    {
                        m_fireSystem.ExtinguishAll();
                    }
                }
            }
        }

        private void ApplyWindEffects()
        {
            if (m_weatherSystem == null || m_fireSystem == null) return;

            float windSpeed = m_weatherSystem.CurrentWindSpeed;

            if (windSpeed > m_windSpreadThreshold)
            {
                Debug.Log($"[WeatherFire] High wind ({windSpeed} km/h) increasing fire spread");

                var fire = FindObjectOfType<BuildingFireSystem>();
                if (fire != null)
                {
                    Vector3 windDirection = m_weatherSystem.CurrentWindDirection;
                    Vector3 spreadDirection = fire.transform.position + windDirection * 5f;
                }
            }
        }

        private void ApplySnowEffects()
        {
            if (m_fireSystem == null) return;

            if (Random.value < m_snowFireExtinguishChance)
            {
                Debug.Log("[WeatherFire] Snow extinguished a fire!");
                m_fireSystem.ExtinguishAll();
            }
        }

        private void ApplyCloudEffects()
        {
            if (m_fireSystem == null) return;

            Debug.Log("[WeatherFire] Cloudy - normal fire behavior");
        }

        private void ApplyFogEffects()
        {
            if (m_fireSystem == null) return;

            Debug.Log("[WeatherFire] Fog - dampening fire spread");
        }

        private void ResetWeatherEffects()
        {
            Debug.Log("[WeatherFire] Clear weather - fire can spread normally");
        }

        private void ContinuousWeatherEffects()
        {
            if (m_weatherSystem == null || m_fireSystem == null) return;

            WeatherSystem.WeatherType weather = m_weatherSystem.CurrentWeather;

            if (weather == WeatherSystem.WeatherType.Rain || weather == WeatherSystem.WeatherType.Storm)
            {
                ContinuousRainEffect();
            }

            if (m_weatherSystem.CurrentWindSpeed > m_windSpreadThreshold)
            {
                ContinuousWindEffect();
            }

            if (weather == WeatherSystem.WeatherType.Snow)
            {
                ContinuousSnowEffect();
            }
        }

        private void ContinuousRainEffect()
        {
            float intensity = m_weatherSystem.CurrentWeather == WeatherSystem.WeatherType.Storm 
                ? m_stormFireReduction 
                : m_rainFireReduction;

            if (Random.value < intensity * 0.01f)
            {
                m_fireSystem.ExtinguishAt(
                    m_fireSystem.transform.position + Random.insideUnitSphere * 10f,
                    5f
                );
            }
        }

        private void ContinuousWindEffect()
        {
            if (m_fireSystem == null) return;

            float windSpeed = m_weatherSystem.CurrentWindSpeed;
            float spreadChance = (windSpeed - m_windSpreadThreshold) * 0.01f * m_windSpreadMultiplier;

            if (Random.value < spreadChance)
            {
                Debug.Log("[WeatherFire] Wind spreading fire to nearby buildings");
            }
        }

        private void ContinuousSnowEffect()
        {
            if (m_fireSystem == null) return;

            if (Random.value < m_snowFireExtinguishChance * 0.1f)
            {
                m_fireSystem.ExtinguishAll();
            }
        }

        public float GetFireRisk()
        {
            if (m_weatherSystem == null) return 0.5f;

            WeatherSystem.WeatherType weather = m_weatherSystem.CurrentWeather;

            switch (weather)
            {
                case WeatherSystem.WeatherType.Clear:
                case WeatherSystem.WeatherType.Sunny:
                    return 0.8f;

                case WeatherSystem.WeatherType.Cloudy:
                    return 0.5f;

                case WeatherSystem.WeatherType.Rain:
                    return 0.2f;

                case WeatherSystem.WeatherType.Storm:
                    return 0.1f;

                case WeatherSystem.WeatherType.Snow:
                    return 0.05f;

                case WeatherSystem.WeatherType.Fog:
                    return 0.3f;

                default:
                    return 0.5f;
            }
        }

        public void SetWeatherImpactEnabled(bool enabled)
        {
            m_enableWeatherImpact = enabled;
        }

        private void OnDestroy()
        {
            if (m_weatherSystem != null)
            {
                m_weatherSystem.OnWeatherChanged -= OnWeatherChanged;
            }
        }
    }
}
