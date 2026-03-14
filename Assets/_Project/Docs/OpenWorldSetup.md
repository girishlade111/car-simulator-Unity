# OpenWorld_TestDistrict Complete Setup Guide

This guide walks through setting up a complete, playable OpenWorld_TestDistrict scene with all systems connected.

---

## Step 1: Create the Scene

1. **Create new scene**: `OpenWorld_TestDistrict.unity`
2. Save to: `Assets/_Project/Scenes/`

---

## Step 2: Basic Scene Setup

### Add Ground
1. Right-click → 3D Object → Plane
2. Name: `GroundPlane`
3. Scale: (50, 1, 50) → 500x500 units
4. Position: (0, 0, 0)
5. Create material `MAT_Ground_Grass` (green)

### Add Roads
Use SceneSetup script for automatic road creation:

1. Create empty GameObject: `SceneSetup`
2. Add `SceneSetup` component
3. Configure:
   - Ground Size: 500 x 500
   - Main Roads: 2
   - Cross Roads: 2
   - Road Width: 8
   - Building Count: 8
   - Parked Car Count: 10
4. Right-click component → "Setup Scene"

---

## Step 3: Create Player Car

### Option A: Use SceneSetup (Recommended)
1. Assign your car prefab to `SceneSetup.m_playerCarPrefab`
2. Run "Setup Scene"

### Option B: Manual
1. Create empty GameObject: `PlayerCar`
2. Tag as `Player`
3. Add `Rigidbody`:
   - Mass: 1500
4. Add `VehicleController`
5. Add 4 WheelColliders (FL, FR, RL, RR)
6. Add visual wheels (Cylinders)
7. Add `VehicleAudio` for sound

---

## Step 4: Setup Camera

1. Select **Main Camera**
2. Add `FollowCamera` component
3. In SceneConnector (next step), camera auto-connects to player

---

## Step 5: Add Scene Systems

Create an empty GameObject `[SceneSystems]` and add:

### SceneConnector
- Auto-connects all systems
- Links player car to camera
- Connects UI and audio

### GameInitializer
- Ensures player exists
- Runs initialization on start
- Set `Player Car Prefab` if using

### AutoSave
- Enable Auto Save: ✓
- Interval: 120 seconds
- Quick Save Key: F5
- Quick Load Key: F9

---

## Step 6: Add UI

### Create HUD Canvas
1. Right-click → UI → Canvas
2. Canvas Scaler: Scale With Screen Size (1920x1080)
3. Add `HUD` component
4. Create Text elements:
   - Speed Text (bottom-right)
   - Controls Text (bottom-left)

### Create Pause Menu
1. Create Panel: `PausePanel` (disable by default)
2. Add Buttons: Resume, Main Menu, Settings
3. Add `PauseMenu` component
4. Link buttons and panel

### Create Mission UI (Optional)
1. Create Panel: `MissionPanel`
2. Add Text: Title, Description, Objectives, Timer
3. Add `MissionUI` component

---

## Step 7: Add Mission Triggers

1. Create empty GameObject: `MissionTrigger_Start`
2. Add Box Collider (IsTrigger = true)
3. Add `MissionTrigger` component
4. Set Mission ID (must match MissionData)
5. Configure:
   - Trigger Type: OnEnter
   - Radius: 10

---

## Step 8: Configure MissionManager

1. Add `MissionManager` to scene (or use Bootstrap)
2. Create MissionData ScriptableObjects:
   - Right-click → Create → CarSimulator → Mission Data
3. Add missions to MissionManager array

### Sample Mission Data

**Delivery Mission:**
- Mission ID: `delivery_01`
- Mission Name: "First Delivery"
- Type: Delivery
- Objective: Reach Location
- Target Position: (100, 0, 100)
- Currency Reward: 100

---

## Step 9: Complete Hierarchy

```
OpenWorld_TestDistrict
├── Directional Light
├── GroundPlane (Plane)
├── WorldRoot
│   ├── Roads
│   ├── Buildings
│   ├── Vehicles
│   └── Environment
├── PlayerCar (with VehicleController, VehicleAudio)
├── Main Camera (with FollowCamera)
├── [SceneSystems]
│   ├── SceneConnector
│   ├── GameInitializer
│   └── AutoSave
├── [UI]
│   ├── Canvas
│   │   ├── HUD
│   │   ├── PausePanel
│   │   └── MissionPanel
└── EventSystem
```

---

## Step 10: Test

1. Press Play
2. Car should spawn at (0, 0, 0)
3. Camera follows car
4. WASD controls work
5. Speed shows in HUD
6. Escape opens pause menu

---

## Quick Checklist

- [ ] Ground plane exists
- [ ] Roads created
- [ ] Player car has Player tag
- [ ] VehicleController attached
- [ ] WheelColliders on car
- [ ] FollowCamera on Main Camera
- [ ] SceneConnector in scene
- [ ] HUD with speed text
- [ ] Pause menu works
- [ ] No console errors

---

## Common Issues

**Car won't move:**
- Check WheelColliders have radius > 0
- Check Rigidbody mass is set
- Verify InputHandler is in scene

**Camera not following:**
- Ensure player has "Player" tag
- Check FollowCamera target is set

**UI not showing:**
- Check Canvas is in correct layer
- Verify UI elements are enabled

---

## Next Steps

1. Add real 3D models (buildings, cars)
2. Create more missions
3. Add building interiors
4. Implement traffic AI
5. Add more environmental detail
