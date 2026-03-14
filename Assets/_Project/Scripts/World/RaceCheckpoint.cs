using UnityEngine;
using UnityEngine.Events;

namespace CarSimulator.World
{
    public class RaceCheckpoint : MonoBehaviour
    {
        [Header("Checkpoint Settings")]
        [SerializeField] private int m_checkpointIndex;
        [SerializeField] private bool m_isFinishLine;
        [SerializeField] private float m_triggerRadius = 5f;

        [Header("Events")]
        [SerializeField] private UnityEvent m_onCheckpointReached;
        [SerializeField] private UnityEvent m_onFinishLineReached;

        [Header("Visual")]
        [SerializeField] private Color m_checkpointColor = Color.yellow;
        [SerializeField] private Color m_finishColor = Color.green;

        private bool m_hasBeenTriggered;
        private LapTimer m_lapTimer;

        public int CheckpointIndex => m_checkpointIndex;
        public bool IsFinishLine => m_isFinishLine;

        private void Start()
        {
            FindLapTimer();
        }

        private void FindLapTimer()
        {
            var timers = FindObjectsOfType<LapTimer>();
            if (timers.Length > 0)
            {
                m_lapTimer = timers[0];
                m_lapTimer.RegisterCheckpoint(GetCheckpointComponent());
            }
        }

        private LapTimer.Checkpoint GetCheckpointComponent()
        {
            LapTimer.Checkpoint checkpoint = gameObject.AddComponent<LapTimer.Checkpoint>();
            checkpoint.Order = m_checkpointIndex;
            checkpoint.Timer = m_lapTimer;
            return checkpoint;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerCheckpoint(other.transform);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && !m_hasBeenTriggered)
            {
                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist < m_triggerRadius)
                {
                    TriggerCheckpoint(other.transform);
                }
            }
        }

        private void TriggerCheckpoint(Transform player)
        {
            if (m_hasBeenTriggered) return;

            m_hasBeenTriggered = true;

            if (m_isFinishLine)
            {
                m_onFinishLineReached.Invoke();
                Debug.Log($"[RaceCheckpoint] Finish line reached!");
            }
            else
            {
                m_onCheckpointReached.Invoke();
                Debug.Log($"[RaceCheckpoint] Checkpoint {m_checkpointIndex} reached!");
            }

            VisualFeedback();
        }

        private void VisualFeedback()
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = m_isFinishLine ? m_finishColor : m_checkpointColor * 1.5f;
            }
        }

        public void ResetCheckpoint()
        {
            m_hasBeenTriggered = false;
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = m_isFinishLine ? m_finishColor : m_checkpointColor;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_isFinishLine ? m_finishColor : m_checkpointColor;
            
            Vector3 size = Vector3.one * m_triggerRadius * 2f;
            Gizmos.DrawWireCube(transform.position, size);

            if (m_isFinishLine)
            {
                Gizmos.color = new Color(m_finishColor.r, m_finishColor.g, m_finishColor.b, 0.3f);
                Gizmos.DrawCube(transform.position, size);
            }
        }
    }
}
