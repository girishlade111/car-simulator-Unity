using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class DashboardUI : MonoBehaviour
    {
        [Header("Dashboard Elements")]
        [SerializeField] private Canvas m_dashboardCanvas;
        [SerializeField] private RectTransform m_dashboardPanel;

        [Header("Speedometer")]
        [SerializeField] private Image m_speedGauge;
        [SerializeField] private Text m_speedText;
        [SerializeField] private string m_speedFormat = "{0} km/h";

        [Header("Tachometer")]
        [SerializeField] private Image m_rpmGauge;
        [SerializeField] private Text m_rpmText;
        [SerializeField] private float m_maxRPM = 8000f;
        [SerializeField] private Image m_rpmRedline;

        [Header("Gear Display")]
        [SerializeField] private Text m_gearText;
        [SerializeField] private Image m_shiftIndicator;

        [Header("Indicators")]
        [SerializeField] private Image m_turnLeftIndicator;
        [SerializeField] private Image m_turnRightIndicator;
        [SerializeField] private Image m_headlightIndicator;
        [SerializeField] private Image m_handbrakeIndicator;

        [Header("Turbo/Nitrous")]
        [SerializeField] private Image m_turboGauge;
        [SerializeField] private Image m_nitrousGauge;

        [Header("Vehicle Health")]
        [SerializeField] private Image m_healthBar;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;
        [SerializeField] private VehicleInput m_vehicleInput;
        [SerializeField] private GearSystem m_gearSystem;
        [SerializeField] private TurboBoost m_turboBoost;
        [SerializeField] private NitrousOxide m_nitrousOxide;
        [SerializeField] private VehicleDamage m_vehicleDamage;
        [SerializeField] private VehicleLights m_vehicleLights;

        private bool m_isInitialized;

        private void Start()
        {
            FindVehicleComponents();
            InitializeDashboard();
        }

        private void FindVehicleComponents()
        {
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                GameObject vehicle = spawner.CurrentVehicle;
                m_vehiclePhysics = vehicle.GetComponent<VehiclePhysics>();
                m_vehicleInput = vehicle.GetComponent<VehicleInput>();
                m_gearSystem = vehicle.GetComponent<GearSystem>();
                m_turboBoost = vehicle.GetComponent<TurboBoost>();
                m_nitrousOxide = vehicle.GetComponent<NitrousOxide>();
                m_vehicleDamage = vehicle.GetComponent<VehicleDamage>();
                m_vehicleLights = vehicle.GetComponent<VehicleLights>();
            }
        }

        private void InitializeDashboard()
        {
            if (m_dashboardCanvas != null)
            {
                m_dashboardCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            if (m_dashboardPanel != null)
            {
                m_dashboardPanel.anchorMin = new Vector2(0, 0);
                m_dashboardPanel.anchorMax = new Vector2(0, 1);
                m_dashboardPanel.pivot = new Vector2(0, 0.5f);
                m_dashboardPanel.anchoredPosition = Vector2.zero;
                m_dashboardPanel.sizeDelta = new Vector2(300, 0);
            }

            m_isInitialized = true;
        }

        private void Update()
        {
            if (!m_isInitialized) return;

            UpdateSpeed();
            UpdateRPM();
            UpdateGear();
            UpdateIndicators();
            UpdateTurboNitrous();
            UpdateHealth();
        }

        private void UpdateSpeed()
        {
            if (m_vehiclePhysics == null || m_speedGauge == null) return;

            float speed = m_vehiclePhysics.CurrentSpeed;
            float maxSpeed = m_vehiclePhysics.MaxSpeed;
            float fillAmount = Mathf.Clamp01(speed / maxSpeed);

            m_speedGauge.fillAmount = fillAmount;

            if (m_speedText != null)
            {
                m_speedText.text = string.Format(m_speedFormat, Mathf.RoundToInt(speed));
            }
        }

        private void UpdateRPM()
        {
            if (m_gearSystem == null || m_rpmGauge == null) return;

            float rpm = m_gearSystem.CurrentRPM;
            float fillAmount = Mathf.Clamp01(rpm / m_maxRPM);

            m_rpmGauge.fillAmount = fillAmount;

            if (m_rpmText != null)
            {
                m_rpmText.text = Mathf.RoundToInt(rpm).ToString("F0");
            }

            if (m_rpmRedline != null)
            {
                m_rpmRedline.enabled = rpm > m_maxRPM * 0.85f;
            }
        }

        private void UpdateGear()
        {
            if (m_gearSystem == null || m_gearText == null) return;

            int gear = m_gearSystem.CurrentGear;
            m_gearText.text = gear == 0 ? "N" : gear.ToString();

            if (m_shiftIndicator != null)
            {
                m_shiftIndicator.enabled = m_gearSystem.IsShifting;
            }
        }

        private void UpdateIndicators()
        {
            if (m_vehicleInput == null) return;

            float steer = m_vehicleInput.SteerInput;

            if (m_turnLeftIndicator != null)
            {
                m_turnLeftIndicator.enabled = steer < -0.1f;
            }

            if (m_turnRightIndicator != null)
            {
                m_turnRightIndicator.enabled = steer > 0.1f;
            }

            if (m_vehicleLights != null && m_headlightIndicator != null)
            {
                m_headlightIndicator.enabled = m_vehicleLights.AreHeadlightsOn;
            }

            if (m_handbrakeIndicator != null)
            {
                m_handbrakeIndicator.enabled = m_vehicleInput.IsHandbraking;
            }
        }

        private void UpdateTurboNitrous()
        {
            if (m_turboBoost != null && m_turboGauge != null)
            {
                float turboCharge = m_turboBoost.TurboMeter;
                m_turboGauge.fillAmount = turboCharge;
            }

            if (m_nitrousOxide != null && m_nitrousGauge != null)
            {
                float nitrous = m_nitrousOxide.CurrentNitrous / m_nitrousOxide.MaxNitrous;
                m_nitrousGauge.fillAmount = nitrous;
            }
        }

        private void UpdateHealth()
        {
            if (m_vehicleDamage != null && m_healthBar != null)
            {
                m_healthBar.fillAmount = m_vehicleDamage.HealthPercent;
            }
        }

        public void SetDashboardVisible(bool visible)
        {
            if (m_dashboardCanvas != null)
            {
                m_dashboardCanvas.gameObject.SetActive(visible);
            }
        }
    }
}
