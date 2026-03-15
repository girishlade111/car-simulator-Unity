using NUnit.Framework;
using CarSimulator.Vehicle;

namespace CarSimulator.Tests
{
    public class VehicleTuningTests
    {
        [Test]
        public void CreateInstance_HasDefaultValues()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.AreEqual(40f, tuning.maxSteerAngle);
            Assert.AreEqual(2000f, tuning.engineForce);
            Assert.AreEqual(180f, tuning.maxSpeed);
            Assert.AreEqual(3500f, tuning.brakeForce);
        }

        [Test]
        public void SteeringRange_IsValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.maxSteerAngle, 20f);
            Assert.LessOrEqual(tuning.maxSteerAngle, 50f);
        }

        [Test]
        public void EngineValues_AreValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.engineForce, 500f);
            Assert.LessOrEqual(tuning.engineForce, 3000f);
            
            Assert.GreaterOrEqual(tuning.maxSpeed, 50f);
            Assert.LessOrEqual(tuning.maxSpeed, 250f);
        }

        [Test]
        public void BrakeValues_AreValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.brakeForce, 500f);
            Assert.LessOrEqual(tuning.brakeForce, 5000f);
            
            Assert.GreaterOrEqual(tuning.brakeBalance, 0.3f);
            Assert.LessOrEqual(tuning.brakeBalance, 0.8f);
        }

        [Test]
        public void Mass_IsValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.mass, 500f);
            Assert.LessOrEqual(tuning.mass, 3000f);
        }

        [Test]
        public void SuspensionValues_AreValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.suspensionSpring, 10f);
            Assert.LessOrEqual(tuning.suspensionSpring, 50f);
            
            Assert.GreaterOrEqual(tuning.suspensionDamper, 0f);
            Assert.LessOrEqual(tuning.suspensionDamper, 10f);
            
            Assert.GreaterOrEqual(tuning.suspensionDistance, 0f);
            Assert.LessOrEqual(tuning.suspensionDistance, 1f);
        }

        [TearDown]
        public void Cleanup()
        {
            var tunings = UnityEngine.Resources.FindObjectsOfTypeAll<VehicleTuning>();
            if (tunings != null && tunings.Length > 0)
            {
                UnityEngine.Object.DestroyImmediate(tunings[0] as VehicleTuning);
            }
        }
    }
}
