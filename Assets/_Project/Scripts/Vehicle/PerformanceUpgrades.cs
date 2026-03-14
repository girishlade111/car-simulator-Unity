using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public enum UpgradeType
    {
        Engine,
        Transmission,
        Brakes,
        Suspension,
        Turbo,
        WeightReduction
    }

    [System.Serializable]
    public class PerformanceUpgrade
    {
        public string name;
        public UpgradeType type;
        public int level;
        public int maxLevel;
        
        [Header("Effects")]
        public float engineBonus;
        public float speedBonus;
        public float brakeBonus;
        public float handlingBonus;
        public float weightReduction;
        
        [Header("Cost")]
        public int creditCost;
    }

    public class PerformanceUpgrades : MonoBehaviour
    {
        [Header("Upgrade Levels")]
        [SerializeField] private int m_engineLevel;
        [SerializeField] private int m_transmissionLevel;
        [SerializeField] private int m_brakesLevel;
        [SerializeField] private int m_suspensionLevel;
        [SerializeField] private int m_turboLevel;
        [SerializeField] private int m_weightLevel;

        [Header("Settings")]
        [SerializeField] private int m_maxLevel = 5;
        [SerializeField] private float m_levelMultiplier = 1.2f;

        private VehicleTuning m_baseTuning;
        private VehiclePhysics m_vehiclePhysics;

        private void Start()
        {
            m_vehiclePhysics = GetComponent<VehiclePhysics>();
        }

        public void ApplyUpgrade(UpgradeType type)
        {
            int currentLevel = GetUpgradeLevel(type);
            if (currentLevel >= m_maxLevel) return;

            SetUpgradeLevel(type, currentLevel + 1);
            ApplyAllUpgrades();
        }

        public void SetUpgradeLevel(UpgradeType type, int level)
        {
            level = Mathf.Clamp(level, 0, m_maxLevel);

            switch (type)
            {
                case UpgradeType.Engine:
                    m_engineLevel = level;
                    break;
                case UpgradeType.Transmission:
                    m_transmissionLevel = level;
                    break;
                case UpgradeType.Brakes:
                    m_brakesLevel = level;
                    break;
                case UpgradeType.Suspension:
                    m_suspensionLevel = level;
                    break;
                case UpgradeType.Turbo:
                    m_turboLevel = level;
                    break;
                case UpgradeType.WeightReduction:
                    m_weightLevel = level;
                    break;
            }
        }

        public int GetUpgradeLevel(UpgradeType type)
        {
            switch (type)
            {
                case UpgradeType.Engine: return m_engineLevel;
                case UpgradeType.Transmission: return m_transmissionLevel;
                case UpgradeType.Brakes: return m_brakesLevel;
                case UpgradeType.Suspension: return m_suspensionLevel;
                case UpgradeType.Turbo: return m_turboLevel;
                case UpgradeType.WeightReduction: return m_weightLevel;
                default: return 0;
            }
        }

        public void ApplyAllUpgrades()
        {
            if (m_vehiclePhysics == null) return;

            VehicleTuning tuning = m_vehiclePhysics.m_tuning;
            if (tuning == null) return;

            float engineMult = 1f + (m_engineLevel * 0.15f * m_levelMultiplier);
            float speedMult = 1f + (m_transmissionLevel * 0.1f * m_levelMultiplier);
            float brakeMult = 1f + (m_brakesLevel * 0.2f * m_levelMultiplier);
            float handlingMult = 1f + (m_suspensionLevel * 0.1f * m_levelMultiplier);
            float turboMult = 1f + (m_turboLevel * 0.25f * m_levelMultiplier);
            float weightMult = 1f - (m_weightLevel * 0.05f);

            tuning.engineForce *= engineMult;
            tuning.maxSpeed *= speedMult;
            tuning.brakeForce *= brakeMult;
            tuning.maxSteerAngle *= handlingMult;
            tuning.downforce *= (1f + (m_turboLevel * 0.2f));
            tuning.mass *= weightMult;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = tuning.mass;
            }
        }

        public float GetTotalPerformanceMultiplier()
        {
            float total = 1f;
            total *= 1f + (m_engineLevel * 0.15f);
            total *= 1f + (m_transmissionLevel * 0.1f);
            total *= 1f + (m_brakesLevel * 0.2f);
            total *= 1f + (m_suspensionLevel * 0.1f);
            total *= 1f + (m_turboLevel * 0.25f);
            return total;
        }

        public int GetTotalUpgradeLevel()
        {
            return m_engineLevel + m_transmissionLevel + m_brakesLevel + 
                   m_suspensionLevel + m_turboLevel + m_weightLevel;
        }

        public void ResetUpgrades()
        {
            m_engineLevel = 0;
            m_transmissionLevel = 0;
            m_brakesLevel = 0;
            m_suspensionLevel = 0;
            m_turboLevel = 0;
            m_weightLevel = 0;
        }
    }
}
