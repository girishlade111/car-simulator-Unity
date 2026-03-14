# Audio System Setup Guide

## Music Manager

### Setup
1. Add `MusicManager` to scene (auto-created by Bootstrap)
2. Add Audio Source component
3. Add tracks to Tracks array

### Creating Tracks
1. Create audio clips in `Assets/_Project/Audio/Music/`
2. In MusicManager Inspector:
   - Click + to add track
   - Set name (e.g., "Menu", "Driving")
   - Assign AudioClip

### Usage
```csharp
MusicManager.Instance.PlayTrack("Menu");
MusicManager.Instance.SetVolume(0.5f);
MusicManager.Instance.Pause();
```

---

## SFX Manager

### Setup
1. Add `SFXManager` to scene
2. Add SFX Library entries

### Creating SFX
1. Add audio clips to `Assets/_Project/Audio/SFX/`
2. Add entries to SFX Library:
   - Name: "Engine", "Horn", "Tire", etc.
   - Clip: Drag audio file

### Usage
```csharp
SFXManager.Instance.PlaySFX("Horn");
SFXManager.Instance.PlaySFX("Tire", transform.position);
```

---

## Vehicle Audio

### Setup
1. Select PlayerCar
2. Add `VehicleAudio` component
3. Assign audio clips:
   - Engine Clip: Looping engine sound
   - Tire Clip: Looping tire screech
   - Horn Clip: Single honk sound
4. Component auto-links to VehiclePhysics

### Parameters
- Min Engine Pitch: 0.5 (idle)
- Max Engine Pitch: 2.0 (top speed)
- Engine Volume: 0.8
- Tire Volume: 0.5

### Controls
- H: Horn
- Shift: Tire screech

---

## Audio Files

Create folder: `Assets/_Project/Audio/`

### Recommended Structure
```
Audio/
├── Music/
│   ├── Menu.wav
│   └── Driving.wav
└── SFX/
    ├── Engine.wav
    ├── Horn.wav
    └── Tire.wav
```

---

## Settings Integration

Audio settings connect to SettingsPersistence:
```csharp
// In SettingsController
MusicManager.Instance.SetVolume(settings.audio.musicVolume);
SFXManager.Instance.SetVolume(settings.audio.sfxVolume);
```
