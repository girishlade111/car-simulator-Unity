using UnityEngine;

namespace CarSimulator.Camera
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform Target => m_target;

        [Header("Target")]
        [SerializeField] private Transform m_target;
        [SerializeField] private Vector3 m_offset = new Vector3(0, 5, -10);

        [Header("Settings")]
        [SerializeField] private float m_followSpeed = 8f;
        [SerializeField] private float m_rotationSpeed = 5f;
        [SerializeField] private float m_lookAheadDistance = 5f;

        [Header("Vehicle Camera")]
        [SerializeField] private bool m_useVehicleHeight = true;
        [SerializeField] private float m_minHeight = 3f;
        [SerializeField] private float m_speedHeightBonus = 0.02f;

        [Header("Collision")]
        [SerializeField] private float m_minDistance = 2f;
        [SerializeField] private LayerMask m_obstacleLayers;

        [Header("Input-based Rotation")]
        [SerializeField] private bool m_enableInputRotation = true;
        [SerializeField] private float m_inputRotationSpeed = 2f;

        private Vector3 m_velocity;
        private float m_currentHeightBonus;

        private void Start()
        {
            FindTarget();
        }

        public void FindTarget()
        {
            if (m_target != null) return;

            var vehicleSpawner = FindObjectOfType<Vehicle.VehicleSpawner>();
            if (vehicleSpawner != null && vehicleSpawner.VehicleTransform != null)
            {
                m_target = vehicleSpawner.VehicleTransform;
            }
            else
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) m_target = player.transform;
            }

            if (m_target != null)
            {
                Vector3 initialPos = m_target.TransformPoint(m_offset);
                transform.position = initialPos;
                transform.LookAt(m_target);
            }
        }

        public void SetTarget(Transform target)
        {
            m_target = target;
        }

        private void LateUpdate()
        {
            if (m_target == null)
            {
                FindTarget();
                if (m_target == null) return;
            }

            UpdateHeightBonus();
            Vector3 targetPos = m_target.position;
            Vector3 adjustedOffset = GetAdjustedOffset();
            Vector3 desiredPos = m_target.TransformPoint(adjustedOffset);

            HandleCollision(ref desiredPos, targetPos);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref m_velocity, 1f / m_followSpeed);

            Vector3 lookTarget = targetPos + m_target.forward * m_lookAheadDistance;
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        }

        private void UpdateHeightBonus()
        {
            if (!m_useVehicleHeight) return;

            var rb = m_target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float speed = rb.velocity.magnitude;
                m_currentHeightBonus = Mathf.Lerp(m_currentHeightBonus, speed * m_speedHeightBonus, Time.deltaTime * 2f);
            }
        }

        private Vector3 GetAdjustedOffset()
        {
            Vector3 offset = m_offset;
            offset.y += m_currentHeightBonus;
            offset.y = Mathf.Max(offset.y, m_minHeight);
            return offset;
        }

        private void HandleCollision(ref Vector3 cameraPos, Vector3 targetPos)
        {
            Vector3 direction = cameraPos - targetPos;
            float distance = direction.magnitude;

            if (distance < 0.1f) return;

            if (Physics.Raycast(targetPos, direction, out RaycastHit hit, distance, m_obstacleLayers))
            {
                cameraPos = targetPos + direction.normalized * Mathf.Max(hit.distance - m_minDistance, m_minDistance);
            }
        }
    }
}
