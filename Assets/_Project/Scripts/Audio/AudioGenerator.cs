using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("CarSimulator/Audio/Generate Placeholder Audio")]
    public static void GeneratePlaceholderAudio()
    {
        string folderPath = "Assets/_Project/Audio";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/_Project", "Audio");
        }

        CreateToneAudioClip(folderPath + "/EngineLoop.wav", 440f, 2f, true);
        CreateToneAudioClip(folderPath + "/TireScreech.wav", 800f, 1f, false);
        CreateToneAudioClip(folderPath + "/Collision.wav", 200f, 0.3f, false);
        CreateToneAudioClip(folderPath + "/Horn.wav", 520f, 0.5f, false);
        CreateToneAudioClip(folderPath + "/Checkpoint.wav", 880f, 0.2f, false);
        CreateToneAudioClip(folderPath + "/MissionComplete.wav", 660f, 1f, false);
        CreateToneAudioClip(folderPath + "/MenuClick.wav", 1000f, 0.1f, false);

        Debug.Log("[AudioGenerator] Placeholder audio clips generated in " + folderPath);
    }

    private static void CreateToneAudioClip(string path, float frequency, float duration, bool loop)
    {
        int sampleRate = 44100;
        int sampleLength = (int)(sampleRate * duration);
        if (loop) sampleLength = sampleRate * 2;

        float[] samples = new float[sampleLength];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / sampleRate;
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * 0.5f;
            if (loop && i > sampleRate)
            {
                float fade = 1f - (float)(i - sampleRate) / sampleRate;
                samples[i] *= fade;
            }
        }

        AudioClip clip = AudioClip.Create(System.IO.Path.GetFileNameWithoutExtension(path), 
            sampleLength, 1, sampleRate, false);
        clip.SetData(samples, 0);

        string folder = System.IO.Path.GetDirectoryName(path);
        if (!System.IO.Directory.Exists(folder))
        {
            System.IO.Directory.CreateDirectory(folder);
        }

        AssetDatabase.CreateAsset(clip, path);
    }
#endif
}

public class AudioMissingHandler : MonoBehaviour
{
    [Header("Fallback Settings")]
    [SerializeField] private bool m_logMissingAudio = true;

    private void Start()
    {
        CheckAudioSetup();
    }

    private void CheckAudioSetup()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("[AudioMissingHandler] MusicManager not found in scene!");
        }

        if (SFXManager.Instance == null)
        {
            Debug.LogWarning("[AudioMissingHandler] SFXManager not found in scene!");
        }

        if (VehicleAudio == null)
        {
            var vehicleAudio = FindObjectOfType<VehicleAudio>();
            if (vehicleAudio == null)
            {
                Debug.LogWarning("[AudioMissingHandler] VehicleAudio not found on player car!");
            }
        }
    }

    public void PlaySFXSafe(string sfxName)
    {
        if (SFXManager.Instance != null && SFXManager.Instance.HasSFX(sfxName))
        {
            SFXManager.Instance.PlaySFX(sfxName);
        }
        else if (m_logMissingAudio)
        {
            Debug.Log($"[AudioMissingHandler] SFX '{sfxName}' not found. Add to SFXManager library.");
        }
    }

    public void PlayMusicSafe(string trackName)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayTrack(trackName);
        }
    }

    private VehicleAudio VehicleAudio
    {
        get
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            return player != null ? player.GetComponent<VehicleAudio>() : null;
        }
    }
}

public static class AudioConstants
{
    public const string SFX_ENGINE = "Engine";
    public const string SFX_TIRE_SCREECH = "TireScreech";
    public const string SFX_COLLISION = "Collision";
    public const string SFX_HORN = "Horn";
    public const string SFX_CHECKPOINT = "Checkpoint";
    public const string SFX_MISSION_COMPLETE = "MissionComplete";
    public const string SFX_MENU_CLICK = "MenuClick";
    public const string SFX_UI_HOVER = "UIHover";
    public const string SFX_CAR_HONK = "CarHonk";
    public const string SFX_TIRE_BRAKE = "TireBrake";

    public const string MUSIC_MENU = "Menu";
    public const string MUSIC_OPENWORLD = "OpenWorld";
    public const string MUSIC_MISSION = "Mission";
    public const string MUSIC_GARAGE = "Garage";
}
