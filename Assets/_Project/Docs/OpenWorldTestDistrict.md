# OpenWorld Test District Setup Guide

## Quick Setup

### 1. Create Scene
1. Create new scene: `OpenWorld_TestDistrict.unity`
2. Save to: `Assets/_Project/Scenes/`

### 2. Add Builder
1. Create empty GameObject: `WorldBuilder`
2. Add `OpenWorldBuilder` component

### 3. Assign Player Car
1. Drag your PlayerCar prefab to `Player Car Prefab` field

### 4. Build World
1. Right-click `OpenWorldBuilder` component → **Build World**

---

## Hierarchy Created

```
OpenWorld_TestDistrict
├── WorldRoot
│   ├── Ground
│   ├── Roads
│   │   └── Road (x4)
│   ├── Buildings
│   │   └── Building (x8)
│   ├── ParkedVehicles
│   │   └── ParkedCar (x10)
│   ├── Environment
│   │   ├── Tree (x30)
│   │   └── Rock (x20)
│   └── PlayerCar
├── Directional Light
└── Main Camera (+ FollowCamera)
```

---

## Settings

| Setting | Default | Description |
|---------|---------|-------------|
| World Size | 500x500 | Total area |
| Main Roads | 2 | North-south roads |
| Cross Roads | 2 | East-west roads |
| Road Width | 8 | Road thickness |
| Buildings | 8 | Apartment count |
| Parked Cars | 10 | Static vehicles |
| Trees | 30 | Environment |
| Rocks | 20 | Environment |

---

## Required Materials

Create in `Assets/_Project/Materials/`:

| Material | Color | Purpose |
|----------|-------|---------|
| MAT_Ground | Green | Ground plane |
| MAT_Road | Dark Gray | Roads |
| MAT_Building | Gray | Buildings |
| MAT_Roof | Dark Gray | Building tops |

---

## Player Car Requirements

Your PlayerCar prefab must have:
- Tag: `Player`
- Rigidbody component
- VehiclePhysics component
- 4 WheelColliders

---

## Testing

1. Press Play
2. Car spawns at (0, 0, 0)
3. Drive with WASD
4. Camera follows
5. Speed displays (if SpeedDisplay added)

---

## Adding Speed Display

1. Create Canvas
2. Add Text (bottom-right)
3. Add `SpeedDisplay` script
4. Link Text and PlayerCar
