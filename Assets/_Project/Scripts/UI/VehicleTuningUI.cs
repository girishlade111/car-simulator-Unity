using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class VehicleTuningUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private GameObject m_panel;
        [SerializeField] private Text m_currentTuningText;
        [SerializeField] private Button m_sportButton;
        [SerializeField] private Button m_driftButton;
        [SerializeField] private Button m_defaultButton;
        [SerializeField] private Button m_offroadButton;

        [Header("Settings")]
        [SerializeField] private KeyCode m_toggleKey = KeyCode.T;
        [SerializeField] private bool m_showOnStart = false;

        private bool m_isVisible;
        private VehicleSpawner m_vehicleSpawner;

        private void Start()
        {
            SetupButtons();
            m_isVisible = m_showOnStart;
            UpdatePanelVisibility();
            UpdateTuningText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(m_toggleKey))
            {
                ToggleUI();
            }
        }

        private void SetupButtons()
        {
            if (m_sportButton != null)
                m_sportButton.onClick.AddListener(() => SetTuning(VehicleTuningPresets.TuningType.Sport));
            if (m_driftButton != null)
                m_driftButton.onClick.AddListener(() => SetTuning(VehicleTuningPresets.TuningType.Drift));
            if (m_defaultButton != null)
                m_defaultButton.onClick.AddListener(() => SetTuning(VehicleTuningPresets.TuningType.Default));
            if (m_offroadButton != null)
                m_offroadButton.onClick.AddListener(() => SetTuning(VehicleTuningPresets.TuningType.Offroad));
        }

        public void ToggleUI()
        {
            m_isVisible = !m_isVisible;
            UpdatePanelVisibility();
        }

        private void UpdatePanelVisibility()
        {
            if (m_panel != null)
            {
                m_panel.SetActive(m_isVisible);
            }
        }

        private void UpdateTuningText()
        {
            if (m_currentTuningText != null)
            {
                var spawner = GetVehicleSpawner();
                if (spawner != null)
                {
                    m_currentTuningText.text = $"Tuning: {spawner.CurrentTuningType}";
                }
            }
        }

        private void SetTuning(VehicleTuningPresets.TuningType type)
        {
            var spawner = GetVehicleSpawner();
            if (spawner != null)
            {
                spawner.SetTuningType(type);
                UpdateTuningText();
                Debug.Log($"[VehicleTuningUI] Switched to {type} tuning");
            }
        }

        private VehicleSpawner GetVehicleSpawner()
        {
            if (m_vehicleSpawner == null)
            {
                m_vehicleSpawner = FindObjectOfType<VehicleSpawner>();
            }
            return m_vehicleSpawner;
        }

        public void Show() { m_isVisible = true; UpdatePanelVisibility(); }
        public void Hide() { m_isVisible = false; UpdatePanelVisibility(); }
    }
}
