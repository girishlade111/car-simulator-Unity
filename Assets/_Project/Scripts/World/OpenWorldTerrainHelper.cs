using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.TerrainUtils;

namespace CarSimulator.World
{
    public class OpenWorldTerrainHelper : MonoBehaviour
    {
        [Header("Terrain Settings")]
        [SerializeField] private Terrain m_terrain;
        [SerializeField] private TerrainLayer[] m_terrainLayers;

        [Header("Terrain Size")]
        [SerializeField] private Vector3 m_terrainSize = new Vector3(500, 100, 500);
        [SerializeField] private int m_terrainResolution = 513;

        [Header("Terrain Layers")]
        [SerializeField] private TerrainLayerSettings m_layerSettings;

        [Header("Heightmap")]
        [SerializeField] private float m_heightmapScale = 0.02f;
        [SerializeField] private AnimationCurve m_heightCurve = new AnimationCurve();

        [Header("Performance")]
        [SerializeField] private bool m_pixelErrorEnabled = true;
        [SerializeField] private float m_pixelError = 5f;
        [SerializeField] private int m_baseMapResolution = 1024;

        private void Start()
        {
            if (m_terrain == null)
            {
                m_terrain = GetComponent<Terrain>();
            }

            if (m_terrain == null && ShouldCreateTerrain())
            {
                CreateTerrain();
            }

            SetupTerrain();
        }

        private bool ShouldCreateTerrain()
        {
            return GetComponent<TerrainCollider>() != null;
        }

        private void CreateTerrain()
        {
            GameObject terrainObj = new GameObject("Terrain");
            terrainObj.transform.SetParent(transform);

            m_terrain = terrainObj.AddComponent<Terrain>();
            TerrainCollider collider = terrainObj.AddComponent<TerrainCollider>();

            m_terrain.terrainData = new TerrainData();
            m_terrain.terrainData.size = m_terrainSize;
            m_terrain.terrainData.heightmapResolution = m_terrainResolution;

            collider.terrainData = m_terrain.terrainData;

            m_terrain.gameObject.layer = LayerMask.NameToLayer("Ground");

            Debug.Log("[OpenWorldTerrainHelper] Created terrain");
        }

        private void SetupTerrain()
        {
            if (m_terrain == null) return;

            m_terrain.drawInstanced = true;
            m_terrain.castShadows = true;
            m_terrain.receiveShadows = true;

            if (m_pixelErrorEnabled)
            {
                m_terrain.heightmapPixelError = m_pixelError;
            }

            m_terrain.basemapDistance = m_terrainSize.x * 0.8f;

            if (m_layerSettings != null)
            {
                ApplyTerrainLayers();
            }
        }

        private void ApplyTerrainLayers()
        {
            if (m_terrain == null || m_terrainLayers == null) return;

            m_terrain.terrainData.terrainLayers = m_terrainLayers;
        }

        public void SetTerrainLayers(TerrainLayer[] layers)
        {
            m_terrainLayers = layers;
            ApplyTerrainLayers();
        }

        public void AddTerrainLayer(TerrainLayer layer)
        {
            if (m_terrain == null || m_terrain.terrainData == null) return;

            TerrainLayer[] currentLayers = m_terrain.terrainData.terrainLayers;
            TerrainLayer[] newLayers = new TerrainLayer[currentLayers.Length + 1];

            for (int i = 0; i < currentLayers.Length; i++)
            {
                newLayers[i] = currentLayers[i];
            }

            newLayers[newLayers.Length - 1] = layer;
            m_terrain.terrainData.terrainLayers = newLayers;
        }

        public void ApplyHeightmap(Texture2D heightmap)
        {
            if (m_terrain == null || m_terrain.terrainData == null || heightmap == null) return;

            float[,] heights = new float[m_terrainResolution, m_terrainResolution];

            for (int y = 0; y < m_terrainResolution; y++)
            {
                for (int x = 0; x < m_terrainResolution; x++)
                {
                    float tx = (float)x / (m_terrainResolution - 1) * heightmap.width;
                    float ty = (float)y / (m_terrainResolution - 1) * heightmap.height;

                    float height = heightmap.GetPixelBilinear(tx, ty).grayscale;

                    if (m_heightCurve != null && m_heightCurve.keys.Length > 0)
                    {
                        height = m_heightCurve.Evaluate(height);
                    }

                    heights[y, x] = height * m_heightmapScale;
                }
            }

            m_terrain.terrainData.SetHeights(0, 0, heights);
            Debug.Log("[OpenWorldTerrainHelper] Applied heightmap");
        }

        public void SmoothHeightmap(int iterations = 1)
        {
            if (m_terrain == null || m_terrain.terrainData == null) return;

            for (int i = 0; i < iterations; i++)
            {
                m_terrain.terrainData.SyncHeightmap();
            }
        }

        public Terrain GetTerrain() => m_terrain;

        public void SetTerrainEnabled(bool enabled)
        {
            if (m_terrain != null)
            {
                m_terrain.enabled = enabled;
            }
        }

        public void SetCastShadows(bool cast)
        {
            if (m_terrain != null)
            {
                m_terrain.castShadows = cast;
            }
        }

        public void SetReceiveShadows(bool receive)
        {
            if (m_terrain != null)
            {
                m_terrain.receiveShadows = receive;
            }
        }

        public float GetTerrainHeight(Vector3 worldPosition)
        {
            if (m_terrain == null) return 0;

            return m_terrain.SampleHeight(worldPosition);
        }

        public Vector3 GetTerrainNormal(Vector3 worldPosition)
        {
            if (m_terrain == null) return Vector3.up;

            return m_terrain.terrainData.GetInterpolatedNormal(
                (worldPosition.x - transform.position.x) / m_terrainSize.x,
                (worldPosition.z - transform.position.z) / m_terrainSize.z
            );
        }

        public bool IsOnTerrain(Vector3 position)
        {
            Bounds bounds = new Bounds(transform.position + m_terrainSize / 2, m_terrainSize);
            return bounds.Contains(position);
        }

        private void OnDrawGizmosSelected()
        {
            if (m_terrain == null)
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawWireCube(transform.position + m_terrainSize / 2, m_terrainSize);
            }
        }
    }

    [System.Serializable]
    public class TerrainLayerSettings
    {
        [SerializeField] private TerrainLayer[] m_layers;

        public TerrainLayer[] Layers => m_layers;

        public void SetLayers(TerrainLayer[] layers)
        {
            m_layers = layers;
        }
    }
}
