using NUnit.Framework;
using CarSimulator.SaveSystem;
using System;

namespace CarSimulator.Tests
{
    public class GameSaveDataTests
    {
        [Test]
        public void Constructor_SetsDefaultValues()
        {
            GameSaveData save = new GameSaveData();
            
            Assert.AreEqual(GameSaveData.CURRENT_VERSION, save.version);
            Assert.IsNotNull(save.saveId);
            Assert.Greater(save.saveId.Length, 0);
            Assert.AreEqual("New Save", save.saveName);
            Assert.IsNotNull(save.player);
            Assert.IsNotNull(save.missions);
            Assert.IsNotNull(save.world);
            Assert.IsNotNull(save.settings);
        }

        [Test]
        public void CreateDefault_ReturnsValidSave()
        {
            GameSaveData save = GameSaveData.CreateDefault();
            
            Assert.IsNotNull(save);
            Assert.AreEqual(GameSaveData.CURRENT_VERSION, save.version);
        }

        [Test]
        public void Validate_ReturnsTrue_ForValidSave()
        {
            GameSaveData save = new GameSaveData();
            
            Assert.IsTrue(save.Validate());
        }

        [Test]
        public void Validate_ReturnsFalse_ForInvalidSaveId()
        {
            GameSaveData save = new GameSaveData();
            save.saveId = "";
            
            Assert.IsFalse(save.Validate());
        }

        [Test]
        public void Validate_ReturnsFalse_ForFutureVersion()
        {
            GameSaveData save = new GameSaveData();
            save.version = GameSaveData.CURRENT_VERSION + 1;
            
            Assert.IsFalse(save.Validate());
        }

        [Test]
        public void MigrateToCurrentVersion_HandlesNullReferences()
        {
            GameSaveData save = new GameSaveData();
            save.player = null;
            save.missions = null;
            save.world = null;
            save.settings = null;
            
            save.MigrateToCurrentVersion();
            
            Assert.IsNotNull(save.player);
            Assert.IsNotNull(save.missions);
            Assert.IsNotNull(save.world);
            Assert.IsNotNull(save.settings);
        }

        [Test]
        public void PlayTime_StartsAtZero()
        {
            GameSaveData save = new GameSaveData();
            
            Assert.AreEqual(0f, save.playTime);
        }

        [Test]
        public void CreatedAt_IsSet()
        {
            DateTime before = DateTime.Now;
            GameSaveData save = new GameSaveData();
            DateTime after = DateTime.Now;
            
            Assert.GreaterOrEqual(save.createdAt, before);
            Assert.LessOrEqual(save.createdAt, after);
        }
    }

    public class PlayerDataTests
    {
        [Test]
        public void Constructor_SetsDefaultValues()
        {
            PlayerData player = new PlayerData();
            
            Assert.IsNotNull(player.lastPosition);
            Assert.IsNotNull(player.lastRotation);
            Assert.GreaterOrEqual(player.credits, 0);
        }

        [Test]
        public void Credits_DefaultIsZero()
        {
            PlayerData player = new PlayerData();
            
            Assert.AreEqual(0, player.credits);
        }
    }

    public class MissionProgressDataTests
    {
        [Test]
        public void Constructor_InitializesCollections()
        {
            MissionProgressData missions = new MissionProgressData();
            
            Assert.IsNotNull(missions.completedMissions);
            Assert.IsNotNull(missions.failedMissions);
        }
    }
}
