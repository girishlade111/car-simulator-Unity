using UnityEngine;

namespace CarSimulator.World
{
    public class RoadSegment : MonoBehaviour
    {
        public enum RoadType
        {
            Straight,
            Intersection,
            Curve,
            Roundabout
        }

        [Header("Road Settings")]
        [SerializeField] private RoadType m_roadType = RoadType.Straight;
        [SerializeField] private float m_length = 20f;
        [SerializeField] private float m_width = 8f;
        [SerializeField] private float m_laneCount = 2f;

        [Header("Connections")]
        [SerializeField] private bool m_connectNorth;
        [SerializeField] private bool m_connectSouth;
        [SerializeField] private bool m_connectEast;
        [SerializeField] private bool m_connectWest;

        public float Length => m_length;
        public float Width => m_width;
        public Vector3 Center => transform.position;
        public Vector3 Forward => transform.forward;

        public Vector3 GetConnectionPoint(string direction)
        {
            switch (direction.ToLower())
            {
                case "north":
                case "forward":
                    return transform.position + transform.forward * (m_length / 2f);
                case "south":
                case "back":
                    return transform.position - transform.forward * (m_length / 2f);
                case "east":
                case "right":
                    return transform.position + transform.right * (m_width / 2f);
                case "west":
                case "left":
                    return transform.position - transform.right * (m_width / 2f);
                default:
                    return transform.position;
            }
        }

        public bool HasConnection(string direction)
        {
            switch (direction.ToLower())
            {
                case "north":
                case "forward":
                    return m_connectNorth;
                case "south":
                case "back":
                    return m_connectSouth;
                case "east":
                case "right":
                    return m_connectEast;
                case "west":
                case "left":
                    return m_connectWest;
                default:
                    return false;
            }
        }

        private void OnDrawGizmos()
        {
            DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        private void DrawGizmos()
        {
            Gizmos.color = Color.gray;
            Vector3 size = new Vector3(m_width, 0.1f, m_length);
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.05f, size);

            Gizmos.color = Color.yellow;
            if (m_connectNorth)
            {
                Gizmos.DrawLine(transform.position + transform.forward * (m_length / 2f),
                    transform.position + transform.forward * (m_length / 2f + 2f));
            }
            if (m_connectSouth)
            {
                Gizmos.DrawLine(transform.position - transform.forward * (m_length / 2f),
                    transform.position - transform.forward * (m_length / 2f + 2f));
            }
            if (m_connectEast)
            {
                Gizmos.DrawLine(transform.position + transform.right * (m_width / 2f),
                    transform.position + transform.right * (m_width / 2f + 2f));
            }
            if (m_connectWest)
            {
                Gizmos.DrawLine(transform.position - transform.right * (m_width / 2f),
                    transform.position - transform.right * (m_width / 2f + 2f));
            }
        }
    }
}
