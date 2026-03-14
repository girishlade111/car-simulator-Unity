# Apartment Building System

## Overview
Modular apartment building system for the open-world car simulator with exterior-first workflow, LOD support, and future interior expansion hooks.

---

## Files Created

| File | Purpose |
|------|---------|
| `ApartmentBuildingData.cs` | ScriptableObject for building configuration (floors, facade, colors, parking) |
| `ApartmentBuilding.cs` | Component for spawned buildings with entry points, parking, portals |
| `ApartmentBuildingGenerator.cs` | Spawns modular buildings with variations |
| `BuildingLODSetup.cs` | LOD group helper for mobile optimization |
| `BuildingInteriorPortal.cs` | Placeholder for future interior entry system |

---

## Folder Structure

```
Assets/_Project/
├── Prefabs/
│   └── Buildings/
│       ├── Apartments/
│       │   ├── PF_Apartment_Modern_3Floor.prefab
│       │   ├── PF_Apartment_Modern_5Floor.prefab
│       │   ├── PF_Apartment_Modern_8Floor.prefab
│       │   ├── PF_Apartment_Classic_3Floor.prefab
│       │   ├── PF_Apartment_Classic_5Floor.prefab
│       │   ├── PF_Apartment_Brutalist.prefab
│       │   ├── PF_Apartment_ArtDeco.prefab
│       │   └── LOD/
│       │       ├── PF_Apartment_Modern_LOD1.prefab
│       │       └── PF_Apartment_Modern_LOD2.prefab
│       ├── Interiors/
│       │   ├── PF_Apartment_Interior_Placeholder.prefab
│       │   └── (future: detailed interiors)
│       └── Common/
│           ├── PF_BuildingEntry.prefab
│           ├── PF_ParkingSpot.prefab
│           └── PF_InteriorPortal.prefab
│
├── ScriptableObjects/
│   └── BuildingData/
│       ├── SO_Apartment_Modern.asset
│       ├── SO_Apartment_Classic.asset
│       ├── SO_Apartment_Brutalist.asset
│       └── SO_Apartment_ArtDeco.asset
│
├── Materials/
│   └── Buildings/
│       ├── Apartment_Modern.mat
│       ├── Apartment_Classic.mat
│       ├── Apartment_Brutalist.mat
│       ├── Window_Frame.mat
│       ├── Roof_Flat.mat
│       ├── Roof_Pitched.mat
│       └── Interior_Portal.mat
│
└── Scripts/
    └── Buildings/
        ├── ApartmentBuildingData.cs
        ├── ApartmentBuilding.cs
        ├── ApartmentBuildingGenerator.cs
        ├── BuildingLODSetup.cs
        ├── BuildingInteriorPortal.cs
        ├── BuildingData.cs (existing)
        ├── BuildingSpawner.cs (existing)
        ├── BuildingEntry.cs (existing)
        └── BuildingPlacer.cs (existing)
```

---

## Hierarchy & Naming Rules

### Building Prefab Hierarchy
```
PF_Apartment_[Style]_[Floors]Floor
├── BuildingBody (Cube)
├── Windows (Quads)
├── Roof (Cube)
├── Balconies (optional)
├── EntryPoints
│   ├── Entry_0
│   │   └── EntryMarker
│   └── Entry_1 (if duplex)
├── ParkingArea (if applicable)
│   ├── ParkingSpot_0
│   ├── ParkingSpot_1
│   └── ...
└── InteriorPortals (if applicable)
    └── Portal_Lobby
```

### Naming Conventions
- **Prefabs**: `PF_Apartment_[Style]_[Floors]Floor_[Variant]`
- **Scripts**: PascalCase (`ApartmentBuilding.cs`)
- **Variables**: camelCase with `m_` prefix for serialized fields
- **Materials**: `MAT_[BuildingType]_[Part]_[Variant]`
- **Tags**: Use existing tags (`Building`, `Prop`, `Ground`)

---

## Setup Instructions

### 1. Create Building Data
1. Right-click in Project window
2. Select `Create > CarSimulator > Apartment Building Data`
3. Configure: floors, facade type, colors, parking spaces
4. Save as `SO_Apartment_[Style].asset`

### 2. Create Building Prefab
1. Create Empty GameObject: `PF_Apartment_Modern_5Floor`
2. Add components:
   - `ApartmentBuilding` script
   - `BoxCollider` (size = building dimensions)
   - `LODGroup` (optional, use `BuildingLODSetup`)
3. Add child objects:
   - Building body (Cube)
   - Windows (Quads)
   - Roof
   - Entry point markers

### 3. Setup in Scene
1. Add `ApartmentBuildingGenerator` to scene
2. Assign building data array
3. Configure spawn area and settings
4. Press Play to generate

---

## Key Components

### ApartmentBuildingData
- **Floors**: min/max range for random variation
- **Facade**: brick, concrete, stucco, metal, glass
- **Roof**: flat, pitched, terraced
- **Windows**: count per floor, style options
- **Parking**: space count, area size
- **Portals**: interior entry points

### ApartmentBuilding (Runtime)
- Auto-generates entry points
- Manages parking spots
- LOD group setup
- Color/variation application
- Portal management

### BuildingInteriorPortal
- Future hook for interior entry
- Shows interaction prompt
- Supports locked/unlocked states
- Glow effect

---

## Future Interior Expansion

### Adding Interiors
1. Create interior prefab with `BuildingInteriorPortal` components
2. Link to `ApartmentBuildingData.InteriorPrefab`
3. Portals become interactive when player approaches

### Interior Data Structure
```csharp
// Each portal connects to:
public class InteriorPortalData
{
    public string portalId;      // Unique identifier
    public Vector3 position;    // Local position in building
    public Vector3 size;        // Portal dimensions
    public bool isLocked;       // Locked until requirement met
    public string sceneName;    // OR interior prefab reference
}
```

### Interior Requirements System
- Quest completion
- Key item possession
- Time of day
- Reputation level

---

## LOD Setup

### Distance Guidelines
| Level | Distance | Detail Level |
|-------|----------|--------------|
| LOD 0 | 0-50m | Full detail |
| LOD 1 | 50-100m | Simplified mesh |
| LOD 2 | 100-150m | Billboard or very simple |

### Setup via BuildingLODSetup.cs
1. Add component to building prefab
2. Assign LOD models (or use same model)
3. Call `SetupLOD()` or enable `m_setupOnStart`

---

## Mobile Optimization

### Performance Tips
- Use LOD groups (required for mobile)
- Limit active buildings to 20-30
- Use simple placeholder meshes
- Batch static geometry
- Disable shadows on distant buildings

### Draw Call Optimization
- Share materials between buildings
- Use GPU instancing
- Merge static meshes per building type

---

## Placeholder Replacements

### Current Placeholders
- Building body: Cube primitive
- Windows: Quad primitives
- Roof: Cube primitive
- Entry markers: Cube primitives

### Artist Deliverables
1. **Building Models** (FBX)
   - LOD0: Full detail (~5000 tris)
   - LOD1: Simplified (~2000 tris)
   - LOD2: Very simple (~500 tris)

2. **Materials** (PBR)
   - Diffuse, Normal, Roughness, AO
   - 2K resolution recommended
   - Tiling enabled

3. **Interior Models**
   - Room layouts
   - Prop placements
   - Lighting setups

---

## Troubleshooting

### Buildings Not Spawning
- Check spawn area size
- Verify ground layer is set
- Increase min distance between buildings

### Portals Not Working
- Ensure player has "Player" tag
- Check activation distance
- Verify portal is not locked

### LOD Not Working
- Add LODGroup component
- Assign renderers to LOD levels
- Check camera distance
