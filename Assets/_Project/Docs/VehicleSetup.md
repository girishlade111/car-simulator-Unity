# Vehicle Setup Guide

## Creating the Car Prefab

### 1. Create Vehicle Root
1. Create empty GameObject named `PlayerCar`
2. Tag as `Player`
3. Add `Rigidbody`:
   - Mass: 1500
   - Drag: 0
   - Angular Drag: 0.05

### 2. Add Vehicle Components
Add in this order:
1. `VehicleInput` - Keyboard input
2. `VehiclePhysics` - Wheel physics
3. `VehicleController` - Reset/respawn logic

### 3. Create Tuning
1. Right-click → Create → CarSimulator → Vehicle Tuning
2. Name: `Tuning_Default`
3. Assign to VehiclePhysics component

### 4. Add WheelColliders

**Front Left:**
- Position: (-1, 0.3, 1.5)
- Radius: 0.35
- Suspension Distance: 0.3

**Front Right:**
- Position: (1, 0.3, 1.5)
- Same settings

**Rear Left:**
- Position: (-1, 0.3, -1.5)

**Rear Right:**
- Position: (1, 0.3, -1.5)

### 5. Assign Wheel Arrays
In VehiclePhysics Inspector:
- Front Wheels: FL, FR
- Rear Wheels: RL, RR

### 6. Center of Mass
1. Create empty child named `CenterOfMass`
2. Position: (0, -0.3, 0)
3. Assign to VehiclePhysics

### 7. Wheel Visuals
For each wheel:
1. Create Cylinder child
2. Scale: (0.7, 0.1, 0.7)
3. Rotation: (90, 0, 0)
4. Add `WheelVisual` script
5. Link WheelCollider

---

## Tuning Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| Max Steer Angle | 35 | Steering lock |
| Engine Force | 1500 | Acceleration |
| Max Speed | 150 | Speed limit |
| Brake Force | 3000 | Braking power |
| Brake Balance | 0.6 | Front/rear bias |
| Downforce | 10 | High-speed grip |
| Anti-Roll | 500 | Roll stability |

---

## Controls

| Key | Action |
|-----|--------|
| W / ↑ | Accelerate |
| S / ↓ | Brake/Reverse |
| A / ← | Steer Left |
| D / → | Steer Right |
| Space | Brake |
| Shift | Handbrake |
| R | Reset vehicle |
| Escape | Pause |

---

## Inspector Assignments

```
PlayerCar (GameObject)
├── Rigidbody (mass: 1500)
├── VehicleInput
├── VehiclePhysics
│   ├── Tuning: Tuning_Default
│   ├── Input: VehicleInput (drag in)
│   ├── CenterOfMass: CenterOfMass
│   ├── Front Wheels: [WheelCollider, WheelCollider]
│   └── Rear Wheels: [WheelCollider, WheelCollider]
├── VehicleController
│   ├── Input: VehicleInput
│   └── Physics: VehiclePhysics
└── Wheels (visuals)
    ├── Wheel_FL + WheelVisual
    ├── Wheel_FR + WheelVisual
    ├── Wheel_RL + WheelVisual
    └── Wheel_RR + WheelVisual
```

---

## Follow Camera Setup

1. Select Main Camera
2. Add `FollowCamera` component
3. Target: Drag PlayerCar
4. Offset: (0, 4, -8)
5. Follow Speed: 5
6. Rotation Speed: 3

---

## Speed Display Setup

1. Create Canvas
2. Add Text element (bottom-right)
3. Add `SpeedDisplay` script
4. Speed Text: Drag Text element

---

## Quick Test Checklist

- [ ] Car accelerates with W
- [ ] Car steers with A/D
- [ ] Car brakes with Space
- [ ] Speed displays correctly
- [ ] Camera follows smoothly
- [ ] R resets car
- [ ] Car respawns if falls
