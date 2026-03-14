# Environment Assets TODO List

## Overview
This document outlines placeholder assets that need to be replaced with artist-made assets for the OpenWorld_TestDistrict.

---

## Priority 1: Essential Replacements

### 1.1 Terrain
- [ ] **Ground Texture** - Replace procedural placeholder with seamless terrain textures
  - Location: `TerrainZone.cs` material assignments
  - Recommended: 4K seamless textures (diffuse + normal + height)
  - Types needed: Grass, Dirt, Asphalt, Gravel

### 1.2 Trees
- [ ] **Tree Prefab (High Detail)** - Replace placeholder cylinder+sphere
  - Location: `NatureManager.m_treePrefab`, `TreeSpawnPoint.m_treePrefab`
  - Recommended: Low-poly pine/oak (500-1500 tris), billboard LOD
  - Setup: Add Wind Shader for natural movement

- [ ] **Tree Prefab (Low Detail)** - LOD replacement
  - Location: `TreeSpawnPoint.m_lowLODPrefab`
  - Recommended: Billboard sprite or simplified mesh

### 1.3 Roads
- [ ] **Road Surface Material**
  - Types: Highway, City Street, Parking Lot
  - Recommended: PBR with roughness variation, tire marks decals

---

## Priority 2: Visual Enhancement

### 2.1 Rocks
- [ ] **Rock Prefab** - Replace placeholder spheres
  - Location: `NatureManager.m_rockPrefab`, `RockCluster.m_rockPrefab`
  - Recommended: 3-5 rock variations (large boulder, medium, small pebble)
  - Types: Granite, Limestone, Sandstone

### 2.2 Grass
- [ ] **Grass Prefab** - Replace placeholder quads
  - Location: `GrassZone.m_grassPrefab`, `NatureManager.m_grassPrefab`
  - Recommended: GPU instanced grass blades (T4M or custom shader)
  - Setup: Wind animation shader

### 2.3 Buildings
- [ ] **Apartment Building** - Replace placeholder cubes
  - Location: `BuildingSpawner` prefab references
- [ ] **Commercial Building** - Replace placeholder cubes
- [ ] **House** - Replace placeholder cubes

---

## Priority 3: Polish

### 3.1 Props
- [ ] Street lamps
- [ ] Traffic signs
- [ ] Benches
- [ ] Trash cans
- [ ] Bus stops

### 3.2 Vehicles
- [ ] Traffic cars (sedan, SUV, truck, sports car)
- [ ] Parked cars

### 3.3 Details
- [ ] Crosswalks
- [ ] Road markings
- [ ] Manholes
- [ ] Drain grates

---

## Asset Pipeline Notes

### Recommended Asset Types
```
Textures:
- 4096x4096 terrain layers (RGB+A for height)
- 2048x2048 props (PBR: Diffuse, Normal, Roughness, AO)

Models:
- Trees: FBX with LOD0/LOD1/LOD2
- Rocks: FBX with 3-5 variations
- Buildings: Modular wall sections for interior expansion

Prefabs:
- All prefabs should have proper colliders
- Use prefab variants for color/style variations
```

### Performance Targets
- Mobile: <50k triangles visible, GPU instancing required
- PC: <200k triangles visible, shadows enabled
- Tree count: 100-500 depending on platform
- Draw calls target: <500

### Shader Recommendations
- Use Terrain Layers for ground blending
- Use GPU Instancing for vegetation
- Add Wind shader for trees/grass
- Use LOD groups for all large objects

---

## How to Replace Placeholders

### Step 1: Import Assets
1. Import models into `Assets/_Project/Prefabs/Environment/`
2. Import textures into `Assets/_Project/Materials/Environment/`

### Step 2: Create Prefabs
1. Drag models into scene
2. Add colliders
3. Add to prefab folder
4. Reference in scripts

### Step 3: Update Script References
1. Open scene with NatureManager
2. Drag new prefabs into inspector slots
3. Test generation

### Step 4: Optimize
1. Add LOD groups
2. Enable GPU instancing on materials
3. Set up static batching for non-moving objects

---

## File Locations

| Script | Prefab Fields | Line |
|--------|--------------|------|
| NatureManager | m_treePrefab, m_rockPrefab, m_grassPrefab | 24-36 |
| TreeSpawnPoint | m_treePrefab, m_lowLODPrefab | 10-12 |
| RockCluster | m_rockPrefab | 9 |
| GrassZone | m_grassPrefab | 10 |
| TerrainZone | m_terrainMaterial | 14 |

---

## Contact
For questions about asset specifications, refer to CLAUDE.md or contact the art team.
