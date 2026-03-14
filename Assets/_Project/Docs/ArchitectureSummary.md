# Car Simulator - Complete Architecture Summary

## Project Overview

An open-world car simulator with modular architecture for future expansion into missions, traffic, interiors, and optimization for PC and Android.

---

## Module Architecture

### Core (`Scripts/Core/`)
| Script | Purpose |
|--------|---------|
| `GameConstants.cs` | Tags, layers, scene names |
| `GameManager.cs` | State management (Menu/Playing/Paused) |
| `SceneNavigator.cs` | Static scene navigation |

### Bootstrap (`Scripts/Bootstrap/`)
| Script | Purpose |
|--------|---------|
| `Bootstrap.cs` | Initializes managers, loads first scene |

### Runtime (`Scripts/Runtime/`)
| Script | Purpose |
|--------|---------|
| `SceneLoader.cs` | Async scene loading |
| `ServiceLocator.cs` | Service registry pattern |

### Utils (`Scripts/Utils/`)
| Script | Purpose |
|--------|---------|
| `DebugLogger.cs` | Conditional logging |

### Config (`Scripts/Config/`)
| Script | Purpose |
|--------|---------|
| `GameConfig.cs` | ScriptableObject for game settings |

---

### Vehicle (`Scripts/Vehicle/`)
| Script | Purpose |
|--------|---------|
| `VehicleInput.cs` | Keyboard input (WASD, Space, Shift, R) |
| `VehiclePhysics.cs` | Arcade-sim physics with WheelColliders |
| `VehicleController.cs` | Reset/respawn logic |
| `VehicleTuning.cs` | ScriptableObject for tuning |
| `WheelVisual.cs` | Syncs wheel mesh to collider |

### Camera (`Scripts/Camera/`)
| Script | Purpose |
|--------|---------|
| `FollowCamera.cs` | Smooth follow with collision |

### World (`Scripts/World/`)
| Script | Purpose |
|--------|---------|
| `DistrictManager.cs` | Chunk-based world management |
| `PlaceholderFactory.cs` | Creates primitive placeholders |
| `OpenWorldBuilder.cs` | One-click world generation |

### UI (`Scripts/UI/`)
| Script | Purpose |
|--------|---------|
| `MainMenuController.cs` | Main menu navigation |
| `SettingsController.cs` | Audio/graphics/controls settings |
| `SaveLoadController.cs` | Save/load game slots |
| `PauseMenuController.cs` | In-game pause menu |
| `SpeedDisplay.cs` | On-screen speed text |
| `MissionUI.cs` | Mission display |

### Missions (`Scripts/Missions/`)
| Script | Purpose |
|--------|---------|
| `MissionData.cs` | ScriptableObject for missions |
| `MissionManager.cs` | Mission tracking, objectives |
| `MissionTrigger.cs` | Mission activation |

### Audio (`Scripts/Audio/`)
| Script | Purpose |
|--------|---------|
| `MusicManager.cs` | Background music |
| `SFXManager.cs` | Pooled sound effects |
| `VehicleAudio.cs` | Engine, tire, horn sounds |

### Optimization (`Scripts/Optimization/`)
| Script | Purpose |
|--------|---------|
| `SimpleLOD.cs` | Distance-based detail switching |
| `ObjectPool.cs` | Pooled object management |
| `MobileHelper.cs` | Mobile detection and optimization |

### SaveSystem (`Scripts/SaveSystem/`)
| Script | Purpose |
|--------|---------|
| `SaveManager.cs` | Save/load game data |
| `SettingsPersistence.cs` | Settings save/load |
| `GameSaveData.cs` | Serializable save structures |

---

## Scene Structure

### Bootstrap Scene
```
Bootstrap
└── Bootstrap (Bootstrap.cs)
    - First Scene: MainMenu
```

### MainMenu Scene
```
MainMenu
├── Canvas
│   ├── MainMenuController
│   ├── Settings Panel
│   └── Load Game Panel
```

### OpenWorld_TestDistrict Scene
```
OpenWorld
├── WorldRoot
│   ├── Ground
│   ├── Roads
│   ├── Buildings
│   ├── ParkedVehicles
│   └── Environment
├── PlayerCar
│   ├── VehicleInput
│   ├── VehiclePhysics
│   ├── VehicleController
│   └── VehicleAudio
├── Main Camera + FollowCamera
├── MissionManager
├── MusicManager
└── SFXManager
```

---

## Game Flow

```
Bootstrap Start
    ↓
Initialize Managers (GameManager, DebugLogger)
    ↓
Load MainMenu
    ↓
[New Game] → Load OpenWorld → Play → AutoSave
[Load Game] → Load SaveData → OpenWorld
[Settings] → Graphics/Audio/Controls
[Quit] → Exit
    ↓
In Game: Escape → Pause Menu
```

---

## Folder Structure

```
Assets/_Project/
├── Scripts/
│   ├── Audio/
│   ├── Bootstrap/
│   ├── Camera/
│   ├── Config/
│   ├── Core/
│   ├── Missions/
│   ├── Optimization/
│   ├── SaveSystem/
│   ├── UI/
│   ├── Utils/
│   ├── Vehicle/
│   └── World/
├── Scenes/
│   ├── Bootstrap.unity
│   ├── MainMenu.unity
│   ├── OpenWorld_TestDistrict.unity
│   └── Garage_Test.unity
└── Docs/
```

---

## Roadmap

### Phase 1: Foundation (Complete)
- [x] Bootstrap and scene loading
- [x] Basic vehicle controller
- [x] Follow camera
- [x] Open world generation
- [x] Main menu with settings
- [x] Save/load system
- [x] Mission system
- [x] Audio system
- [x] Optimization basics

### Phase 2: Content Expansion
- [ ] Traffic AI system
- [ ] Pedestrian NPCs
- [ ] Building interiors
- [ ] More vehicle types
- [ ] Vehicle customization

### Phase 3: Polish
- [ ] Day/night cycle
- [ ] Weather effects
- [ ] Visual effects
- [ ] UI polish
- [ ] Sound polish

### Phase 4: Optimization
- [ ] LOD system
- [ ] Object pooling
- [ ] Mobile controls
- [ ] Android build
- [ ] Level streaming

---

## Next Steps

1. **Create Unity Project**: Open in Unity Hub
2. **Setup Scenes**: Bootstrap, MainMenu, OpenWorld_TestDistrict
3. **Build Player Car**: Follow VehicleSetup.md
4. **Setup World**: Use OpenWorldBuilder
5. **Test Gameplay**: Drive, pause, save

---

## Key Conventions

- **Namespaces**: `CarSimulator.{Module}`
- **Scripts**: PascalCase (e.g., `VehicleController.cs`)
- **Variables**: camelCase with `m_` prefix for private
- **Constants**: PascalCase (e.g., `GameConstants`)
- **Tags**: Defined in `GameConstants.cs`
- **Layers**: Defined in `GameConstants.cs`
