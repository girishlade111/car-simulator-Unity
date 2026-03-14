using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class LaunchControl : MonoBehaviour
    {
        [Header("Launch Control Settings")]
        [SerializeField] private bool m_enableLaunchControl = true;
        [SerializeField] private KeyCode m_launchKey = KeyCode.F;
        [SerializeField] private float m_launchRPM = 4000f;
        [SerializeField] private float m_launchClutchDelay = 0.2f;
        [SerializeField] private float m_launchTorqueMultiplier = 1.3f;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;
        [SerializeField] private GearSystem m_gearSystem;

        private bool m_isLaunchControlActive;
        private bool m_isLaunchReady;
        private float m_launchTimer;

        public bool IsLaunchActive => m_isLaunchControlActive;
        public bool IsLaunchReady => m_isLaunchReady;

        private void Start()
        {
            FindComponents();
        }

        private void FindComponents()
        {
            if (m_vehiclePhysics == null)
                m_vehiclePhysics = GetComponent<VehiclePhysics>();
            if (m_gearSystem == null)
                m_gearSystem = GetComponent<GearSystem>();
        }

        private void Update()
        {
            if (!m_enableLaunchControl) return;

            HandleLaunchInput();
            UpdateLaunchControl();
        }

        private void HandleLaunchInput()
        {
            if (Input.GetKeyDown(m_launchKey))
            {
                if (m_isLaunchControlActive)
                {
                    DeactivateLaunchControl();
                }
                else
                {
                    ActivateLaunchControl();
                }
            }
        }

        private void ActivateLaunchControl()
        {
            if (m_vehiclePhysics == null || m_gearSystem == null) return;

            float speed = m_vehiclePhysics.CurrentSpeed;
            if (speed > 5f) return;

            m_isLaunchControlActive = true;
            m_isLaunchReady = false;
            m_launchTimer = 0f;

            m_gearSystem.ShiftToGear(1);

            Debug.Log("[LaunchControl] Launch control activated");
        }

        private void DeactivateLaunchControl()
        {
            m_isLaunchControlActive = false;
            m_isLaunchReady = false;
            Debug.Log("[LaunchControl] Launch control deactivated");
        }

        private void UpdateLaunchControl()
        {
            if (!m_isLaunchControlActive || m_vehiclePhysics == null) return;

            float speed = m_vehiclePhysics.CurrentSpeed;

            if (speed < 2f)
            {
                m_launchTimer += Time.deltaTime;
                
                if (m_launchTimer >= m_launchClutchDelay)
                {
                    m_isLaunchReady = true;
                }
            }
            else
            {
                if (m_isLaunchReady && speed > 30f)
                {
                    DeactivateLaunchControl();
                }
            }
        }

        public float GetLaunchTorqueMultiplier()
        {
            if (!m_isLaunchControlActive || !m_isLaunchReady)
                return 1f;

            return m_launchTorqueMultiplier;
        }

        public float GetTargetLaunchRPM()
        {
            if (!m_isLaunchControlActive)
                return 0f;

            return m_launchRPM;
        }

        private void OnDisable()
        {
            DeactivateLaunchControl();
        }
    }
}
