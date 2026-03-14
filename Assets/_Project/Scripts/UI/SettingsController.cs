using UnityEngine;
using UnityEngine.UI;
using CarSimulator.SaveSystem;

namespace CarSimulator.UI
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private Slider m_masterVolume;
        [SerializeField] private Slider m_musicVolume;
        [SerializeField] private Slider m_sfxVolume;
        [SerializeField] private Toggle m_muteToggle;

        [Header("Graphics")]
        [SerializeField] private Dropdown m_qualityDropdown;
        [SerializeField] private Toggle m_fullscreenToggle;
        [SerializeField] private Toggle m_vsyncToggle;

        [Header("Controls")]
        [SerializeField] private Toggle m_invertYToggle;
        [SerializeField] private Slider m_sensitivitySlider;

        [Header("Actions")]
        [SerializeField] private Button m_applyButton;
        [SerializeField] private Button m_resetButton;
        [SerializeField] private Button m_backButton;

        private GameSettings m_settings;

        private void Start()
        {
            LoadSettings();
            SetupListeners();
        }

        private void LoadSettings()
        {
            if (SettingsPersistence.Instance != null)
            {
                m_settings = SettingsPersistence.Instance.Settings;
            }
            else
            {
                m_settings = new GameSettings();
            }

            ApplyToUI();
        }

        private void SetupListeners()
        {
            if (m_applyButton) m_applyButton.onClick.AddListener(ApplySettings);
            if (m_resetButton) m_resetButton.onClick.AddListener(ResetToDefaults);
            if (m_backButton) m_backButton.onClick.AddListener(OnBack);
        }

        private void ApplyToUI()
        {
            if (m_masterVolume) m_masterVolume.value = m_settings.audio.masterVolume;
            if (m_musicVolume) m_musicVolume.value = m_settings.audio.musicVolume;
            if (m_sfxVolume) m_sfxVolume.value = m_settings.audio.sfxVolume;
            if (m_muteToggle) m_muteToggle.isOn = m_settings.audio.muteAudio;

            if (m_qualityDropdown) m_qualityDropdown.value = m_settings.graphics.qualityLevel;
            if (m_fullscreenToggle) m_fullscreenToggle.isOn = m_settings.graphics.fullscreen;
            if (m_vsyncToggle) m_vsyncToggle.isOn = m_settings.graphics.vsync;

            if (m_invertYToggle) m_invertYToggle.isOn = m_settings.input.invertY;
            if (m_sensitivitySlider) m_sensitivitySlider.value = m_settings.input.mouseSensitivity;
        }

        public void ApplySettings()
        {
            if (m_settings == null || SettingsPersistence.Instance == null) return;

            m_settings.audio.masterVolume = m_masterVolume != null ? m_masterVolume.value : 1f;
            m_settings.audio.musicVolume = m_musicVolume != null ? m_musicVolume.value : 1f;
            m_settings.audio.sfxVolume = m_sfxVolume != null ? m_sfxVolume.value : 1f;
            m_settings.audio.muteAudio = m_muteToggle != null && m_muteToggle.isOn;

            m_settings.graphics.qualityLevel = m_qualityDropdown != null ? m_qualityDropdown.value : 2;
            m_settings.graphics.fullscreen = m_fullscreenToggle != null && m_fullscreenToggle.isOn;
            m_settings.graphics.vsync = m_vsyncToggle != null && m_vsyncToggle.isOn;

            m_settings.input.invertY = m_invertYToggle != null && m_invertYToggle.isOn;
            m_settings.input.mouseSensitivity = m_sensitivitySlider != null ? m_sensitivitySlider.value : 1f;

            SettingsPersistence.Instance.ApplySettings();
            SettingsPersistence.Instance.SaveSettings();
        }

        public void ResetToDefaults()
        {
            if (SettingsPersistence.Instance != null)
            {
                SettingsPersistence.Instance.ResetToDefaults();
                LoadSettings();
            }
        }

        public void OnBack()
        {
            gameObject.SetActive(false);
        }
    }
}
