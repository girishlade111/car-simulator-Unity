# Project Context

This repository is a Unity C# open-world car simulator game.
The player can freely drive in an open area with roads, apartments, parked cars, paper/debris props, trees, grass, rocks, and other nature elements.
This project must stay modular, easy to extend, and suitable for future PC and Android optimization.

## Main Goal

Build a clean, playable MVP with:
- Free driving
- One open-world test district
- Third-person car controller
- Apartments and building prefabs
- Parked cars
- Paper/debris and street props
- Nature environment
- Basic UI
- Mission loop
- Save/load support
- Future-ready structure for traffic, interiors, and larger map streaming

## Tech Stack

- Engine: Unity
- Language: C#
- Editor: Visual Studio Code
- AI coding assistant: Claude Code
- Source control: Git

## Project Priorities

- Readability over cleverness
- Stable gameplay over overcomplicated realism
- Modular systems over one-file solutions
- Placeholder-friendly development
- Mobile-aware architecture, even if PC is used first
- Reusable prefabs and data-driven setup where possible

## Workflow Rules

- Explore first, then plan, then implement
- Before changing multiple systems, explain the plan first
- Before writing code, inspect existing scripts and follow current patterns
- Prefer editing existing systems instead of duplicating logic
- Stop after completing the requested step and report:
  1. files created/edited
  2. what was implemented
  3. manual Unity editor steps
  4. remaining TODOs
- If requirements are unclear, ask before making architectural changes
- Do not silently rewrite large systems without approval

## Code Style

- Use clear, simple C# naming
- One responsibility per class
- Keep MonoBehaviours small
- Prefer composition over giant manager classes
- Use [SerializeField] private fields for inspector-driven references
- Expose tuning values in Inspector where useful
- Avoid magic numbers; use config fields or constants
- Use regions only if the file becomes hard to navigate
- Add comments only when they help future editing
- Avoid noisy or obvious comments

## Unity Architecture

Use these modules:

- Core
- Vehicle
- Camera
- World
- Buildings
- Props
- UI
- Missions
- SaveSystem
- Audio
- Optimization

Recommended high-level folders:

- Assets/_Project/Scripts/Core
- Assets/_Project/Scripts/Vehicle
- Assets/_Project/Scripts/Camera
- Assets/_Project/Scripts/World
- Assets/_Project/Scripts/Buildings
- Assets/_Project/Scripts/Props
- Assets/_Project/Scripts/UI
- Assets/_Project/Scripts/Missions
- Assets/_Project/Scripts/SaveSystem
- Assets/_Project/Scripts/Audio
- Assets/_Project/Scripts/Optimization
- Assets/_Project/Scenes
- Assets/_Project/Prefabs
- Assets/_Project/Materials
- Assets/_Project/ScriptableObjects
- Assets/_Project/Docs

## Scene Strategy

Main scenes:

- Bootstrap
- MainMenu
- OpenWorld_TestDistrict
- Garage_Test

Scene rules:

- Bootstrap handles initialization and scene flow
- OpenWorld_TestDistrict is the first playable prototype scene
- Keep scenes additive-friendly for future expansion
- Avoid putting too much permanent logic directly in scenes
- Prefer prefabs and modular root containers

## Naming Rules

Scripts:
- PascalCase for class names and file names
- Example: VehicleController, FollowCamera, DistrictSpawnManager

GameObjects:
- Use descriptive names
- Example: PlayerCar, DistrictRoot, ApartmentBlock_A, PropGroup_Papers_01

Prefabs:
- Prefix by type when useful
- Example: PF_PlayerCar, PF_ApartmentBlock_A, PF_ParkedCar_Sedan_01

ScriptableObjects:
- Prefix with SO_
- Example: SO_VehicleTuning_Default

Materials:
- Prefix with MAT_
- Example: MAT_Road_Asphalt, MAT_Building_Concrete

## Vehicle Rules

- Focus on stable arcade-sim driving, not full realism
- Separate input, movement, and visual logic
- Physics logic goes in FixedUpdate
- Input gathering goes in Update
- Keep car tuning adjustable in Inspector
- Include reset/respawn support
- Support future mobile controls without tightly coupling current keyboard input

## World Rules

- Build the world in small districts first
- Do not attempt a full city in one step
- Use placeholder roads, buildings, trees, and props when art is missing
- Keep scene hierarchy organized under root containers
- Prepare for future world streaming or additive district loading
- Avoid runtime-heavy generation unless requested

## Building Rules

- Apartments should be modular and easy to duplicate
- Exterior-first workflow
- Add entry point markers for future interiors
- Add parking anchors where useful
- Keep models and prefabs easy to replace later
- Do not create overly complex procedural mesh systems unless asked

## Prop Rules

- Props include paper/debris, bins, benches, poles, barriers, signs, and roadside details
- Keep props grouped and organized
- Support manual placement first
- Random scatter tools must remain controllable and lightweight
- Do not give unnecessary rigidbody physics to every small prop

## Performance Rules

- Always consider mobile-friendly structure
- Minimize unnecessary Update usage
- Cache references where appropriate
- Use pooled systems only when useful
- Prefer simple colliders when possible
- Group props logically for future culling/optimization
- Avoid generating huge numbers of objects at runtime
- Preserve clean hierarchy for profiling and debugging

## Testing Rules

After implementing a feature:
- Check for compile errors
- Check for null reference risks
- Explain required Inspector assignments
- Explain scene setup steps
- Verify the feature in the relevant scene
- If possible, add lightweight debug validation
- Do not claim success without describing how it was verified

## Manual Setup Rules

When editor setup is required:
- Clearly list required tags, layers, colliders, prefabs, and scene references
- Clearly separate auto-generated work from manual Unity steps
- Never assume hidden editor links are already assigned
- If a feature cannot be completed fully in code, leave a TODO and explain exactly what I must do in Unity

## Commands Claude Should Prefer

When useful, prefer these actions:
- Read existing scripts before implementing new ones
- Search for related systems before creating duplicates
- Keep changes scoped to the requested feature
- Summarize changed files at the end
- Suggest the next prompt after finishing each step

## Never Do

- Do not rewrite unrelated systems
- Do not invent nonexistent assets or packages
- Do not hardcode scene-specific references unless clearly marked
- Do not create giant god classes
- Do not use paid assets by assumption
- Do not add advanced AI traffic unless explicitly requested
- Do not optimize prematurely in ways that reduce clarity
- Do not claim a feature is complete without manual setup notes if Unity links are required

## Success Criteria

A task is only considered complete when:
- Code compiles cleanly
- The feature fits current architecture
- Manual Unity setup is documented
- Files changed are listed
- The next recommended step is provided
