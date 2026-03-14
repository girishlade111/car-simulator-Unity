using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.AI
{
    public class TrafficWaypoint : MonoBehaviour
    {
        [Header("Waypoint Settings")]
        [SerializeField] private float m_connectDistance = 20f;
        [SerializeField] private bool m_isIntersection;

        [Header("Traffic Settings")]
        [SerializeField] private float m_stopDuration;
        [SerializeField] private float m_speedMultiplier = 1f;
        [SerializeField] private bool m_isStopSign;

        private List<TrafficWaypoint> m_connections = new List<TrafficWaypoint>();

        public float StopDuration => m_stopDuration;
        public float SpeedMultiplier => m_speedMultiplier;
        public bool IsIntersection => m_isIntersection;
        public bool IsStopSign => m_isStopSign;
        public List<TrafficWaypoint> Connections => m_connections;

        private void Start()
        {
            FindConnections();
        }

        private void FindConnections()
        {
            m_connections.Clear();
            
            var allWaypoints = FindObjectsOfType<TrafficWaypoint>();
            
            foreach (var wp in allWaypoints)
            {
                if (wp == this) continue;

                float dist = Vector3.Distance(transform.position, wp.transform.position);
                if (dist <= m_connectDistance)
                {
                    m_connections.Add(wp);
                }
            }
        }

        public TrafficWaypoint GetNextWaypoint()
        {
            if (m_connections.Count == 0) return null;
            return m_connections[Random.Range(0, m_connections.Count)];
        }

        public TrafficWaypoint GetRandomConnection()
        {
            if (m_connections.Count == 0) return null;
            return m_connections[Random.Range(0, m_connections.Count)];
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = m_isStopSign ? Color.red : (m_isIntersection ? Color.blue : Color.green);
            Gizmos.DrawSphere(transform.position, 1f);

            FindConnections();

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            foreach (var conn in m_connections)
            {
                if (conn != null)
                {
                    Gizmos.DrawLine(transform.position, conn.transform.position);
                }
            }

            if (m_isStopSign)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 3f);
            }
        }
    }
}
