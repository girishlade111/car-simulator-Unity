# GamePlan.md - Car Simulator Project Plan

## Gameplay Loop

### Core Loop
1. **Launch Game** → Bootstrap scene initializes core systems
2. **Main Menu** → Player selects New Game, Continue, Settings, or Quit
3. **Open World Drive** → Player spawns in test district, drives freely
4. **Explore** → Discover roads, apartments, parked cars, props, nature
5. **Complete Missions** → (MVP) Simple pickup/delivery quests
6. **Save Progress** → Auto-save or manual save via menu
7. **Garage** → (MVP placeholder) Vehicle selection screen

### Secondary Loops
- **Vehicle Tuning** → Adjust handling in garage (future)
- **Customization** → Change colors/parts (future)
- **Building Interiors** → Enter apartments (future expansion)

---

## Core Systems

### 1. Scene Management (Bootstrap)
- Handles all scene transitions
- Manages initialization order
- Shows loading screens
- Supports additive scene loading for expansion

### 2. Vehicle System
- **VehicleController** - Core driving physics
- **WheelCollider** - Per-wheel traction, braking, steering
- **InputHandler** - Keyboard/mobile input abstraction
- **VehicleCamera** - Third-person follow camera
- **RespawnSystem** - Reset vehicle position on stuck/fall

### 3. World System
- **DistrictManager** - Manages test district areas
- **RoadNetwork** - Placeholder road meshes and colliders
- **TerrainChunks** - Ground plane management
- **StreamingManager** - Future district streaming

### 4. Building System
- **BuildingSpawner** - Places apartment/commercial buildings
- **BuildingEntry** - Entry point markers for interiors
- **ParkingAnchors** - Spawn points for parked cars
- **ModularBuildings** - Wall/roof sections for variety

### 5. Prop System
- **PropManager** - Manages street props
- **PropScatter** - Random placement system
- **PropGroup** - Bundled prop containers
- Categories: debris, bins, benches, poles, signs

### 6. Environment System
- **NaturePlacement** - Trees, rocks, grass
- **LightingSetup** - Day/night cycle (future)
- **WeatherSystem** - Rain/fog effects (future)

### 7. UI System
- **MenuController** - Main menu navigation
- **HUD** - Speed, minimap, mission objectives
- **LoadingScreen** - Scene transition feedback
- **SettingsMenu** - Graphics, audio, controls

### 8. Mission System
- **MissionManager** - Quest tracking and spawning
- **MissionObjective** - Pickup, delivery, time trials
- **MissionTrigger** - Area-based quest activation
- **MissionRewards** - Currency, unlockables

### 9. Save System
- **SaveManager** - JSON/binary serialization
- **ProfileManager** - Multiple save slots
- **SettingsPersistence** - Options save/load

### 10. Audio System
- **MusicManager** - Background music
- **SFXManager** - Sound effects pool
- **VehicleAudio** - Engine, tire, collision sounds
- **SpatialAudio** - 3D positioned sounds

---

## MVP Scope

### Must Have (MVP)
- [ ] Bootstrap scene with initialization
- [ ] Main menu (New Game, Continue, Settings, Quit)
- [ ] OpenWorld_TestDistrict with:
  - [ ] Flat ground plane (placeholder terrain)
  - [ ] Simple road mesh (placeholder)
  - [ ] 3-5 apartment building prefabs
  - [ ] 5-10 parked car prefabs
  - [ ] Basic prop scatter (paper, debris)
  - [ ] Simple trees/rocks (primitives)
- [ ] Player vehicle with:
  - [ ] Arcade-sim physics (not full simulation)
  - [ ] WASD/Arrow key input
  - [ ] Third-person follow camera
  - [ ] Basic UI (speed, controls hint)
- [ ] Basic HUD showing speed
- [ ] Simple mission (drive from A to B)
- [ ] Save/Load game progress
- [ ] Basic collision detection

### Should Have (MVP+)
- [ ] Respawn system when stuck
- [ ] Multiple vehicle presets
- [ ] Basic garage menu
- [ ] Pause menu
- [ ] Engine sound effects
- [ ] Collision feedback

### Won't Have (MVP)
- Traffic AI
- Building interiors
- Multiplayer
- Advanced vehicle customization
- Realistic damage
- Day/night cycle

---

## Later Expansion Scope

### Phase 2: Traffic & Population
- Basic traffic AI on roads
- Pedestrian NPCs
- Traffic lights and signs
- More vehicle types

### Phase 3: Interiors
- Apartment interior loading
- Shop/building entry system
- Basic interior decoration

### Phase 4: Content Expansion
- Multiple districts
- More mission types
- Vehicle customization shop
- Garage with upgrades

### Phase 5: Optimization
- LOD system for buildings/vehicles
- Object pooling
- Mobile controls
- Android build
- Level streaming

### Phase 6: Polish
- Day/night, weather
- Advanced sounds
- Visual effects
- UI polish

---

## Technical Risks

### Risk 1: Physics Stability
- **Issue**: WheelCollider can be unstable at high speeds
- **Mitigation**: Use arcade physics with clamped values, add respawn
- **Severity**: Medium

### Risk 2: Performance on Mobile
- **Issue**: Open world with many props can lag
- **Mitigation**: Design mobile-first, use LOD, limit draw calls
- **Severity**: High

### Risk 3: Asset Gaps
- **Issue**: No 3D models for buildings, vehicles, props
- **Mitigation**: Use placeholder primitives with TODO comments
- **Severity**: Medium

### Risk 4: Scene Management Complexity
- **Issue**: Multiple scenes can cause reference issues
- **Mitigation**: Use prefabs, avoid hardcoded scene refs
- **Severity**: Low

### Risk 5: Scope Creep
- **Issue**: Feature list grows beyond MVP
- **Mitigation**: Strict MVP boundaries, future phase planning
- **Severity**: Medium

---

## Milestone Roadmap

### Milestone 1: Foundation (Week 1-2)
- [ ] Unity project setup
- [ ] Bootstrap and scene loading
- [ ] Basic vehicle controller
- [ ] Follow camera
- [ ] Project compiles and runs

### Milestone 2: World Building (Week 3-4)
- [ ] Test district with placeholder terrain
- [ ] Basic road mesh
- [ ] Apartment building prefabs (3-5)
- [ ] Prop scatter system
- [ ] Nature elements (trees, rocks)

### Milestone 3: Gameplay Loop (Week 5-6)
- [ ] Main menu
- [ ] HUD with speed display
- [ ] Basic mission system
- [ ] Save/Load system
- [ ] Parked cars

### Milestone 4: Polish (Week 7-8)
- [ ] Respawn system
- [ ] Audio foundation
- [ ] Settings menu
- [ ] Garage placeholder
- [ ] Bug fixes and testing

### Milestone 5: Expansion Ready (Week 9+)
- [ ] Documentation complete
- [ ] Architecture supports traffic
- [ ] Architecture supports interiors
- [ ] Mobile-ready structure
- [ ] Performance optimization path defined

---

## File Dependencies

```
Bootstrap → GameManager, EventSystem
GameManager → SceneManager, SaveManager, UIManager
VehicleController → InputHandler, WheelCollider
FollowCamera → VehicleController (target)
DistrictManager → BuildingSpawner, PropManager
MissionManager → MissionObjective, MissionTrigger
SaveManager → ProfileManager, SettingsPersistence
```

---

## Manual Unity Setup Required

1. Create Tags: Player, Vehicle, Building, Prop, Ground
2. Create Layers: Player, Vehicle, Building, Prop, Ground, UI
3. Configure Input System (or use legacy Input Manager)
4. Set up Project Settings (Color Space, API Compatibility)
5. Create Bootstrap scene with Bootstrap prefab
6. Create MainMenu scene with canvas
7. Create OpenWorld_TestDistrict scene with terrain
8. Assign Physics layers properly

---

## Next Implementation Prompt

"Implement Milestone 1: Create Bootstrap system, scene loading, and basic vehicle controller with follow camera in OpenWorld_TestDistrict."
