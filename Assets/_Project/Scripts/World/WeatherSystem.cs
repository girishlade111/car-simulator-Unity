using UnityEngine;

namespace CarSimulator.World
{
    public enum WeatherType
    {
        Clear,
        Rain,
        Fog,
        Storm,
        Snow
    }

    public class WeatherSystem : MonoBehaviour
    {
        public static WeatherSystem Instance { get; private set; }

        [Header("Weather Settings")]
        [SerializeField] private WeatherType m_currentWeather = WeatherType.Clear;
        [SerializeField] private float m_transitionDuration = 5f;

        [Header("Rain Settings")]
        [SerializeField] private ParticleSystem m_rainParticles;
        [SerializeField] private float m_rainIntensity = 100f;

        [Header("Fog Settings")]
        [SerializeField] private float m_fogDensity = 0.02f;
        [SerializeField] private Color m_fogColor = Color.gray;

        [Header("Wind Settings")]
        [SerializeField] private float m_windStrength = 5f;
        [SerializeField] private Vector3 m_windDirection = Vector3.forward;

        [Header("Visuals")]
        [SerializeField] private Light m_directionalLight;
        [SerializeField] private Color m_clearSkyColor = new Color(0.5f, 0.7f, 1f);
        [SerializeField] private Color m_rainSkyColor = new Color(0.3f, 0.35f, 0.45f);
        [SerializeField] private Color m_stormSkyColor = new Color(0.15f, 0.15f, 0.2f);

        private float m_transitionProgress;
        private WeatherType m_targetWeather;
        private bool m_isTransitioning;

        public WeatherType CurrentWeather => m_currentWeather;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupWeatherParticles();
        }

        private void Start()
        {
            ApplyWeatherEffect(m_currentWeather, true);
        }

        private void SetupWeatherParticles()
        {
            if (m_rainParticles == null)
            {
                GameObject rainObj = new GameObject("RainParticles");
                rainObj.transform.SetParent(transform);
                rainObj.transform.position = Vector3.up * 20f;

                m_rainParticles = rainObj.AddComponent<ParticleSystem>();
                
                var main = m_rainParticles.main;
                main.startLifetime = 2f;
                main.startSpeed = 20f;
                main.startSize = 0.1f;
                main.maxParticles = 1000;
                main.simulationSpace = ParticleSystemSimulationSpace.World;

                var emission = m_rainParticles.emission;
                emission.rateOverTime = 0;

                var shape = m_rainParticles.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(100f, 1f, 100f);

                var renderer = rainObj.GetComponent<ParticleSystemRenderer>();
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }
        }

        private void Update()
        {
            if (m_isTransitioning)
            {
                UpdateTransition();
            }
        }

        public void SetWeather(WeatherType weather)
        {
            if (weather == m_currentWeather && !m_isTransitioning) return;

            m_targetWeather = weather;
            m_isTransitioning = true;
            m_transitionProgress = 0f;
        }

        private void UpdateTransition()
        {
            m_transitionProgress += Time.deltaTime / m_transitionDuration;

            if (m_transitionProgress >= 1f)
            {
                m_transitionProgress = 1f;
                m_isTransitioning = false;
                m_currentWeather = m_targetWeather;
            }

            float t = Mathf.SmoothStep(0f, 1f, m_transitionProgress);
            InterpolateWeather(m_currentWeather, m_targetWeather, t);
        }

        private void InterpolateWeather(WeatherType from, WeatherType to, float t)
        {
            ApplyWeatherEffect(from, false);
            ApplyWeatherEffect(to, false);
        }

        private void ApplyWeatherEffect(WeatherType weather, bool instant)
        {
            switch (weather)
            {
                case WeatherType.Clear:
                    ApplyClearWeather(instant);
                    break;
                case WeatherType.Rain:
                    ApplyRainWeather(instant);
                    break;
                case WeatherType.Fog:
                    ApplyFogWeather(instant);
                    break;
                case WeatherType.Storm:
                    ApplyStormWeather(instant);
                    break;
                case WeatherType.Snow:
                    ApplySnowWeather(instant);
                    break;
            }
        }

        private void ApplyClearWeather(bool instant)
        {
            SetRainIntensity(0f);
            RenderSettings.fog = false;
            
            if (m_directionalLight != null)
            {
                m_directionalLight.intensity = 1f;
            }
            
            RenderSettings.skybox.SetColor("_Tint", m_clearSkyColor);
        }

        private void ApplyRainWeather(bool instant)
        {
            SetRainIntensity(m_rainIntensity);
            
            RenderSettings.fog = true;
            RenderSettings.fogDensity = m_fogDensity * 0.5f;
            RenderSettings.fogColor = m_rainSkyColor;

            if (m_directionalLight != null)
            {
                m_directionalLight.intensity = 0.7f;
            }
        }

        private void ApplyFogWeather(bool instant)
        {
            SetRainIntensity(0f);
            
            RenderSettings.fog = true;
            RenderSettings.fogDensity = m_fogDensity;
            RenderSettings.fogColor = m_fogColor;
        }

        private void ApplyStormWeather(bool instant)
        {
            SetRainIntensity(m_rainIntensity * 2f);
            
            RenderSettings.fog = true;
            RenderSettings.fogDensity = m_fogDensity * 2f;
            RenderSettings.fogColor = m_stormSkyColor;

            if (m_directionalLight != null)
            {
                m_directionalLight.intensity = 0.3f;
                m_directionalLight.color = Color.gray;
            }
        }

        private void ApplySnowWeather(bool instant)
        {
            SetRainIntensity(m_rainIntensity * 0.3f);
            
            RenderSettings.fog = true;
            RenderSettings.fogDensity = m_fogDensity * 0.7f;
            RenderSettings.fogColor = Color.white;
        }

        private void SetRainIntensity(float intensity)
        {
            if (m_rainParticles == null) return;

            var emission = m_rainParticles.emission;
            emission.rateOverTime = intensity;
        }

        public void NextWeather()
        {
            int nextIndex = ((int)m_currentWeather + 1) % System.Enum.GetValues(typeof(WeatherType)).Length;
            SetWeather((WeatherType)nextIndex);
        }

        public Vector3 GetWindDirection()
        {
            return m_windDirection.normalized * m_windStrength;
        }
    }
}
