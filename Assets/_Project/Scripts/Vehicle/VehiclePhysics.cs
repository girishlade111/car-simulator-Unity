using UnityEngine;

namespace CarSimulator.Vehicle
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehiclePhysics : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VehicleInput m_input;
        [SerializeField] private VehicleTuning m_tuning;
        [SerializeField] private Transform m_centerOfMass;
        [SerializeField] private WheelCollider[] m_frontWheels;
        [SerializeField] private WheelCollider[] m_rearWheels;

        [Header("State")]
        [SerializeField] private float m_currentSpeed;
        [SerializeField] private bool m_isDrifting;
        public float CurrentSpeed => m_currentSpeed;
        public float MaxSpeed => m_tuning != null ? m_tuning.maxSpeed : 150f;
        public bool IsDrifting => m_isDrifting;

        private Rigidbody m_rb;
        private bool m_isFlipped;
        private float m_flipTimer;

        private void Awake()
        {
            m_rb = GetComponent<Rigidbody>();
            m_rb.mass = m_tuning != null ? m_tuning.mass : 1500f;

            if (m_centerOfMass != null)
            {
                m_rb.centerOfMass = m_centerOfMass.localPosition;
            }

            SetupSuspension();
        }

        private void SetupSuspension()
        {
            if (m_tuning == null) return;

            WheelCollider[] allWheels = GetComponentsInChildren<WheelCollider>();
            foreach (var wheel in allWheels)
            {
                JointSpring spring = wheel.suspensionSpring;
                spring.spring = m_tuning.suspensionSpring;
                spring.damper = m_tuning.suspensionDamper;
                wheel.suspensionSpring = spring;
                wheel.suspensionDistance = m_tuning.suspensionDistance;
            }
        }

        private void FixedUpdate()
        {
            if (m_input == null || m_tuning == null) return;

            ApplySteering();
            ApplyThrottle();
            ApplyBraking();
            ApplyDownforce();
            Stabilize();

            m_currentSpeed = m_rb.velocity.magnitude * 3.6f;
            CheckFlipped();
        }

        private void ApplySteering()
        {
            float steer = m_input.SteerInput * m_tuning.maxSteerAngle;
            foreach (var wheel in m_frontWheels)
            {
                wheel.steerAngle = steer;
            }
        }

        private void ApplyThrottle()
        {
            if (m_currentSpeed >= m_tuning.maxSpeed && m_input.ThrottleInput > 0f)
                return;

            float force = m_input.ThrottleInput * m_tuning.engineForce;
            foreach (var wheel in m_rearWheels)
            {
                wheel.motorTorque = force;
            }
        }

        private void ApplyBraking()
        {
            float brake = m_input.IsBraking ? m_tuning.brakeForce : 0f;
            float balance = m_tuning.brakeBalance;

            float frontBrake = brake * balance;
            float rearBrake = brake * (1f - balance);

            foreach (var wheel in m_frontWheels)
            {
                wheel.brakeTorque = frontBrake;
            }
            foreach (var wheel in m_rearWheels)
            {
                wheel.brakeTorque = rearBrake;
            }

            ApplyHandbrake();
        }

        private void ApplyHandbrake()
        {
            if (m_input.IsHandbraking)
            {
                m_isDrifting = true;

                foreach (var wheel in m_rearWheels)
                {
                    wheel.brakeTorque = m_tuning.brakeForce * 0.8f;
                }

                ReduceRearWheelFriction(0.3f);

                ApplyLateralDrift();
            }
            else
            {
                m_isDrifting = false;
                RestoreWheelFriction();
            }
        }

        private void ReduceRearWheelFriction(float factor)
        {
            foreach (var wheel in m_rearWheels)
            {
                WheelFrictionCurve curve = wheel.forwardFriction;
                curve.stiffness = factor;
                wheel.forwardFriction = curve;

                curve = wheel.sideFriction;
                curve.stiffness = factor;
                wheel.sideFriction = curve;
            }
        }

        private void RestoreWheelFriction()
        {
            foreach (var wheel in m_rearWheels)
            {
                WheelFrictionCurve curve = wheel.forwardFriction;
                curve.stiffness = 1f;
                wheel.forwardFriction = curve;

                curve = wheel.sideFriction;
                curve.stiffness = 1f;
                wheel.sideFriction = curve;
            }
        }

        private void ApplyLateralDrift()
        {
            if (m_rearWheels.Length < 2) return;

            float speed = m_rb.velocity.magnitude;
            if (speed < 5f) return;

            WheelHit hit;
            bool isGrounded = m_rearWheels[0].GetGroundHit(out hit);

            if (isGrounded && Mathf.Abs(m_input.SteerInput) > 0.1f)
            {
                float driftForce = m_input.SteerInput * speed * 5f;
                m_rb.AddForce(transform.right * driftForce, ForceMode.Acceleration);
            }
        }

        private void ApplyDownforce()
        {
            float speedFactor = Mathf.Clamp01(m_currentSpeed / m_tuning.maxSpeed);
            m_rb.AddForce(-transform.up * m_tuning.downforce * speedFactor, ForceMode.Acceleration);
        }

        private void Stabilize()
        {
            if (m_frontWheels.Length < 2 || m_rearWheels.Length < 2) return;

            WheelHit hit;
            float travelL = m_frontWheels[0].GetGroundHit(out hit) ? hit.force : 0f;
            float travelR = m_frontWheels[1].GetGroundHit(out hit) ? hit.force : 0f;

            float antiRoll = (travelL - travelR) * m_tuning.antiRollForce;
            m_rb.AddForceAtPosition(transform.right * antiRoll, m_frontWheels[0].transform.position);
            m_rb.AddForceAtPosition(transform.right * -antiRoll, m_frontWheels[1].transform.position);
        }

        private void CheckFlipped()
        {
            float upDot = Vector3.Dot(transform.up, Vector3.up);

            if (upDot < 0.2f)
            {
                m_flipTimer += Time.fixedDeltaTime;
                if (m_flipTimer > 2f)
                {
                    m_isFlipped = true;
                }
            }
            else
            {
                m_flipTimer = 0f;
                m_isFlipped = false;
            }
        }

        public void ResetVehicle()
        {
            m_rb.velocity = Vector3.zero;
            m_rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.position += Vector3.up * 2f;
            m_flipTimer = 0f;
            m_isFlipped = false;
        }

        public bool IsFlipped => m_isFlipped;
    }
}
