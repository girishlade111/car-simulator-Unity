# Environment & Scene Setup Guide

This guide explains how to set up the OpenWorld_TestDistrict scene using the provided scripts.

---

## Quick Setup (Recommended)

### Using SceneSetup Script

1. **Create an empty GameObject** in your scene named "SceneSetup"
2. **Add the SceneSetup component** to it
3. **Configure settings** in Inspector:
   - Ground Size: 500 x 500
   - Main Roads: 2, Cross Roads: 2
   - Road Width: 8
   - Building Count: 8
   - Parked Car Count: 10
   - Tree Count: 30, Rock Count: 20

4. **Assign Player Car Prefab** (drag your car prefab)
5. **Right-click the SceneSetup component** → "Setup Scene"

The script will automatically create:
- Ground plane
- Road network
- Buildings
- Parked cars
- Trees and rocks
- Player car spawn
- Lighting

---

## Manual Setup

### 1. Create Ground

1. Right-click → 3D Object → Plane
2. Name: `GroundPlane`
3. Scale: (50, 1, 50)
4. Position: (0, 0, 0)
5. Create Material: `MAT_Ground_Grass` (green)

### 2. Create Roads

**Main Road (North-South):**
- Create Cube
- Scale: (8, 0.1, 500)
- Position: (0, 0.05, 0)
- Material: `MAT_Road_Asphalt` (dark gray)

**Cross Road (East-West):**
- Create Cube
- Scale: (500, 0.1, 8)
- Position: (0, 0.05, 0)

### 3. Create Buildings

Use BuildingPlacer or create manually:

**Simple Apartment:**
- Body: Cube (15, 20, 15) at (0, 10, 0)
- Roof: Cube (16, 1, 16) at (0, 21, 0)
- Add Box Collider

### 4. Create Parked Cars

**Parked Car Prefab:**
- Body: Cube (2, 0.8, 4) at (0, 0.5, 0)
- Roof: Cube (1.8, 0.7, 2) at (0, 1.2, -0.3)
- 4 Wheels: Cylinders at corners

### 5. Environment

**Tree:**
- Trunk: Cylinder (0.4, 1.5, 0.4) at (0, 1.5, 0)
- Foliage: Sphere (3, 3, 3) at (0, 4, 0)

**Rock:**
- Sphere with random scale
- Position: scattered around map edges

---

## Using EnvironmentSpawner

1. Add EnvironmentSpawner to scene
2. Configure:
   - Area Size: 200 x 200
   - Tree Count: 30
   - Rock Count: 20
3. Enable "Spawn On Start"

---

## Using PlaceholderAssets

Static helper for creating placeholder objects:

```csharp
// Create a road
PlaceholderAssets.Instance.CreateRoadSegment(
    new Vector3(0, 0.05f, 0),
    new Vector3(8, 0.1f, 100f),
    0f
);

// Create a building
PlaceholderAssets.Instance.CreateApartmentBuilding(
    new Vector3(30, 0, 30)
);

// Create a parked car
PlaceholderAssets.Instance.CreateParkedCar(
    new Vector3(10, 0, 10),
    90f
);
```

---

## Scene Hierarchy Template

```
OpenWorld_TestDistrict
├── Directional Light
├── WorldRoot
│   ├── GroundPlane
│   ├── Roads
│   │   ├── Road_NS
│   │   └── Road_EW
│   ├── Buildings
│   │   ├── Building_01
│   │   ├── Building_02
│   │   └── ...
│   ├── Vehicles
│   │   ├── PlayerCar
│   │   └── ParkedCar_01...
│   └── Environment
│       ├── Tree_01...
│       └── Rock_01...
├── Main Camera (with FollowCamera)
├── EventSystem
├── GameManager
├── MissionManager
└── SaveManager
```

---

## Layers to Create

| Layer | Usage |
|-------|-------|
| Default | Ground, roads |
| Ground | Ground plane |
| Building | Buildings |
| Prop | Trees, rocks |
| Player | Player car |
| Vehicle | Parked cars |

---

## Tags to Create

| Tag | Usage |
|-----|-------|
| Player | Player car |
| Vehicle | Parked cars |
| Building | Buildings |
| Prop | Environment |

---

## Testing Checklist

- [ ] Ground plane renders correctly
- [ ] Roads are connected
- [ ] Player car can move
- [ ] Camera follows car
- [ ] Buildings don't overlap roads
- [ ] Parked cars are on road shoulders
- [ ] Trees/rocks don't block paths
- [ ] No console errors

---

## Next Steps

1. Test driving physics
2. Add more building variations
3. Implement mission triggers
4. Add UI elements
5. Test save/load
