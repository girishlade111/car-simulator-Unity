using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class GearSystem : MonoBehaviour
    {
        public enum GearShiftMode
        {
            Automatic,
            Manual
        }

        [Header("Gear Settings")]
        [SerializeField] private int m_gearCount = 6;
        [SerializeField] private GearShiftMode m_shiftMode = GearShiftMode.Automatic;
        [SerializeField] private float m_shiftUpRPM = 6500f;
        [SerializeField] private float m_shiftDownRPM = 2500f;
        [SerializeField] private float m_shiftDelay = 0.3f;

        [Header("Gear Ratios")]
        [SerializeField] private float[] m_gearRatios = new float[] { 3.5f, 2.5f, 1.8f, 1.4f, 1.1f, 0.9f };
        [SerializeField] private float m_finalDriveRatio = 3.7f;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;

        private int m_currentGear;
        private float m_currentRPM;
        private float m_shiftTimer;
        private bool m_isShifting;

        public int CurrentGear => m_currentGear;
        public float CurrentRPM => m_currentRPM;
        public float[] GearRatios => m_gearRatios;
        public bool IsShifting => m_isShifting;

        private void Start()
        {
            if (m_vehiclePhysics == null)
            {
                m_vehiclePhysics = GetComponent<VehiclePhysics>();
            }

            InitializeGears();
        }

        private void InitializeGears()
        {
            if (m_gearRatios == null || m_gearRatios.Length == 0)
            {
                m_gearRatios = new float[m_gearCount];
                for (int i = 0; i < m_gearCount; i++)
                {
                    m_gearRatios[i] = 3.5f - (i * 0.5f);
                }
            }
        }

        private void Update()
        {
            if (m_vehiclePhysics == null) return;

            UpdateRPM();
            
            if (m_shiftMode == GearShiftMode.Automatic)
            {
                HandleAutomaticShifting();
            }
            else
            {
                HandleManualShifting();
            }
        }

        private void UpdateRPM()
        {
            float speed = m_vehiclePhysics.CurrentSpeed;
            float maxSpeed = m_vehiclePhysics.MaxSpeed;

            if (m_currentGear == 0)
            {
                m_currentRPM = 800f + (speed / maxSpeed) * 3000f;
            }
            else
            {
                float gearSpeed = speed / GetGearSpeedRatio(m_currentGear);
                float speedPercent = Mathf.Clamp01(gearSpeed / maxSpeed);
                m_currentRPM = Mathf.Lerp(m_shiftDownRPM, m_shiftUpRPM, speedPercent);
            }
        }

        private void HandleAutomaticShifting()
        {
            if (m_isShifting)
            {
                m_shiftTimer -= Time.deltaTime;
                if (m_shiftTimer <= 0)
                {
                    m_isShifting = false;
                }
                return;
            }

            if (m_currentRPM > m_shiftUpRPM && m_currentGear < m_gearCount - 1)
            {
                ShiftUp();
            }
            else if (m_currentRPM < m_shiftDownRPM && m_currentGear > 0)
            {
                ShiftDown();
            }
        }

        private void HandleManualShifting()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShiftUp();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ShiftDown();
            }
        }

        public void ShiftUp()
        {
            if (m_currentGear >= m_gearCount - 1 || m_isShifting) return;

            m_currentGear++;
            m_isShifting = true;
            m_shiftTimer = m_shiftDelay;
        }

        public void ShiftDown()
        {
            if (m_currentGear <= 0 || m_isShifting) return;

            m_currentGear--;
            m_isShifting = true;
            m_shiftTimer = m_shiftDelay;
        }

        public void ShiftToGear(int gear)
        {
            gear = Mathf.Clamp(gear, 0, m_gearCount - 1);
            m_currentGear = gear;
        }

        private float GetGearSpeedRatio(int gear)
        {
            if (gear <= 0 || gear >= m_gearRatios.Length) return 1f;
            return m_gearRatios[gear] / m_gearRatios[0];
        }

        public float GetWheelTorque()
        {
            if (m_currentGear == 0) return m_vehiclePhysics != null ? m_vehiclePhysics.CurrentSpeed * 50f : 0f;

            float ratio = m_gearRatios[m_currentGear] * m_finalDriveRatio;
            float speed = m_vehiclePhysics != null ? m_vehiclePhysics.CurrentSpeed : 0f;
            
            return speed * ratio;
        }

        public float GetOptimalShiftSpeed(int targetGear)
        {
            if (targetGear <= 0 || targetGear >= m_gearRatios.Length) return 0f;
            
            float currentRatio = m_gearRatios[m_currentGear];
            float targetRatio = m_gearRatios[targetGear];
            float maxSpeed = m_vehiclePhysics != null ? m_vehiclePhysics.MaxSpeed : 180f;

            return maxSpeed * (currentRatio / targetRatio) * 0.1f;
        }
    }
}
