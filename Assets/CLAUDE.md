# CLAUDE.md - Car Simulator Project Guide

## Project Vision

An open-world car simulator where players drive freely through a large area featuring roads, apartment buildings, parked vehicles, and environmental details. The architecture prioritizes modularity for future expansion into missions, traffic systems, building interiors, and optimization for both PC and Android platforms.

## Core Principles

- **Readability over cleverness** - Code should be easily understood by any developer
- **Stable gameplay over complexity** - Focus on working systems first
- **Modular architecture** - Small, composable systems over giant managers
- **Mobile-aware** - Design decisions should consider future Android port
- **Placeholder-friendly** - Missing assets get simple placeholders with TODO comments

---

## Coding Rules

### General Style
- Use PascalCase for class names and file names (`VehicleController.cs`)
- Use camelCase for local variables and method parameters
- Use `m_` prefix for private class members (Unity convention)
- One responsibility per class - keep MonoBehaviours small
- Prefer composition over giant manager classes

### Inspector Exposure
```csharp
[SerializeField] private float m_speed;
[SerializeField] private Transform m_wheelMesh;
[SerializeField] private WheelCollider m_wheelCollider;
```

### Constants and Config
- Avoid magic numbers; use `private const float MAX_SPEED = 150f;`
- Expose tuning values in Inspector for gameplay tweaking
- Use ScriptableObjects for shared configuration

### Update/FixedUpdate Pattern
```csharp
private void Update()
{
    GatherInput(); // Input in Update
}

private void FixedUpdate()
{
    ApplyPhysics(); // Physics in FixedUpdate
}
```

### Comments
- Add comments only when they help future editing
- Avoid obvious or redundant comments
- Use TODO comments for incomplete work: `// TODO: Implement interior loading`

---

## Folder Structure

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/           # Game lifecycle, managers, utilities
│   │   ├── Vehicle/        # Car physics, controls, tuning
│   │   ├── Camera/         # Camera systems, follow, cinematic
│   │   ├── World/          # District management, terrain, roads
│   │   ├── Buildings/      # Building spawners, modular buildings
│   │   ├── Props/          # Street props, scatter systems
│   │   ├── UI/             # Menus, HUDs, screens
│   │   ├── Missions/       # Mission system, objectives, triggers
│   │   ├── SaveSystem/     # Save/load, persistence
│   │   ├── Audio/          # Sound managers, music, SFX
│   │   └── Optimization/   # LOD, culling, pooling, mobile helpers
│   │
│   ├── Scenes/
│   │   ├── Bootstrap.unity
│   │   ├── MainMenu.unity
│   │   ├── OpenWorld_TestDistrict.unity
│   │   └── Garage_Test.unity
│   │
│   ├── Prefabs/
│   │   ├── Vehicles/       # Player cars, parked cars
│   │   ├── Buildings/      # Apartment blocks, commercial
│   │   ├── Props/          # Street furniture, debris
│   │   ├── Environment/    # Trees, rocks, nature
│   │   └── UI/             # Menu canvases, HUD prefabs
│   │
│   ├── Materials/
│   │   ├── Vehicles/
│   │   ├── Buildings/
│   │   ├── Roads/
│   │   ├── Environment/
│   │   └── Props/
│   │
│   ├── ScriptableObjects/
│   │   ├── VehicleTuning/
│   │   ├── MissionData/
│   │   └── GameSettings/
│   │
│   ├── Docs/
│   │   ├── GamePlan.md
│   │   └── Architecture.md
│   │
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   │
│   ├── UI/
│   │   └── Fonts/
│   │
│   └── Settings/
│       └── ProjectSettings.asset
│
├── Gizmos/
│   └── Custom editor icons
│
└── Resources/
    └── Runtime-loaded assets
```

---

## Naming Conventions

### Scripts
- PascalCase: `VehicleController.cs`, `FollowCamera.cs`, `DistrictSpawnManager.cs`
- One class per file

### GameObjects
- Descriptive names: `PlayerCar`, `DistrictRoot`, `ApartmentBlock_A`
- Use prefixes for organization: `[Zone]`, `[Container]`, `[SpawnPoint]`

### Prefabs
- Prefix with `PF_`: `PF_PlayerCar`, `PF_ApartmentBlock_A`, `PF_ParkedCar_Sedan_01`
- Use suffixes for variants: `_LowPoly`, `_HighDetail`, `_Damaged`

### ScriptableObjects
- Prefix with `SO_`: `SO_VehicleTuning_Default`, `SO_Mission_01`
- Group related data in folders matching script folders

### Materials
- Prefix with `MAT_`: `MAT_Road_Asphalt`, `MAT_Building_Concrete`, `MAT_Car_Paint_Red`
- Use descriptive suffixes: `_Smooth`, `_Rough`, `_Weathered`

### Tags and Layers
- Create as needed in Unity (e.g., `Player`, `Vehicle`, `Building`, `Prop`, `Ground`)
- Document required tags/layers in setup notes

---

## Scene Strategy

### Initial Scenes

1. **Bootstrap** - Initialization, loading screen, scene flow control
2. **MainMenu** - Title screen, new game, continue, settings, quit
3. **OpenWorld_TestDistrict** - First playable open-world area
4. **Garage_Test** - Vehicle selection/customization (placeholder for now)

### Scene Rules

- **Bootstrap** handles all initialization and scene transitions
- Keep scene logic minimal; prefer prefabs with their own controllers
- Design scenes to be additive-friendly for future expansion
- Use root containers to organize hierarchy (e.g., `WorldRoot`, `VehicleRoot`, `UIRoot`)
- Avoid hardcoded scene-specific references in scripts

### Loading Strategy
```csharp
// Scene loading via Bootstrap
Bootstrap.LoadScene("OpenWorld_TestDistrict");
```

---

## Prefab Strategy

### Vehicle Prefabs
- Separate visual meshes from physics colliders
- Include WheelColliders for each wheel
- Add mount points for camera, lights, particles
- Use child objects for wheels, doors, hood (future interior access)

### Building Prefabs
- Modular wall sections for future interior loading
- Entry point markers with `BuildingEntry` component
- Parking anchors for parked car spawning
- LOD variants for mobile optimization (future)

### Prop Prefabs
- Static props: simple MeshColliders or box colliders
- Dynamic props: Rigidbody only when interaction needed
- Group props in containers: `PropGroup_Papers_01`
- Keep poly count low for mobile compatibility

### UI Prefabs
- Self-contained canvas per screen
- Include all needed controllers on root
- Reference other UI elements via Inspector

---

## Performance Rules

### Mobile-Aware Architecture
- Design for Android first, optimize PC later
- Keep draw calls under 1000 for mobile
- Use texture atlases for props
- Implement LOD for buildings and vehicles

### Update Optimization
- Minimize `Update()` usage; use events where possible
- Cache component references in `Awake()` or `Start()`
- Use `[SerializeField]` to avoid `GetComponent` in loops

### Physics
- Use simple colliders (box, sphere, capsule) over mesh colliders
- Disable rigidbodies on static objects
- Limit physics objects in view

### Object Management
- Pool frequently spawned objects (projectiles, debris)
- Group props logically for future culling
- Avoid runtime instantiation of large numbers of objects

### Profiling
- Keep hierarchy clean for easy profiling
- Add `[Conditional("DEBUG")]` for verbose logging
- Use `UnityEngine.Profiling` for performance-critical code

---

## Testing Checklist

Before marking a feature complete:

- [ ] Code compiles without errors
- [ ] No null reference risks in Inspector-linked fields
- [ ] Tested in relevant scene
- [ ] Documented required Inspector assignments
- [ ] Documented manual Unity setup steps
- [ ] Debug validation added where useful

---

## Never Do Mistakes

1. **Do not rewrite unrelated systems** - Stay scoped to the task
2. **Do not invent nonexistent assets** - Use placeholders with TODO
3. **Do not hardcode scene-specific references** - Use prefabs or查找
4. **Do not create god classes** - Modular is mandatory
5. **Do not assume paid assets** - Build with primitives first
6. **Do not add AI traffic without request** - MVP first
7. **Do not optimize prematurely** - Clarity over micro-optimizations
8. **Do not skip manual setup notes** - Document every Unity step
9. **Do not use GetComponent in Update loops** - Cache references
10. **Do not leave TODO without explanation** - Add clear context

---

## Required Unity Setup

### Tags to Create
- `Player`
- `Vehicle`
- `Building`
- `Prop`
- `Ground`
- `Water`

### Layers to Create
- `Default`
- `Player`
- `Vehicle`
- `Building`
- `Prop`
- `Ground`
- `Water`
- `UI`

### Project Settings
- Set default namespace: (none for now)
- Enable "Enter Play Mode Options" for faster iteration
- Configure mobile-friendly input system

---

## Code Module Overview

| Module | Purpose |
|--------|---------|
| **Core** | Bootstrap, GameManager, EventSystem, Utilities |
| **Vehicle** | Car physics, input, wheel handling, tuning |
| **Camera** | Follow camera, cinematic modes, shake |
| **World** | District management, terrain chunks, road system |
| **Buildings** | Building spawners, modular buildings, entries |
| **Props** | Prop scatter, grouping, static/dynamic props |
| **UI** | Menus, HUD, loading screens, tutorials |
| **Missions** | Mission system, objectives, triggers, rewards |
| **SaveSystem** | Save/load, profile management, settings |
| **Audio** | Music manager, SFX, spatial audio |
| **Optimization** | LOD, object pooling, mobile helpers |

---

## Next Steps

After this planning phase, implement the MVP in order:

1. Bootstrap and scene loading system
2. Basic vehicle controller
3. Follow camera
4. Simple test district with placeholder roads/buildings
5. Basic UI and main menu
6. Save system foundation
