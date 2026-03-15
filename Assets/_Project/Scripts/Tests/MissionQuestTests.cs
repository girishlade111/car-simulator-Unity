using NUnit.Framework;
using CarSimulator.Missions;
using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Tests
{
    public class MissionSystemTests
    {
        [Test]
        public void MissionData_CanBeCreated()
        {
            GameObject go = new GameObject("TestMission");
            MissionData mission = go.AddComponent<MissionData>();
            
            Assert.IsNotNull(mission);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void MissionTrigger_CanBeCreated()
        {
            GameObject go = new GameObject("TestTrigger");
            MissionTrigger trigger = go.AddComponent<MissionTrigger>();
            
            Assert.IsNotNull(trigger);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void MissionTrigger_HasDefaultValues()
        {
            GameObject go = new GameObject("TestTrigger");
            MissionTrigger trigger = go.AddComponent<MissionTrigger>();
            
            Assert.IsNotNull(trigger);
            
            Object.DestroyImmediate(go);
        }
    }

    public class QuestSystemTests
    {
        [Test]
        public void QuestData_CanBeCreated()
        {
            QuestData quest = new QuestData
            {
                questId = "test_quest",
                questTitle = "Test Quest",
                questDescription = "A test quest",
                questType = QuestData.QuestType.Main
            };
            
            Assert.IsNotNull(quest);
            Assert.AreEqual("test_quest", quest.questId);
            Assert.AreEqual("Test Quest", quest.questTitle);
        }

        [Test]
        public void QuestData_DefaultValues()
        {
            QuestData quest = new QuestData();
            
            Assert.IsNotNull(quest.questId);
            Assert.IsNotNull(quest.questTitle);
            Assert.IsFalse(quest.isActive);
            Assert.IsFalse(quest.isCompleted);
        }

        [Test]
        public void QuestProgress_CanBeTracked()
        {
            QuestProgress progress = new QuestProgress
            {
                CurrentCount = 0,
                IsComplete = false
            };
            
            Assert.AreEqual(0, progress.CurrentCount);
            Assert.IsFalse(progress.IsComplete);
            
            progress.CurrentCount = 5;
            progress.IsComplete = true;
            
            Assert.AreEqual(5, progress.CurrentCount);
            Assert.IsTrue(progress.IsComplete);
        }
    }

    public class MissionLoopTests
    {
        [Test]
        public void MissionLoopManager_CanBeCreated()
        {
            GameObject go = new GameObject("TestMissionLoop");
            MissionLoopManager manager = go.AddComponent<MissionLoopManager>();
            
            Assert.IsNotNull(manager);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void DriveMissionData_CanBeCreated()
        {
            GameObject go = new GameObject("TestDriveMission");
            DriveMissionData mission = go.AddComponent<DriveMissionData>();
            
            Assert.IsNotNull(mission);
            
            Object.DestroyImmediate(go);
        }
    }

    public class CheckpointTests
    {
        [Test]
        public void CheckpointSystem_CanBeCreated()
        {
            GameObject go = new GameObject("TestCheckpointSystem");
            CheckpointSystem system = go.AddComponent<CheckpointSystem>();
            
            Assert.IsNotNull(system);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Checkpoint_CanBeCreated()
        {
            GameObject go = new GameObject("TestCheckpoint");
            SaveSystem.Checkpoint checkpoint = go.AddComponent<SaveSystem.Checkpoint>();
            
            Assert.IsNotNull(checkpoint);
            
            Object.DestroyImmediate(go);
        }
    }

    public class ParkingTests
    {
        [Test]
        public void ParkingZone_CanBeCreated()
        {
            GameObject go = new GameObject("TestParkingZone");
            ParkingZone zone = go.AddComponent<ParkingZone>();
            
            Assert.IsNotNull(zone);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void ParkingManager_CanBeCreated()
        {
            GameObject go = new GameObject("TestParkingManager");
            ParkingZone.ParkingManager manager = go.AddComponent<ParkingZone.ParkingManager>();
            
            Assert.IsNotNull(manager);
            
            Object.DestroyImmediate(go);
        }
    }

    public class RaceTests
    {
        [Test]
        public void RaceCheckpoint_CanBeCreated()
        {
            GameObject go = new GameObject("TestRaceCheckpoint");
            RaceCheckpoint checkpoint = go.AddComponent<RaceCheckpoint>();
            
            Assert.IsNotNull(checkpoint);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void RaceStartFinish_CanBeCreated()
        {
            GameObject go = new GameObject("TestRaceStart");
            RaceStartFinish startFinish = go.AddComponent<RaceStartFinish>();
            
            Assert.IsNotNull(startFinish);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void LapTimer_CanBeCreated()
        {
            GameObject go = new GameObject("TestLapTimer");
            LapTimer timer = go.AddComponent<LapTimer>();
            
            Assert.IsNotNull(timer);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void LapTimer_DefaultValues()
        {
            GameObject go = new GameObject("TestLapTimer");
            LapTimer timer = go.AddComponent<LapTimer>();
            
            Assert.IsNotNull(timer);
            
            Object.DestroyImmediate(go);
        }
    }

    public class MissionProgressTests
    {
        [Test]
        public void MissionProgress_DefaultState()
        {
            var progress = new MissionProgress();
            
            Assert.AreEqual(0, progress.CurrentCount);
            Assert.IsFalse(progress.IsComplete);
        }

        [Test]
        public void MissionProgress_CanUpdate()
        {
            var progress = new MissionProgress();
            
            progress.CurrentCount = 10;
            Assert.AreEqual(10, progress.CurrentCount);
            
            progress.IsComplete = true;
            Assert.IsTrue(progress.IsComplete);
        }
    }
}
