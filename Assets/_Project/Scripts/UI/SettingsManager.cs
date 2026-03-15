using UnityEngine;
using UnityEngine.UI;

namespace CarSimulator.UI
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [Header("Graphics Settings")]
        [SerializeField] private Dropdown m_qualityDropdown;
        [SerializeField] private Toggle m_fullscreenToggle;
        [SerializeField] private Slider m_fovSlider;
        [SerializeField] private Dropdown m_resolutionDropdown;

        [Header("Audio Settings")]
        [SerializeField] private Slider m_masterVolumeSlider;
        [SerializeField] private Slider m_musicVolumeSlider;
        [SerializeField] private Slider m_sfxVolumeSlider;

        [Header("Gameplay Settings")]
        [SerializeField] private Toggle m_invertYToggle;
        [SerializeField] private Slider m_sensitivitySlider;
        [SerializeField] private Toggle m_subtitlesToggle;

        [Header("Defaults")]
        [SerializeField] private int m_defaultQuality = 2;
        [SerializeField] private float m_defaultFOV = 60f;
        [SerializeField] private float m_defaultMasterVolume = 1f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadSettings();
            SetupUI();
        }

        private void SetupUI()
        {
            if (m_qualityDropdown != null)
            {
                m_qualityDropdown.ClearOptions();
                m_qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
                m_qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            }

            if (m_fullscreenToggle != null)
            {
                m_fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            }

            if (m_fovSlider != null)
            {
                m_fovSlider.minValue = 40f;
                m_fovSlider.maxValue = 120f;
                m_fovSlider.onValueChanged.AddListener(OnFOVChanged);
            }

            if (m_masterVolumeSlider != null)
            {
                m_masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }

            if (m_musicVolumeSlider != null)
            {
                m_musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (m_sfxVolumeSlider != null)
            {
                m_sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }

            if (m_invertYToggle != null)
            {
                m_invertYToggle.onValueChanged.AddListener(OnInvertYChanged);
            }

            if (m_sensitivitySlider != null)
            {
                m_sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            }

            if (m_subtitlesToggle != null)
            {
                m_subtitlesToggle.onValueChanged.AddListener(OnSubtitlesChanged);
            }
        }

        public void LoadSettings()
        {
            if (m_qualityDropdown != null)
            {
                int quality = PlayerPrefs.GetInt("QualityLevel", m_defaultQuality);
                QualitySettings.SetQualityLevel(quality, true);
                m_qualityDropdown.value = quality;
            }

            if (m_fullscreenToggle != null)
            {
                bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
                Screen.fullScreen = fullscreen;
                m_fullscreenToggle.isOn = fullscreen;
            }

            if (m_fovSlider != null)
            {
                float fov = PlayerPrefs.GetFloat("FOV", m_defaultFOV);
                m_fovSlider.value = fov;
                SetFOV(fov);
            }

            if (m_masterVolumeSlider != null)
            {
                float volume = PlayerPrefs.GetFloat("MasterVolume", m_defaultMasterVolume);
                m_masterVolumeSlider.value = volume;
                SetMasterVolume(volume);
            }

            if (m_musicVolumeSlider != null)
            {
                float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
                m_musicVolumeSlider.value = volume;
            }

            if (m_sfxVolumeSlider != null)
            {
                float volume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
                m_sfxVolumeSlider.value = volume;
            }

            if (m_invertYToggle != null)
            {
                bool invert = PlayerPrefs.GetInt("InvertY", 0) == 1;
                m_invertYToggle.isOn = invert;
            }

            if (m_sensitivitySlider != null)
            {
                float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
                m_sensitivitySlider.value = sensitivity;
            }

            if (m_subtitlesToggle != null)
            {
                bool subtitles = PlayerPrefs.GetInt("Subtitles", 1) == 1;
                m_subtitlesToggle.isOn = subtitles;
            }
        }

        public void SaveSettings()
        {
            if (m_qualityDropdown != null)
            {
                PlayerPrefs.SetInt("QualityLevel", m_qualityDropdown.value);
            }

            if (m_fullscreenToggle != null)
            {
                PlayerPrefs.SetInt("Fullscreen", m_fullscreenToggle.isOn ? 1 : 0);
            }

            if (m_fovSlider != null)
            {
                PlayerPrefs.SetFloat("FOV", m_fovSlider.value);
            }

            if (m_masterVolumeSlider != null)
            {
                PlayerPrefs.SetFloat("MasterVolume", m_masterVolumeSlider.value);
            }

            if (m_musicVolumeSlider != null)
            {
                PlayerPrefs.SetFloat("MusicVolume", m_musicVolumeSlider.value);
            }

            if (m_sfxVolumeSlider != null)
            {
                PlayerPrefs.SetFloat("SFXVolume", m_sfxVolumeSlider.value);
            }

            if (m_invertYToggle != null)
            {
                PlayerPrefs.SetInt("InvertY", m_invertYToggle.isOn ? 1 : 0);
            }

            if (m_sensitivitySlider != null)
            {
                PlayerPrefs.SetFloat("Sensitivity", m_sensitivitySlider.value);
            }

            if (m_subtitlesToggle != null)
            {
                PlayerPrefs.SetInt("Subtitles", m_subtitlesToggle.isOn ? 1 : 0);
            }

            PlayerPrefs.Save();
        }

        public void ResetToDefaults()
        {
            PlayerPrefs.DeleteAll();
            LoadSettings();
        }

        private void OnQualityChanged(int value)
        {
            QualitySettings.SetQualityLevel(value, true);
            PlayerPrefs.SetInt("QualityLevel", value);
            PlayerPrefs.Save();
        }

        private void OnFullscreenChanged(bool value)
        {
            Screen.fullScreen = value;
            PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnFOVChanged(float value)
        {
            SetFOV(value);
            PlayerPrefs.SetFloat("FOV", value);
            PlayerPrefs.Save();
        }

        private void SetFOV(float fov)
        {
            var cameras = Camera.allCameras;
            foreach (var cam in cameras)
            {
                if (cam.CompareTag("MainCamera") || cam.name.Contains("Main"))
                {
                    cam.fieldOfView = fov;
                }
            }
        }

        private void OnMasterVolumeChanged(float value)
        {
            SetMasterVolume(value);
            PlayerPrefs.SetFloat("MasterVolume", value);
            PlayerPrefs.Save();
        }

        private void SetMasterVolume(float volume)
        {
            AudioListener.volume = volume;
        }

        private void OnMusicVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            PlayerPrefs.Save();
        }

        private void OnSFXVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            PlayerPrefs.Save();
        }

        private void OnInvertYChanged(bool value)
        {
            PlayerPrefs.SetInt("InvertY", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnSensitivityChanged(float value)
        {
            PlayerPrefs.SetFloat("Sensitivity", value);
            PlayerPrefs.Save();
        }

        private void OnSubtitlesChanged(bool value)
        {
            PlayerPrefs.SetInt("Subtitles", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public int GetQualityLevel() => QualitySettings.GetQualityLevel();
        public float GetFOV() => PlayerPrefs.GetFloat("FOV", m_defaultFOV);
        public float GetMasterVolume() => PlayerPrefs.GetFloat("MasterVolume", m_defaultMasterVolume);
    }

    public class SettingsUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject m_settingsPanel;
        [SerializeField] private GameObject m_graphicsPanel;
        [SerializeField] private GameObject m_audioPanel;
        [SerializeField] private GameObject m_gameplayPanel;

        [Header("Buttons")]
        [SerializeField] private Button m_closeButton;
        [SerializeField] private Button m_graphicsTabButton;
        [SerializeField] private Button m_audioTabButton;
        [SerializeField] private Button m_gameplayTabButton;
        [SerializeField] private Button m_resetButton;

        private void Start()
        {
            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(CloseSettings);

            if (m_graphicsTabButton != null)
                m_graphicsTabButton.onClick.AddListener(() => ShowTab(m_graphicsPanel));

            if (m_audioTabButton != null)
                m_audioTabButton.onClick.AddListener(() => ShowTab(m_audioPanel));

            if (m_gameplayTabButton != null)
                m_gameplayTabButton.onClick.AddListener(() => ShowTab(m_gameplayPanel));

            if (m_resetButton != null)
                m_resetButton.onClick.AddListener(ResetSettings);

            if (m_settingsPanel != null)
                m_settingsPanel.SetActive(false);
        }

        public void OpenSettings()
        {
            if (m_settingsPanel != null)
                m_settingsPanel.SetActive(true);

            ShowTab(m_graphicsPanel);
        }

        public void CloseSettings()
        {
            SettingsManager.Instance?.SaveSettings();

            if (m_settingsPanel != null)
                m_settingsPanel.SetActive(false);
        }

        private void ShowTab(GameObject tab)
        {
            if (m_graphicsPanel != null) m_graphicsPanel.SetActive(false);
            if (m_audioPanel != null) m_audioPanel.SetActive(false);
            if (m_gameplayPanel != null) m_gameplayPanel.SetActive(false);

            if (tab != null) tab.SetActive(true);
        }

        private void ResetSettings()
        {
            SettingsManager.Instance?.ResetToDefaults();
        }
    }
}
