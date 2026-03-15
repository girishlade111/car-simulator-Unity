# Next Features - Future Development Guide

This document outlines recommended features and improvements for the car simulator beyond the MVP.

---

## Priority 1: Essential Gameplay

### 1.1 Traffic AI System

**Current State:** Basic scripts exist (TrafficManager, TrafficCar, TrafficLight)

**To Complete:**
- Implement pathfinding for traffic vehicles
- Add vehicle behavior (lane keeping, stopping at lights)
- Create traffic car prefabs with AI
- Add spawn zones on roads
- Implement intersection logic

**Scripts to Use:**
- `EnhancedTrafficManager.cs`
- `TrafficLight.cs`
- `TrafficPath.cs`
- `TrafficSpawnZone.cs`

### 1.2 Pedestrian AI

**Current State:** Basic manager exists (PedestrianManager, Pedestrian)

**To Complete:**
- Implement NPC behaviors (walking, standing, sitting)
- Add interaction dialogue system
- Create pedestrian prefabs with animations
- Implement schedule system (Day/Night cycles)

**Scripts to Use:**
- `PedestrianManager.cs`
- `Pedestrian.cs`
- `InteriorNPC.cs`
- `NPCSchedule.cs`

### 1.3 Garage & Customization

**Current State:** UI scripts exist (PaintShop, TireShop, RimShop, CustomizationShop)

**To Complete:**
- Create vehicle prefab variants
- Implement paint system with material swapping
- Add wheel model swapping
- Create tuning parameter application
- Build garage scene with display platforms

**Scripts to Use:**
- `GarageController.cs`
- `PaintShop.cs`
- `TireShop.cs`
- `VehicleTuning.cs`

---

## Priority 2: World Expansion

### 2.1 Larger World Streaming

**To Implement:**
- Add chunk-based world loading
- Implement LOD for distant districts
- Create seamless border transitions
- Add fast travel system
- Optimize for mobile (reduce draw calls)

**Scripts to Use:**
- `DistrictManager.cs` (expand for streaming)
- `ChunkLoader.cs` (new, for async loading)

### 2.2 Better Building Interiors

**Current State:** Basic interior loading exists

**To Complete:**
- Create detailed interior furniture prefabs
- Add door interaction (open/close)
- Implement light switching
- Add NPC spawners inside buildings
- Create multiple interior variants

**Scripts to Use:**
- `InteriorLoader.cs`
- `InteriorFurniture.cs`
- `BuildingInteriorPortal.cs`

### 2.3 Enhanced Environment

**To Add:**
- Weather effects (rain, snow, fog)
- Dynamic time of day lighting
- Seasonal changes
- Ambient soundscapes
- Wildlife (birds, animals)

**Scripts to Use:**
- `WeatherSystem.cs`
- `EnhancedTimeOfDay.cs`
- `BuildingWeatherEffects.cs`

---

## Priority 3: Mobile Support

### 3.1 Touch Controls

**To Implement:**
- Virtual joystick for steering
- Touch buttons for brake/handbrake
- Swipe gestures for camera
- On-screen prompts

**Scripts to Add:**
- `MobileInputManager.cs`
- `VirtualJoystick.cs`
- `TouchButton.cs`

### 3.2 Performance Optimization

**To Optimize:**
- Texture atlases for props
- GPU instancing for vegetation
- LOD systems for buildings/cars
- Reduced physics calculations
- Async scene loading

**Scripts to Use:**
- `SimpleLOD.cs`
- `ObjectPool.cs`
- `PerformanceManager.cs`

---

## Priority 4: Content Expansion

### 4.1 More Vehicle Types

- Sports cars
- Trucks/SUVs
- Motorcycles
- Emergency vehicles (police, ambulance, fire)
- Public transport (buses, trams)

### 4.2 Mission System Expansion

- Story missions with cutscenes
- Time trials
- Challenge events
- Daily/weekly quests
- Multiplayer races

### 4.3 Economy System

- Career progression
- Vehicle earning/purchasing
- Tuning upgrades
- Property ownership

---

## Priority 5: Multiplayer (Long-term)

### 5.1 Network Architecture

- Player synchronization
- Vehicle state replication
- Chat system
- Friend lists
- Leaderboards

**Scripts to Use:**
- `NetworkManager.cs`
- `NetworkedPlayer.cs`

### 5.2 Multiplayer Features

- Free roam together
- Race mode
- Tag/chase games
- Custom events

---

## Audio Improvements

### Priority Enhancements

1. **Engine Sound Synthesis**
   - Dynamic pitch based on RPM
   - Turbo whistle effects
   - Transmission sounds

2. **Environment Audio**
   - Wind noise at speed
   - Tire sounds (road type)
   - Weather sounds

3. **UI Audio**
   - Menu sounds
   - Notification sounds
   - Achievement jingles

**Scripts to Use:**
- `EngineAudio.cs`
- `ExhaustMod.cs`
- `CarRadio.cs`
- `TireScreechAudio.cs`

---

## Graphics Enhancements

### Visual Polish

1. **Post-Processing**
   - Bloom for lights
   - Ambient occlusion
   - Color grading
   - Vignette

2. **Vehicle Effects**
   - Dirt accumulation
   - Weather effects on car
   - Damage visualization
   - Neon underglow

3. **Environment**
   - Reflections (planar/probes)
   - Dynamic shadows
   - Particle effects (exhaust, tire smoke)

---

## Testing & QA

### Recommended Test Suite

- Unit tests for core systems
- Integration tests for save/load
- Playtests for gameplay feel
- Performance profiling on target devices
- Mobile compatibility testing
- Network stress testing (multiplayer)

---

## Contributing Guidelines

When implementing new features:

1. Follow coding conventions in CLAUDE.md
2. Add comments for complex logic
3. Test thoroughly before committing
4. Update documentation
5. Keep modules independent
6. Optimize for mobile when possible

---

## Version Roadmap

| Version | Focus | Features |
|---------|-------|----------|
| 1.0 | MVP | Basic driving, test district |
| 1.1 | Traffic | AI traffic, better spawning |
| 1.2 | Interiors | Building interiors, NPCs |
| 1.3 | Customization | Garage, paint, tuning |
| 1.4 | Mobile | Touch controls, optimization |
| 2.0 | Expansion | Larger world, more content |
| 2.1+ | Multiplayer | Online features |

---

## Notes

- Keep the project modular - avoid god classes
- Test on target platform frequently
- Document any manual Unity setup steps
- Use placeholder assets until final art is ready
- Maintain backward compatibility for saves
