using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class WindshieldEffects : MonoBehaviour
    {
        [Header("Rain Effect")]
        [SerializeField] private bool m_enableRain = true;
        [SerializeField] private ParticleSystem m_rainParticles;
        [SerializeField] private float m_rainIntensity = 50f;

        [Header("Dirt Effect")]
        [SerializeField] private bool m_enableDirt = true;
        [SerializeField] private Material m_dirtMaterial;
        [SerializeField] private float m_dirtAmount;
        [SerializeField] private float m_dirtBuildupRate = 0.1f;
        [SerializeField] private float m_dirtClearRate = 0.2f;

        [Header("Fog Effect")]
        [SerializeField] private bool m_enableFog = true;
        [SerializeField] private Material m_fogMaterial;
        [SerializeField] private float m_fogAmount;
        [SerializeField] private float m_fogClearRate = 0.3f;

        [Header("References")]
        [SerializeField] private Transform m_windshield;
        [SerializeField] private WeatherSystem m_weatherSystem;

        private Renderer m_windshieldRenderer;
        private bool m_isRaining;

        private void Start()
        {
            SetupWindshield();
            FindWeatherSystem();
            CreateEffects();
        }

        private void SetupWindshield()
        {
            if (m_windshield == null)
            {
                GameObject ws = new GameObject("Windshield");
                ws.transform.SetParent(transform);
                ws.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
                ws.transform.localRotation = Quaternion.Euler(30, 0, 0);
                
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.SetParent(ws.transform);
                quad.transform.localPosition = Vector3.zero;
                quad.transform.localScale = new Vector3(2f, 1f, 1f);
                quad.transform.localRotation = Quaternion.Euler(180, 0, 0);
                
                m_windshield = ws.transform;
            }

            m_windshieldRenderer = m_windshield.GetComponentInChildren<Renderer>();
        }

        private void FindWeatherSystem()
        {
            if (m_weatherSystem == null)
            {
                m_weatherSystem = FindObjectOfType<WeatherSystem>();
            }
        }

        private void CreateEffects()
        {
            if (m_rainParticles == null && m_enableRain)
            {
                GameObject rainObj = new GameObject("WindshieldRain");
                rainObj.transform.SetParent(transform);
                rainObj.transform.localPosition = new Vector3(0, 2f, 1f);

                m_rainParticles = rainObj.AddComponent<ParticleSystem>();
                
                var main = m_rainParticles.main;
                main.startLifetime = 0.5f;
                main.startSpeed = 10f;
                main.startSize = 0.05f;
                main.maxParticles = 200;

                var emission = m_rainParticles.emission;
                emission.rateOverTime = 0;

                var shape = m_rainParticles.shape;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(2f, 0.5f, 0.1f);

                var renderer = rainObj.GetComponent<ParticleSystemRenderer>();
                renderer.renderMode = ParticleSystemRenderMode.Stretch;
            }

            if (m_enableDirt && m_dirtMaterial == null)
            {
                m_dirtMaterial = new Material(Shader.Find("Standard"));
                m_dirtMaterial.color = new Color(0.4f, 0.35f, 0.3f, 0f);
            }

            if (m_enableFog && m_fogMaterial == null)
            {
                m_fogMaterial = new Material(Shader.Find("Standard"));
                m_fogMaterial.color = new Color(0.8f, 0.9f, 1f, 0f);
            }
        }

        private void Update()
        {
            UpdateWeatherEffects();
            UpdateEffects();
        }

        private void UpdateWeatherEffects()
        {
            if (m_weatherSystem == null) return;

            bool wasRaining = m_isRaining;
            m_isRaining = m_weatherSystem.CurrentWeather == World.WeatherType.Rain ||
                          m_weatherSystem.CurrentWeather == World.WeatherType.Storm;

            if (m_isRaining != wasRaining)
            {
                UpdateRainParticles();
            }
        }

        private void UpdateRainParticles()
        {
            if (m_rainParticles == null) return;

            var emission = m_rainParticles.emission;
            emission.rateOverTime = m_isRaining ? m_rainIntensity : 0f;
        }

        private void UpdateEffects()
        {
            if (m_isRaining && m_enableDirt)
            {
                m_dirtAmount += m_dirtBuildupRate * Time.deltaTime;
            }
            else if (m_enableDirt)
            {
                m_dirtAmount -= m_dirtClearRate * Time.deltaTime;
            }

            if (m_enableFog)
            {
                float speed = 0f;
                var physics = GetComponent<VehiclePhysics>();
                if (physics != null)
                {
                    speed = physics.CurrentSpeed;
                }

                if (speed > 50f)
                {
                    m_fogAmount += 0.05f * Time.deltaTime;
                }
                else
                {
                    m_fogAmount -= m_fogClearRate * Time.deltaTime;
                }
            }

            m_dirtAmount = Mathf.Clamp01(m_dirtAmount);
            m_fogAmount = Mathf.Clamp01(m_fogAmount);

            UpdateMaterials();
        }

        private void UpdateMaterials()
        {
            if (m_dirtMaterial != null && m_windshieldRenderer != null)
            {
                m_dirtMaterial.color = new Color(0.4f, 0.35f, 0.3f, m_dirtAmount * 0.5f);
            }

            if (m_fogMaterial != null && m_windshieldRenderer != null)
            {
                m_fogMaterial.color = new Color(0.8f, 0.9f, 1f, m_fogAmount * 0.3f);
            }
        }

        public void CleanWindshield()
        {
            m_dirtAmount = 0f;
            m_fogAmount = 0f;
            UpdateMaterials();
        }

        public void SetRainIntensity(float intensity)
        {
            m_rainIntensity = intensity;
            UpdateRainParticles();
        }

        public float GetDirtAmount() => m_dirtAmount;
        public float GetFogAmount() => m_fogAmount;
    }
}
