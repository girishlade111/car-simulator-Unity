using UnityEngine;
using UnityEngine.TerrainTools;

namespace CarSimulator.World
{
    public class TerrainLayerGenerator : MonoBehaviour
    {
        [Header("Terrain Reference")]
        [SerializeField] private Terrain m_terrain;

        [Header("Layer Settings")]
        [SerializeField] private int m_layerCount = 4;
        [SerializeField] private TerrainLayer[] m_generatedLayers;

        [Header("Texture Settings")]
        [SerializeField] private Texture2D[] m_diffuseTextures;
        [SerializeField] private Texture2D[] m_normalMapTextures;
        [SerializeField] private float[] m_tileSizes = { 20f, 20f, 20f, 20f };

        [Header("Channel Masks")]
        [SerializeField] private Vector4[] m_channelMasks;

        [Header("Auto-Generate")]
        [SerializeField] private bool m_generateOnStart = false;

        private void Start()
        {
            if (m_terrain == null)
            {
                m_terrain = GetComponent<Terrain>();
            }

            if (m_generateOnStart)
            {
                GenerateTerrainLayers();
            }
        }

        public void GenerateTerrainLayers()
        {
            if (m_terrain == null)
            {
                Debug.LogWarning("[TerrainLayerGenerator] No terrain assigned!");
                return;
            }

            m_generatedLayers = new TerrainLayer[m_layerCount];

            for (int i = 0; i < m_layerCount; i++)
            {
                TerrainLayer layer = CreateTerrainLayer(i);
                m_generatedLayers[i] = layer;
            }

            m_terrain.terrainData.terrainLayers = m_generatedLayers;

            Debug.Log($"[TerrainLayerGenerator] Generated {m_layerCount} terrain layers");
        }

        private TerrainLayer CreateTerrainLayer(int index)
        {
            TerrainLayer layer = new TerrainLayer();

            layer.tileSize = new Vector2(
                m_tileSizes != null && index < m_tileSizes.Length ? m_tileSizes[index] : 20f,
                m_tileSizes != null && index < m_tileSizes.Length ? m_tileSizes[index] : 20f
            );

            if (m_diffuseTextures != null && index < m_diffuseTextures.Length && m_diffuseTextures[index] != null)
            {
                layer.diffuseTexture = m_diffuseTextures[index];
            }
            else
            {
                layer.diffuseTexture = CreateDefaultDiffuse(index);
            }

            if (m_normalMapTextures != null && index < m_normalMapTextures.Length && m_normalMapTextures[index] != null)
            {
                layer.normalMapTexture = m_normalMapTextures[index];
            }
            else
            {
                layer.normalMapTexture = CreateDefaultNormal();
            }

            if (m_channelMasks != null && index < m_channelMasks.Length)
            {
                layer.channelMask = m_channelMasks[index];
            }
            else
            {
                layer.channelMask = GetDefaultMask(index);
            }

            layer.metallic = 0f;
            layer.smoothness = 0f;

            return layer;
        }

        private Texture2D CreateDefaultDiffuse(int index)
        {
            Texture2D tex = new Texture2D(256, 256);
            Color baseColor = GetColorForIndex(index);

            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.2f;
                    tex.SetPixel(x, y, baseColor * (1f + noise - 0.1f));
                }
            }

            tex.Apply();
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;

            return tex;
        }

        private Texture2D CreateDefaultNormal()
        {
            Texture2D tex = new Texture2D(256, 256);

            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    float heightL = Mathf.PerlinNoise((x - 1) * 0.1f, y * 0.1f);
                    float heightR = Mathf.PerlinNoise((x + 1) * 0.1f, y * 0.1f);
                    float heightD = Mathf.PerlinNoise(x * 0.1f, (y - 1) * 0.1f);
                    float heightU = Mathf.PerlinNoise(x * 0.1f, (y + 1) * 0.1f);

                    Vector3 normal = new Vector3(heightL - heightR, 2f, heightD - heightU).normalized;

                    tex.SetPixel(x, y, new Color(
                        normal.x * 0.5f + 0.5f,
                        normal.y * 0.5f + 0.5f,
                        normal.z * 0.5f + 0.5f,
                        1f
                    ));
                }
            }

            tex.Apply();
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;

            return tex;
        }

        private Vector4 GetDefaultMask(int index)
        {
            switch (index)
            {
                case 0: return new Vector4(1, 0, 0, 0);
                case 1: return new Vector4(0, 1, 0, 0);
                case 2: return new Vector4(0, 0, 1, 0);
                case 3: return new Vector4(0, 0, 0, 1);
                default: return new Vector4(1, 0, 0, 0);
            }
        }

        private Color GetColorForIndex(int index)
        {
            switch (index)
            {
                case 0: return new Color(0.3f, 0.5f, 0.2f);
                case 1: return new Color(0.5f, 0.4f, 0.3f);
                case 2: return new Color(0.6f, 0.6f, 0.5f);
                case 3: return new Color(0.2f, 0.3f, 0.15f);
                default: return Color.gray;
            }
        }

        public void AddLayer(TerrainLayer layer)
        {
            TerrainLayer[] current = m_terrain.terrainData.terrainLayers;
            TerrainLayer[] updated = new TerrainLayer[current.Length + 1];

            for (int i = 0; i < current.Length; i++)
            {
                updated[i] = current[i];
            }

            updated[updated.Length - 1] = layer;
            m_terrain.terrainData.terrainLayers = updated;
        }

        public void RemoveLayer(int index)
        {
            TerrainLayer[] current = m_terrain.terrainData.terrainLayers;
            if (index >= current.Length) return;

            TerrainLayer[] updated = new TerrainLayer[current.Length - 1];
            int offset = 0;

            for (int i = 0; i < current.Length; i++)
            {
                if (i != index)
                {
                    updated[i - offset] = current[i];
                }
                else
                {
                    offset = 1;
                }
            }

            m_terrain.terrainData.terrainLayers = updated;
        }

        public TerrainLayer[] GetLayers() => m_generatedLayers;
        public void SetTerrain(Terrain terrain) => m_terrain = terrain;
    }
}
