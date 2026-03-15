using UnityEngine;

namespace CarSimulator.World
{
    public class BuildingWeatherEffects : MonoBehaviour
    {
        [Header("Weather Settings")]
        [SerializeField] private bool m_enableRainEffects = true;
        [SerializeField] private bool m_enableSnowEffects = true;
        [SerializeField] private bool m_enableWindEffects = true;

        [Header("Rain Effects")]
        [SerializeField] private Material m_wetMaterial;
        [SerializeField] private float m_wetnessIntensity = 0.5f;
        [SerializeField] private GameObject m_dripParticles;
        [SerializeField] private float m_dripInterval = 0.5f;

        [Header("Snow Effects")]
        [SerializeField] private Material m_snowMaterial;
        [SerializeField] private float m_snowAccumulation = 0.3f;
        [SerializeField] private GameObject m_snowParticles;
        [SerializeField] private Transform[] m_roofTransforms;

        [Header("Wind Effects")]
        [SerializeField] private float m_windowShakeIntensity = 0.02f;
        [SerializeField] private Transform[] m_looseObjects;
        [SerializeField] private float m_windAffectedThreshold = 20f;

        [Header("Temperature")]
        [SerializeField] private float m_freezingThreshold = 0f;
        [SerializeField] private bool m_showFrost;
        [SerializeField] private Material m_frostMaterial;

        [Header("Optimization")]
        [SerializeField] private float m_updateInterval = 0.1f;

        private Renderer[] m_buildingRenderers;
        private float m_currentWetness;
        private float m_currentSnow;
        private float m_dripTimer;
        private float m_lastUpdateTime;
        private WeatherSystem.WeatherType m_currentWeather;

        private void Start()
        {
            GetBuildingRenderers();
            FindWeatherSystem();
        }

        private void GetBuildingRenderers()
        {
            m_buildingRenderers = GetComponentsInChildren<Renderer>();
        }

        private void FindWeatherSystem()
        {
            var weather = FindObjectOfType<WeatherSystem>();
            if (weather != null)
            {
                weather.OnWeatherChanged += OnWeatherChanged;
            }
        }

        private void OnWeatherChanged(WeatherSystem.WeatherType weather)
        {
            m_currentWeather = weather;
            ApplyWeatherEffects(weather);
        }

        private void Update()
        {
            if (Time.time - m_lastUpdateTime < m_updateInterval) return;
            m_lastUpdateTime = Time.time;

            UpdateWeatherEffects();
        }

        private void UpdateWeatherEffects()
        {
            if (m_currentWeather == WeatherSystem.WeatherType.Rain || m_currentWeather == WeatherSystem.WeatherType.Storm)
            {
                UpdateRainEffects();
            }
            else if (m_currentWeather == WeatherSystem.WeatherType.Snow)
            {
                UpdateSnowEffects();
            }
            else
            {
                DryBuilding();
            }

            if (m_enableWindEffects && m_currentWeather != WeatherSystem.WeatherType.Clear)
            {
                UpdateWindEffects();
            }
        }

        private void ApplyWeatherEffects(WeatherSystem.WeatherType weather)
        {
            switch (weather)
            {
                case WeatherSystem.WeatherType.Clear:
                case WeatherSystem.WeatherType.Sunny:
                    DryBuilding();
                    RemoveSnow();
                    break;

                case WeatherSystem.WeatherType.Cloudy:
                    DryBuilding();
                    break;

                case WeatherSystem.WeatherType.Rain:
                case WeatherSystem.WeatherType.Storm:
                    WetBuilding();
                    RemoveSnow();
                    break;

                case WeatherSystem.WeatherType.Snow:
                    WetBuilding();
                    AccumulateSnow();
                    break;

                case WeatherSystem.WeatherType.Fog:
                    ApplyFogEffect();
                    break;
            }
        }

        private void WetBuilding()
        {
            if (m_buildingRenderers == null) return;

            foreach (var renderer in m_buildingRenderers)
            {
                if (renderer == null) continue;

                Material mat = renderer.material;
                if (mat != null)
                {
                    mat.SetFloat("_Glossiness", m_wetnessIntensity);
                    mat.SetFloat("_Wetness", m_currentWetness);
                }
            }

            m_currentWetness = Mathf.Lerp(m_currentWetness, 1f, Time.deltaTime * 0.5f);
        }

        private void DryBuilding()
        {
            if (m_buildingRenderers == null) return;

            foreach (var renderer in m_buildingRenderers)
            {
                if (renderer == null) continue;

                Material mat = renderer.material;
                if (mat != null)
                {
                    mat.SetFloat("_Glossiness", 0f);
                    mat.SetFloat("_Wetness", m_currentWetness);
                }
            }

            m_currentWetness = Mathf.Lerp(m_currentWetness, 0f, Time.deltaTime * 0.3f);
        }

        private void UpdateRainEffects()
        {
            m_dripTimer += Time.deltaTime;

            if (m_dripTimer >= m_dripInterval)
            {
                m_dripTimer = 0f;
                SpawnDrip();
            }
        }

        private void SpawnDrip()
        {
            if (m_dripParticles == null || m_buildingRenderers == null || m_buildingRenderers.Length == 0) return;

            Renderer randomRenderer = m_buildingRenderers[Random.Range(0, m_buildingRenderers.Length)];
            if (randomRenderer == null) return;

            Vector3 spawnPos = randomRenderer.bounds.center;
            spawnPos.y = randomRenderer.bounds.max.y;

            GameObject drip = Instantiate(m_dripParticles, spawnPos, Quaternion.identity);
            Destroy(drip, 2f);
        }

        private void AccumulateSnow()
        {
            m_currentSnow = Mathf.Lerp(m_currentSnow, m_snowAccumulation, Time.deltaTime * 0.1f);

            foreach (var roof in m_roofTransforms)
            {
                if (roof == null) continue;

                Vector3 scale = roof.localScale;
                scale.y = m_currentSnow;
                roof.localScale = scale;
            }

            if (m_snowParticles != null)
            {
                if (!m_snowParticles.activeSelf)
                {
                    m_snowParticles.SetActive(true);
                }
            }
        }

        private void RemoveSnow()
        {
            m_currentSnow = Mathf.Lerp(m_currentSnow, 0f, Time.deltaTime * 0.2f);

            if (m_snowParticles != null && m_currentSnow <= 0.01f)
            {
                m_snowParticles.SetActive(false);
            }
        }

        private void UpdateSnowEffects()
        {
            if (m_currentSnow < m_snowAccumulation)
            {
                AccumulateSnow();
            }
        }

        private void UpdateWindEffects()
        {
            if (m_looseObjects == null) return;

            float windStrength = GetWindStrength();

            foreach (var obj in m_looseObjects)
            {
                if (obj == null) continue;

                float shake = Mathf.Sin(Time.time * 10f) * m_windowShakeIntensity * windStrength;
                obj.localPosition += new Vector3(shake, 0, shake);
            }
        }

        private float GetWindStrength()
        {
            var weather = FindObjectOfType<WeatherSystem>();
            if (weather != null)
            {
                return weather.CurrentWindSpeed;
            }
            return 10f;
        }

        private void ApplyFogEffect()
        {
            if (m_buildingRenderers == null) return;

            foreach (var renderer in m_buildingRenderers)
            {
                if (renderer == null) continue;

                Material mat = renderer.material;
                if (mat != null)
                {
                    mat.SetFloat("_FogDensity", 0.3f);
                }
            }
        }

        private void OnDestroy()
        {
            var weather = FindObjectOfType<WeatherSystem>();
            if (weather != null)
            {
                weather.OnWeatherChanged -= OnWeatherChanged;
            }
        }
    }
}
