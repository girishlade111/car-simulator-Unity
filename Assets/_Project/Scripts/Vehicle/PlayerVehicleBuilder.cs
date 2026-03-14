using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class PlayerVehicleBuilder : MonoBehaviour
    {
        [Header("Vehicle Settings")]
        [SerializeField] private string m_vehicleName = "PlayerCar";
        [SerializeField] private Vector3 m_bodySize = new Vector3(2f, 0.8f, 4f);
        [SerializeField] private Vector3 m_cabinSize = new Vector3(1.8f, 0.7f, 2f);

        [Header("Wheel Settings")]
        [SerializeField] private float m_wheelRadius = 0.35f;
        [SerializeField] private float m_wheelWidth = 0.25f;
        [SerializeField] private float m_frontWheelPositionZ = 1.2f;
        [SerializeField] private float m_rearWheelPositionZ = -1.2f;
        [SerializeField] private float m_wheelPositionX = 1f;

        [Header("Components")]
        [SerializeField] private VehicleTuning m_tuning;

        public GameObject BuildVehicle()
        {
            GameObject vehicle = new GameObject(m_vehicleName);
            vehicle.tag = "Player";
            vehicle.layer = LayerMask.NameToLayer("Player");

            Rigidbody rb = vehicle.AddComponent<Rigidbody>();
            rb.mass = m_tuning != null ? m_tuning.mass : 1500f;

            GameObject body = CreateBody(vehicle.transform);
            GameObject cabin = CreateCabin(vehicle.transform);
            Transform[] wheels = CreateWheels(vehicle.transform);
            
            CreateWheelColliders(vehicle.transform, wheels);
            AddVehicleComponents(vehicle.transform);

            return vehicle;
        }

        private GameObject CreateBody(Transform parent)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(parent);
            body.transform.localPosition = new Vector3(0, 0.5f, 0);
            body.transform.localScale = m_bodySize;
            body.layer = parent.gameObject.layer;
            return body;
        }

        private GameObject CreateCabin(Transform parent)
        {
            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(parent);
            cabin.transform.localPosition = new Vector3(0, 1.2f, -0.3f);
            cabin.transform.localScale = m_cabinSize;
            cabin.layer = parent.gameObject.layer;
            return cabin;
        }

        private Transform[] CreateWheels(Transform parent)
        {
            Transform[] wheels = new Transform[4];
            string[] wheelNames = { "Wheel_FL", "Wheel_FR", "Wheel_RL", "Wheel_RR" };
            Vector3[] positions = {
                new Vector3(-m_wheelPositionX, 0.35f, m_frontWheelPositionZ),
                new Vector3(m_wheelPositionX, 0.35f, m_frontWheelPositionZ),
                new Vector3(-m_wheelPositionX, 0.35f, m_rearWheelPositionZ),
                new Vector3(m_wheelPositionX, 0.35f, m_rearWheelPositionZ)
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                wheel.name = wheelNames[i];
                wheel.transform.SetParent(parent);
                wheel.transform.localPosition = positions[i];
                wheel.transform.localScale = new Vector3(m_wheelRadius * 2f, m_wheelRadius, m_wheelRadius);
                wheel.transform.rotation = Quaternion.Euler(0, 0, 90);
                wheel.layer = parent.gameObject.layer;
                
                wheels[i] = wheel.transform;
            }

            return wheels;
        }

        private void CreateWheelColliders(Transform parent, Transform[] wheelMeshes)
        {
            WheelCollider[] frontWheels = new WheelCollider[2];
            WheelCollider[] rearWheels = new WheelCollider[2];

            for (int i = 0; i < 2; i++)
            {
                GameObject frontColliderObj = new GameObject($"FrontWheel_{i}");
                frontColliderObj.transform.SetParent(parent);
                frontColliderObj.transform.localPosition = wheelMeshes[i].localPosition;
                frontColliderObj.transform.localRotation = Quaternion.identity;

                WheelCollider frontCollider = frontColliderObj.AddComponent<WheelCollider>();
                frontCollider.radius = m_wheelRadius;
                frontCollider.width = m_wheelWidth;
                ConfigureWheelCollider(frontCollider);
                frontWheels[i] = frontCollider;
            }

            for (int i = 0; i < 2; i++)
            {
                GameObject rearColliderObj = new GameObject($"RearWheel_{i}");
                rearColliderObj.transform.SetParent(parent);
                rearColliderObj.transform.localPosition = wheelMeshes[i + 2].localPosition;
                rearColliderObj.transform.localRotation = Quaternion.identity;

                WheelCollider rearCollider = rearColliderObj.AddComponent<WheelCollider>();
                rearCollider.radius = m_wheelRadius;
                rearCollider.width = m_wheelWidth;
                ConfigureWheelCollider(rearCollider);
                rearWheels[i] = rearCollider;
            }

            WheelVisuals[] visuals = parent.GetComponentsInChildren<WheelVisuals>();
            for (int i = 0; i < wheelMeshes.Length && i < visuals.Length; i++)
            {
                visuals[i].m_wheelMesh = wheelMeshes[i];
            }
        }

        private void ConfigureWheelCollider(WheelCollider collider)
        {
            WheelFrictionCurve curve = collider.forwardFriction;
            curve.stiffness = 1f;
            curve.asymptoteValue = 0.5f;
            curve.asymptoteSlope = 0.1f;
            curve.extremumValue = 1f;
            curve.extremumSlope = 0.1f;
            collider.forwardFriction = curve;

            curve = collider.sideFriction;
            curve.stiffness = 1f;
            curve.asymptoteValue = 0.5f;
            curve.asymptoteSlope = 0.1f;
            curve.extremumValue = 1f;
            curve.extremumSlope = 0.1f;
            collider.sideFriction = curve;

            collider.alignedFriction = 0.1f;
        }

        private void AddVehicleComponents(Transform parent)
        {
            GameObject centerOfMass = new GameObject("CenterOfMass");
            centerOfMass.transform.SetParent(parent);
            centerOfMass.transform.localPosition = new Vector3(0, 0.3f, 0);

            VehicleInput input = parent.gameObject.AddComponent<VehicleInput>();
            
            VehiclePhysics physics = parent.gameObject.AddComponent<VehiclePhysics>();
            physics.m_input = input;
            physics.m_tuning = m_tuning;
            physics.m_centerOfMass = centerOfMass.transform;

            var colliders = parent.GetComponentsInChildren<WheelCollider>();
            WheelCollider[] front = new WheelCollider[2];
            WheelCollider[] rear = new WheelCollider[2];

            int frontIndex = 0, rearIndex = 0;
            foreach (var col in colliders)
            {
                if (col.transform.localPosition.z > 0)
                {
                    front[frontIndex++] = col;
                }
                else
                {
                    rear[rearIndex++] = col;
                }
            }

            physics.m_frontWheels = front;
            physics.m_rearWheels = rear;

            parent.gameObject.AddComponent<VehicleController>();
        }

        public void SetTuning(VehicleTuning tuning)
        {
            m_tuning = tuning;
        }
    }
}
