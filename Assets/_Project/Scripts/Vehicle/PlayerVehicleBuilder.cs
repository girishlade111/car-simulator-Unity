using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class PlayerVehicleBuilder : MonoBehaviour
    {
        [Header("Vehicle Settings")]
        [SerializeField] private string m_vehicleName = "PlayerCar";
        [SerializeField] private Vector3 m_bodySize = new Vector3(2f, 0.8f, 4f);
        [SerializeField] private Vector3 m_cabinSize = new Vector3(1.8f, 0.7f, 2f);
        [SerializeField] private Color m_bodyColor = Color.red;
        [SerializeField] private Color m_cabinColor = Color.gray;

        [Header("Wheel Settings")]
        [SerializeField] private float m_wheelRadius = 0.35f;
        [SerializeField] private float m_wheelWidth = 0.25f;
        [SerializeField] private float m_frontWheelPositionZ = 1.2f;
        [SerializeField] private float m_rearWheelPositionZ = -1.2f;
        [SerializeField] private float m_wheelPositionX = 1f;

        [Header("Effects")]
        [SerializeField] private bool m_addWheelParticles = true;

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
            GameObject hood = CreateHood(vehicle.transform);
            Transform[] wheels = CreateWheels(vehicle.transform);
            
            WheelCollider[] colliders = CreateWheelColliders(vehicle.transform, wheels);
            AddVehicleComponents(vehicle.transform, colliders);

            if (m_addWheelParticles)
            {
                AddWheelParticles(vehicle.transform, colliders);
            }

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

            ApplyColor(body, m_bodyColor);
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

            ApplyColor(cabin, m_cabinColor);
            return cabin;
        }

        private GameObject CreateHood(Transform parent)
        {
            GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hood.name = "Hood";
            hood.transform.SetParent(parent);
            hood.transform.localPosition = new Vector3(0, 0.9f, 1.3f);
            hood.transform.localScale = new Vector3(1.6f, 0.2f, 1.2f);
            hood.layer = parent.gameObject.layer;

            ApplyColor(hood, m_bodyColor * 0.8f);
            return hood;
        }

        private void ApplyColor(GameObject obj, Color color)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = color;
            }
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

                ApplyColor(wheel, Color.black);
                
                wheels[i] = wheel.transform;
            }

            return wheels;
        }

        private WheelCollider[] CreateWheelColliders(Transform parent, Transform[] wheelMeshes)
        {
            WheelCollider[] allColliders = new WheelCollider[4];

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
                allColliders[i] = frontCollider;
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
                allColliders[i + 2] = rearCollider;
            }

            WheelVisuals[] visuals = parent.GetComponentsInChildren<WheelVisuals>();
            for (int i = 0; i < wheelMeshes.Length && i < visuals.Length; i++)
            {
                visuals[i].m_wheelMesh = wheelMeshes[i];
            }

            return allColliders;
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

        private void AddVehicleComponents(Transform parent, WheelCollider[] colliders)
        {
            GameObject centerOfMass = new GameObject("CenterOfMass");
            centerOfMass.transform.SetParent(parent);
            centerOfMass.transform.localPosition = new Vector3(0, 0.3f, 0);

            VehicleInput input = parent.gameObject.AddComponent<VehicleInput>();
            
            VehiclePhysics physics = parent.gameObject.AddComponent<VehiclePhysics>();
            physics.m_input = input;
            physics.m_tuning = m_tuning;
            physics.m_centerOfMass = centerOfMass.transform;

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

            AddEngineAudio(parent.gameObject, physics);
            GearSystem gearSystem = AddGearSystem(parent.gameObject, physics);
            AddTireScreech(parent.gameObject, physics);
            AddLaunchControl(parent.gameObject, physics, gearSystem);
            AddTurboBoost(parent.gameObject, physics);
            AddGearShiftAnimation(parent.gameObject);
            AddNitrousOxide(parent.gameObject, physics);
            AddVehicleDamage(parent.gameObject);
            AddVehicleLights(parent.gameObject);
            AddNeonUnderglow(parent.gameObject);
            AddCarHorn(parent.gameObject);
            AddWindshieldEffects(parent.gameObject);
            AddCrashSounds(parent.gameObject);
            AddMirrors(parent.gameObject);
            AddCarRadio(parent.gameObject);
            AddVehicleWrap(parent.gameObject);
        }

        private void AddVehicleWrap(GameObject vehicle)
        {
            VehicleWrap wrap = vehicle.AddComponent<VehicleWrap>();
            wrap.m_enableWraps = true;
            wrap.m_currentWrapColor = m_bodyColor;
        }

        private void AddCarRadio(GameObject vehicle)
        {
            CarRadio radio = vehicle.AddComponent<CarRadio>();
            radio.m_radioEnabled = true;
        }

        private void AddMirrors(GameObject vehicle)
        {
            GameObject leftMirror = CreateMirror(vehicle, new Vector3(-1.1f, 1f, -0.5f), VehicleMirror.MirrorType.Side);
            GameObject rightMirror = CreateMirror(vehicle, new Vector3(1.1f, 1f, -0.5f), VehicleMirror.MirrorType.Side);
            GameObject rearMirror = CreateMirror(vehicle, new Vector3(0, 1.2f, -1.8f), VehicleMirror.MirrorType.Rear);
        }

        private GameObject CreateMirror(GameObject vehicle, Vector3 localPos, VehicleMirror.MirrorType type)
        {
            GameObject mirrorObj = new GameObject($"Mirror_{type}");
            mirrorObj.transform.SetParent(vehicle.transform);
            mirrorObj.transform.localPosition = localPos;

            GameObject mirrorSurface = GameObject.CreatePrimitive(PrimitiveType.Quad);
            mirrorSurface.transform.SetParent(mirrorObj.transform);
            mirrorSurface.transform.localPosition = Vector3.zero;
            mirrorSurface.transform.localRotation = type == VehicleMirror.MirrorType.Side ? 
                Quaternion.Euler(0, 90, 0) : Quaternion.identity;
            mirrorSurface.transform.localScale = new Vector3(0.3f, 0.2f, 1f);

            VehicleMirror mirror = mirrorObj.AddComponent<VehicleMirror>();
            mirror.m_mirrorType = type;

            return mirrorObj;
        }

        public void SetBodyColor(Color color)
        {
            m_bodyColor = color;
        }
    }
}
