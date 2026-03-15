# Test Checklist - Car Simulator

## Overview
This checklist provides smoke tests, gameplay tests, and failure case verification for the car simulator project. Run these tests after any significant changes or before release.

---

## 1. Smoke Tests

### 1.1 Project Launch
- [ ] Project compiles without errors
- [ ] Bootstrap scene loads correctly
- [ ] Main menu is accessible
- [ ] No null reference errors in console on startup

### 1.2 Core Systems
- [ ] GameManager singleton initializes
- [ ] SaveManager singleton initializes
- [ ] ProfileManager singleton initializes
- [ ] SettingsPersistence singleton initializes

### 1.3 Scene Loading
- [ ] MainMenu scene loads from Bootstrap
- [ ] OpenWorld scene loads from menu
- [ ] Garage scene loads from menu
- [ ] Scene transitions work without errors

---

## 2. Gameplay Tests

### 2.1 Vehicle Controller
- [ ] Player vehicle spawns correctly
- [ ] Vehicle responds to WASD/Arrow input
- [ ] Vehicle accelerates and decelerates
- [ ] Steering works at various speeds
- [ ] Braking works (Space key)
- [ ] Handbrake works (Shift key)
- [ ] Vehicle resets with R key
- [ ] Vehicle respawns when falling below map

### 2.2 Camera System
- [ ] FollowCamera follows player vehicle
- [ ] Camera collision detection works
- [ ] Camera height adjusts with speed
- [ ] Camera rotation is smooth

### 2.3 Physics
- [ ] Vehicle doesn't flip too easily
- [ ] Wheels spin correctly
- [ ] Suspension feels reasonable
- [ ] Collision with objects works

---

## 3. Interaction Tests

### 3.1 World Interaction
- [ ] E key triggers interactions
- [ ] Interaction prompts appear
- [ ] Interactables can be activated

### 3.2 Building Interaction
- [ ] Apartment entrances are detectable
- [ ] Interior loading works
- [ ] Exit building works

### 3.3 Parking
- [ ] Parking zones are detectable
- [ ] Parking validation works
- [ ] Parking progress shows in UI

---

## 4. Mission Tests

### 4.1 Mission System
- [ ] MissionManager initializes
- [ ] Missions can be started
- [ ] Mission objectives track progress
- [ ] Mission completion triggers rewards
- [ ] Mission failure works with time limits

### 4.2 Quest System
- [ ] QuestManager initializes
- [ ] Quest givers can give quests
- [ ] Quest progress updates
- [ ] Quest completion rewards work

### 4.3 Race System
- [ ] Race checkpoints work
- [ ] Lap counting works
- [ ] Race timer displays correctly
- [ ] Race finish triggers rewards

---

## 5. Save/Load Tests

### 5.1 Save System
- [ ] Manual save works
- [ ] Auto-save triggers
- [ ] Save on quit works
- [ ] Multiple save slots work

### 5.2 Load System
- [ ] Manual load works
- [ ] Player position restores correctly
- [ ] Vehicle state restores
- [ ] Profile data restores

### 5.3 Settings
- [ ] Graphics settings apply
- [ ] Audio settings apply
- [ ] Controls settings apply
- [ ] Settings persist between sessions

---

## 6. Common Failure Cases

### 6.1 Null Reference Issues
- [ ] No errors when Player tag missing
- [ ] No errors when no vehicle spawned
- [ ] No errors when managers not initialized
- [ ] No errors in Update() when components missing

### 6.2 Scene Issues
- [ ] Missing scene names don't crash
- [ ] Corrupted save doesn't crash game
- [ ] Missing prefabs have fallbacks

### 6.3 Performance Issues
- [ ] No frame drops in open world
- [ ] Memory usage stays reasonable
- [ ] No infinite loops in code

### 6.4 Input Issues
- [ ] Gamepad input works
- [ ] Keyboard input works
- [ ] No input locking issues

---

## 7. Platform-Specific Tests

### 7.1 PC
- [ ] All PC controls work
- [ ] Mouse input works
- [ ] Windowed/fullscreen modes work

### 7.2 Mobile (Future)
- [ ] Touch controls work
- [ ] Performance is acceptable
- [ ] Screen rotation handled

---

## Debug Commands

Use these in console for testing:
```
Debug commands:
- /spawnvehicle [type] - Spawn a vehicle
- /teleport [x] [y] [z] - Teleport player
- /money [amount] - Add money
- /unlockall - Unlock all content
- /resetprogress - Reset save data
```

---

## Reporting Issues

When reporting failures, include:
1. Steps to reproduce
2. Expected behavior
3. Actual behavior
4. Console errors
5. Unity version
6. Platform (PC/Mobile)

---

## Test Environment

- **Unity Version**: [Insert Version]
- **Build Target**: Windows/Mac/Android
- **Test Date**: [Insert Date]
- **Tester**: [Insert Name]
