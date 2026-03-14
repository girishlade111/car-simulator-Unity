using UnityEngine;

namespace CarSimulator.Camera
{
    public class FollowCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform m_target;
        [SerializeField] private Vector3 m_offset = new Vector3(0, 4, -8);

        [Header("Settings")]
        [SerializeField] private float m_followSpeed = 5f;
        [SerializeField] private float m_rotationSpeed = 3f;
        [SerializeField] private float m_lookAheadDistance = 2f;

        [Header("Collision")]
        [SerializeField] private float m_minDistance = 2f;
        [SerializeField] private LayerMask m_obstacleLayers;

        private Vector3 m_velocity;

        private void Start()
        {
            if (m_target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) m_target = player.transform;
            }

            if (m_target != null)
            {
                transform.position = m_target.TransformPoint(m_offset);
                transform.LookAt(m_target);
            }
        }

        private void LateUpdate()
        {
            if (m_target == null) return;

            Vector3 targetPos = m_target.position;
            Vector3 desiredPos = m_target.TransformPoint(m_offset);

            HandleCollision(ref desiredPos, targetPos);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref m_velocity, 1f / m_followSpeed);

            Vector3 lookTarget = targetPos + m_target.forward * m_lookAheadDistance;
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        }

        private void HandleCollision(ref Vector3 cameraPos, Vector3 targetPos)
        {
            Vector3 direction = cameraPos - targetPos;
            float distance = direction.magnitude;

            if (Physics.Raycast(targetPos, direction, out RaycastHit hit, distance, m_obstacleLayers))
            {
                cameraPos = targetPos + direction.normalized * Mathf.Max(hit.distance - m_minDistance, m_minDistance);
            }
        }

        public void SetTarget(Transform target) => m_target = target;
    }
}
