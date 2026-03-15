using NUnit.Framework;
using UnityEngine;

namespace CarSimulator.Tests
{
    public class GameConstantsTests
    {
        [Test]
        public void DefaultScene_IsNotEmpty()
        {
            Assert.IsNotNull(GameConstants.DEFAULT_SCENE);
            Assert.Greater(GameConstants.DEFAULT_SCENE.Length, 0);
        }

        [Test]
        public void OpenWorldScene_HasValidName()
        {
            Assert.IsNotNull(GameConstants.OPEN_WORLD_SCENE);
            StringAssert.IsNotEmpty(GameConstants.OPEN_WORLD_SCENE);
        }

        [Test]
        public void GarageScene_HasValidName()
        {
            Assert.IsNotNull(GameConstants.GARAGE_SCENE);
            StringAssert.IsNotEmpty(GameConstants.GARAGE_SCENE);
        }

        [Test]
        public void TimeScales_AreValid()
        {
            Assert.AreEqual(1f, GameConstants.DEFAULT_TIME_SCALE);
            Assert.AreEqual(0f, GameConstants.PAUSED_TIME_SCALE);
        }

        [Test]
        public void SaveSlotCount_IsPositive()
        {
            Assert.Greater(GameConstants.SAVE_SLOT_COUNT, 0);
        }

        [Test]
        public void AutoSaveInterval_IsPositive()
        {
            Assert.Greater(GameConstants.AUTO_SAVE_INTERVAL, 0f);
        }

        [Test]
        public void Tags_AreDefined()
        {
            StringAssert.IsNotEmpty(GameConstants.Tags.Player);
            StringAssert.IsNotEmpty(GameConstants.Tags.Vehicle);
            StringAssert.IsNotEmpty(GameConstants.Tags.Building);
        }

        [Test]
        public void Layers_AreDefined()
        {
            StringAssert.IsNotEmpty(GameConstants.Layers.Default);
            StringAssert.IsNotEmpty(GameConstants.Layers.Player);
            StringAssert.IsNotEmpty(GameConstants.Layers.UI);
        }
    }
}
