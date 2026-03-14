using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class NitrousOxide : MonoBehaviour
    {
        [Header("Nitrous Settings")]
        [SerializeField] private bool m_enableNitrous = true;
        [SerializeField] private KeyCode m_nitrousKey = KeyCode.N;
        [SerializeField] private float m_nitrousForce = 2500f;
        [SerializeField] private float m_nitrousDuration = 1.5f;
        [SerializeField] private float m_nitrousCooldown = 8f;

        [Header("N2O Capacity")]
        [SerializeField] private float m_maxNitrous = 100f;
        [SerializeField] private float m_nitrousRechargeRate = 2f;
        [SerializeField] private float m_nitrousUseRate = 30f;

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem m_nitrousParticles;
        [SerializeField] private ParticleSystem m_flameParticles;
        [SerializeField] private Light m_nitrousLight;
        [SerializeField] private Color m_flameColor = new Color(0f, 0.8f, 1f);
        [SerializeField] private float m_lightIntensity = 3f;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;

        private bool m_isActive;
        private float m_nitrousTimer;
        private float m_cooldownTimer;
        private float m_currentNitrous;

        public float CurrentNitrous => m_currentNitrous;
        public float MaxNitrous => m_maxNitrous;
        public bool IsActive => m_isActive;
        public bool CanUse => m_cooldownTimer <= 0f && m_currentNitrous > 10f;

        private void Start()
        {
            if (m_vehiclePhysics == null)
                m_vehiclePhysics = GetComponent<VehiclePhysics>();

            m_currentNitrous = m_maxNitrous;
            CreateVisualEffects();
        }

        private void CreateVisualEffects()
        {
            if (m_nitrousParticles == null)
            {
                m_nitrousParticles = CreateParticleSystem("NitrousTrail", Color.cyan, 0.6f);
                if (m_nitrousParticles != null)
                {
                    var main = m_nitrousParticles.main;
                    main.startSize = 0.3f;
                    main.startSpeed = 5f;
                }
            }

            if (m_flameParticles == null)
            {
                m_flameParticles = CreateParticleSystem("NitrousFlame", m_flameColor, 0.8f);
                if (m_flameParticles != null)
                {
                    var main = m_flameParticles.main;
                    main.startSize = 0.5f;
                    main.startSpeed = 10f;
                    main.startLifetime = 0.2f;
                }
            }

            if (m_nitrousLight == null)
            {
                GameObject lightObj = new GameObject("NitrousLight");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = new Vector3(0, 0.5f, -2.5f);
                
                m_nitrousLight = lightObj.AddComponent<Light>();
                m_nitrousLight.color = m_flameColor;
                m_nitrousLight.range = 8f;
                m_nitrousLight.intensity = 0f;
            }
        }

        private ParticleSystem CreateParticleSystem(string name, Color color, float alpha)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;

            ParticleSystem particles = obj.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.loop = true;
            main.startLifetime = 0.5f;
            main.startSpeed = 3f;
            main.maxParticles = 100;

            var emission = particles.emission;
            emission.rateOverTime = 0;

            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.material.color = new Color(color.r, color.g, color.b, alpha);
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            return particles;
        }

        private void Update()
        {
            if (!m_enableNitrous) return;

            HandleInput();
            UpdateNitrous();
            UpdateVisuals();
        }

        private void HandleInput()
        {
            if (Input.GetKey(m_nitrousKey) && CanUse && !m_isActive)
            {
                ActivateNitrous();
            }
        }

        private void ActivateNitrous()
        {
            m_isActive = true;
            m_nitrousTimer = m_nitrousDuration;
            
            if (m_nitrousParticles != null)
                m_nitrousParticles.Play();
            if (m_flameParticles != null)
                m_flameParticles.Play();
        }

        private void DeactivateNitrous()
        {
            m_isActive = false;
            m_cooldownTimer = m_nitrousCooldown;

            if (m_nitrousParticles != null)
                m_nitrousParticles.Stop();
            if (m_flameParticles != null)
                m_flameParticles.Stop();
        }

        private void UpdateNitrous()
        {
            if (m_isActive)
            {
                m_currentNitrous -= m_nitrousUseRate * Time.deltaTime;
                m_nitrousTimer -= Time.deltaTime;

                if (m_vehiclePhysics != null)
                {
                    Vector3 boostDir = transform.forward;
                    m_vehiclePhysics.GetComponent<Rigidbody>().AddForce(boostDir * m_nitrousForce, ForceMode.Acceleration);
                }

                if (m_nitrousTimer <= 0f || m_currentNitrous <= 0f)
                {
                    DeactivateNitrous();
                }
            }
            else
            {
                if (m_cooldownTimer > 0f)
                {
                    m_cooldownTimer -= Time.deltaTime;
                }
                else
                {
                    m_currentNitrous += m_nitrousRechargeRate * Time.deltaTime;
                    m_currentNitrous = Mathf.Clamp(m_currentNitrous, 0f, m_maxNitrous);
                }
            }
        }

        private void UpdateVisuals()
        {
            if (m_nitrousLight != null)
            {
                float targetIntensity = m_isActive ? m_lightIntensity : 0f;
                m_nitrousLight.intensity = Mathf.Lerp(m_nitrousLight.intensity, targetIntensity, Time.deltaTime * 10f);
            }

            if (m_nitrousParticles != null)
            {
                var emission = m_nitrousParticles.emission;
                emission.rateOverTime = m_isActive ? 50f : 0f;
            }

            if (m_flameParticles != null)
            {
                var emission = m_flameParticles.emission;
                emission.rateOverTime = m_isActive ? 30f : 0f;
            }
        }

        public void AddNitrous(float amount)
        {
            m_currentNitrous = Mathf.Clamp(m_currentNitrous + amount, 0f, m_maxNitrous);
        }
    }
}
