using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class VehicleLights : MonoBehaviour
    {
        [Header("Headlights")]
        [SerializeField] private Light[] m_headlights;
        [SerializeField] private bool m_headlightsOn = true;
        [SerializeField] private Color m_headlightColor = Color.white;
        [SerializeField] private float m_headlightIntensity = 2f;
        [SerializeField] private float m_headlightRange = 50f;

        [Header("Taillights")]
        [SerializeField] private Light[] m_taillights;
        [SerializeField] private bool m_taillightsOn = true;
        [SerializeField] private Color m_taillightColor = Color.red;
        [SerializeField] private float m_taillightIntensity = 1f;
        [SerializeField] private float m_taillightRange = 10f;

        [Header("Brake Lights")]
        [SerializeField] private Light[] m_brakeLights;
        [SerializeField] private bool m_brakeLightsOn;
        [SerializeField] private float m_brakeIntensity = 2f;

        [Header("References")]
        [SerializeField] private VehicleInput m_vehicleInput;

        private void Start()
        {
            SetupLights();
            
            if (m_vehicleInput == null)
                m_vehicleInput = GetComponent<VehicleInput>();
        }

        private void SetupLights()
        {
            if (m_headlights == null || m_headlights.Length == 0)
            {
                m_headlights = CreateHeadlights();
            }

            if (m_taillights == null || m_taillights.Length == 0)
            {
                m_taillights = CreateTaillights();
            }

            if (m_brakeLights == null || m_brakeLights.Length == 0)
            {
                m_brakeLights = CreateBrakeLights();
            }

            UpdateHeadlights();
            UpdateTaillights();
        }

        private Light[] CreateHeadlights()
        {
            Light[] lights = new Light[2];
            
            for (int i = 0; i < 2; i++)
            {
                GameObject lightObj = new GameObject($"Headlight_{i}");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = new Vector3(i == 0 ? -0.6f : 0.6f, 0.5f, 1.8f);
                
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Spot;
                light.color = m_headlightColor;
                light.intensity = 0;
                light.range = m_headlightRange;
                light.spotAngle = 30f;
                light.innerSpotAngle = 20f;
                
                lights[i] = light;
            }

            return lights;
        }

        private Light[] CreateTaillights()
        {
            Light[] lights = new Light[2];
            
            for (int i = 0; i < 2; i++)
            {
                GameObject lightObj = new GameObject($"Taillight_{i}");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = new Vector3(i == 0 ? -0.6f : 0.6f, 0.5f, -1.9f);
                
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = m_taillightColor;
                light.intensity = 0;
                light.range = m_taillightRange;
                
                lights[i] = light;
            }

            return lights;
        }

        private Light[] CreateBrakeLights()
        {
            Light[] lights = new Light[2];
            
            for (int i = 0; i < 2; i++)
            {
                GameObject lightObj = new GameObject($"BrakeLight_{i}");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = new Vector3(i == 0 ? -0.6f : 0.6f, 0.6f, -1.9f);
                
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = Color.red;
                light.intensity = 0;
                light.range = 8f;
                
                lights[i] = light;
            }

            return lights;
        }

        private void Update()
        {
            if (m_vehicleInput != null)
            {
                SetBrakeLights(m_vehicleInput.IsBraking);
            }
        }

        public void SetHeadlights(bool on)
        {
            m_headlightsOn = on;
            UpdateHeadlights();
        }

        public void ToggleHeadlights()
        {
            SetHeadlights(!m_headlightsOn);
        }

        public void SetTaillights(bool on)
        {
            m_taillightsOn = on;
            UpdateTaillights();
        }

        public void SetBrakeLights(bool on)
        {
            m_brakeLightsOn = on;
            UpdateBrakeLights();
        }

        private void UpdateHeadlights()
        {
            if (m_headlights == null) return;

            float targetIntensity = m_headlightsOn ? m_headlightIntensity : 0f;
            
            foreach (var light in m_headlights)
            {
                if (light != null)
                {
                    light.enabled = m_headlightsOn;
                    light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * 5f);
                }
            }
        }

        private void UpdateTaillights()
        {
            if (m_taillights == null) return;

            float targetIntensity = m_taillightsOn ? m_taillightIntensity : 0f;
            
            foreach (var light in m_taillights)
            {
                if (light != null)
                {
                    light.enabled = m_taillightsOn;
                    light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * 5f);
                }
            }
        }

        private void UpdateBrakeLights()
        {
            if (m_brakeLights == null) return;

            float targetIntensity = m_brakeLightsOn ? m_brakeIntensity : 0f;
            
            foreach (var light in m_brakeLights)
            {
                if (light != null)
                {
                    light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * 10f);
                }
            }
        }

        public void SetHeadlightColor(Color color)
        {
            m_headlightColor = color;
            foreach (var light in m_headlights)
            {
                if (light != null) light.color = color;
            }
        }

        public void SetHeadlightIntensity(float intensity)
        {
            m_headlightIntensity = intensity;
            if (m_headlightsOn) UpdateHeadlights();
        }

        public bool AreHeadlightsOn => m_headlightsOn;
        public bool AreTaillightsOn => m_taillightsOn;
    }
}
