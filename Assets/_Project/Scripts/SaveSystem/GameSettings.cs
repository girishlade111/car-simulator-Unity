using System;

[Serializable]
public class GameSettings
{
    public GraphicsSettings graphics;
    public AudioSettings audio;
    public InputSettings input;

    public GameSettings()
    {
        graphics = new GraphicsSettings();
        audio = new AudioSettings();
        input = new InputSettings();
    }
}

[Serializable]
public class GraphicsSettings
{
    public int qualityLevel = 2;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public bool fullscreen = true;
    public bool vsync = true;
    public float viewDistance = 500f;
    public bool shadows = true;
    public float shadowDistance = 50f;
    public int antiAliasing = 2;

    public GraphicsSettings() { }
}

[Serializable]
public class AudioSettings
{
    public float masterVolume = 1f;
    public float musicVolume = 0.8f;
    public float sfxVolume = 1f;
    public float dialogueVolume = 1f;
    public bool muteAudio = false;

    public AudioSettings() { }
}

[Serializable]
public class InputSettings
{
    public bool invertY = false;
    public float mouseSensitivity = 1f;
    public bool controllerEnabled = false;
    public float controllerSensitivity = 1f;

    public InputSettings() { }
}
