using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class NeonUnderglow : MonoBehaviour
    {
        [Header("Neon Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private Color m_neonColor = Color.cyan;
        [SerializeField] private float m_glowIntensity = 2f;
        [SerializeField] private float m_glowRadius = 3f;

        [Header("Pulse Effect")]
        [SerializeField] private bool m_pulseEnabled = true;
        [SerializeField] private float m_pulseSpeed = 2f;
        [SerializeField] private float m_pulseIntensity = 0.5f;

        [Header("Lights")]
        [SerializeField] private Light[] m_neonLights;
        [SerializeField] private GameObject[] m_glowMeshes;

        private void Start()
        {
            if (m_enabled)
            {
                CreateNeonLights();
            }
        }

        private void Update()
        {
            if (!m_enabled || m_neonLights == null) return;

            if (m_pulseEnabled)
            {
                float pulse = Mathf.PingPong(Time.time * m_pulseSpeed, m_pulseIntensity);
                float intensity = m_glowIntensity + pulse;

                foreach (var light in m_neonLights)
                {
                    if (light != null)
                    {
                        light.intensity = intensity;
                    }
                }
            }
        }

        private void CreateNeonLights()
        {
            m_neonLights = new Light[4];
            m_glowMeshes = new GameObject[4];

            Vector3[] positions = {
                new Vector3(-0.8f, -0.3f, 1.5f),
                new Vector3(0.8f, -0.3f, 1.5f),
                new Vector3(-0.8f, -0.3f, -1.5f),
                new Vector3(0.8f, -0.3f, -1.5f)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject lightObj = new GameObject($"NeonLight_{i}");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = positions[i];

                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = m_neonColor;
                light.intensity = m_glowIntensity;
                light.range = m_glowRadius;
                light.shadows = LightShadows.None;

                m_neonLights[i] = light;

                GameObject glowMesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
                glowMesh.name = $"NeonGlow_{i}";
                glowMesh.transform.SetParent(lightObj.transform);
                glowMesh.transform.localPosition = Vector3.zero;
                glowMesh.transform.localRotation = Quaternion.Euler(90, 0, 0);
                glowMesh.transform.localScale = new Vector3(0.5f, 1.5f, 1f);

                Renderer renderer = glowMesh.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
                renderer.material.color = new Color(m_neonColor.r, m_neonColor.g, m_neonColor.b, 0.8f);
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", m_neonColor * m_glowIntensity);

                m_glowMeshes[i] = glowMesh;
            }
        }

        public void SetNeonColor(Color color)
        {
            m_neonColor = color;

            foreach (var light in m_neonLights)
            {
                if (light != null)
                    light.color = color;
            }

            foreach (var mesh in m_glowMeshes)
            {
                if (mesh != null)
                {
                    Renderer renderer = mesh.GetComponent<Renderer>();
                    renderer.material.color = new Color(color.r, color.g, color.b, 0.8f);
                    renderer.material.SetColor("_EmissionColor", color * m_glowIntensity);
                }
            }
        }

        public void SetNeonEnabled(bool enabled)
        {
            m_enabled = enabled;

            foreach (var light in m_neonLights)
            {
                if (light != null)
                    light.enabled = enabled;
            }

            foreach (var mesh in m_glowMeshes)
            {
                if (mesh != null)
                    mesh.SetActive(enabled);
            }
        }

        public void ToggleNeon()
        {
            SetNeonEnabled(!m_enabled);
        }

        public void CycleColor()
        {
            float hue = Random.value;
            Color newColor = Color.HSVToRGB(hue, 1f, 1f);
            SetNeonColor(newColor);
        }
    }
}
