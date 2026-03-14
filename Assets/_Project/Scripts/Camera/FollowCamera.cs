using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform m_target;
    [SerializeField] private Vector3 m_targetOffset = new Vector3(0f, 3f, -7f);

    [Header("Settings")]
    [SerializeField] private float m_followSpeed = 5f;
    [SerializeField] private float m_rotationSpeed = 3f;
    [SerializeField] private float m_lookAheadDistance = 2f;

    [Header("Collision")]
    [SerializeField] private float m_minDistance = 2f;
    [SerializeField] private LayerMask m_obstacleLayers;

    private Vector3 m_velocity;
    private float m_currentDistance;

    private void Start()
    {
        if (m_target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_target = player.transform;
            }
        }

        Vector3 targetPos = m_target != null ? m_target.position : transform.position;
        transform.position = targetPos + m_targetOffset;
        transform.LookAt(targetPos);
        m_currentDistance = m_targetOffset.magnitude;
    }

    private void LateUpdate()
    {
        if (m_target == null) return;

        Vector3 targetPos = m_target.position;
        Vector3 idealOffset = GetIdealOffset();
        Vector3 idealPosition = targetPos + idealOffset;

        HandleCollision(ref idealPosition, targetPos);

        transform.position = Vector3.SmoothDamp(transform.position, idealPosition, ref m_velocity, 1f / m_followSpeed);

        Vector3 lookTarget = targetPos + m_target.forward * m_lookAheadDistance;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetIdealOffset()
    {
        if (m_target == null) return m_targetOffset;

        return m_target.TransformDirection(m_targetOffset);
    }

    private void HandleCollision(ref Vector3 cameraPosition, Vector3 targetPosition)
    {
        Vector3 direction = cameraPosition - targetPosition;
        float targetDistance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(targetPosition, direction, out hit, targetDistance, m_obstacleLayers))
        {
            float hitDistance = hit.distance - m_minDistance;
            cameraPosition = targetPosition + direction.normalized * Mathf.Max(hitDistance, m_minDistance);
            m_currentDistance = hitDistance;
        }
        else
        {
            m_currentDistance = targetDistance;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        m_target = newTarget;
    }
}
