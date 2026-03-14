using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class GarageController : MonoBehaviour
    {
        [Header("Garage UI")]
        [SerializeField] private GameObject m_garagePanel;
        [SerializeField] private Canvas m_garageCanvas;
        
        [Header("Color Selection")]
        [SerializeField] private Slider m_redSlider;
        [SerializeField] private Slider m_greenSlider;
        [SerializeField] private Slider m_blueSlider;
        [SerializeField] private Image m_colorPreview;
        [SerializeField] private Button[] m_presetColorButtons;

        [Header("Tuning Selection")]
        [SerializeField] private Dropdown m_tuningDropdown;
        [SerializeField] private Text m_tuningDescription;

        [Header("Preview")]
        [SerializeField] private Transform m_vehiclePreview;
        [SerializeField] private float m_rotationSpeed = 50f;

        [Header("Actions")]
        [SerializeField] private Button m_applyButton;
        [SerializeField] private Button m_exitButton;

        private VehicleSpawner m_vehicleSpawner;
        private Color m_selectedColor;
        private VehicleTuningPresets.TuningType m_selectedTuning;
        private bool m_isOpen;

        private void Start()
        {
            SetupGarage();
            CloseGarage();
        }

        private void SetupGarage()
        {
            m_vehicleSpawner = FindObjectOfType<VehicleSpawner>();

            if (m_redSlider != null)
                m_redSlider.onValueChanged.AddListener(OnColorChanged);
            if (m_greenSlider != null)
                m_greenSlider.onValueChanged.AddListener(OnColorChanged);
            if (m_blueSlider != null)
                m_blueSlider.onValueChanged.AddListener(OnColorChanged);

            if (m_applyButton != null)
                m_applyButton.onClick.AddListener(ApplyCustomization);
            if (m_exitButton != null)
                m_exitButton.onClick.AddListener(CloseGarage);

            SetupTuningDropdown();
            SetupPresetColors();
            UpdateColorPreview();
        }

        private void SetupTuningDropdown()
        {
            if (m_tuningDropdown == null) return;

            m_tuningDropdown.ClearOptions();
            
            var options = new System.Collections.Generic.List<string>();
            foreach (VehicleTuningPresets.TuningType type in System.Enum.GetValues(typeof(VehicleTuningPresets.TuningType)))
            {
                options.Add(type.ToString());
            }
            
            m_tuningDropdown.AddOptions(options);
            m_tuningDropdown.onValueChanged.AddListener(OnTuningChanged);
        }

        private void SetupPresetColors()
        {
            if (m_presetColorButtons == null) return;

            Color[] presets = {
                Color.red, Color.blue, Color.green, Color.yellow,
                Color.white, Color.black, new Color(1f, 0.5f, 0f), new Color(0.5f, 0f, 1f)
            };

            for (int i = 0; i < m_presetColorButtons.Length && i < presets.Length; i++)
            {
                int colorIndex = i;
                m_presetColorButtons[i].onClick.AddListener(() => SelectPresetColor(presets[colorIndex]));
            }
        }

        private void Update()
        {
            if (m_isOpen && m_vehiclePreview != null)
            {
                m_vehiclePreview.Rotate(Vector3.up, m_rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleGarage();
            }
        }

        public void ToggleGarage()
        {
            if (m_isOpen)
                CloseGarage();
            else
                OpenGarage();
        }

        public void OpenGarage()
        {
            if (m_garagePanel != null)
            {
                m_garagePanel.SetActive(true);
                m_isOpen = true;

                LoadCurrentSettings();
            }
        }

        public void CloseGarage()
        {
            if (m_garagePanel != null)
            {
                m_garagePanel.SetActive(false);
                m_isOpen = false;
            }
        }

        private void LoadCurrentSettings()
        {
            if (m_vehicleSpawner != null)
            {
                m_selectedColor = Color.red;
                m_selectedTuning = m_vehicleSpawner.CurrentTuningType;
            }

            UpdateSliders();
            UpdateColorPreview();
        }

        private void UpdateSliders()
        {
            if (m_redSlider != null) m_redSlider.value = m_selectedColor.r;
            if (m_greenSlider != null) m_greenSlider.value = m_selectedColor.g;
            if (m_blueSlider != null) m_blueSlider.value = m_selectedColor.b;
        }

        private void OnColorChanged(float value)
        {
            m_selectedColor = new Color(
                m_redSlider != null ? m_redSlider.value : 1f,
                m_greenSlider != null ? m_greenSlider.value : 0f,
                m_blueSlider != null ? m_blueSlider.value : 0f
            );
            UpdateColorPreview();
        }

        private void SelectPresetColor(Color color)
        {
            m_selectedColor = color;
            UpdateSliders();
            UpdateColorPreview();
        }

        private void UpdateColorPreview()
        {
            if (m_colorPreview != null)
            {
                m_colorPreview.color = m_selectedColor;
            }
        }

        private void OnTuningChanged(int index)
        {
            m_selectedTuning = (VehicleTuningPresets.TuningType)index;
            UpdateTuningDescription();
        }

        private void UpdateTuningDescription()
        {
            if (m_tuningDescription == null) return;

            string description = m_selectedTuning switch
            {
                VehicleTuningPresets.TuningType.Default => "Balanced handling for everyday driving",
                VehicleTuningPresets.TuningType.Sport => "Higher top speed and grip for track use",
                VehicleTuningPresets.TuningType.Drift => "Low grip for sideways action",
                VehicleTuningPresets.TuningType.Offroad => "Soft suspension for rough terrain",
                _ => "Custom tuning"
            };

            m_tuningDescription.text = description;
        }

        private void ApplyCustomization()
        {
            if (m_vehicleSpawner != null)
            {
                m_vehicleSpawner.SetVehicleColor(m_selectedColor);
                m_vehicleSpawner.SetTuningType(m_selectedTuning);
                m_vehicleSpawner.RespawnVehicle();

                Debug.Log($"[Garage] Applied - Color: {m_selectedColor}, Tuning: {m_selectedTuning}");
            }

            CloseGarage();
        }
    }
}
