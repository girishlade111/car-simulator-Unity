using NUnit.Framework;
using CarSimulator.Core;
using UnityEngine;

namespace CarSimulator.Tests
{
    public class SingletonTests
    {
        [Test]
        public void GameManager_HasStaticInstance()
        {
            Assert.IsNotNull(GameManager.Instance);
        }

        [Test]
        public void Bootstrap_StaticProperties_AreValid()
        {
            Assert.IsNotNull(Bootstrap.OpenWorldScene);
            Assert.IsNotNull(Bootstrap.GarageScene);
        }

        [Test]
        public void Bootstrap_OpenWorldScene_IsValidSceneName()
        {
            string scene = Bootstrap.OpenWorldScene;
            StringAssert.IsNotEmpty(scene);
            StringAssert.DoesNotContain(" ", scene);
        }

        [Test]
        public void Bootstrap_GarageScene_IsValidSceneName()
        {
            string scene = Bootstrap.GarageScene;
            StringAssert.IsNotEmpty(scene);
            StringAssert.DoesNotContain(" ", scene);
        }
    }

    public class SceneLoaderTests
    {
        [Test]
        public void SceneLoader_StaticLoadMethod_Exists()
        {
            // Verify the static Load method exists and is callable
            var type = typeof(Runtime.SceneLoader);
            Assert.IsNotNull(type);
            
            var loadMethod = type.GetMethod("Load");
            Assert.IsNotNull(loadMethod, "SceneLoader.Load method should exist");
        }
    }
}
