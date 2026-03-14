using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject m_settingsPanel;
    [SerializeField] private GameObject m_graphicsPanel;
    [SerializeField] private GameObject m_audioPanel;
    [SerializeField] private GameObject m_controlsPanel;

    [Header("Graphics Settings")]
    [SerializeField] private Dropdown m_qualityDropdown;
    [SerializeField] private Toggle m_fullscreenToggle;
    [SerializeField] private Slider m_viewDistanceSlider;
    [SerializeField] private Toggle m_shadowsToggle;
    [SerializeField] private Toggle m_vsyncToggle;

    [Header("Audio Settings")]
    [SerializeField] private Slider m_masterVolumeSlider;
    [SerializeField] private Slider m_musicVolumeSlider;
    [SerializeField] private Slider m_sfxVolumeSlider;
    [SerializeField] private Toggle m_muteToggle;

    [Header("Controls Settings")]
    [SerializeField] private Toggle m_invertYToggle;
    [SerializeField] private Slider m_mouseSensitivitySlider;
    [SerializeField] private Toggle m_controllerToggle;

    [Header("Resolution")]
    [SerializeField] private Dropdown m_resolutionDropdown;

    private Resolution[] m_resolutions;

    private void Start()
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        if (SettingsPersistence.Instance == null)
        {
            Debug.LogWarning("[SettingsMenu] SettingsPersistence not found!");
            return;
        }

        LoadSettingsToUI();
        SetupEventListeners();
    }

    private void LoadSettingsToUI()
    {
        var settings = SettingsPersistence.Instance.Settings;

        if (m_qualityDropdown != null)
        {
            m_qualityDropdown.ClearOptions();
            m_qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
            m_qualityDropdown.value = settings.graphics.qualityLevel;
        }

        if (m_fullscreenToggle != null)
            m_fullscreenToggle.isOn = settings.graphics.fullscreen;

        if (m_viewDistanceSlider != null)
            m_viewDistanceSlider.value = settings.graphics.viewDistance;

        if (m_shadowsToggle != null)
            m_shadowsToggle.isOn = settings.graphics.shadows;

        if (m_vsyncToggle != null)
            m_vsyncToggle.isOn = settings.graphics.vsync;

        if (m_masterVolumeSlider != null)
            m_masterVolumeSlider.value = settings.audio.masterVolume;

        if (m_musicVolumeSlider != null)
            m_musicVolumeSlider.value = settings.audio.musicVolume;

        if (m_sfxVolumeSlider != null)
            m_sfxVolumeSlider.value = settings.audio.sfxVolume;

        if (m_muteToggle != null)
            m_muteToggle.isOn = settings.audio.muteAudio;

        if (m_invertYToggle != null)
            m_invertYToggle.isOn = settings.input.invertY;

        if (m_mouseSensitivitySlider != null)
            m_mouseSensitivitySlider.value = settings.input.mouseSensitivity;

        if (m_controllerToggle != null)
            m_controllerToggle.isOn = settings.input.controllerEnabled;

        SetupResolutionDropdown();
    }

    private void SetupResolutionDropdown()
    {
        if (m_resolutionDropdown == null) return;

        m_resolutions = Screen.resolutions;
        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < m_resolutions.Length; i++)
        {
            string option = m_resolutions[i].width + " x " + m_resolutions[i].height;
            options.Add(option);

            if (m_resolutions[i].width == Screen.width && m_resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        m_resolutionDropdown.ClearOptions();
        m_resolutionDropdown.AddOptions(options);
        m_resolutionDropdown.value = currentResolutionIndex;
    }

    private void SetupEventListeners()
    {
        if (m_qualityDropdown != null)
            m_qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

        if (m_fullscreenToggle != null)
            m_fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

        if (m_viewDistanceSlider != null)
            m_viewDistanceSlider.onValueChanged.AddListener(OnViewDistanceChanged);

        if (m_shadowsToggle != null)
            m_shadowsToggle.onValueChanged.AddListener(OnShadowsChanged);

        if (m_vsyncToggle != null)
            m_vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);

        if (m_masterVolumeSlider != null)
            m_masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (m_musicVolumeSlider != null)
            m_musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (m_sfxVolumeSlider != null)
            m_sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (m_muteToggle != null)
            m_muteToggle.onValueChanged.AddListener(OnMuteChanged);

        if (m_invertYToggle != null)
            m_invertYToggle.onValueChanged.AddListener(OnInvertYChanged);

        if (m_mouseSensitivitySlider != null)
            m_mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

        if (m_controllerToggle != null)
            m_controllerToggle.onValueChanged.AddListener(OnControllerChanged);

        if (m_resolutionDropdown != null)
            m_resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    public void ShowSettings()
    {
        if (m_settingsPanel != null)
            m_settingsPanel.SetActive(true);

        LoadSettingsToUI();
    }

    public void HideSettings()
    {
        if (m_settingsPanel != null)
            m_settingsPanel.SetActive(false);

        SaveCurrentSettings();
    }

    public void ShowGraphicsPanel()
    {
        HideAllSubPanels();
        if (m_graphicsPanel != null)
            m_graphicsPanel.SetActive(true);
    }

    public void ShowAudioPanel()
    {
        HideAllSubPanels();
        if (m_audioPanel != null)
            m_audioPanel.SetActive(true);
    }

    public void ShowControlsPanel()
    {
        HideAllSubPanels();
        if (m_controlsPanel != null)
            m_controlsPanel.SetActive(true);
    }

    private void HideAllSubPanels()
    {
        if (m_graphicsPanel != null) m_graphicsPanel.SetActive(false);
        if (m_audioPanel != null) m_audioPanel.SetActive(false);
        if (m_controlsPanel != null) m_controlsPanel.SetActive(false);
    }

    public void SaveCurrentSettings()
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.SaveSettings();
            Debug.Log("[SettingsMenu] Settings saved");
        }
    }

    public void ResetToDefaults()
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.ResetToDefaults();
            LoadSettingsToUI();
        }
    }

    private void OnQualityChanged(int value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.UpdateGraphicsQuality(value);
        }
    }

    private void OnFullscreenChanged(bool value)
    {
        if (SettingsPersistence.Instance != null)
        {
            var settings = SettingsPersistence.Instance.Settings;
            SettingsPersistence.Instance.UpdateResolution(settings.graphics.resolutionWidth, settings.graphics.resolutionHeight, value);
        }
    }

    private void OnViewDistanceChanged(float value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.Settings.graphics.viewDistance = value;
        }
    }

    private void OnShadowsChanged(bool value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.Settings.graphics.shadows = value;
            QualitySettings.shadowQuality = value ? ShadowQuality.All : ShadowQuality.Disable;
        }
    }

    private void OnVSyncChanged(bool value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.Settings.graphics.vsync = value;
            QualitySettings.vSyncCount = value ? 1 : 0;
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.UpdateMasterVolume(value);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.UpdateMusicVolume(value);
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.UpdateSFXVolume(value);
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetVolume(value);
        }
    }

    private void OnMuteChanged(bool value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.ToggleMute(value);
        }
    }

    private void OnInvertYChanged(bool value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.UpdateInputInvertY(value);
        }
    }

    private void OnMouseSensitivityChanged(float value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.UpdateMouseSensitivity(value);
        }
    }

    private void OnControllerChanged(bool value)
    {
        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.Settings.input.controllerEnabled = value;
        }
    }

    private void OnResolutionChanged(int index)
    {
        if (m_resolutions == null || index < 0 || index >= m_resolutions.Length) return;

        Resolution resolution = m_resolutions[index];

        if (SettingsPersistence.Instance != null)
        {
            var settings = SettingsPersistence.Instance.Settings;
            SettingsPersistence.Instance.UpdateResolution(resolution.width, resolution.height, settings.graphics.fullscreen);
        }
    }
}
