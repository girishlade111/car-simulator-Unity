using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.AI
{
    public class TrafficCar : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float m_maxSpeed = 60f;
        [SerializeField] private float m_acceleration = 5f;
        [SerializeField] private float m_steeringSpeed = 2f;
        [SerializeField] private float m_followDistance = 10f;
        [SerializeField] private float m_avoidanceDistance = 15f;

        [Header("Path Following")]
        [SerializeField] private bool m_followPath = true;
        [SerializeField] private Transform[] m_waypoints;
        [SerializeField] private int m_currentWaypoint;
        [SerializeField] private bool m_loopPath = true;

        [Header("Obstacle Detection")]
        [SerializeField] private LayerMask m_obstacleLayer;
        [SerializeField] private float m_raycastDistance = 20f;

        private Rigidbody m_rb;
        private bool m_isPlayerNearby;
        private Transform m_playerTransform;

        private void Start()
        {
            m_rb = GetComponent<Rigidbody>();
            if (m_rb == null)
            {
                m_rb = gameObject.AddComponent<Rigidbody>();
            }
            m_rb.mass = 1200f;
            m_rb.useGravity = true;
            m_rb.isKinematic = false;
            m_rb.interpolation = RigidbodyInterpolation.Interpolate;

            FindPlayer();
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
            if (m_playerTransform != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, m_playerTransform.position);
                m_isPlayerNearby = distToPlayer < 100f;
            }

            if (!m_isPlayerNearby && m_followPath)
            {
                MoveAlongPath();
            }
            else
            {
                DriveForward();
            }

            AvoidObstacles();
        }

        private void MoveAlongPath()
        {
            if (m_waypoints == null || m_waypoints.Length == 0) return;

            Transform target = m_waypoints[m_currentWaypoint];
            if (target == null) return;

            Vector3 direction = (target.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance < 3f)
            {
                m_currentWaypoint++;
                if (m_currentWaypoint >= m_waypoints.Length)
                {
                    m_currentWaypoint = m_loopPath ? 0 : m_waypoints.Length - 1;
                }
            }

            SteerTowards(direction);
            Accelerate();
        }

        private void DriveForward()
        {
            SteerTowards(transform.forward);
            Accelerate();
        }

        private void SteerTowards(Vector3 direction)
        {
            Vector3 steerDir = transform.InverseTransformDirection(direction);
            float steerAmount = Mathf.Atan2(steerDir.x, steerDir.z) * Mathf.Rad2Deg;
            
            float steer = Mathf.Clamp(steerAmount * m_steeringSpeed * Time.fixedDeltaTime, -1f, 1f);
            transform.Rotate(Vector3.up, steer * m_steeringSpeed);
        }

        private void Accelerate()
        {
            float currentSpeed = m_rb.velocity.magnitude * 3.6f;
            
            if (currentSpeed < m_maxSpeed)
            {
                m_rb.AddForce(transform.forward * m_acceleration, ForceMode.Acceleration);
            }
        }

        private void AvoidObstacles()
        {
            RaycastHit hit;
            Vector3[] directions = { transform.forward, transform.forward + transform.right * 0.5f, transform.forward - transform.right * 0.5f };

            foreach (var dir in directions)
            {
                if (Physics.Raycast(transform.position, dir, out hit, m_raycastDistance, m_obstacleLayer))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        continue;
                    }

                    Vector3 avoidDir = Vector3.Reflect(dir, hit.normal);
                    transform.Rotate(Vector3.up, avoidDir.x * 2f);
                    m_rb.AddForce(-transform.forward * m_acceleration * 0.5f, ForceMode.Acceleration);
                }
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
    }
}
