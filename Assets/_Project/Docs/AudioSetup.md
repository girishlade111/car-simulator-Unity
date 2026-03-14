# Audio Setup Guide

This guide explains how to set up audio in your car simulator.

---

## Generating Placeholder Audio

### Option 1: Use AudioGenerator (Editor)

1. In Unity Editor, go to menu: **CarSimulator → Audio → Generate Placeholder Audio**
2. This creates basic WAV files in `Assets/_Project/Audio/`

### Option 2: Manual Audio Files

Create audio files in `Assets/_Project/Audio/`:

| Folder | Files Needed |
|--------|--------------|
| `SFX/` | EngineLoop.wav, TireScreech.wav, Collision.wav, Horn.wav, Checkpoint.wav |
| `Music/` | Menu.wav, OpenWorld.wav, Mission.wav, Garage.wav |

### Recommended Audio Settings

- **Sample Rate**: 44100 Hz
- **Channels**: Stereo for music, Mono for SFX
- **Bit Depth**: 16-bit or 24-bit

---

## Setting Up MusicManager

1. Add `MusicManager` to your scene (or use Bootstrap auto-creation)
2. In Inspector, expand **Music Tracks** array
3. Add tracks with these names:
   - `Menu` - Background music for main menu
   - `OpenWorld` - Music for driving
   - `Mission` - Music during missions
   - `Garage` - Music for garage scene

4. Assign AudioClips to each track

---

## Setting Up SFXManager

1. Add `SFXManager` to your scene
2. In Inspector, expand **SFX Library** array
3. Add entries:

| Name | Recommended Clip | Loop |
|------|------------------|------|
| Engine | EngineLoop.wav | ✓ |
| TireScreech | TireScreech.wav | ✓ |
| Collision | Collision.wav | ✗ |
| Horn | Horn.wav | ✗ |
| Checkpoint | Checkpoint.wav | ✗ |
| MissionComplete | MissionComplete.wav | ✗ |
| MenuClick | MenuClick.wav | ✗ |

---

## Setting Up VehicleAudio

1. Select your **PlayerCar** GameObject
2. Add `VehicleAudio` component
3. Assign audio clips:

| Field | Clip |
|-------|------|
| Engine Clip | EngineLoop.wav |
| Tire Screech Clip | TireScreech.wav |
| Horn Clip | Horn.wav |
| Collision Clips | Collision.wav (add multiple) |

4. Configure:
   - Engine Volume: 0.8
   - Tire Volume: 0.5
   - Min Engine Pitch: 0.5
   - Max Engine Pitch: 2.0

---

## Audio Constants

Use these constants in code:

```csharp
// SFX
SFXManager.Instance.PlaySFX(AudioConstants.SFX_ENGINE);
SFXManager.Instance.PlaySFX(AudioConstants.SFX_HORN);
SFXManager.Instance.PlaySFX(AudioConstants.SFX_COLLISION);

// Music
MusicManager.Instance.PlayTrack(AudioConstants.MUSIC_OPENWORLD);
MusicManager.Instance.SetVolume(0.5f);
```

---

## Volume Settings

Control volume through SettingsPersistence:

```csharp
SettingsPersistence.Instance.UpdateMasterVolume(0.8f);
SettingsPersistence.Instance.UpdateMusicVolume(0.5f);
SettingsPersistence.Instance.UpdateSFXVolume(1f);
```

---

## Troubleshooting

**No sound playing?**
- Check AudioListener on Main Camera
- Verify AudioSource components are enabled
- Check volume levels in SettingsPersistence

**Audio missing warnings?**
- Add AudioMissingHandler to scene
- It will log missing audio references

---

## Next Steps

1. Generate placeholder audio with AudioGenerator
2. Assign clips to MusicManager and SFXManager
3. Add VehicleAudio to player car
4. Test audio in gameplay
