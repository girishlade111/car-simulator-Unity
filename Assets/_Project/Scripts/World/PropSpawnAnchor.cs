using UnityEngine;

namespace CarSimulator.World
{
    public enum PropCategory
    {
        Street,
        Nature,
        Building,
        Vehicle,
        Decorative
    }

    public class PropSpawnAnchor : MonoBehaviour
    {
        [Header("Anchor Settings")]
        [SerializeField] private PropCategory m_category;
        [SerializeField] private string m_propType;
        [SerializeField] private bool m_isActive = true;
        [SerializeField] private float m_density = 1f;

        [Header("Variation")]
        [SerializeField] private float m_randomRotation = 30f;
        [SerializeField] private float m_randomScale = 0.2f;

        [Header("Placement")]
        [SerializeField] private bool m_alignToSurface;
        [SerializeField] private float m_surfaceOffset;

        public PropCategory Category => m_category;
        public string PropType => m_propType;
        public bool IsActive => m_isActive;
        public float Density => m_density;

        public Vector3 GetSpawnPosition()
        {
            Vector3 pos = transform.position;

            if (m_alignToSurface)
            {
                RaycastHit hit;
                if (Physics.Raycast(pos + Vector3.up * 10f, Vector3.down, out hit, 20f))
                {
                    pos = hit.point + Vector3.up * m_surfaceOffset;
                }
            }

            return pos;
        }

        public Quaternion GetSpawnRotation()
        {
            float rotY = transform.eulerAngles.y + Random.Range(-m_randomRotation, m_randomRotation);
            return Quaternion.Euler(0, rotY, 0);
        }

        public Vector3 GetSpawnScale()
        {
            float scale = 1f + Random.Range(-m_randomScale, m_randomScale);
            return Vector3.one * scale;
        }

        private void OnDrawGizmos()
        {
            Color gizmoColor = GetCategoryColor();
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            if (!string.IsNullOrEmpty(m_propType))
            {
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
                Vector3 dir = transform.forward * 1f;
                Gizmos.DrawLine(transform.position, transform.position + dir);
            }
        }

        private Color GetCategoryColor()
        {
            switch (m_category)
            {
                case PropCategory.Street:
                    return Color.yellow;
                case PropCategory.Nature:
                    return Color.green;
                case PropCategory.Building:
                    return Color.magenta;
                case PropCategory.Vehicle:
                    return Color.cyan;
                case PropCategory.Decorative:
                    return Color.white;
                default:
                    return Color.gray;
            }
        }
    }
}
