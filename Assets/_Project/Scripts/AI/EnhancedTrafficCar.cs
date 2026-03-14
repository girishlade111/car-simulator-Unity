using UnityEngine;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.AI
{
    public class EnhancedTrafficCar : MonoBehaviour
    {
        public enum VehicleType
        {
            Sedan,
            SUV,
            Truck,
            SportsCar,
            Taxi,
            Police,
            Ambulance,
            Bus,
            Motorcycle
        }

        [Header("Vehicle Type")]
        [SerializeField] private VehicleType m_vehicleType = VehicleType.Sedan;

        [Header("AI Settings")]
        [SerializeField] private float m_maxSpeed = 60f;
        [SerializeField] private float m_acceleration = 5f;
        [SerializeField] private float m_brakeForce = 8f;
        [SerializeField] private float m_steeringSpeed = 2f;
        [SerializeField] private float m_followDistance = 15f;
        [SerializeField] private float m_safeDistance = 8f;

        [Header("Path Following")]
        [SerializeField] private bool m_followPath = true;
        [SerializeField] private Transform[] m_waypoints;
        [SerializeField] private int m_currentWaypoint;
        [SerializeField] private bool m_loopPath = true;

        [Header("Obstacle Detection")]
        [SerializeField] private LayerMask m_obstacleLayer;
        [SerializeField] private float m_raycastDistance = 25f;
        [SerializeField] private float m_sideRayDistance = 10f;

        [Header("Behavior")]
        [SerializeField] private bool m_obeyTrafficLights = true;
        [SerializeField] private bool m_yieldToPedestrians = true;
        [SerializeField] private bool m_useSignals = true;
        [SerializeField] private float m_reactionTime = 0.5f;

        [Header("Visual")]
        [SerializeField] private Renderer[] m_bodyRenderers;
        [SerializeField] private Light[] m_headlights;
        [SerializeField] private Light[] m_taillights;
        [SerializeField] private GameObject m_leftSignal;
        [SerializeField] private GameObject m_rightSignal;
        [SerializeField] private GameObject m_brakeLights;

        private Rigidbody m_rb;
        private bool m_isPlayerNearby;
        private Transform m_playerTransform;
        private float m_currentSpeed;
        private bool m_isBraking;
        private bool m_isTurning;
        private float m_signalTimer;
        private int m_laneOffset;
        private float m_stuckTimer;
        private Vector3 m_lastPosition;

        private Dictionary<VehicleType, VehicleTypeSettings> m_typeSettings;

        private struct VehicleTypeSettings
        {
            public float maxSpeed;
            public float acceleration;
            public float brakeForce;
            public Color defaultColor;
            public bool hasSiren;
            public float length;
            public float width;
        }

        private void Awake()
        {
            InitializeTypeSettings();
        }

        private void InitializeTypeSettings()
        {
            m_typeSettings = new Dictionary<VehicleType, VehicleTypeSettings>
            {
                { VehicleType.Sedan, new VehicleTypeSettings { maxSpeed = 80f, acceleration = 6f, brakeForce = 8f, defaultColor = Color.gray, hasSiren = false, length = 4.5f, width = 1.8f } },
                { VehicleType.SUV, new VehicleTypeSettings { maxSpeed = 70f, acceleration = 5f, brakeForce = 7f, defaultColor = Color.black, hasSiren = false, length = 5f, width = 2f } },
                { VehicleType.Truck, new VehicleTypeSettings { maxSpeed = 50f, acceleration = 3f, brakeForce = 6f, defaultColor = Color.white, hasSiren = false, length = 8f, width = 2.5f } },
                { VehicleType.SportsCar, new VehicleTypeSettings { maxSpeed = 120f, acceleration = 10f, brakeForce = 12f, defaultColor = Color.red, hasSiren = false, length = 4.5f, width = 1.9f } },
                { VehicleType.Taxi, new VehicleTypeSettings { maxSpeed = 70f, acceleration = 5f, brakeForce = 7f, defaultColor = new Color(1f, 0.9f, 0.2f), hasSiren = false, length = 4.5f, width = 1.8f } },
                { VehicleType.Police, new VehicleTypeSettings { maxSpeed = 100f, acceleration = 8f, brakeForce = 10f, defaultColor = Color.black, hasSiren = true, length = 4.8f, width = 1.9f } },
                { VehicleType.Ambulance, new VehicleTypeSettings { maxSpeed = 90f, acceleration = 6f, brakeForce = 8f, defaultColor = Color.white, hasSiren = true, length = 5.5f, width = 2f } },
                { VehicleType.Bus, new VehicleTypeSettings { maxSpeed = 40f, acceleration = 2f, brakeForce = 5f, defaultColor = Color.yellow, hasSiren = false, length = 12f, width = 2.5f } },
                { VehicleType.Motorcycle, new VehicleTypeSettings { maxSpeed = 110f, acceleration = 9f, brakeForce = 10f, defaultColor = Color.black, hasSiren = false, length = 2.5f, width = 0.8f } }
            };
        }

        private void Start()
        {
            SetupRigidbody();
            SetupVehicle();
            FindPlayer();
            m_lastPosition = transform.position;
        }

        private void SetupRigidbody()
        {
            m_rb = GetComponent<Rigidbody>();
            if (m_rb == null)
            {
                m_rb = gameObject.AddComponent<Rigidbody>();
            }

            VehicleTypeSettings settings = GetCurrentSettings();
            m_rb.mass = GetMassForType();
            m_rb.useGravity = true;
            m_rb.isKinematic = false;
            m_rb.interpolation = RigidbodyInterpolation.Interpolate;
            m_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            m_maxSpeed = settings.maxSpeed;
            m_acceleration = settings.acceleration;
            m_brakeForce = settings.brakeForce;
        }

        private float GetMassForType()
        {
            switch (m_vehicleType)
            {
                case VehicleType.Motorcycle: return 200f;
                case VehicleType.SportsCar: return 1200f;
                case VehicleType.Sedan: return 1400f;
                case VehicleType.SUV: return 1800f;
                case VehicleType.Taxi: return 1400f;
                case VehicleType.Police: return 1500f;
                case VehicleType.Ambulance: return 2000f;
                case VehicleType.Truck: return 3000f;
                case VehicleType.Bus: return 8000f;
                default: return 1400f;
            }
        }

        private void SetupVehicle()
        {
            if (m_bodyRenderers == null || m_bodyRenderers.Length == 0)
            {
                m_bodyRenderers = GetComponentsInChildren<Renderer>();
            }

            ApplyVehicleColor();

            if (m_useSignals)
            {
                SetupSignals();
            }
        }

        private VehicleTypeSettings GetCurrentSettings()
        {
            if (m_typeSettings.ContainsKey(m_vehicleType))
            {
                return m_typeSettings[m_vehicleType];
            }
            return m_typeSettings[VehicleType.Sedan];
        }

        private void ApplyVehicleColor()
        {
            VehicleTypeSettings settings = GetCurrentSettings();
            if (m_bodyRenderers != null)
            {
                foreach (var renderer in m_bodyRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = settings.defaultColor;
                    }
                }
            }
        }

        private void SetupSignals()
        {
            if (m_leftSignal == null)
            {
                m_leftSignal = CreateSignalLight(new Vector3(-0.8f, 0.3f, 1.8f), Color.green);
            }
            if (m_rightSignal == null)
            {
                m_rightSignal = CreateSignalLight(new Vector3(0.8f, 0.3f, 1.8f), Color.green);
            }
            if (m_brakeLights == null)
            {
                m_brakeLights = CreateSignalLight(new Vector3(0, 0.4f, -2.2f), Color.red);
            }
        }

        private GameObject CreateSignalLight(Vector3 position, Color color)
        {
            GameObject lightObj = new GameObject("SignalLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = position;

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = 0f;
            light.range = 3f;

            return lightObj;
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void FixedUpdate()
        {
            CheckIfStuck();
            UpdatePlayerProximity();
            UpdateAI();
            UpdateVisuals();
        }

        private void CheckIfStuck()
        {
            float movedDistance = Vector3.Distance(transform.position, m_lastPosition);
            m_lastPosition = transform.position;

            if (movedDistance < 0.1f)
            {
                m_stuckTimer += Time.fixedDeltaTime;
                if (m_stuckTimer > 3f)
                {
                    TryUnstuck();
                }
            }
            else
            {
                m_stuckTimer = 0f;
            }
        }

        private void TryUnstuck()
        {
            transform.Rotate(Vector3.up, Random.Range(-30f, 30f));
            m_rb.AddForce(transform.forward * m_acceleration * 2f, ForceMode.Impulse);
            m_stuckTimer = 0f;
        }

        private void UpdatePlayerProximity()
        {
            if (m_playerTransform != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
                m_isPlayerNearby = distToPlayer < 150f;

                if (m_isPlayerNearby && m_vehicleType == VehicleType.Police)
                {
                    CheckPoliceSiren();
                }
            }
        }

        private void CheckPoliceSiren()
        {
            VehicleTypeSettings settings = GetCurrentSettings();
            if (settings.hasSiren)
            {
                // TODO: Trigger siren audio and lights
            }
        }

        private void UpdateAI()
        {
            if (!m_isPlayerNearby && m_followPath)
            {
                MoveAlongPath();
            }
            else
            {
                DriveForward();
            }

            DetectAndAvoidObstacles();
            HandleBraking();
        }

        private void MoveAlongPath()
        {
            if (m_waypoints == null || m_waypoints.Length == 0) return;

            Transform target = m_waypoints[m_currentWaypoint];
            if (target == null) return;

            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance < 5f + (m_currentSpeed * 0.1f))
            {
                m_currentWaypoint++;
                if (m_currentWaypoint >= m_waypoints.Length)
                {
                    m_currentWaypoint = m_loopPath ? 0 : m_waypoints.Length - 1;
                }
            }

            SteerTowards(direction);
            Drive();
        }

        private void DriveForward()
        {
            SteerTowards(transform.forward);
            Drive();
        }

        private void SteerTowards(Vector3 direction)
        {
            Vector3 steerDir = transform.InverseTransformDirection(direction);
            float steerAmount = Mathf.Atan2(steerDir.x, steerDir.z) * Mathf.Rad2Deg;
            
            float steer = Mathf.Clamp(steerAmount * m_steeringSpeed * Time.fixedDeltaTime, -1f, 1f);
            transform.Rotate(Vector3.up, steer * m_steeringSpeed);
        }

        private void Drive()
        {
            m_currentSpeed = m_rb.velocity.magnitude * 3.6f;

            if (!m_isBraking && m_currentSpeed < m_maxSpeed)
            {
                m_rb.AddForce(transform.forward * m_acceleration, ForceMode.Acceleration);
            }
        }

        private void HandleBraking()
        {
            if (m_brakeLights != null)
            {
                Light brakeLight = m_brakeLights.GetComponent<Light>();
                if (brakeLight != null)
                {
                    brakeLight.intensity = m_isBraking ? 2f : 0f;
                }
            }
        }

        private void DetectAndAvoidObstacles()
        {
            m_isBraking = false;

            Vector3[] directions = {
                transform.forward,
                transform.forward + transform.right * 0.3f,
                transform.forward - transform.right * 0.3f,
                transform.forward * 0.5f + transform.right * 0.5f,
                transform.forward * 0.5f - transform.right * 0.5f
            };

            float closestDist = float.MaxValue;
            Vector3 avoidDirection = Vector3.zero;

            foreach (var dir in directions)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dir, out hit, m_raycastDistance, m_obstacleLayer))
                {
                    if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Traffic"))
                    {
                        continue;
                    }

                    float dist = hit.distance;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        avoidDirection = Vector3.Reflect(dir, hit.normal);
                    }

                    if (dist < m_safeDistance)
                    {
                        m_isBraking = true;
                        m_rb.AddForce(-transform.forward * m_brakeForce, ForceMode.Acceleration);
                    }
                }
            }

            if (avoidDirection != Vector3.zero && closestDist < m_raycastDistance * 0.5f)
            {
                transform.Rotate(Vector3.up, avoidDirection.x * 3f);
            }
        }

        private void UpdateVisuals()
        {
            if (m_useSignals && Time.time > m_signalTimer + 3f)
            {
                m_signalTimer = Time.time;
                bool left = Random.value > 0.5f;
                UpdateSignals(left ? 1 : -1);
            }
        }

        private void UpdateSignals(int direction)
        {
            if (m_leftSignal != null)
            {
                m_leftSignal.GetComponent<Light>().intensity = direction == -1 ? 1f : 0f;
            }
            if (m_rightSignal != null)
            {
                m_rightSignal.GetComponent<Light>().intensity = direction == 1 ? 1f : 0f;
            }
        }

        public void SetWaypoints(Transform[] waypoints)
        {
            m_waypoints = waypoints;
        }

        public void SetSpeed(float speed)
        {
            m_maxSpeed = speed;
        }

        public void SetVehicleType(VehicleType type)
        {
            m_vehicleType = type;
            SetupRigidbody();
            SetupVehicle();
        }

        public float GetCurrentSpeed() => m_currentSpeed;
    }
}
