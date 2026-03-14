using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class TerrainZone : MonoBehaviour
    {
        public enum ZoneType
        {
            Grass,
            Dirt,
            Sand,
            Mud,
            Gravel,
            Forest,
            Water,
            Road,
            Parking,
            Offroad
        }

        [Header("Zone Settings")]
        [SerializeField] private ZoneType m_zoneType = ZoneType.Grass;
        [SerializeField] private Color m_zoneColor = Color.green;
        [SerializeField] private bool m_showGizmo = true;

        [Header("Terrain Settings")]
        [SerializeField] private Material m_terrainMaterial;
        [SerializeField] private PhysicMaterial m_physicsMaterial;
        [SerializeField] private float m_frictionMultiplier = 1f;

        [Header("Zone Properties")]
        [SerializeField] private bool m_isOffroad;
        [SerializeField] private bool m_isDrivable = true;
        [SerializeField] private float m_speedModifier = 1f;
        [SerializeField] private float m_dragMultiplier = 1f;

        [Header("Vegetation")]
        [SerializeField] private bool m_allowTrees = true;
        [SerializeField] private bool m_allowRocks = true;
        [SerializeField] private bool m_allowGrass = true;
        [SerializeField] private int m_vegetationDensity = 1;

        [Header("Boundaries")]
        [SerializeField] private Vector2 m_zoneSize = new Vector2(50f, 50f);
        [SerializeField] private bool m_useBoundary = false;
        [SerializeField] private bool m_isDangerous;

        [Header("Tagging")]
        [SerializeField] private string m_customTag = "";

        private Renderer m_renderer;
        private Collider m_collider;

        public ZoneType Type => m_zoneType;
        public bool IsOffroad => m_isOffroad;
        public float SpeedModifier => m_speedModifier;
        public float DragMultiplier => m_dragMultiplier;
        public bool IsDrivable => m_isDrivable;
        public Vector2 ZoneSize => m_zoneSize;

        private void Awake()
        {
            m_renderer = GetComponent<Renderer>();
            m_collider = GetComponent<Collider>();

            ApplyZoneProperties();
            SetTag();
        }

        private void ApplyZoneProperties()
        {
            switch (m_zoneType)
            {
                case ZoneType.Grass:
                    m_zoneColor = new Color(0.3f, 0.6f, 0.2f);
                    m_isOffroad = false;
                    m_speedModifier = 1f;
                    m_dragMultiplier = 1f;
                    m_allowTrees = true;
                    m_allowRocks = true;
                    m_allowGrass = true;
                    break;

                case ZoneType.Dirt:
                    m_zoneColor = new Color(0.5f, 0.35f, 0.2f);
                    m_isOffroad = true;
                    m_speedModifier = 0.85f;
                    m_dragMultiplier = 1.3f;
                    m_allowTrees = false;
                    m_allowRocks = true;
                    m_allowGrass = false;
                    break;

                case ZoneType.Sand:
                    m_zoneColor = new Color(0.9f, 0.85f, 0.6f);
                    m_isOffroad = true;
                    m_speedModifier = 0.7f;
                    m_dragMultiplier = 1.5f;
                    m_allowTrees = false;
                    m_allowRocks = false;
                    m_allowGrass = false;
                    break;

                case ZoneType.Mud:
                    m_zoneColor = new Color(0.3f, 0.25f, 0.15f);
                    m_isOffroad = true;
                    m_speedModifier = 0.5f;
                    m_dragMultiplier = 2f;
                    m_allowTrees = false;
                    m_allowRocks = true;
                    m_allowGrass = false;
                    break;

                case ZoneType.Gravel:
                    m_zoneColor = new Color(0.6f, 0.6f, 0.55f);
                    m_isOffroad = false;
                    m_speedModifier = 0.95f;
                    m_dragMultiplier = 1.1f;
                    m_allowTrees = false;
                    m_allowRocks = true;
                    m_allowGrass = false;
                    break;

                case ZoneType.Forest:
                    m_zoneColor = new Color(0.15f, 0.35f, 0.1f);
                    m_isOffroad = false;
                    m_speedModifier = 0.98f;
                    m_dragMultiplier = 1.05f;
                    m_allowTrees = true;
                    m_allowRocks = true;
                    m_allowGrass = true;
                    break;

                case ZoneType.Water:
                    m_zoneColor = new Color(0.2f, 0.4f, 0.8f);
                    m_isOffroad = true;
                    m_speedModifier = 0.1f;
                    m_dragMultiplier = 5f;
                    m_isDrivable = false;
                    m_isDangerous = true;
                    break;

                case ZoneType.Road:
                    m_zoneColor = new Color(0.25f, 0.25f, 0.25f);
                    m_isOffroad = false;
                    m_speedModifier = 1f;
                    m_dragMultiplier = 1f;
                    m_allowTrees = false;
                    m_allowRocks = false;
                    m_allowGrass = false;
                    break;

                case ZoneType.Parking:
                    m_zoneColor = new Color(0.4f, 0.4f, 0.45f);
                    m_isOffroad = false;
                    m_speedModifier = 0.7f;
                    m_dragMultiplier = 1.2f;
                    m_allowTrees = false;
                    m_allowRocks = false;
                    m_allowGrass = false;
                    break;

                case ZoneType.Offroad:
                    m_zoneColor = new Color(0.4f, 0.3f, 0.2f);
                    m_isOffroad = true;
                    m_speedModifier = 0.6f;
                    m_dragMultiplier = 1.8f;
                    m_allowTrees = true;
                    m_allowRocks = true;
                    m_allowGrass = true;
                    break;
            }

            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            if (m_renderer != null && m_terrainMaterial != null)
            {
                m_renderer.material = m_terrainMaterial;
            }
            else if (m_renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = m_zoneColor;
                m_renderer.material = mat;
            }
        }

        private void SetTag()
        {
            if (!string.IsNullOrEmpty(m_customTag))
            {
                gameObject.tag = m_customTag;
            }
            else if (m_isOffroad)
            {
                gameObject.tag = "Offroad";
            }
            else
            {
                gameObject.tag = "Ground";
            }
        }

        public Vector3 GetRandomPointInZone()
        {
            float x = Random.Range(-m_zoneSize.x / 2f, m_zoneSize.x / 2f);
            float z = Random.Range(-m_zoneSize.y / 2f, m_zoneSize.y / 2f);

            Vector3 point = transform.position + new Vector3(x, 0, z);
            point.y = GetGroundHeight(point);

            return point;
        }

        public float GetGroundHeight(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hit, 100f))
            {
                return hit.point.y;
            }
            return transform.position.y;
        }

        public bool ContainsPoint(Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            return Mathf.Abs(localPoint.x) <= m_zoneSize.x / 2f &&
                   Mathf.Abs(localPoint.z) <= m_zoneSize.y / 2f;
        }

        public void SetZoneType(ZoneType type)
        {
            m_zoneType = type;
            ApplyZoneProperties();
        }

        private void OnDrawGizmosSelected()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_zoneColor;
            Gizmos.DrawCube(transform.position, new Vector3(m_zoneSize.x, 0.5f, m_zoneSize.y));

            Gizmos.color = new Color(m_zoneColor.r, m_zoneColor.g, m_zoneColor.b, 0.5f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_zoneSize.x, 5f, m_zoneSize.y));

            if (m_useBoundary)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, new Vector3(m_zoneSize.x, 10f, m_zoneSize.y));
            }
        }
    }
}
