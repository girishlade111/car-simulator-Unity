# Scene Setup Guides

## Bootstrap Scene Setup

### Create Scene
1. Create new scene: `Bootstrap.unity`
2. Save to: `Assets/_Project/Scenes/`

### Setup
1. Create empty GameObject `Bootstrap`
2. Add `Bootstrap` script
3. Inspector settings:
   - First Scene: `MainMenu`
   - Persistent Systems Prefab: (optional)

### Hierarchy
```
Bootstrap
└── Bootstrap (Bootstrap.cs)
```

---

## MainMenu Scene Setup

### Create Scene
1. Create new scene: `MainMenu.unity`
2. Save to: `Assets/_Project/Scenes/`

### Setup Canvas
1. Right-click → UI → Canvas
2. Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080
   - Screen Match Mode: Match Width Or Height (0.5)

### Add Background
1. Create UI → Image as child of Canvas
2. Name: `Background`
3. Anchor: Stretch
4. Color: Dark gray or add background image

### Add Title
1. Create UI → Text
2. Name: `TitleText`
3. Text: "Car Simulator"
4. Font Size: 72
5. Position: Top center

### Add Buttons
Create UI → Button for each:
1. **NewGameButton**
   - Position: Center, slightly above middle
   - Text: "New Game"
   
2. **ContinueButton**
   - Position: Below New Game
   - Text: "Continue"
   
3. **SettingsButton**
   - Position: Below Continue
   - Text: "Settings"
   
4. **QuitButton**
   - Position: Below Settings
   - Text: "Quit"

### Add MainMenu Script
1. Create empty GameObject `MenuController`
2. Add `MainMenu` script
3. Assign buttons in Inspector

### Add Settings Panel (Optional)
1. Create Panel
2. Name: `SettingsPanel`
3. Add placeholder sliders for Volume, Quality
4. Assign to MainMenu script

### Hierarchy
```
MainMenu
├── Canvas
│   ├── Background (Image)
│   ├── TitleText (Text)
│   ├── NewGameButton (Button)
│   ├── ContinueButton (Button)
│   ├── SettingsButton (Button)
│   ├── QuitButton (Button)
│   └── SettingsPanel (Panel, disabled)
└── MenuController (MainMenu.cs)
```

---

## Garage_Test Scene Setup (Placeholder)

### Create Scene
1. Create new scene: `Garage_Test.unity`
2. Save to: `Assets/_Project/Scenes/`

### Setup
1. Add basic camera
2. Add ground plane
3. Placeholder: "Garage coming soon" text

---

## Build Order

1. Bootstrap → loads MainMenu
2. MainMenu → New Game → OpenWorld_TestDistrict
3. MainMenu → Garage → Garage_Test

---

## Important: Scene in Build Settings

1. Open: File → Build Settings
2. Add scenes in order:
   - Bootstrap (index 0)
   - MainMenu
   - OpenWorld_TestDistrict
   - Garage_Test
3. Set Bootstrap as first scene
4. Click "Build" to test

---

## Quick Start Checklist

- [ ] Create Bootstrap scene with Bootstrap.cs
- [ ] Create MainMenu scene with Canvas and MainMenu.cs
- [ ] Create OpenWorld_TestDistrict scene with car prefab
- [ ] Add all scenes to Build Settings
- [ ] Press Play → Bootstrap loads → MainMenu appears
- [ ] Click "New Game" → Car driving scene loads
