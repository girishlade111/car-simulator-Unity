using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class TurboBoost : MonoBehaviour
    {
        [Header("Boost Settings")]
        [SerializeField] private bool m_enableBoost = true;
        [SerializeField] private KeyCode m_boostKey = KeyCode.LeftControl;
        [SerializeField] private float m_boostForce = 1500f;
        [SerializeField] private float m_boostDuration = 2f;
        [SerializeField] private float m_boostCooldown = 5f;

        [Header("Turbo Settings")]
        [SerializeField] private float m_turboChargeRate = 0.2f;
        [SerializeField] private float m_turboDrainRate = 0.5f;
        [SerializeField] private float m_turboThreshold = 0.8f;

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem m_boostParticles;
        [SerializeField] private Light m_boostLight;
        [SerializeField] private float m_lightIntensity = 2f;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;

        private bool m_isBoosting;
        private float m_boostTimer;
        private float m_cooldownTimer;
        private float m_turboMeter = 1f;

        public float TurboMeter => m_turboMeter;
        public bool IsBoosting => m_isBoosting;
        public bool CanBoost => m_cooldownTimer <= 0f && m_turboMeter > 0.3f;

        private void Start()
        {
            if (m_vehiclePhysics == null)
                m_vehiclePhysics = GetComponent<VehiclePhysics>();

            if (m_boostLight != null)
            {
                m_boostLight.intensity = 0f;
            }
        }

        private void Update()
        {
            if (!m_enableBoost) return;

            HandleBoostInput();
            UpdateBoost();
            UpdateTurboCharge();
            UpdateVisualEffects();
        }

        private void HandleBoostInput()
        {
            if (Input.GetKey(m_boostKey) && CanBoost && !m_isBoosting)
            {
                StartBoost();
            }
        }

        private void StartBoost()
        {
            m_isBoosting = true;
            m_boostTimer = m_boostDuration;
            
            if (m_boostParticles != null)
            {
                m_boostParticles.Play();
            }
        }

        private void StopBoost()
        {
            m_isBoosting = false;
            m_cooldownTimer = m_boostCooldown;

            if (m_boostParticles != null)
            {
                m_boostParticles.Stop();
            }
        }

        private void UpdateBoost()
        {
            if (!m_isBoosting) return;

            if (m_vehiclePhysics == null) return;

            m_boostTimer -= Time.deltaTime;

            float speedPercent = m_vehiclePhysics.CurrentSpeed / m_vehiclePhysics.MaxSpeed;
            float boostFactor = 1f - speedPercent;
            
            Vector3 boostDir = transform.forward;
            m_vehiclePhysics.GetComponent<Rigidbody>().AddForce(boostDir * m_boostForce * boostFactor, ForceMode.Acceleration);

            if (m_boostTimer <= 0f || speedPercent >= 0.95f)
            {
                StopBoost();
            }
        }

        private void UpdateTurboCharge()
        {
            if (m_vehiclePhysics == null) return;

            float speed = m_vehiclePhysics.CurrentSpeed;

            if (speed > 50f && !m_isBoosting)
            {
                m_turboMeter += m_turboChargeRate * Time.deltaTime;
            }
            else if (m_isBoosting)
            {
                m_turboMeter -= m_turboDrainRate * Time.deltaTime;
            }

            m_turboMeter = Mathf.Clamp01(m_turboMeter);

            if (m_cooldownTimer > 0f)
            {
                m_cooldownTimer -= Time.deltaTime;
            }
        }

        private void UpdateVisualEffects()
        {
            if (m_boostLight != null)
            {
                float targetIntensity = m_isBoosting ? m_lightIntensity : 0f;
                m_boostLight.intensity = Mathf.Lerp(m_boostLight.intensity, targetIntensity, Time.deltaTime * 10f);
            }
        }

        public float GetBoostMultiplier()
        {
            if (!m_isBoosting) return 1f;
            return 1f + (m_boostForce / 1000f);
        }

        public void AddTurboCharge(float amount)
        {
            m_turboMeter = Mathf.Clamp01(m_turboMeter + amount);
        }
    }
}
