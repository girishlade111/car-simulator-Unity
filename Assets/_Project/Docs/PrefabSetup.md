# Player Car Prefab Setup Guide

This guide explains how to create a player car prefab using placeholder primitives.

## Creating the Car Prefab

### 1. Create the Car Root
1. Create an empty GameObject named `PF_PlayerCar`
2. Add `Rigidbody` component:
   - Mass: 1500
   - Drag: 0
   - Angular Drag: 0.05
3. Add `VehicleController` script
4. Set Layer to `Player`
5. Tag as `Player`

### 2. Add Car Body (Placeholder)
1. Create a Cube as child of the car root
2. Name: `Body`
3. Scale: (2, 0.5, 4)
4. Position: (0, 0.5, 0)
5. Create Materials/MAT_Car_Body and assign

### 3. Add Wheel Colliders

#### Front Left Wheel
1. Create empty GameObject child named `Wheel_FL`
2. Position: (-1, 0.3, 1.5)
3. Add `WheelCollider` component:
   - Radius: 0.35
   - Suspension Distance: 0.3
   - Spring: 30
   - Damper: 4
   - Forward Friction: 1
   - Side Friction: 1
4. Add `WheelVisuals` script

#### Front Right Wheel
1. Duplicate `Wheel_FL` → `Wheel_FR`
2. Position: (1, 0.3, 1.5)

#### Rear Left Wheel
1. Duplicate `Wheel_FL` → `Wheel_RL`
2. Position: (-1, 0.3, -1.5)
3. Remove from front wheel array in VehicleController

#### Rear Right Wheel
1. Duplicate `Wheel_RL` → `Wheel_RR`
2. Position: (1, 0.3, -1.5)

### 4. Add Wheel Meshes (Visual)

For each wheel (Wheel_FL, Wheel_FR, Wheel_RL, Wheel_RR):
1. Create a Cylinder as child
2. Scale: (0.7, 0.1, 0.7)
3. Rotation: (90, 0, 0)
4. Assign to `WheelVisuals.m_wheelMesh`

### 5. Configure VehicleController

In Inspector for `PF_PlayerCar`:
- **Front Wheels**: Assign Wheel_FL, Wheel_FR
- **Rear Wheels**: Assign Wheel_RL, Wheel_RR
- **Center of Mass**: Create empty child at (0, -0.2, 0), assign

### 6. Add Camera Mount
1. Create empty child named `CameraMount`
2. Position: (0, 2, -5)

---

## Tuning Values (Inspector)

| Parameter | Default | Description |
|-----------|---------|-------------|
| Max Steer Angle | 35 | Steering limit in degrees |
| Max Engine Force | 1500 | Acceleration power |
| Max Brake Force | 3000 | Braking power |
| Downforce | 10 | High-speed stability |
| Anti-Roll Force | 500 | Roll prevention |

---

## Controls

| Key | Action |
|-----|--------|
| W / ↑ | Accelerate |
| S / ↓ | Brake / Reverse |
| A / ← | Steer Left |
| D / → | Steer Right |
| Space | Brake |
| Shift | Handbrake |
| R | Respawn |
| Escape | Pause |
| H | Toggle Controls |

---

## Creating as Prefab

1. Drag `PF_PlayerCar` into Assets/_Project/Prefabs/Vehicles/
2. Delete from scene
3. Use prefab in OpenWorld_TestDistrict scene
