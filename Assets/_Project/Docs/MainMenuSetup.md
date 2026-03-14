# Main Menu Setup Guide

## Scene Setup

### 1. Create MainMenu Scene
1. Create scene: `Assets/_Project/Scenes/MainMenu.unity`
2. Add Canvas (Scale With Screen Size, 1920x1080)

### 2. Create Main Menu Panel
Add UI elements:
- Title Text
- New Game Button
- Continue Button  
- Load Game Button
- Settings Button
- Quit Button

### 3. Add MainMenuController
1. Create empty GameObject: `MainMenuController`
2. Add `MainMenuController` component
3. Link buttons in Inspector

---

## Settings Panel

### 1. Create Panel (child of Canvas)
- Add `SettingsController` component
- Add sliders: Master Volume, Music Volume, SFX Volume
- Add toggles: Mute, Fullscreen, VSync, Invert Y
- Add dropdown: Quality
- Add buttons: Apply, Reset, Back

### 2. Link References
- Drag sliders/toggles to SettingsController

---

## Load Game Panel

### 1. Create Panel
- Add `SaveLoadController` component
- Add vertical layout container for slots
- Create slot prefab with:
  - Slot Text
  - Info Text  
  - Select Button
  - Delete Button

### 2. Link References
- Container: ScrollView content
- Prefab: Slot template

---

## Pause Menu (OpenWorld)

### 1. Add to OpenWorld Scene
- Create Canvas with PauseMenuController
- Create Panel with buttons:
  - Resume
  - Restart
  - Settings
  - Main Menu
  - Quit

### 2. Link References

---

## Required Tags

Create in Unity:
- `Player`
- `Vehicle`

---

## Required Layers

Create in Unity:
- `Default`
- `Player`
- `UI`

---

## Bootstrap Setup

1. Open Bootstrap scene
2. Ensure Bootstrap component has `First Scene: MainMenu`

---

## Flow

```
Bootstrap → MainMenu
    ↓
[New Game] → OpenWorld
[Continue/Load] → OpenWorld (with save data)
[Settings] → Settings Panel
[Quit] → Exit
    ↓
In Game: Escape → Pause Menu
```
