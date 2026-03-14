using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class VehicleTuningPresets : MonoBehaviour
    {
        public static VehicleTuningPresets Instance { get; private set; }

        [Header("Default Tuning")]
        [SerializeField] private VehicleTuning m_defaultTuning;
        [SerializeField] private VehicleTuning m_sportTuning;
        [SerializeField] private VehicleTuning m_driftTuning;
        [SerializeField] private VehicleTuning m_offroadTuning;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsurePresetsExist();
        }

        private void EnsurePresetsExist()
        {
            if (m_defaultTuning == null) m_defaultTuning = CreatePreset("Default", GetDefaultValues());
            if (m_sportTuning == null) m_sportTuning = CreatePreset("Sport", GetSportValues());
            if (m_driftTuning == null) m_driftTuning = CreatePreset("Drift", GetDriftValues());
            if (m_offroadTuning == null) m_offroadTuning = CreatePreset("Offroad", GetOffroadValues());
        }

        public VehicleTuning GetDefaultTuning() => m_defaultTuning;
        public VehicleTuning GetSportTuning() => m_sportTuning;
        public VehicleTuning GetDriftTuning() => m_driftTuning;
        public VehicleTuning GetOffroadTuning() => m_offroadTuning;

        public VehicleTuning GetTuningByType(TuningType type)
        {
            switch (type)
            {
                case TuningType.Sport: return m_sportTuning;
                case TuningType.Drift: return m_driftTuning;
                case TuningType.Offroad: return m_offroadTuning;
                default: return m_defaultTuning;
            }
        }

        private float[] GetDefaultValues() => new float[] { 40f, 2000f, 180f, 3500f, 0.55f, 15f, 600f, 1400f, 30f, 4f, 0.3f, -15f, 2f };
        private float[] GetSportValues() => new float[] { 45f, 2500f, 220f, 4000f, 0.5f, 25f, 800f, 1300f, 40f, 5f, 0.25f, -20f, 2f };
        private float[] GetDriftValues() => new float[] { 50f, 2200f, 200f, 3000f, 0.4f, 10f, 400f, 1200f, 25f, 3f, 0.35f, -15f, 3f };
        private float[] GetOffroadValues() => new float[] { 35f, 1800f, 140f, 2500f, 0.6f, 5f, 300f, 1800f, 20f, 2f, 0.5f, -10f, 1.5f };

        private VehicleTuning CreatePreset(string name, float[] values)
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            tuning.name = $"Tuning_{name}";
            
            tuning.maxSteerAngle = values[0];
            tuning.engineForce = values[1];
            tuning.maxSpeed = values[2];
            tuning.brakeForce = values[3];
            tuning.brakeBalance = values[4];
            tuning.downforce = values[5];
            tuning.antiRollForce = values[6];
            tuning.mass = values[7];
            tuning.suspensionSpring = values[8];
            tuning.suspensionDamper = values[9];
            tuning.suspensionDistance = values[10];
            tuning.respawnHeight = values[11];
            tuning.resetDelay = values[12];

            return tuning;
        }

        public enum TuningType
        {
            Default,
            Sport,
            Drift,
            Offroad
        }
    }
}
