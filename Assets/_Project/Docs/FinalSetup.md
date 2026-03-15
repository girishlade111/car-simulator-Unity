# Final Setup Guide - Car Simulator MVP

This guide provides complete steps to set up and build the car simulator MVP in Unity.

---

## Project Overview

**Status:** Scripts Complete, Unity Scene Setup Required
**Unity Version:** 2021.3 LTS or newer recommended
**Build Target:** Windows (PC) - Android ready architecture

---

## 1. Unity Project Setup

### 1.1 Create New Project
1. Open Unity Hub
2. Create new project: **3D (URP)** or **3D (Built-in)**
3. Project name: `CarSimulator`
4. Location: Choose appropriate folder

### 1.2 Import Scripts
1. Copy `Assets/_Project/` folder into your Unity project
2. Wait for script compilation
3. Check Console for any errors

### 1.3 Project Settings

**Player Settings (Edit → Project Settings → Player):**
- **Company Name:** YourName
- **Product Name:** CarSimulator
- **Default Canvas Width:** 1920
- **Default Canvas Height:** 1080
- **Run In Background:** Checked
- **Allow 'Unsafe' Code:** Checked (for some optimizations)

**Editor Settings:**
- Enter Play Mode Options: Enabled (faster testing)
- Asset Serialization: Force Text

---

## 2. Required Tags

Create these tags in Unity (Edit → Project Settings → Tags and Layers):

```
Player
Vehicle
Building
Prop
Ground
Water
Interactable
ParkingZone
Checkpoint
```

---

## 3. Required Layers

Create these layers:

```
Default
Player
Vehicle
Building
Prop
Ground
Water
UI
```

---

## 4. Scene Setup Order

### 4.1 Bootstrap Scene (First Scene)

1. Create new scene: `Bootstrap.unity`
2. Add empty GameObject: `[Bootstrap]`
3. Add component: `Bootstrap`
4. Configure:
   - First Scene: `MainMenu`
   - Open World Scene: `OpenWorld_TestDistrict`
   - Garage Scene: `Garage_Test`
5. Add empty GameObject: `[GameManager]`
6. Add component: `GameManager`
7. Add empty GameObject: `[SaveManager]`
8. Add component: `SaveManager`
9. Add empty GameObject: `[SettingsPersistence]`
10. Add component: `SettingsPersistence`
11. **File → Build Settings → Add Open Scenes**

### 4.2 MainMenu Scene

1. Create new scene: `MainMenu.unity`
2. Create Canvas (Scale Mode: Scale With Screen Size)
3. Add UI elements:
   - Title Text: "CAR SIMULATOR"
   - New Game Button → OnClick → Bootstrap.StartNewGame()
   - Continue Button → OnClick → Bootstrap.ContinueGame()
   - Settings Button → OnClick → Open Settings Panel
   - Quit Button → OnClick → Application.Quit()
4. Add empty GameObject: `[MainMenu]`
5. Add component: `MainMenu`
6. Link UI references in Inspector
7. **File → Build Settings → Add Open Scenes**

### 4.3 OpenWorld_TestDistrict Scene

1. Create new scene: `OpenWorld_TestDistrict.unity`
2. Add empty GameObject: `[WorldRoot]`
3. Add component: `DistrictManager`
4. Add empty GameObject: `[SpawnManager]`
5. Add component: `SpawnManager`
6. Configure SpawnManager:
   - Player Vehicle Prefab: (create or assign car prefab)
   - Player Spawn Point: (create empty transform at spawn location)
7. Add empty GameObject: `[TrafficManager]`
8. Add component: `TrafficManager`
9. Add empty GameObject: `[PedestrianManager]`
10. Add component: `PedestrianManager`
11. Add empty GameObject: `[WeatherSystem]`
12. Add component: `WeatherSystem`
13. Add empty GameObject: `[TimeOfDay]`
14. Add component: `EnhancedTimeOfDay`
15. Add empty GameObject: `[QuestManager]`
16. Add component: `QuestManager`
17. **File → Build Settings → Add Open Scenes**

### 4.4 Garage_Test Scene

1. Create new scene: `Garage_Test.unity`
2. Create floor plane
3. Add example vehicle prefabs
4. Add Camera with GarageCamera script
5. Add UI: Vehicle selection buttons
6. **File → Build Settings → Add Open Scenes**

---

## 5. Prefab Requirements

### 5.1 Player Vehicle Prefab

Create prefab following this structure:

```
PF_PlayerCar (Empty Root)
├── Body (Mesh)
│   ├── Materials: MAT_Car_Paint, etc.
├── Wheels (4x)
│   ├── WheelCollider
│   ├── WheelMesh
├── CameraMount (Empty)
├── AudioMount (Empty)
└── VehicleController (Script)
    ├── VehicleInput
    ├── VehiclePhysics
    ├── FollowCamera
```

Required Components:
- Rigidbody (Mass: 1400)
- VehicleController script
- VehicleInput script
- VehiclePhysics script
- 4x WheelCollider

### 5.2 Building Prefabs

Create apartment/commercial building prefabs:
- Mesh collider or box collider
- ApartmentEntrance script on entry points
- LODGroup (optional, for mobile)

### 5.3 Props

Create prop prefabs:
- PF_Tree_01, PF_Tree_02
- PF_Rock_01, PF_Rock_02
- PF_Bench, PF_LampPost
- PF_ParkedCar_Sedan, PF_ParkedCar_SUV

---

## 6. Input Setup

### 6.1 Input Manager

Edit → Project Settings → Input Manager:

| Axis | Type | Positive Button |
|------|------|-----------------|
| Horizontal | Key or Mouse | a, d, left, right |
| Vertical | Key or Mouse | w, s, up, down |

### 6.2 Default Controls

| Action | Key |
|--------|-----|
| Steer | A/D or Left/Right |
| Accelerate | W or Up |
| Brake/Reverse | S or Down |
| Handbrake | Left Shift |
| Reset Vehicle | R |
| Interact | E |
| Pause | Escape |

---

## 7. Build Preparation

### 7.1 Check Scene Order

In Build Settings:
1. Bootstrap (index 0)
2. MainMenu
3. OpenWorld_TestDistrict
4. Garage_Test

### 7.2 Player Settings

- **Platform:** Windows/Mac
- **Color Space:** Linear (recommended)
- **Auto Graphics API:** Uncheck, use WebGL 2.0 or Direct3D 11

### 7.3 Quality Settings

Create medium quality preset:
- Shadow Distance: 50
- Shadow Cascades: 2
- Anti-Aliasing: 2x or 4x

### 7.4 Build

1. File → Build Settings
2. Select target platform
3. Click Build
4. Choose output folder

---

## 8. Testing Checklist

Before first play:

- [ ] All scripts compile without errors
- [ ] Player car spawns at spawn point
- [ ] Vehicle controls respond (WASD)
- [ ] Camera follows vehicle
- [ ] Can drive around test district
- [ ] UI elements visible (speed, etc.)
- [ ] Pause menu works (Escape)
- [ ] Save/Load functions work
- [ ] No console errors on startup

---

## 9. Troubleshooting

### "Player not found" error
- Ensure Player Vehicle has tag "Player"
- Check SpawnManager has prefab assigned

### "Scene not found" error
- Verify scene is in Build Settings
- Check Bootstrap scene names match

### Vehicle falls through ground
- Check WheelCollider radius
- Ensure Ground has collider

### No UI visible
- Check Canvas Scaler settings
- Verify UI elements have RectTransform anchors

---

## 10. Quick Start Script

Use SceneSetup.cs for automatic scene generation:

1. Create empty GameObject: `[SceneSetup]`
2. Add SceneSetup component
3. Assign Player Vehicle Prefab
4. Right-click → "Setup Scene"

---

## Documentation Index

- **CLAUDE.md** - Project conventions and rules
- **GamePlan.md** - Original feature planning
- **ArchitectureSummary.md** - System architecture
- **OptimizationChecklist.md** - Performance tips
- **TestChecklist.md** - QA testing procedures
- **NextFeatures.md** - Future development guide
