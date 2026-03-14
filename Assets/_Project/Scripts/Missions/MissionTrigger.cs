using UnityEngine;

namespace CarSimulator.Missions
{
    public class MissionTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string m_missionId;
        [SerializeField] private TriggerType m_triggerType = TriggerType.OnEnter;

        [Header("Area")]
        [SerializeField] private float m_radius = 10f;

        public enum TriggerType
        {
            OnEnter,
            OnExit,
            OnPress
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_triggerType == TriggerType.OnEnter && other.CompareTag("Player"))
            {
                TryStartMission();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_triggerType == TriggerType.OnExit && other.CompareTag("Player"))
            {
                TryStartMission();
            }
        }

        private void Update()
        {
            if (m_triggerType == TriggerType.OnPress && Input.GetKeyDown(KeyCode.E))
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        TryStartMission();
                        break;
                    }
                }
            }
        }

        private void TryStartMission()
        {
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.StartMission(m_missionId);
                Debug.Log($"[MissionTrigger] Started mission: {m_missionId}");
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_radius);
        }
    }
}
