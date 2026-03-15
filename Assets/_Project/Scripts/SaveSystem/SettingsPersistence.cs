using System;
using System.IO;
using UnityEngine;
using CarSimulator.Utils;

public class SettingsPersistence : MonoBehaviour
{
    public static SettingsPersistence Instance { get; private set; }

    private const string SETTINGS_FILE = "settings.json";

    [SerializeField] private GameSettings m_settings;

    public GameSettings Settings => m_settings;

    private string SettingsPath => Path.Combine(Application.persistentDataPath, SETTINGS_FILE);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        m_settings = new GameSettings();
        LoadSettings();
    }

    public void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                GameSettings loaded = JsonUtility.FromJson<GameSettings>(json);

                if (loaded != null)
                {
                    m_settings = loaded;
                    Debug.Log("[SettingsPersistence] Settings loaded");
                }
            }
            else
            {
                Debug.Log("[SettingsPersistence] No settings file found, using defaults");
                SaveSettings();
            }
        }
        catch (Exception e)
        {
            ErrorHandler.LogError("SettingsPersistence.LoadSettings", e);
            m_settings = new GameSettings();
        }

        ApplySettings();
    }

    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(m_settings, true);
            File.WriteAllText(SettingsPath, json);
            Debug.Log("[SettingsPersistence] Settings saved");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SettingsPersistence] Failed to save settings: {e.Message}");
        }
    }

    public void ApplySettings()
    {
        ApplyGraphicsSettings();
        ApplyAudioSettings();
        ApplyInputSettings();
    }

    private void ApplyGraphicsSettings()
    {
        if (m_settings.graphics == null) return;

        QualitySettings.SetQualityLevel(m_settings.graphics.qualityLevel);
        QualitySettings.vSyncCount = m_settings.graphics.vsync ? 1 : 0;

        Screen.SetResolution(
            m_settings.graphics.resolutionWidth,
            m_settings.graphics.resolutionHeight,
            m_settings.graphics.fullscreen
        );
    }

    private void ApplyAudioSettings()
    {
        if (m_settings.audio == null) return;

        AudioListener.volume = m_settings.audio.muteAudio ? 0 : m_settings.audio.masterVolume;
    }

    private void ApplyInputSettings()
    {
        // Input settings applied when needed
    }

    public void UpdateGraphicsQuality(int level)
    {
        m_settings.graphics.qualityLevel = level;
        QualitySettings.SetQualityLevel(level);
    }

    public void UpdateResolution(int width, int height, bool fullscreen)
    {
        m_settings.graphics.resolutionWidth = width;
        m_settings.graphics.resolutionHeight = height;
        m_settings.graphics.fullscreen = fullscreen;
        Screen.SetResolution(width, height, fullscreen);
    }

    public void UpdateMasterVolume(float volume)
    {
        m_settings.audio.masterVolume = Mathf.Clamp01(volume);
        if (!m_settings.audio.muteAudio)
        {
            AudioListener.volume = m_settings.audio.masterVolume;
        }
    }

    public void UpdateMusicVolume(float volume)
    {
        m_settings.audio.musicVolume = Mathf.Clamp01(volume);
    }

    public void UpdateSFXVolume(float volume)
    {
        m_settings.audio.sfxVolume = Mathf.Clamp01(volume);
    }

    public void ToggleMute(bool mute)
    {
        m_settings.audio.muteAudio = mute;
        AudioListener.volume = mute ? 0 : m_settings.audio.masterVolume;
    }

    public void UpdateInputInvertY(bool invert)
    {
        m_settings.input.invertY = invert;
    }

    public void UpdateMouseSensitivity(float sensitivity)
    {
        m_settings.input.mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
    }

    public void ResetToDefaults()
    {
        m_settings = new GameSettings();
        ApplySettings();
        SaveSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}
