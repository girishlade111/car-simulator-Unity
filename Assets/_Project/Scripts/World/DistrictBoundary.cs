using UnityEngine;

namespace CarSimulator.World
{
    public class DistrictBoundary : MonoBehaviour
    {
        public enum BoundaryType
        {
            SoftLimit,
            HardWall,
            WarningZone
        }

        [Header("Boundary Settings")]
        [SerializeField] private BoundaryType m_boundaryType = BoundaryType.SoftLimit;
        [SerializeField] private Vector2 m_size = new Vector2(500f, 500f);
        [SerializeField] private float m_wallHeight = 20f;
        [SerializeField] private float m_warningDistance = 50f;
        [SerializeField] private bool m_showWarning = true;

        [Header("Visual Feedback")]
        [SerializeField] private Color m_boundaryColor = Color.red;
        [SerializeField] private Color m_warningColor = new Color(1f, 0.5f, 0f, 0.3f);

        public Vector2 Size => m_size;
        public Vector2 Center => new Vector2(transform.position.x, transform.position.z);

        public bool IsPositionInside(Vector2 position)
        {
            float halfX = m_size.x / 2f;
            float halfZ = m_size.y / 2f;

            return position.x >= -halfX && position.x <= halfX &&
                   position.y >= -halfZ && position.y <= halfZ;
        }

        public Vector2 GetClosestPointInside(Vector2 position)
        {
            float halfX = m_size.x / 2f - m_warningDistance;
            float halfZ = m_size.y / 2f - m_warningDistance;

            return new Vector2(
                Mathf.Clamp(position.x, -halfX, halfX),
                Mathf.Clamp(position.y, -halfZ, halfZ)
            );
        }

        public float GetDistanceToBoundary(Vector2 position)
        {
            float halfX = m_size.x / 2f;
            float halfZ = m_size.y / 2f;

            float distX = Mathf.Max(halfX - Mathf.Abs(position.x), 0);
            float distZ = Mathf.Max(halfZ - Mathf.Abs(position.y), 0);

            return Mathf.Min(distX, distZ);
        }

        public bool IsInWarningZone(Vector2 position)
        {
            return GetDistanceToBoundary(position) < m_warningDistance;
        }

        private void OnDrawGizmos()
        {
            DrawBoundaryGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            DrawBoundaryGizmos();
        }

        private void DrawBoundaryGizmos()
        {
            Vector3 center = transform.position;
            Vector3 size = new Vector3(m_size.x, m_wallHeight, m_size.y);

            Gizmos.color = m_boundaryColor;
            Gizmos.DrawWireCube(center, size);

            if (m_showWarning && m_warningDistance > 0)
            {
                float warnX = m_size.x / 2f - m_warningDistance;
                float warnZ = m_size.y / 2f - m_warningDistance;

                if (warnX > 0 && warnZ > 0)
                {
                    Gizmos.color = m_warningColor;
                    Vector3 warnSize = new Vector3(warnX * 2f, m_wallHeight, warnZ * 2f);
                    Gizmos.DrawWireCube(center, warnSize);
                }
            }
        }
    }
}
