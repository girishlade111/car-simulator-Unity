using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class VehicleHUD : MonoBehaviour
    {
        [Header("Speedometer")]
        [SerializeField] private Text m_speedText;
        [SerializeField] private Text m_speedUnitText;
        [SerializeField] private Image m_speedNeedle;
        [SerializeField] private float m_maxSpeed = 220f;
        [SerializeField] private bool m_useKMH = true;

        [Header("Tachometer")]
        [SerializeField] private Text m_rpmText;
        [SerializeField] private Image m_rpmNeedle;
        [SerializeField] private float m_maxRPM = 8000f;
        [SerializeField] private Image m_rpmWarning;

        [Header("Gear Display")]
        [SerializeField] private Text m_gearText;
        [SerializeField] private Text m_gearLabel;

        [Header("Tuning Display")]
        [SerializeField] private Text m_tuningText;

        [Header("Drift Indicator")]
        [SerializeField] private GameObject m_driftIndicator;
        [SerializeField] private Text m_driftText;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;
        [SerializeField] private GearSystem m_gearSystem;
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        [Header("Optimization")]
        [SerializeField] private float m_updateInterval = 0.033f;

        private bool m_isDrifting;
        private float m_lastUpdateTime;

        private void Start()
        {
            FindVehicleComponents();
            InitializeHUD();
        }

        private void Update()
        {
            if (Time.time - m_lastUpdateTime < m_updateInterval) return;
            m_lastUpdateTime = Time.time;

            if (m_vehiclePhysics == null)
            {
                FindVehicleComponents();
                if (m_vehiclePhysics == null) return;
            }

            UpdateSpeedometer();
            UpdateTachometer();
            UpdateGearDisplay();
            UpdateDriftIndicator();
            UpdateTuningDisplay();
        }

        private void FindVehicleComponents()
        {
            if (m_vehiclePhysics == null)
            {
                var vehicle = FindObjectOfType<VehicleSpawner>();
                if (vehicle != null && vehicle.CurrentVehicle != null)
                {
                    m_vehiclePhysics = vehicle.CurrentVehicle.GetComponent<VehiclePhysics>();
                    m_gearSystem = vehicle.CurrentVehicle.GetComponent<GearSystem>();
                }
            }
        }

        private void InitializeHUD()
        {
            if (m_speedUnitText != null)
            {
                m_speedUnitText.text = m_useKMH ? "km/h" : "mph";
            }
        }

        private void UpdateSpeedometer()
        {
            if (m_vehiclePhysics == null) return;

            float speed = m_vehiclePhysics.CurrentSpeed;
            if (!m_useKMH) speed *= 0.621371f;

            if (m_speedText != null)
            {
                m_speedText.text = Mathf.RoundToInt(speed).ToString("F0");
            }

            if (m_speedNeedle != null)
            {
                float angle = Mathf.Lerp(-120f, 120f, speed / m_maxSpeed);
                m_speedNeedle.transform.localRotation = Quaternion.Euler(0, 0, -angle);
            }
        }

        private void UpdateTachometer()
        {
            if (m_gearSystem == null) return;

            float rpm = m_gearSystem.CurrentRPM;

            if (m_rpmText != null)
            {
                m_rpmText.text = Mathf.RoundToInt(rpm).ToString("F0");
            }

            if (m_rpmNeedle != null)
            {
                float angle = Mathf.Lerp(-120f, 120f, rpm / m_maxRPM);
                m_rpmNeedle.transform.localRotation = Quaternion.Euler(0, 0, -angle);
            }

            if (m_rpmWarning != null)
            {
                float rpmPercent = rpm / m_maxRPM;
                m_rpmWarning.enabled = rpmPercent > 0.85f;
                
                if (m_rpmWarning.enabled)
                {
                    float flash = Mathf.PingPong(Time.time * 5f, 1f);
                    m_rpmWarning.color = new Color(1f, 0f, 0f, flash * 0.5f);
                }
            }
        }

        private void UpdateGearDisplay()
        {
            if (m_gearSystem == null || m_gearText == null) return;

            int gear = m_gearSystem.CurrentGear;
            m_gearText.text = gear == 0 ? "N" : gear.ToString();

            if (m_gearLabel != null)
            {
                m_gearLabel.text = m_gearSystem.IsShifting ? "SHIFTING" : "GEAR";
            }
        }

        private void UpdateDriftIndicator()
        {
            if (m_vehiclePhysics == null) return;

            bool isDrifting = m_vehiclePhysics.IsDrifting;

            if (m_driftIndicator != null)
            {
                m_driftIndicator.SetActive(isDrifting);
            }

            if (m_driftText != null && isDrifting)
            {
                m_driftText.text = "DRIFT!";
            }
        }

        private void UpdateTuningDisplay()
        {
            if (m_tuningText == null) return;

            var spawner = m_vehicleSpawner;
            if (spawner == null)
            {
                spawner = FindObjectOfType<VehicleSpawner>();
            }

            if (spawner != null)
            {
                m_tuningText.text = spawner.CurrentTuningType.ToString().ToUpper();
            }
        }

        public void SetUseKMH(bool useKMH)
        {
            m_useKMH = useKMH;
            InitializeHUD();
        }

        public void SetMaxSpeed(float maxSpeed)
        {
            m_maxSpeed = maxSpeed;
        }
    }
}
