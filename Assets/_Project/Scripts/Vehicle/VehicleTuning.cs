using UnityEngine;

namespace CarSimulator.Vehicle
{
    [CreateAssetMenu(fileName = "VehicleTuning", menuName = "CarSimulator/Vehicle Tuning")]
    public class VehicleTuning : ScriptableObject
    {
        [Header("Steering")]
        [Range(20, 50)] public float maxSteerAngle = 40f;

        [Header("Engine")]
        [Range(500, 3000)] public float engineForce = 2000f;
        [Range(50, 250)] public float maxSpeed = 180f;

        [Header("Braking")]
        [Range(500, 5000)] public float brakeForce = 3500f;
        [Range(0.3f, 0.8f)] public float brakeBalance = 0.55f;

        [Header("Stability")]
        [Range(0, 50)] public float downforce = 15f;
        [Range(100, 1000)] public float antiRollForce = 600f;
        [Range(500, 3000)] public float mass = 1400f;

        [Header("Suspension")]
        [Range(10, 50)] public float suspensionSpring = 30f;
        [Range(0, 10)] public float suspensionDamper = 4f;
        [Range(0, 1)] public float suspensionDistance = 0.3f;

        [Header("Respawn")]
        public float respawnHeight = -15f;
        public float resetDelay = 2f;
    }
}
