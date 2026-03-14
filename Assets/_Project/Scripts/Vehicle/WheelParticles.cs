using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class WheelParticles : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WheelCollider m_wheelCollider;
        [SerializeField] private Transform m_wheelMesh;

        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem m_tireSmokeParticles;
        [SerializeField] private ParticleSystem m_sparkParticles;
        [SerializeField] private ParticleSystem m_burnoutParticles;

        [Header("Effects Settings")]
        [SerializeField] private float m_slipThreshold = 0.3f;
        [SerializeField] private float m_sparkThreshold = 0.8f;
        [SerializeField] private float m_burnoutThreshold = 0.9f;
        [SerializeField] private float m_effectIntensity = 1f;

        [Header("Particle Config")]
        [SerializeField] private int m_maxParticles = 50;
        [SerializeField] private float m_particleLifetime = 1f;
        [SerializeField] private float m_particleSize = 0.5f;

        private bool m_isEmitting;
        private float m_currentSlip;

        private void Start()
        {
            CreateParticleSystems();
        }

        private void FixedUpdate()
        {
            if (m_wheelCollider == null) return;

            CalculateSlip();
            UpdateParticleEmission();
        }

        private void CalculateSlip()
        {
            WheelHit hit;
            if (m_wheelCollider.GetGroundHit(out hit))
            {
                m_currentSlip = Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip);
            }
            else
            {
                m_currentSlip = 0f;
            }
        }

        private void UpdateParticleEmission()
        {
            float slipFactor = m_currentSlip / 2f;
            slipFactor = Mathf.Clamp01(slipFactor * m_effectIntensity);

            if (m_tireSmokeParticles != null)
            {
                var emission = m_tireSmokeParticles.emission;
                emission.rateOverTime = slipFactor * m_maxParticles * 0.5f;
            }

            if (m_sparkParticles != null)
            {
                var emission = m_sparkParticles.emission;
                bool shouldSpark = m_currentSlip > m_sparkThreshold;
                emission.rateOverTime = shouldSpark ? m_maxParticles * 0.3f : 0f;
            }

            if (m_burnoutParticles != null)
            {
                var emission = m_burnoutParticles.emission;
                bool shouldBurnout = m_currentSlip > m_burnoutThreshold && Mathf.Abs(GetThrottleInput()) > 0.5f;
                emission.rateOverTime = shouldBurnout ? m_maxParticles : 0f;
            }
        }

        private float GetThrottleInput()
        {
            var input = GetComponent<VehicleInput>();
            return input != null ? input.ThrottleInput : 0f;
        }

        private void CreateParticleSystems()
        {
            if (m_tireSmokeParticles == null)
            {
                m_tireSmokeParticles = CreateParticleSystem("TireSmoke", Color.gray, 0.3f);
            }

            if (m_sparkParticles == null)
            {
                m_sparkParticles = CreateParticleSystem("Sparks", Color.yellow, 0.1f);
                if (m_sparkParticles != null)
                {
                    var main = m_sparkParticles.main;
                    main.startSpeed = 5f;
                    main.gravityModifier = 1f;
                }
            }

            if (m_burnoutParticles == null)
            {
                m_burnoutParticles = CreateParticleSystem("Burnout", new Color(0.2f, 0.2f, 0.2f), 0.5f);
                if (m_burnoutParticles != null)
                {
                    var main = m_burnoutParticles.main;
                    main.startSpeed = 2f;
                }
            }
        }

        private ParticleSystem CreateParticleSystem(string name, Color color, float alpha)
        {
            GameObject particleObj = new GameObject(name);
            particleObj.transform.SetParent(transform);
            particleObj.transform.localPosition = Vector3.zero;

            ParticleSystem particles = particleObj.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.loop = true;
            main.startLifetime = m_particleLifetime;
            main.startSpeed = 3f;
            main.startSize = m_particleSize;
            main.maxParticles = m_maxParticles;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = particles.emission;
            emission.rateOverTime = 0;

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 0.2f;

            var renderer = particleObj.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.material.color = new Color(color.r, color.g, color.b, alpha);
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            return particles;
        }

        public void SetIntensity(float intensity)
        {
            m_effectIntensity = Mathf.Clamp01(intensity);
        }
    }
}
