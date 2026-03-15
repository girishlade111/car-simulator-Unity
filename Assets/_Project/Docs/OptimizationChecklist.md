# Optimization Checklist

## Quick Wins (Do First)

### Scene Setup
- [ ] **Static Batching**: Mark all static geometry (buildings, roads, props) as "Static" in Inspector
- [ ] **Lightmap**: Bake lighting for static objects (Window > Lighting > Generate Lighting)
- [ ] **Occlusion Culling**: Enable in Project Settings > Occlusion Culling
- [ ] **Skybox**: Use simple procedural skybox, avoid HDR

### Player Settings
- [ ] **VSync**: Enable in Quality settings
- [ ] **Shadows**: Set to "Hard Shadows Only" for mobile, "Hard and Soft" for PC
- [ ] **Shadow Distance**: Set to 50-100 (not 500!)
- [ ] **Shadow Cascades**: Use 2 cascades max
- [ ] **Anti-Aliasing**: Set to 2x or disabled for mobile

### Hierarchy
- [ ] Parent all spawned objects under root containers:
  - `[Vehicles]`
  - `[Traffic]`
  - `[NPCs]`
  - `[Props]`
  - `[Effects]`

## Texture & Material Guidelines

### Texture Settings (Project Settings > Quality)
```
Desktop:
  - Texture Quality: Full Res
  - Anisotropic Textures: Forced On
  - Adaptive Tessellation: On

Mobile:
  - Texture Quality: Half Res  
  - Anisotropic Textures: Per Texture
  - Max Texture Size: 1024-2048
```

### Material Guidelines
- Use **Mobile/Diffuse** shaders for mobile
- Use **Standard** shader for PC
- Avoid **Standard (Specular setup)** - use Metallic instead
- Set **GPU Instancing** checkbox on materials where possible
- Keep material count under 100 for mobile

## Lighting Settings

### For Open World
1. **Directional Light** (Sun):
   - Mode: Mixed
   - Shadow Type: Hard Shadows
   - Shadow Resolution: Low (mobile) / Medium (PC)
   
2. **Realtime GI**: Disabled
3. **Baked GI**: Enabled for static geometry

### Light Probes
- Add LightProbeGroup to areas with dynamic objects
- Place probes around player spawn points

## Collider Guidelines

### Static Geometry
- Use **Box Collider** for buildings (not Mesh Collider)
- Combine adjacent colliders
- Remove colliders from visual-only objects

### Dynamic Objects
- **Vehicle**: Box Collider or Wheel Colliders only
- **NPCs**: Capsule Collider
- **Triggers**: Sphere Collider (isTrigger = true)

### Mesh Colliders
- **NEVER** use Mesh Collider on moving objects
- Only use on static, complex geometry
- Convex: On only if needed

## Batching Guidelines

### Draw Call Reduction
1. Share materials between similar objects
2. Use atlases for props (multiple textures in one)
3. Enable **GPU Instancing** on particle materials
4. Avoid changing materials at runtime

### Target Draw Calls
```
Mobile: < 100
PC: < 500
```

## Script Optimizations

### Update Loops
- **NEVER** call `FindObjectsOfType` in Update
- **NEVER** call `GetComponent` in Update - cache in Start/Awake
- Use timers for expensive operations:
```csharp
private Timer m_throttledUpdate = new Timer(0.5f);

void Update() {
    if (m_throttledUpdate.Tick(Time.deltaTime)) {
        // Expensive operation
    }
}
```

### Physics
- Set Rigidbody to `isKinematic = true` when not moving
- Use `Interpolation` on player vehicle only
- Set `CollisionDetectionMode` to Continuous for player
- Disable physics on objects far from player

### Memory
- Avoid `new string` in Update - use StringBuilder or cached strings
- Pool particle systems
- Don't destroy/create objects - use object pooling

## Performance Profiling

### Unity Profiler (Window > Profiler)
1. Run game in Editor
2. Look for:
   - **Scripts**: Update() taking > 1ms
   - **Rendering**: Too many draw calls
   - **Physics**: Too many active Rigidbodies
   - **Garbage Collection**: Frequent GC spikes

### In-Game Stats
Press `Ctrl+Shift+P` to show:
- FPS
- Draw calls
- Triangles
- Set pass calls

## Mobile-Specific

### Project Settings > Player > Android
- **Color Space**: Gamma (simpler than Linear)
- **Auto Graphics API**: On
- **Strip Engine Code**: Managed Strip Level Low
- **Target Architecture**: ARM64 + ARMv7

### Quality Settings (Per-Device)
Create 2-3 quality levels:
1. **Low**: No shadows, low res textures, simple shaders
2. **Medium**: Hard shadows, medium textures  
3. **High**: Full quality

## Object Lifecycle

### Spawning
```csharp
// Bad - creates garbage
void Update() {
    if (condition) {
        Instantiate(prefab, position, rotation);
    }
}

// Good - use pooling
private ObjectPooler m_pool;

void Update() {
    if (condition) {
        m_pool.Get("myPrefab", position, rotation);
    }
}
```

### Destroying
```csharp
// Bad
Destroy(object);

// Good - return to pool
object.GetComponent<PooledObject>()?.ReturnToPool();
```

## Quick Reference

| Category | Mobile Target | PC Target |
|----------|--------------|-----------|
| Draw Calls | < 100 | < 500 |
| Triangles | < 100K | < 1M |
| SetPass Calls | < 50 | < 200 |
| FPS | 30+ | 60 |
| Texture Memory | < 256MB | < 1GB |

## Common Performance Issues

1. **High Update() time**: Throttle expensive operations
2. **Too many draw calls**: Batch static geometry, share materials
3. **Physics lag**: Disable Rigidbody when far from player
4. **Memory spikes**: Use object pooling, avoid runtime allocations
5. **Garbage Collection**: Cache references, reuse collections
