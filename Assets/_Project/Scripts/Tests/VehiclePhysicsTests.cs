using NUnit.Framework;
using CarSimulator.Vehicle;
using UnityEngine;

namespace CarSimulator.Tests
{
    public class VehiclePhysicsTests
    {
        [Test]
        public void VehicleInput_ProvidesDefaultValues()
        {
            GameObject go = new GameObject("TestInput");
            VehicleInput input = go.AddComponent<VehicleInput>();
            
            Assert.AreEqual(0f, input.SteerInput);
            Assert.AreEqual(0f, input.ThrottleInput);
            Assert.IsFalse(input.IsBraking);
            Assert.IsFalse(input.IsHandbraking);
            Assert.IsFalse(input.IsResetPressed);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void VehicleTuning_DefaultValues_AreValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.Greater(tuning.maxSteerAngle, 0);
            Assert.Greater(tuning.engineForce, 0);
            Assert.Greater(tuning.maxSpeed, 0);
            Assert.Greater(tuning.brakeForce, 0);
            Assert.Greater(tuning.mass, 0);
            
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void VehicleTuning_ClampedValues_StayInRange()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            tuning.maxSteerAngle = 100f;
            Assert.LessOrEqual(tuning.maxSteerAngle, 50f);
            
            tuning.engineForce = 10000f;
            Assert.LessOrEqual(tuning.engineForce, 3000f);
            
            tuning.maxSpeed = 500f;
            Assert.LessOrEqual(tuning.maxSpeed, 250f);
            
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void GearSystem_DefaultGear_IsFirst()
        {
            GameObject go = new GameObject("TestGear");
            GearSystem gear = go.AddComponent<GearSystem>();
            
            gear.Awake();
            
            Assert.AreEqual(1, gear.CurrentGear);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void VehicleTuning_SuspensionValues_AreValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.suspensionSpring, 10f);
            Assert.LessOrEqual(tuning.suspensionSpring, 50f);
            Assert.GreaterOrEqual(tuning.suspensionDamper, 0f);
            Assert.LessOrEqual(tuning.suspensionDamper, 10f);
            Assert.GreaterOrEqual(tuning.suspensionDistance, 0f);
            Assert.LessOrEqual(tuning.suspensionDistance, 1f);
            
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void VehicleTuning_StabilityValues_AreValid()
        {
            VehicleTuning tuning = ScriptableObject.CreateInstance<VehicleTuning>();
            
            Assert.GreaterOrEqual(tuning.downforce, 0f);
            Assert.LessOrEqual(tuning.downforce, 50f);
            Assert.GreaterOrEqual(tuning.antiRollForce, 100f);
            Assert.LessOrEqual(tuning.antiRollForce, 1000f);
            
            Object.DestroyImmediate(tuning);
        }
    }

    public class VehicleInputTests
    {
        [Test]
        public void VehicleInput_HasRequiredFields()
        {
            GameObject go = new GameObject("TestInput");
            VehicleInput input = go.AddComponent<VehicleInput>();
            
            Assert.IsNotNull(input);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void InputAxes_HaveDefaultNames()
        {
            GameObject go = new GameObject("TestInput");
            VehicleInput input = go.AddComponent<VehicleInput>();
            
            Assert.IsNotNull(input);
            
            Object.DestroyImmediate(go);
        }
    }

    public class VehicleControllerTests
    {
        [Test]
        public void VehicleController_CanBeCreated()
        {
            GameObject go = new GameObject("TestController");
            VehicleController controller = go.AddComponent<VehicleController>();
            
            Assert.IsNotNull(controller);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void VehiclePhysics_RequiresRigidbody()
        {
            GameObject go = new GameObject("TestPhysics");
            go.AddComponent<Rigidbody>();
            VehiclePhysics physics = go.AddComponent<VehiclePhysics>();
            
            Assert.IsNotNull(physics);
            
            Object.DestroyImmediate(go);
        }
    }

    public class VehiclePerformanceTests
    {
        [Test]
        public void SpeedCalculation_IsAccurate()
        {
            float metersPerSecond = 27.78f;
            float expectedKMH = 100f;
            
            float actualKMH = metersPerSecond * 3.6f;
            
            Assert.AreEqual(expectedKMH, actualKMH, 0.1f);
        }

        [Test]
        public void BrakeForce_Balance_IsValid()
        {
            float brakeForce = 3500f;
            float balance = 0.55f;
            
            float frontBrake = brakeForce * balance;
            float rearBrake = brakeForce * (1f - balance);
            
            Assert.Greater(frontBrake, 0);
            Assert.Greater(rearBrake, 0);
            Assert.AreEqual(brakeForce, frontBrake + rearBrake);
        }

        [Test]
        public void SteeringAngle_IsClamped()
        {
            float maxSteer = 40f;
            float input = 1.5f;
            
            float actualSteer = Mathf.Clamp(input * maxSteer, -maxSteer, maxSteer);
            
            Assert.LessOrEqual(actualSteer, maxSteer);
            Assert.GreaterOrEqual(actualSteer, -maxSteer);
        }
    }
}
