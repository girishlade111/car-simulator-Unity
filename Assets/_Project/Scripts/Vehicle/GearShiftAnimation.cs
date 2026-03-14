using UnityEngine;
using UnityEngine.UI;

namespace CarSimulator.Vehicle
{
    public class GearShiftAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private bool m_enableAnimation = true;
        [SerializeField] private float m_shiftDuration = 0.3f;
        [SerializeField] private float m_shiftIntensity = 0.1f;

        [Header("Visual Elements")]
        [SerializeField] private GameObject m_shiftIndicator;
        [SerializeField] private Text m_shiftText;
        [SerializeField] private Image m_shiftFlash;

        [Header("References")]
        [SerializeField] private GearSystem m_gearSystem;

        private bool m_isShifting;
        private float m_shiftTimer;
        private Vector3 m_originalCameraOffset;
        private Camera m_mainCamera;
        private Transform m_vehicleTransform;

        private void Start()
        {
            FindComponents();
            HideShiftIndicator();
        }

        private void FindComponents()
        {
            if (m_gearSystem == null)
                m_gearSystem = GetComponent<GearSystem>();

            m_mainCamera = Camera.main;
            
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner != null && spawner.CurrentVehicle != null)
            {
                m_vehicleTransform = spawner.CurrentVehicle.transform;
            }
        }

        private void Update()
        {
            if (m_gearSystem == null || !m_enableAnimation) return;

            bool isShifting = m_gearSystem.IsShifting;

            if (isShifting && !m_isShifting)
            {
                OnGearShiftStart();
            }
            else if (!isShifting && m_isShifting)
            {
                OnGearShiftEnd();
            }

            if (m_isShifting)
            {
                UpdateShiftAnimation();
            }
        }

        private void OnGearShiftStart()
        {
            m_isShifting = true;
            m_shiftTimer = 0f;

            int newGear = m_gearSystem.CurrentGear;
            ShowShiftIndicator(newGear);
        }

        private void OnGearShiftEnd()
        {
            m_isShifting = false;
            HideShiftIndicator();
        }

        private void UpdateShiftAnimation()
        {
            m_shiftTimer += Time.deltaTime;
            float progress = m_shiftTimer / m_shiftDuration;

            if (progress >= 1f)
            {
                return;
            }

            float shake = Mathf.Sin(progress * Mathf.PI * 4f) * m_shiftIntensity * (1f - progress);
            
            if (m_vehicleTransform != null)
            {
                m_vehicleTransform.position += Vector3.up * shake * 0.5f;
            }

            if (m_shiftFlash != null)
            {
                float flash = (1f - progress) * 0.3f;
                m_shiftFlash.color = new Color(1f, 1f, 1f, flash);
            }
        }

        private void ShowShiftIndicator(int gear)
        {
            if (m_shiftIndicator != null)
            {
                m_shiftIndicator.SetActive(true);
            }

            if (m_shiftText != null)
            {
                m_shiftText.text = gear == 0 ? "N" : gear.ToString();
            }
        }

        private void HideShiftIndicator()
        {
            if (m_shiftIndicator != null)
            {
                m_shiftIndicator.SetActive(false);
            }

            if (m_shiftFlash != null)
            {
                m_shiftFlash.color = Color.clear;
            }
        }

        public void TriggerShiftIndicator(int gear)
        {
            ShowShiftIndicator(gear);
            Invoke(nameof(HideShiftIndicator), m_shiftDuration);
        }
    }
}
