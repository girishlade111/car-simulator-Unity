# Mission System Setup Guide

## Creating a Mission

### 1. Create Mission Data
1. Right-click → Create → CarSimulator → Mission
2. Set Mission ID (e.g., `delivery_01`)
3. Set Mission Name (e.g., "First Delivery")
4. Add Description

### 2. Add Objectives
Each objective needs:
- **Objective ID**: Unique identifier
- **Description**: Display text
- **Type**: ReachLocation, DriveDistance, CollectItems, TimeTrial
- **Target Position**: For ReachLocation
- **Target Distance**: 5m default
- **Target Count**: For DriveDistance/CollectItems

### 3. Set Rewards
- Currency Reward: e.g., 100
- Time Limit: e.g., 120s (0 = no limit)

---

## Mission Types

### Delivery
- Type: ReachLocation
- Target Position: Drop-off point
- Target Distance: 10

### Time Trial
- Type: TimeTrial
- Time Limit: Required

### Collection
- Type: CollectItems
- Target Count: Number of items

### Drive Distance
- Type: DriveDistance
- Target Count: Distance in meters

---

## Adding Mission to Scene

### Option 1: Mission Trigger
1. Create empty GameObject
2. Add Box Collider (IsTrigger = true)
3. Add `MissionTrigger` component
4. Set Mission ID
5. Set Trigger Type (OnEnter/OnExit/OnPress)

### Option 2: Direct Start
```csharp
MissionManager.Instance.StartMission("delivery_01");
```

---

## Mission UI Setup

1. Create Canvas
2. Add Panel (MissionPanel)
3. Add Text elements:
   - Title Text
   - Objectives Text
   - Timer Text
4. Add `MissionUI` script
5. Link references

---

## MissionManager Setup

1. Add `MissionManager` to scene (or auto-created)
2. Add Mission Data assets to Available Missions array

---

## Testing

1. Create mission asset
2. Add trigger to scene
3. Enter trigger area
4. Complete objectives
5. Verify completion
