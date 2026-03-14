using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class EnhancedTimeOfDay : MonoBehaviour
    {
        public static EnhancedTimeOfDay Instance { get; private set; }

        [Header("Time Settings")]
        [SerializeField] private float m_dayDuration = 600f;
        [SerializeField] private float m_startTime = 8f;
        [SerializeField] private bool m_paused = false;
        [SerializeField] private float m_timeScale = 1f;

        [Header("Time of Day")]
        [Range(0f, 24f)] public float m_sunriseTime = 6f;
        [Range(0f, 24f)] public float m_sunsetTime = 18f;
        [Range(0f, 24f)] public float m_noonTime = 12f;
        [Range(0f, 24f)] public float m_midnightTime = 0f;

        [Header("Sun Settings")]
        [SerializeField] private Light m_sunLight;
        [SerializeField] private float m_sunIntensity = 1.5f;
        [SerializeField] private float m_sunNoonIntensity = 1.5f;
        [SerializeField] private float m_sunSunsetIntensity = 0.8f;

        [Header("Moon Settings")]
        [SerializeField] private Light m_moonLight;
        [SerializeField] private float m_moonIntensity = 0.3f;

        [Header("Sky")]
        [SerializeField] private Material m_skyMaterial;
        [SerializeField] private Color m_daySkyColor = new Color(0.5f, 0.7f, 1f);
        [SerializeField] private Color m_nightSkyColor = new Color(0.02f, 0.02f, 0.08f);
        [SerializeField] private Color m_sunriseSkyColor = new Color(1f, 0.6f, 0.4f);
        [SerializeField] private Color m_sunsetSkyColor = new Color(0.9f, 0.4f, 0.3f);

        [Header("Ambient")]
        [SerializeField] private Color m_dayAmbient = new Color(0.5f, 0.5f, 0.5f);
        [SerializeField] private Color m_nightAmbient = new Color(0.05f, 0.05f, 0.1f);
        [SerializeField] private Color m_sunriseAmbient = new Color(0.6f, 0.4f, 0.3f);
        [SerializeField] private Color m_sunsetAmbient = new Color(0.5f, 0.3f, 0.25f);

        [Header("Street Lights")]
        [SerializeField] private bool m_autoToggleLights = true;
        [SerializeField] private List<GameObject> m_streetLights = new List<GameObject>();
        [SerializeField] private float m_lightsOnThreshold = 18f;
        [SerializeField] private float m_lightsOffThreshold = 6f;

        [Header("Vehicle Lights")]
        [SerializeField] private bool m_autoVehicleLights = true;
        [SerializeField] private float m_vehicleLightsOnSpeed = 50f;

        [Header("Fog")]
        [SerializeField] private bool m_enableFog = true;
        [SerializeField] private Color m_dayFogColor = new Color(0.7f, 0.8f, 0.9f);
        [SerializeField] private Color m_nightFogColor = new Color(0.1f, 0.1f, 0.15f);
        [SerializeField] private float m_dayFogDensity = 0.005f;
        [SerializeField] private float m_nightFogDensity = 0.01f;

        [Header("Exposure")]
        [SerializeField] private bool m_autoExposure = true;
        [SerializeField] private float m_dayExposure = 1f;
        [SerializeField] private float m_nightExposure = 0.3f;

        [Header("Stars")]
        [SerializeField] private bool m_showStars = true;
        [SerializeField] private GameObject m_starField;
        [SerializeField] private float m_starVisibilityThreshold = 19f;

        public float CurrentTime { get; private set; }
        public float DayProgress => CurrentTime / 24f;
        public float SunProgress => Mathf.InverseLerp(m_sunriseTime, m_sunsetTime, CurrentTime);
        public bool IsDay => CurrentTime > m_sunriseTime && CurrentTime < m_sunsetTime;
        public bool IsNight => !IsDay;
        public bool IsSunrise => CurrentTime >= m_sunriseTime - 1f && CurrentTime < m_sunriseTime + 1f;
        public bool IsSunset => CurrentTime >= m_sunsetTime - 1f && CurrentTime < m_sunsetTime + 1f;

        public event System.Action OnSunrise;
        public event System.Action OnSunset;
        public event System.Action OnNightStart;
        public event System.Action OnDayStart;

        private bool m_wasDay;
        private float m_currentExposure;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CurrentTime = m_startTime;
            m_wasDay = IsDay;

            SetupLights();
            SetupSky();
            SetupFog();
            SetupStars();
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
                m_sunLight.shadows = LightShadows.Soft;
            }

            if (m_moonLight == null)
            {
                GameObject moon = new GameObject("MoonLight");
                moon.transform.SetParent(transform);
                m_moonLight = moon.AddComponent<Light>();
                m_moonLight.type = LightType.Directional;
                m_moonLight.intensity = 0;
                m_moonLight.shadows = LightShadows.None;
            }
        }

        private void SetupSky()
        {
            if (m_skyMaterial == null && RenderSettings.skybox != null)
            {
                m_skyMaterial = RenderSettings.skybox;
            }
        }

        private void SetupFog()
        {
            if (m_enableFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.ExponentialSquared;
                RenderSettings.fogDensity = m_dayFogDensity;
            }
        }

        private void SetupStars()
        {
            if (m_showStars && m_starField == null)
            {
                CreateStarField();
            }
        }

        private void CreateStarField()
        {
            m_starField = new GameObject("StarField");
            m_starField.transform.SetParent(transform);
            m_starField.transform.position = Vector3.up * 500f;

            for (int i = 0; i < 500; i++)
            {
                GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                star.transform.SetParent(m_starField.transform);
                star.transform.localPosition = Random.onUnitSphere * 400f;
                star.transform.localScale = Vector3.one * Random.Range(0.5f, 2f);

                Renderer renderer = star.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Unlit/Transparent"));
                renderer.material.color = new Color(1f, 1f, 1f, Random.Range(0.3f, 1f));

                GameObject.Destroy(star.GetComponent<Collider>());
            }

            m_starField.SetActive(false);
        }

        private void Update()
        {
            if (!m_paused)
            {
                CurrentTime += (Time.deltaTime / m_dayDuration) * 24f * m_timeScale;
                if (CurrentTime >= 24f) CurrentTime -= 24f;
            }

            UpdateSun();
            UpdateSky();
            UpdateAmbient();
            UpdateFog();
            UpdateStars();
            UpdateLights();
            UpdateVehicleLights();

            CheckTimeTransitions();
        }

        private void UpdateSun()
        {
            if (m_sunLight == null) return;

            float sunAngle = (CurrentTime - 6f) * 15f;
            m_sunLight.transform.rotation = Quaternion.Euler(sunAngle, 30f, 0f);

            float intensityFactor = 1f;
            if (CurrentTime < m_sunriseTime + 1f)
            {
                intensityFactor = Mathf.InverseLerp(m_sunriseTime - 1f, m_sunriseTime + 1f, CurrentTime);
            }
            else if (CurrentTime > m_sunsetTime - 1f)
            {
                intensityFactor = Mathf.InverseLerp(m_sunsetTime + 1f, m_sunsetTime - 1f, CurrentTime);
            }

            if (CurrentTime > m_sunriseTime && CurrentTime < m_sunsetTime)
            {
                m_sunLight.intensity = Mathf.Lerp(m_sunSunsetIntensity, m_sunNoonIntensity, SunProgress) * intensityFactor;
                m_moonLight.intensity = 0;
            }
            else
            {
                m_sunLight.intensity = 0;
                m_moonLight.intensity = m_moonIntensity;
            }
        }

        private void UpdateSky()
        {
            if (m_skyMaterial == null) return;

            Color skyColor = GetSkyColor();
            m_skyMaterial.SetColor("_SkyTint", skyColor);
            m_skyMaterial.SetColor("_GroundColor", skyColor * 0.5f);
        }

        private Color GetSkyColor()
        {
            if (IsSunrise)
            {
                float t = Mathf.InverseLerp(m_sunriseTime - 1f, m_sunriseTime + 1f, CurrentTime);
                return Color.Lerp(m_nightSkyColor, m_sunriseSkyColor, t);
            }
            else if (CurrentTime > m_sunriseTime && CurrentTime < m_noonTime)
            {
                float t = (CurrentTime - m_sunriseTime) / (m_noonTime - m_sunriseTime);
                return Color.Lerp(m_sunriseSkyColor, m_daySkyColor, t);
            }
            else if (CurrentTime >= m_noonTime && CurrentTime < m_sunsetTime)
            {
                float t = (CurrentTime - m_noonTime) / (m_sunsetTime - m_noonTime);
                return Color.Lerp(m_daySkyColor, m_sunsetSkyColor, t);
            }
            else if (IsSunset)
            {
                float t = Mathf.InverseLerp(m_sunsetTime - 1f, m_sunsetTime + 1f, CurrentTime);
                return Color.Lerp(m_sunsetSkyColor, m_nightSkyColor, t);
            }

            return m_nightSkyColor;
        }

        private void UpdateAmbient()
        {
            Color ambientColor = GetAmbientColor();
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambientColor;
        }

        private Color GetAmbientColor()
        {
            if (IsSunrise)
            {
                float t = Mathf.InverseLerp(m_sunriseTime - 1f, m_sunriseTime + 1f, CurrentTime);
                return Color.Lerp(m_nightAmbient, m_sunriseAmbient, t);
            }
            else if (CurrentTime > m_sunriseTime && CurrentTime < m_noonTime)
            {
                float t = (CurrentTime - m_sunriseTime) / (m_noonTime - m_sunriseTime);
                return Color.Lerp(m_sunriseAmbient, m_dayAmbient, t);
            }
            else if (CurrentTime >= m_noonTime && CurrentTime < m_sunsetTime)
            {
                float t = (CurrentTime - m_noonTime) / (m_sunsetTime - m_noonTime);
                return Color.Lerp(m_dayAmbient, m_sunsetAmbient, t);
            }
            else if (IsSunset)
            {
                float t = Mathf.InverseLerp(m_sunsetTime - 1f, m_sunsetTime + 1f, CurrentTime);
                return Color.Lerp(m_sunsetAmbient, m_nightAmbient, t);
            }

            return m_nightAmbient;
        }

        private void UpdateFog()
        {
            if (!m_enableFog) return;

            float fogFactor = IsDay ? 0f : 1f;
            if (IsSunrise) fogFactor = Mathf.InverseLerp(m_sunriseTime - 1f, m_sunriseTime + 1f, CurrentTime);
            if (IsSunset) fogFactor = Mathf.InverseLerp(m_sunsetTime + 1f, m_sunsetTime - 1f, CurrentTime);

            RenderSettings.fogColor = Color.Lerp(m_dayFogColor, m_nightFogColor, fogFactor);
            RenderSettings.fogDensity = Mathf.Lerp(m_dayFogDensity, m_nightFogDensity, fogFactor);
        }

        private void UpdateStars()
        {
            if (!m_showStars || m_starField == null) return;

            bool showStars = CurrentTime >= m_starVisibilityThreshold || CurrentTime < m_sunriseTime;
            m_starField.SetActive(showStars);
        }

        private void UpdateLights()
        {
            if (!m_autoToggleLights) return;

            bool shouldBeOn = CurrentTime >= m_lightsOnThreshold || CurrentTime < m_lightsOffThreshold;

            foreach (var light in m_streetLights)
            {
                if (light != null)
                {
                    var lights = light.GetComponentsInChildren<Light>();
                    foreach (var l in lights)
                    {
                        if (l.type == LightType.Point)
                        {
                            l.enabled = shouldBeOn;
                        }
                    }
                }
            }
        }

        private void UpdateVehicleLights()
        {
            if (!m_autoVehicleLights) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            var physics = player.GetComponent<Vehicle.VehiclePhysics>();
            if (physics == null) return;

            bool shouldBeOn = physics.CurrentSpeed < m_vehicleLightsOnSpeed || IsNight;
            // TODO: Enable vehicle headlights
        }

        private void CheckTimeTransitions()
        {
            if (m_wasDay != IsDay)
            {
                if (IsDay)
                {
                    OnDayStart?.Invoke();
                }
                else
                {
                    OnNightStart?.Invoke();
                }
                m_wasDay = IsDay;
            }

            if (IsSunrise)
            {
                OnSunrise?.Invoke();
            }
            else if (IsSunset)
            {
                OnSunset?.Invoke();
            }
        }

        public void AddStreetLight(GameObject light)
        {
            if (!m_streetLights.Contains(light))
            {
                m_streetLights.Add(light);
            }
        }

        public void RemoveStreetLight(GameObject light)
        {
            m_streetLights.Remove(light);
        }

        public void SetTime(float time)
        {
            CurrentTime = Mathf.Clamp(time, 0f, 24f);
        }

        public void SetPaused(bool paused)
        {
            m_paused = paused;
        }

        public void SetTimeScale(float scale)
        {
            m_timeScale = Mathf.Clamp(scale, 0f, 10f);
        }

        public string GetTimeString()
        {
            int hours = Mathf.FloorToInt(CurrentTime);
            int minutes = Mathf.FloorToInt((CurrentTime - hours) * 60);
            return $"{hours:00}:{minutes:00}";
        }

        public float GetSunAngle() => (CurrentTime - 6f) * 15f;

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
