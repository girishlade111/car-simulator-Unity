using UnityEngine;

namespace CarSimulator.Vehicle
{
    [CreateAssetMenu(fileName = "VehicleTuning", menuName = "CarSimulator/Vehicle Tuning")]
    public class VehicleTuning : ScriptableObject
    {
        [Header("Steering")]
        [Range(20, 50)] public float maxSteerAngle = 35f;

        [Header("Engine")]
        [Range(500, 3000)] public float engineForce = 1500f;
        [Range(50, 250)] public float maxSpeed = 150f;

        [Header("Braking")]
        [Range(500, 5000)] public float brakeForce = 3000f;
        [Range(0.3f, 0.8f)] public float brakeBalance = 0.6f;

        [Header("Stability")]
        [Range(0, 50)] public float downforce = 10f;
        [Range(100, 1000)] public float antiRollForce = 500f;
        [Range(500, 3000)] public float mass = 1500f;

        [Header("Respawn")]
        public float respawnHeight = -10f;
        public float resetDelay = 2f;
    }
}
