using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class VehicleDamage : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private bool m_enableDamage = true;
        [SerializeField] private float m_maxHealth = 100f;
        [SerializeField] private float m_currentHealth = 100f;
        [SerializeField] private float m_minImpactSpeed = 5f;
        [SerializeField] private float m_damageMultiplier = 5f;

        [Header("Repair Settings")]
        [SerializeField] private float m_autoRepairRate = 2f;
        [SerializeField] private float m_repairThreshold = 30f;

        [Header("Visual Effects")]
        [SerializeField] private bool m_showDamageEffect = true;
        [SerializeField] private Color m_damagedColor = Color.gray;
        [SerializeField] private float m_effectIntensity = 0.5f;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;

        private bool m_isDamaged;
        private float m_lastCollisionTime;
        private Renderer[] m_vehicleRenderers;

        public float Health => m_currentHealth;
        public float MaxHealth => m_maxHealth;
        public float HealthPercent => m_currentHealth / m_maxHealth;
        public bool IsDamaged => m_currentHealth < m_maxHealth;

        private void Start()
        {
            if (m_vehiclePhysics == null)
                m_vehiclePhysics = GetComponent<VehiclePhysics>();

            m_vehicleRenderers = GetComponentsInChildren<Renderer>();
            m_currentHealth = m_maxHealth;
        }

        private void Update()
        {
            if (!m_enableDamage) return;

            AutoRepair();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!m_enableDamage) return;

            float impactSpeed = collision.relativeVelocity.magnitude * 3.6f;

            if (impactSpeed > m_minImpactSpeed)
            {
                float damage = (impactSpeed - m_minImpactSpeed) * m_damageMultiplier;
                ApplyDamage(damage);

                m_lastCollisionTime = Time.time;
                m_isDamaged = true;
            }
        }

        public void ApplyDamage(float amount)
        {
            m_currentHealth -= amount;
            m_currentHealth = Mathf.Max(0, m_currentHealth);

            if (m_showDamageEffect)
            {
                UpdateDamageVisuals();
            }

            if (m_currentHealth <= 0)
            {
                OnVehicleDestroyed();
            }
        }

        public void Repair(float amount)
        {
            m_currentHealth += amount;
            m_currentHealth = Mathf.Min(m_currentHealth, m_maxHealth);

            if (m_currentHealth >= m_maxHealth)
            {
                ResetDamageVisuals();
            }
        }

        public void RepairFull()
        {
            m_currentHealth = m_maxHealth;
            ResetDamageVisuals();
        }

        private void AutoRepair()
        {
            if (m_currentHealth >= m_maxHealth || m_currentHealth < m_repairThreshold) return;

            m_currentHealth += m_autoRepairRate * Time.deltaTime;
            m_currentHealth = Mathf.Min(m_currentHealth, m_maxHealth);

            if (m_currentHealth >= m_maxHealth)
            {
                ResetDamageVisuals();
            }
        }

        private void UpdateDamageVisuals()
        {
            if (m_vehicleRenderers == null) return;

            float damagePercent = 1f - HealthPercent;
            
            foreach (var renderer in m_vehicleRenderers)
            {
                if (renderer == null || renderer.material == null) continue;

                Color baseColor = renderer.material.color;
                renderer.material.color = Color.Lerp(baseColor, m_damagedColor, damagePercent * m_effectIntensity);
            }
        }

        private void ResetDamageVisuals()
        {
            if (m_vehicleRenderers == null) return;

            foreach (var renderer in m_vehicleRenderers)
            {
                if (renderer == null || renderer.material == null) continue;
                
                renderer.material.color = Color.white;
            }
        }

        private void OnVehicleDestroyed()
        {
            Debug.Log("[VehicleDamage] Vehicle destroyed!");
            
            if (m_vehiclePhysics != null)
            {
                m_vehiclePhysics.ResetVehicle();
                RepairFull();
            }
        }

        public void ResetDamage()
        {
            m_currentHealth = m_maxHealth;
            m_isDamaged = false;
            ResetDamageVisuals();
        }
    }
}
