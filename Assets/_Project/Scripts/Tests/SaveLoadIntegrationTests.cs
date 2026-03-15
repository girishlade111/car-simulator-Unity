using NUnit.Framework;
using CarSimulator.SaveSystem;
using System;
using System.IO;
using UnityEngine;

namespace CarSimulator.Tests
{
    public class SaveLoadIntegrationTests
    {
        private string m_testSavePath;
        private const string TEST_SLOT_NAME = "test_save";

        [SetUp]
        public void Setup()
        {
            m_testSavePath = Path.Combine(Application.persistentDataPath, "TestSaves");
            if (!Directory.Exists(m_testSavePath))
            {
                Directory.CreateDirectory(m_testSavePath);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(m_testSavePath))
            {
                try
                {
                    Directory.Delete(m_testSavePath, true);
                }
                catch (Exception)
                {
                    // Ignore cleanup errors
                }
            }
        }

        [Test]
        public void GameSaveData_CanBeSerialized()
        {
            GameSaveData save = new GameSaveData
            {
                saveName = TEST_SLOT_NAME,
                playTime = 100f,
                player = new PlayerData
                {
                    credits = 5000,
                    lastPosition = new Vector3(10, 0, 20),
                    lastRotation = new Quaternion(0, 0, 0, 1)
                }
            };

            string json = JsonUtility.ToJson(save);
            Assert.IsNotNull(json);
            Assert.Greater(json.Length, 0);
        }

        [Test]
        public void GameSaveData_CanBeDeserialized()
        {
            GameSaveData original = new GameSaveData
            {
                saveName = TEST_SLOT_NAME,
                playTime = 100f,
                player = new PlayerData
                {
                    credits = 5000,
                    lastPosition = new Vector3(10, 0, 20)
                }
            };

            string json = JsonUtility.ToJson(original);
            GameSaveData loaded = JsonUtility.FromJson<GameSaveData>(json);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(original.saveName, loaded.saveName);
            Assert.AreEqual(original.playTime, loaded.playTime);
            Assert.AreEqual(original.player.credits, loaded.player.credits);
        }

        [Test]
        public void PlayerData_Serialization_Works()
        {
            PlayerData player = new PlayerData
            {
                credits = 12345,
                lastPosition = new Vector3(100, 50, -25),
                lastRotation = Quaternion.Euler(0, 45, 0)
            };

            string json = JsonUtility.ToJson(player);
            PlayerData loaded = JsonUtility.FromJson<PlayerData>(json);

            Assert.AreEqual(12345, loaded.credits);
            Assert.AreEqual(100, loaded.lastPosition.x);
            Assert.AreEqual(50, loaded.lastPosition.y);
            Assert.AreEqual(-25, loaded.lastPosition.z);
        }

        [Test]
        public void MissionProgress_Serialization_Works()
        {
            MissionProgressData missions = new MissionProgressData
            {
                completedMissions = new System.Collections.Generic.List<string> { "mission_1", "mission_2" },
                failedMissions = new System.Collections.Generic.List<string> { "mission_3" }
            };

            string json = JsonUtility.ToJson(missions);
            MissionProgressData loaded = JsonUtility.FromJson<MissionProgressData>(json);

            Assert.AreEqual(2, loaded.completedMissions.Count);
            Assert.Contains("mission_1", loaded.completedMissions);
            Assert.Contains("mission_2", loaded.completedMissions);
        }

        [Test]
        public void WorldStateData_Serialization_Works()
        {
            WorldStateData world = new WorldStateData
            {
                currentTimeOfDay = 12.5f,
                weatherType = "Rain",
                destroyedBuildings = new System.Collections.Generic.List<string> { "building_1" }
            };

            string json = JsonUtility.ToJson(world);
            WorldStateData loaded = JsonUtility.FromJson<WorldStateData>(json);

            Assert.AreEqual(12.5f, loaded.currentTimeOfDay);
            Assert.AreEqual("Rain", loaded.weatherType);
            Assert.AreEqual(1, loaded.destroyedBuildings.Count);
        }

        [Test]
        public void GameSettingsData_Serialization_Works()
        {
            GameSettingsData settings = new GameSettingsData
            {
                masterVolume = 0.8f,
                musicVolume = 0.5f,
                sfxVolume = 0.9f,
                graphics = new GraphicsSettings
                {
                    qualityLevel = 2,
                    resolutionWidth = 1920,
                    resolutionHeight = 1080,
                    fullscreen = true,
                    vsync = true,
                    viewDistance = 1000
                }
            };

            string json = JsonUtility.ToJson(settings);
            GameSettingsData loaded = JsonUtility.FromJson<GameSettingsData>(json);

            Assert.AreEqual(0.8f, loaded.masterVolume);
            Assert.AreEqual(0.5f, loaded.musicVolume);
            Assert.AreEqual(2, loaded.graphics.qualityLevel);
            Assert.AreEqual(1920, loaded.graphics.resolutionWidth);
            Assert.IsTrue(loaded.graphics.fullscreen);
        }

        [Test]
        public void SaveData_VersionMigration_Works()
        {
            GameSaveData oldSave = new GameSaveData();
            oldSave.version = 0;
            oldSave.player = null;
            oldSave.missions = null;
            oldSave.world = null;
            oldSave.settings = null;

            oldSave.MigrateToCurrentVersion();

            Assert.IsNotNull(oldSave.player);
            Assert.IsNotNull(oldSave.missions);
            Assert.IsNotNull(oldSave.world);
            Assert.IsNotNull(oldSave.settings);
        }

        [Test]
        public void SaveData_Validate_DetectsCorruption()
        {
            GameSaveData valid = new GameSaveData();
            Assert.IsTrue(valid.Validate());

            GameSaveData invalid = new GameSaveData();
            invalid.saveId = "";
            Assert.IsFalse(invalid.Validate());
        }

        [Test]
        public void MultipleSaveSlots_CanBeCreated()
        {
            GameSaveData[] slots = new GameSaveData[10];
            for (int i = 0; i < 10; i++)
            {
                slots[i] = new GameSaveData
                {
                    saveName = $"Slot {i}",
                    saveId = $"save_{i}"
                };
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.IsNotNull(slots[i]);
                Assert.AreEqual($"Slot {i}", slots[i].saveName);
            }
        }
    }
}
