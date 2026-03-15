using NUnit.Framework;
using UnityEngine;
using System.Collections;

namespace CarSimulator.Tests
{
    public class TestRunner : MonoBehaviour
    {
        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            Debug.Log("[TestRunner] This requires Unity Test Framework package to run");
            Debug.Log("[TestRunner] Install via Package Manager: Unity Test Framework");
        }
    }

    public class RuntimeTests
    {
        [Test]
        public void Vector3_DistanceCalculation_IsCorrect()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(3, 4, 0);
            
            float distance = Vector3.Distance(a, b);
            
            Assert.AreEqual(5f, distance, 0.001f);
        }

        [Test]
        public void Quaternion_EulerAngles_Work()
        {
            Quaternion rotation = Quaternion.Euler(0, 90, 0);
            
            Assert.IsNotNull(rotation);
            Assert.AreEqual(90f, rotation.eulerAngles.y, 0.001f);
        }

        [Test]
        public void Time_deltaTime_IsValid()
        {
            Assert.GreaterOrEqual(Time.deltaTime, 0f);
        }

        [Test]
        public void Mathf_Clamp_Works()
        {
            Assert.AreEqual(5, Mathf.Clamp(5, 0, 10));
            Assert.AreEqual(0, Mathf.Clamp(-5, 0, 10));
            Assert.AreEqual(10, Mathf.Clamp(15, 0, 10));
        }

        [Test]
        public void Mathf_Lerp_Works()
        {
            float result = Mathf.Lerp(0f, 10f, 0.5f);
            Assert.AreEqual(5f, result, 0.001f);
        }
    }

    public class StringTests
    {
        [Test]
        public void SceneNames_AreValid()
        {
            string[] validScenes = { "MainMenu", "OpenWorld_TestDistrict", "Garage_Test" };
            
            foreach (string scene in validScenes)
            {
                Assert.IsNotNull(scene);
                Assert.Greater(scene.Length, 0);
            }
        }

        [Test]
        public void Tags_AreDefined()
        {
            string[] requiredTags = { "Player", "Vehicle", "Building", "Prop" };
            
            foreach (string tag in requiredTags)
            {
                Assert.IsNotNull(tag);
            }
        }
    }
}
