using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class TreeSpawnPoint : MonoBehaviour
    {
        [Header("Tree Settings")]
        [SerializeField] private GameObject m_treePrefab;
        [SerializeField] private Vector2 m_scaleRange = new Vector2(0.8f, 1.5f);
        [SerializeField] private Vector2 m_rotationRange = new Vector2(0f, 360f);
        [SerializeField] private bool m_randomScale = true;
        [SerializeField] private bool m_randomRotation = true;

        [Header("Variation")]
        [SerializeField] private bool m_colorVariation = true;
        [SerializeField] private float m_colorVariationAmount = 0.1f;

        [Header("LOD")]
        [SerializeField] private GameObject m_lowLODPrefab;
        [SerializeField] private float m_LODDistance = 80f;

        [Header("Placement")]
        [SerializeField] private bool m_autoPlaceOnStart = true;
        [SerializeField] private float m_groundOffset = 0f;

        private GameObject m_spawnedTree;

        public GameObject SpawnedTree => m_spawnedTree;

        private void Start()
        {
            if (m_autoPlaceOnStart)
            {
                PlaceTree();
            }
        }

        public GameObject PlaceTree()
        {
            if (m_spawnedTree != null)
            {
                Destroy(m_spawnedTree);
            }

            GameObject prefabToUse = Get LODPrefab();

            if (prefabToUse != null)
            {
                m_spawnedTree = Instantiate(prefabToUse, transform.position + Vector3.up * m_groundOffset, transform.rotation);
            }
            else
            {
                m_spawnedTree = CreatePlaceholderTree();
            }

            if (m_spawnedTree != null)
            {
                ApplyTransform();
                ApplyColorVariation();
                SetupLOD();
            }

            return m_spawnedTree;
        }

        private GameObject GetLODPrefab()
        {
            if (m_lowLODPrefab != null)
            {
                float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
                if (distance > m_LODDistance)
                {
                    return m_lowLODPrefab;
                }
            }

            return m_treePrefab;
        }

        private void ApplyTransform()
        {
            if (m_spawnedTree == null) return;

            if (m_randomScale)
            {
                float scale = Random.Range(m_scaleRange.x, m_scaleRange.y);
                m_spawnedTree.transform.localScale = Vector3.one * scale;
            }

            if (m_randomRotation)
            {
                float rotation = Random.Range(m_rotationRange.x, m_rotationRange.y);
                m_spawnedTree.transform.Rotate(Vector3.up, rotation);
            }
        }

        private void ApplyColorVariation()
        {
            if (!m_colorVariation || m_spawnedTree == null) return;

            Renderer[] renderers = m_spawnedTree.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.material != null)
                {
                    Color baseColor = renderer.material.color;
                    float variation = Random.Range(-m_colorVariationAmount, m_colorVariationAmount);
                    baseColor = new Color(
                        Mathf.Clamp01(baseColor.r + variation),
                        Mathf.Clamp01(baseColor.g + variation),
                        Mathf.Clamp01(baseColor.b + variation)
                    );
                    renderer.material.color = baseColor;
                }
            }
        }

        private void SetupLOD()
        {
            if (m_spawnedTree == null || m_lowLODPrefab == null) return;

            LODGroup lodGroup = m_spawnedTree.GetComponent<LODGroup>();
            if (lodGroup == null)
            {
                lodGroup = m_spawnedTree.AddComponent<LODGroup>();
            }

            LOD[] lods = new LOD[2];
            lods[0] = new LOD(1f / m_LODDistance, m_spawnedTree.GetComponentsInChildren<Renderer>());
            lods[1] = new LOD(1f / (m_LODDistance * 2), m_lowLODPrefab.GetComponentsInChildren<Renderer>());

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }

        private GameObject CreatePlaceholderTree()
        {
            GameObject tree = new GameObject("Tree");
            tree.transform.SetParent(transform);
            tree.transform.localPosition = Vector3.zero;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
            trunk.transform.localScale = new Vector3(0.3f, 1.5f, 0.3f);

            Renderer trunkRenderer = trunk.GetComponent<Renderer>();
            if (trunkRenderer != null)
            {
                trunkRenderer.material = new Material(Shader.Find("Standard"));
                trunkRenderer.material.color = new Color(0.4f, 0.25f, 0.1f);
            }

            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.transform.SetParent(tree.transform);
            foliage.transform.localPosition = new Vector3(0, 4f, 0);
            foliage.transform.localScale = new Vector3(2.5f, 3f, 2.5f);

            Renderer foliageRenderer = foliage.GetComponent<Renderer>();
            if (foliageRenderer != null)
            {
                foliageRenderer.material = new Material(Shader.Find("Standard"));
                foliageRenderer.material.color = new Color(0.2f, 0.5f, 0.15f);
            }

            return tree;
        }

        public void ClearTree()
        {
            if (m_spawnedTree != null)
            {
                Destroy(m_spawnedTree);
                m_spawnedTree = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
