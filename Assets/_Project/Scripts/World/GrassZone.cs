using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class GrassZone : MonoBehaviour
    {
        [Header("Grass Settings")]
        [SerializeField] private GameObject m_grassPrefab;
        [SerializeField] private int m_grassCount = 50;
        [SerializeField] private Vector2 m_zoneSize = new Vector2(20f, 20f);

        [Header("Individual Blade")]
        [SerializeField] private Vector2 m_bladeHeight = new Vector2(0.3f, 0.8f);
        [SerializeField] private Vector2 m_bladeWidth = new Vector2(0.05f, 0.15f);
        [SerializeField] private Vector2 m_rotationRange = new Vector2(0f, 360f);

        [Header("Cluster Settings")]
        [SerializeField] private bool m_useClusters = true;
        [SerializeField] private int m_clusterCount = 10;
        [SerializeField] private int m_bladesPerCluster = 5;

        [Header("Materials")]
        [SerializeField] private Material m_grassMaterial;
        [SerializeField] private Gradient m_colorGradient;
        [SerializeField] private Color m_baseColor = new Color(0.2f, 0.5f, 0.15f);

        [Header("Performance")]
        [SerializeField] private bool m_gpuInstancing = true;
        [SerializeField] private bool m_windEffect = true;
        [SerializeField] private float m_windStrength = 0.5f;
        [SerializeField] private float m_windSpeed = 1f;
        [SerializeField] private float m_windUpdateInterval = 0.1f;

        [Header("Placement")]
        [SerializeField] private bool m_autoSpawnOnStart = true;
        [SerializeField] private LayerMask m_groundLayer;
        [SerializeField] private float m_raycastHeight = 10f;

        private List<GameObject> m_spawnedGrass = new List<GameObject>();
        private MaterialPropertyBlock m_propertyBlock;
        private float m_lastWindUpdate;

        private void Start()
        {
            m_propertyBlock = new MaterialPropertyBlock();

            if (m_autoSpawnOnStart)
            {
                SpawnGrass();
            }
        }

        public void SpawnGrass()
        {
            ClearGrass();

            if (m_useClusters)
            {
                SpawnClusteredGrass();
            }
            else
            {
                SpawnDistributedGrass();
            }

            if (m_gpuInstancing)
            {
                EnableGPUInstancing();
            }

            Debug.Log($"[GrassZone] Spawned {m_spawnedGrass.Count} grass blades");
        }

        private void SpawnDistributedGrass()
        {
            for (int i = 0; i < m_grassCount; i++)
            {
                Vector3? pos = GetRandomPosition();
                if (pos.HasValue)
                {
                    SpawnGrassBlade(pos.Value);
                }
            }
        }

        private void SpawnClusteredGrass()
        {
            for (int c = 0; c < m_clusterCount; c++)
            {
                Vector3? clusterCenter = GetRandomPosition();
                if (!clusterCenter.HasValue) continue;

                for (int i = 0; i < m_bladesPerCluster; i++)
                {
                    Vector2 offset = Random.insideUnitCircle * 1.5f;
                    Vector3 pos = clusterCenter.Value + new Vector3(offset.x, 0, offset.y);
                    SpawnGrassBlade(pos);
                }
            }
        }

        private void SpawnGrassBlade(Vector3 position)
        {
            GameObject grass;

            if (m_grassPrefab != null)
            {
                grass = Instantiate(m_grassPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                grass = CreatePlaceholderGrass(position);
            }

            float height = Random.Range(m_bladeHeight.x, m_bladeHeight.y);
            float width = Random.Range(m_bladeWidth.x, m_bladeWidth.y);
            grass.transform.localScale = new Vector3(width, height, width);

            float rotation = Random.Range(m_rotationRange.x, m_rotationRange.y);
            grass.transform.Rotate(Vector3.up, rotation);

            ApplyGrassColor(grass);

            m_spawnedGrass.Add(grass);
        }

        private GameObject CreatePlaceholderGrass(Vector3 position)
        {
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Quad);
            blade.transform.position = position;
            blade.transform.SetParent(transform);
            blade.name = "GrassBlade";

            GameObject.Destroy(blade.GetComponent<Collider>());

            return blade;
        }

        private void ApplyGrassColor(GameObject grass)
        {
            Renderer renderer = grass.GetComponent<Renderer>();
            if (renderer == null) return;

            Color color = m_baseColor;

            if (m_colorGradient != null && m_colorGradient.colorKeys.Length > 0)
            {
                float t = Random.Range(0f, 1f);
                color = m_colorGradient.Evaluate(t);
            }
            else
            {
                float variation = Random.Range(-0.15f, 0.15f);
                color = new Color(
                    Mathf.Clamp01(color.r + variation),
                    Mathf.Clamp01(color.g + variation),
                    Mathf.Clamp01(color.b + variation)
                );
            }

            if (m_propertyBlock == null)
                m_propertyBlock = new MaterialPropertyBlock();

            m_propertyBlock.SetColor("_Color", color);
            renderer.SetPropertyBlock(m_propertyBlock);
        }

        private void EnableGPUInstancing()
        {
            foreach (var grass in m_spawnedGrass)
            {
                if (grass == null) continue;

                Renderer renderer = grass.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enableInstancing = m_gpuInstancing;
                }
            }
        }

        private Vector3? GetRandomPosition()
        {
            float x = Random.Range(-m_zoneSize.x / 2f, m_zoneSize.x / 2f);
            float z = Random.Range(-m_zoneSize.y / 2f, m_zoneSize.y / 2f);

            Vector3 origin = transform.position + new Vector3(x, m_raycastHeight, z);

            if (m_groundLayer != 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, m_raycastHeight * 2f, m_groundLayer))
                {
                    return hit.point;
                }
            }
            else
            {
                return origin - Vector3.up * m_raycastHeight;
            }

            return null;
        }

        public void ClearGrass()
        {
            foreach (var grass in m_spawnedGrass)
            {
                if (grass != null)
                {
                    Destroy(grass);
                }
            }
            m_spawnedGrass.Clear();
        }

        public int GetGrassCount() => m_spawnedGrass.Count;

        private void Update()
        {
            if (!m_windEffect) return;
            if (Time.time - m_lastWindUpdate < m_windUpdateInterval) return;
            m_lastWindUpdate = Time.time;

            ApplyWindEffect();
        }

        private void ApplyWindEffect()
        {
            float windTime = Time.time * m_windSpeed;
            float windOffset = Mathf.Sin(windTime) * m_windStrength;

            foreach (var grass in m_spawnedGrass)
            {
                if (grass == null) continue;

                Vector3 pos = grass.transform.position;
                float windFactor = Mathf.PerlinNoise(pos.x * 0.1f, pos.z * 0.1f + windTime);
                pos.x += windOffset * windFactor * 0.1f;
                grass.transform.position = pos;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_zoneSize.x, 1f, m_zoneSize.y));
        }
    }
}
