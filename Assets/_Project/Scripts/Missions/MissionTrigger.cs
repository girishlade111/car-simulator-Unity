using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string m_missionId;
    [SerializeField] private TriggerType m_triggerType;

    [Header("Trigger Area")]
    [SerializeField] private float m_radius = 10f;
    [SerializeField] private LayerMask m_targetLayer = -1;

    [Header("Display")]
    [SerializeField] private bool m_showGizmos = true;
    [SerializeField] private Color m_gizmoColor = Color.yellow;

    [Header("Cooldown")]
    [SerializeField] private float m_cooldownTime = 5f;

    private bool m_isOnCooldown;
    private float m_cooldownTimer;

    private enum TriggerType
    {
        OnEnter,
        OnExit,
        OnPress,
        Automatic
    }

    private void Update()
    {
        if (m_isOnCooldown)
        {
            m_cooldownTimer -= Time.deltaTime;
            if (m_cooldownTimer <= 0)
            {
                m_isOnCooldown = false;
            }
        }

        if (m_triggerType == TriggerType.OnPress && !m_isOnCooldown)
        {
            CheckPlayerInput();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_triggerType == TriggerType.OnEnter && !m_isOnCooldown)
        {
            if (other.CompareTag("Player"))
            {
                TryStartMission();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_triggerType == TriggerType.OnExit && !m_isOnCooldown)
        {
            if (other.CompareTag("Player"))
            {
                TryStartMission();
            }
        }
    }

    private void CheckPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius, m_targetLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    TryStartMission();
                    break;
                }
            }
        }
    }

    private void TryStartMission()
    {
        if (MissionManager.Instance == null)
        {
            Debug.LogWarning("[MissionTrigger] MissionManager not found!");
            return;
        }

        if (!MissionManager.Instance.CanStartMission(m_missionId))
        {
            Debug.Log($"[MissionTrigger] Cannot start mission: {m_missionId}");
            return;
        }

        MissionManager.Instance.StartMission(m_missionId);
        StartCooldown();
    }

    private void StartCooldown()
    {
        m_isOnCooldown = true;
        m_cooldownTimer = m_cooldownTime;
    }

    private void OnDrawGizmos()
    {
        if (!m_showGizmos) return;

        Gizmos.color = m_gizmoColor;
        Gizmos.DrawWireSphere(transform.position, m_radius);

        if (GetComponent<Collider>() != null)
        {
            Gizmos.color = new Color(m_gizmoColor.r, m_gizmoColor.g, m_gizmoColor.b, 0.3f);
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}
