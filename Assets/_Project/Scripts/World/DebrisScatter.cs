using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    [ExecuteInEditMode]
    public class DebrisScatter : MonoBehaviour
    {
        [Header("Debris Settings")]
        [SerializeField] private string m_zoneName = "Debris Zone";
        [SerializeField] private DebrisType m_debrisType = DebrisType.Mixed;

        [Header("Spawn Settings")]
        [SerializeField] private int m_count = 15;
        [SerializeField] private Vector2 m_areaSize = new Vector2(10f, 10f);
        [SerializeField] private float m_minSpacing = 1f;

        [Header("Variation")]
        [SerializeField] private float m_rotationVariation = 45f;
        [SerializeField] private float m_scaleVariation = 0.3f;
        [SerializeField] private bool m_floorAlignment = true;

        [Header("Density Control")]
        [SerializeField] private float m_density = 1f;
        [SerializeField] private bool m_clustered = false;
        [SerializeField] private int m_clusterCount = 3;
        [SerializeField] private float m_clusterRadius = 3f;

        [Header("Performance")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_lodDistance = 50f;

        [Header("Spawned")]
        [SerializeField] private List<GameObject> m_spawnedDebris = new List<GameObject>();

        [Header("Visualization")]
        [SerializeField] private bool m_showGizmo = true;
        [SerializeField] private Color m_gizmoColor = new Color(0.8f, 0.7f, 0.5f, 0.3f);

        public enum DebrisType
        {
            Mixed,
            Papers,
            Trash,
            Leaves,
            BrokenGlass,
            MetalScraps
        }

        private void OnEnable()
        {
            if (!Application.isPlaying && m_enabled)
            {
                SpawnDebris();
            }
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                ClearDebris();
            }
        }

        public void SpawnDebris()
        {
            if (!m_enabled) return;

            ClearDebris();

            int actualCount = Mathf.RoundToInt(m_count * m_density);

            if (m_clustered)
            {
                SpawnClustered(actualCount);
            }
            else
            {
                SpawnScattered(actualCount);
            }

            Debug.Log($"[DebrisScatter] Spawned {m_spawnedDebris.Count} debris items in {m_zoneName}");
        }

        private void SpawnScattered(int count)
        {
            int attempts = 0;
            int maxAttempts = count * 20;

            List<Vector3> positions = new List<Vector3>();

            while (m_spawnedDebris.Count < count && attempts < maxAttempts)
            {
                attempts++;

                Vector3 pos = GetRandomPosition();

                if (IsTooClose(pos, positions))
                {
                    continue;
                }

                if (m_floorAlignment)
                {
                    pos = AlignToFloor(pos);
                    if (pos.y < -100f) continue;
                }

                GameObject debris = CreateDebrisPiece(pos);
                if (debris != null)
                {
                    positions.Add(pos);
                    m_spawnedDebris.Add(debris);
                }
            }
        }

        private void SpawnClustered(int count)
        {
            List<Vector3> clusterCenters = new List<Vector3>();

            for (int c = 0; c < m_clusterCount; c++)
            {
                Vector3 center = GetRandomPosition();
                if (m_floorAlignment)
                {
                    center = AlignToFloor(center);
                }
                clusterCenters.Add(center);
            }

            int itemsPerCluster = count / m_clusterCount;

            foreach (var center in clusterCenters)
            {
                for (int i = 0; i < itemsPerCluster; i++)
                {
                    Vector3 pos = center + Random.insideUnitSphere * m_clusterRadius;
                    pos.y = center.y;

                    GameObject debris = CreateDebrisPiece(pos);
                    if (debris != null)
                    {
                        m_spawnedDebris.Add(debris);
                    }
                }
            }
        }

        private Vector3 GetRandomPosition()
        {
            float x = Random.Range(-m_areaSize.x / 2f, m_areaSize.x / 2f);
            float z = Random.Range(-m_areaSize.y / 2f, m_areaSize.y / 2f);
            return transform.position + new Vector3(x, 5f, z);
        }

        private Vector3 AlignToFloor(Vector3 pos)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 20f))
            {
                return hit.point;
            }
            return new Vector3(pos.x, -1000f, pos.z);
        }

        private bool IsTooClose(Vector3 pos, List<Vector3> positions)
        {
            foreach (var existingPos in positions)
            {
                if (Vector3.Distance(pos, existingPos) < m_minSpacing)
                {
                    return true;
                }
            }
            return false;
        }

        private GameObject CreateDebrisPiece(Vector3 position)
        {
            GameObject debris = new GameObject($"Debris_{m_spawnedDebris.Count}");
            debris.transform.position = position;

            float rotationY = Random.Range(-m_rotationVariation, m_rotationVariation);
            debris.transform.rotation = Quaternion.Euler(0, rotationY, Random.Range(-10f, 10f));

            float scale = 1f + Random.Range(-m_scaleVariation, m_scaleVariation);
            debris.transform.localScale = Vector3.one * scale;

            Renderer renderer = CreateDebrisMesh(debris, m_debrisType);
            debris.GetComponent<Renderer>().material.color = GetDebrisColor();

            debris.transform.SetParent(transform);

            return debris;
        }

        private Renderer CreateDebrisMesh(GameObject debris, DebrisType type)
        {
            PrimitiveType primitive;
            Vector3 scale;
            string name;

            switch (type)
            {
                case DebrisType.Papers:
                    primitive = PrimitiveType.Quad;
                    scale = new Vector3(0.3f, 0.4f, 0.1f);
                    name = "Paper";
                    break;
                case DebrisType.Trash:
                    primitive = Random.value > 0.5f ? PrimitiveType.Sphere : PrimitiveType.Cube;
                    scale = Vector3.one * Random.Range(0.1f, 0.2f);
                    name = "Trash";
                    break;
                case DebrisType.Leaves:
                    primitive = PrimitiveType.Quad;
                    scale = new Vector3(0.2f, 0.2f, 0.05f);
                    name = "Leaf";
                    break;
                case DebrisType.BrokenGlass:
                    primitive = PrimitiveType.Quad;
                    scale = new Vector3(0.15f, 0.15f, 0.05f);
                    name = "Glass";
                    break;
                case DebrisType.MetalScraps:
                    primitive = PrimitiveType.Cube;
                    scale = new Vector3(0.2f, 0.02f, 0.1f);
                    name = "Metal";
                    break;
                case DebrisType.Mixed:
                default:
                    primitive = Random.value > 0.6f ? PrimitiveType.Quad : PrimitiveType.Cube;
                    scale = Vector3.one * Random.Range(0.1f, 0.3f);
                    name = "Mixed";
                    break;
            }

            GameObject meshObj = GameObject.CreatePrimitive(primitive);
            meshObj.name = name;
            meshObj.transform.SetParent(debris.transform);
            meshObj.transform.localPosition = Vector3.zero;
            meshObj.transform.localScale = scale;

            Destroy(meshObj.GetComponent<Collider>());

            return meshObj.GetComponent<Renderer>();
        }

        private Color GetDebrisColor()
        {
            switch (m_debrisType)
            {
                case DebrisType.Papers:
                    return new Color(0.9f, 0.9f, 0.85f);
                case DebrisType.Trash:
                    return new Color(0.3f, 0.3f, 0.3f);
                case DebrisType.Leaves:
                    return new Color(0.4f, 0.5f, 0.2f);
                case DebrisType.BrokenGlass:
                    return new Color(0.7f, 0.8f, 0.9f);
                case DebrisType.MetalScraps:
                    return new Color(0.5f, 0.5f, 0.55f);
                default:
                    return Color.gray;
            }
        }

        public void ClearDebris()
        {
            foreach (var debris in m_spawnedDebris)
            {
                if (debris != null)
                {
                    DestroyImmediate(debris);
                }
            }
            m_spawnedDebris.Clear();
        }

        public void Refresh()
        {
            SpawnDebris();
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmo) return;

            Gizmos.color = m_gizmoColor;
            Gizmos.DrawCube(transform.position, new Vector3(m_areaSize.x, 0.5f, m_areaSize.y));

            Gizmos.color = m_gizmoColor * 1.5f;
            foreach (var debris in m_spawnedDebris)
            {
                if (debris != null)
                {
                    Gizmos.DrawWireCube(debris.transform.position, Vector3.one * 0.2f);
                }
            }
        }

        public int GetDebrisCount() => m_spawnedDebris.Count;
    }
}
