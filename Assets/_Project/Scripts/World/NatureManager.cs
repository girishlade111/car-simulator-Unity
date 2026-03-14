using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class NatureManager : MonoBehaviour
    {
        public static NatureManager Instance { get; private set; }

        [Header("Organization")]
        [SerializeField] private Transform m_environmentRoot;
        [SerializeField] private Transform m_treesRoot;
        [SerializeField] private Transform m_rocksRoot;
        [SerializeField] private Transform m_grassRoot;
        [SerializeField] private Transform m_detailsRoot;

        [Header("Performance Settings")]
        [SerializeField] private bool m_batchingEnabled = true;
        [SerializeField] private int m_maxTreesPerBatch = 50;
        [SerializeField] private int m_maxRocksPerBatch = 30;

        [Header("Tree Settings")]
        [SerializeField] private GameObject m_treePrefab;
        [SerializeField] private int m_treeCount = 100;
        [SerializeField] private Vector2 m_treeAreaSize = new Vector2(400f, 400f);
        [SerializeField] private float m_minTreeScale = 0.8f;
        [SerializeField] private float m_maxTreeScale = 1.5f;

        [Header("Rock Settings")]
        [SerializeField] private GameObject m_rockPrefab;
        [SerializeField] private int m_rockCount = 50;
        [SerializeField] private Vector2 m_rockAreaSize = new Vector2(300f, 300f);
        [SerializeField] private bool m_clusterRocks = true;
        [SerializeField] private int m_rocksPerCluster = 3;

        [Header("Grass Settings")]
        [SerializeField] private GameObject m_grassPrefab;
        [SerializeField] private int m_grassPatchCount = 200;
        [SerializeField] private Vector2 m_grassAreaSize = new Vector2(350f, 350f);
        [SerializeField] private int m_grassBladesPerPatch = 10;

        [Header("Spawn Rules")]
        [SerializeField] private LayerMask m_groundLayer;
        [SerializeField] private bool m_avoidRoads = true;
        [SerializeField] private float m_minDistanceFromRoads = 8f;
        [SerializeField] private float m_minDistanceFromCenter = 20f;
        [SerializeField] private Collider[] m_roadColliders;

        [Header("LOD Settings")]
        [SerializeField] private float m_LODDistance = 100f;
        [SerializeField] private GameObject m_lowPolyTreePrefab;

        private List<GameObject> m_spawnedTrees = new List<GameObject>();
        private List<GameObject> m_spawnedRocks = new List<GameObject>();
        private List<GameObject> m_spawnedGrass = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateRoots();
        }

        private void CreateRoots()
        {
            if (m_environmentRoot == null)
            {
                m_environmentRoot = new GameObject("EnvironmentRoot").transform;
                m_environmentRoot.SetParent(transform);
            }

            if (m_treesRoot == null)
            {
                m_treesRoot = new GameObject("Trees").transform;
                m_treesRoot.SetParent(m_environmentRoot);
            }

            if (m_rocksRoot == null)
            {
                m_rocksRoot = new GameObject("Rocks").transform;
                m_rocksRoot.SetParent(m_environmentRoot);
            }

            if (m_grassRoot == null)
            {
                m_grassRoot = new GameObject("Grass").transform;
                m_grassRoot.SetParent(m_environmentRoot);
            }

            if (m_detailsRoot == null)
            {
                m_detailsRoot = new GameObject("Details").transform;
                m_detailsRoot.SetParent(m_environmentRoot);
            }
        }

        public void GenerateNature()
        {
            ClearNature();

            SpawnTrees();
            SpawnRocks();
            SpawnGrass();

            ApplyBatching();

            Debug.Log($"[NatureManager] Generated: {m_spawnedTrees.Count} trees, {m_spawnedRocks.Count} rocks, {m_spawnedGrass.Count} grass patches");
        }

        public void ClearNature()
        {
            ClearGroup(m_treesRoot);
            ClearGroup(m_rocksRoot);
            ClearGroup(m_grassRoot);
            ClearGroup(m_detailsRoot);

            m_spawnedTrees.Clear();
            m_spawnedRocks.Clear();
            m_spawnedGrass.Clear();
        }

        private void ClearGroup(Transform root)
        {
            if (root == null) return;

            for (int i = root.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(root.GetChild(i).gameObject);
            }
        }

        private void SpawnTrees()
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = m_treeCount * 3;

            while (spawned < m_treeCount && attempts < maxAttempts)
            {
                Vector3? pos = GetValidPosition(m_treeAreaSize, false);
                if (pos.HasValue)
                {
                    GameObject tree = SpawnTree(pos.Value);
                    if (tree != null)
                    {
                        spawned++;
                    }
                }
                attempts++;
            }
        }

        private GameObject SpawnTree(Vector3 position)
        {
            GameObject tree;

            if (m_treePrefab != null)
            {
                tree = Instantiate(m_treePrefab, position, Quaternion.identity, m_treesRoot);
            }
            else
            {
                tree = CreatePlaceholderTree(position);
                tree.transform.SetParent(m_treesRoot);
            }

            float scale = Random.Range(m_minTreeScale, m_maxTreeScale);
            tree.transform.localScale = Vector3.one * scale;
            tree.transform.Rotate(Vector3.up, Random.Range(0, 360));

            tree.name = $"Tree_{m_spawnedTrees.Count}";
            m_spawnedTrees.Add(tree);

            return tree;
        }

        private GameObject CreatePlaceholderTree(Vector3 position)
        {
            GameObject tree = new GameObject("Tree");
            tree.transform.position = position;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
            trunk.transform.localScale = new Vector3(0.3f, 1.5f, 0.3f);
            trunk.name = "Trunk";

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
            foliage.name = "Foliage";

            Renderer foliageRenderer = foliage.GetComponent<Renderer>();
            if (foliageRenderer != null)
            {
                foliageRenderer.material = new Material(Shader.Find("Standard"));
                foliageRenderer.material.color = new Color(0.2f, 0.5f, 0.15f);
            }

            return tree;
        }

        private void SpawnRocks()
        {
            if (m_clusterRocks)
            {
                SpawnRockClusters();
            }
            else
            {
                SpawnIndividualRocks();
            }
        }

        private void SpawnRockClusters()
        {
            int clusters = m_rockCount / m_rocksPerCluster;

            for (int c = 0; c < clusters; c++)
            {
                Vector3? clusterCenter = GetValidPosition(m_rockAreaSize, true);
                if (!clusterCenter.HasValue) continue;

                for (int i = 0; i < m_rocksPerCluster; i++)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-3f, 3f),
                        0,
                        Random.Range(-3f, 3f)
                    );

                    GameObject rock = SpawnRock(clusterCenter.Value + offset);
                    if (rock != null)
                    {
                        rock.transform.localScale *= Random.Range(0.7f, 1.3f);
                    }
                }
            }
        }

        private void SpawnIndividualRocks()
        {
            for (int i = 0; i < m_rockCount; i++)
            {
                Vector3? pos = GetValidPosition(m_rockAreaSize, true);
                if (pos.HasValue)
                {
                    SpawnRock(pos.Value);
                }
            }
        }

        private GameObject SpawnRock(Vector3 position)
        {
            GameObject rock;

            if (m_rockPrefab != null)
            {
                rock = Instantiate(m_rockPrefab, position, Random.rotation, m_rocksRoot);
            }
            else
            {
                rock = CreatePlaceholderRock(position);
                rock.transform.SetParent(m_rocksRoot);
            }

            rock.name = $"Rock_{m_spawnedRocks.Count}";
            m_spawnedRocks.Add(rock);

            return rock;
        }

        private GameObject CreatePlaceholderRock(Vector3 position)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.transform.position = position + Vector3.up * 0.3f;

            float width = Random.Range(0.8f, 2.5f);
            float height = Random.Range(0.4f, 1.2f);
            float depth = Random.Range(0.8f, 2.5f);
            rock.transform.localScale = new Vector3(width, height, depth);

            rock.transform.rotation = Quaternion.Euler(
                Random.Range(-15f, 15f),
                Random.Range(0, 360),
                Random.Range(-15f, 15f)
            );

            Renderer renderer = rock.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                float gray = Random.Range(0.4f, 0.6f);
                renderer.material.color = new Color(gray, gray, gray);
            }

            return rock;
        }

        private void SpawnGrass()
        {
            for (int i = 0; i < m_grassPatchCount; i++)
            {
                Vector3? pos = GetValidPosition(m_grassAreaSize, true);
                if (pos.HasValue)
                {
                    SpawnGrassPatch(pos.Value);
                }
            }
        }

        private void SpawnGrassPatch(Vector3 position)
        {
            GameObject patch;

            if (m_grassPrefab != null)
            {
                patch = Instantiate(m_grassPrefab, position, Quaternion.identity, m_grassRoot);
            }
            else
            {
                patch = CreatePlaceholderGrass(position);
                patch.transform.SetParent(m_grassRoot);
            }

            patch.name = $"GrassPatch_{m_spawnedGrass.Count}";
            m_spawnedGrass.Add(patch);
        }

        private GameObject CreatePlaceholderGrass(Vector3 position)
        {
            GameObject patch = new GameObject("GrassPatch");
            patch.transform.position = position;

            for (int i = 0; i < m_grassBladesPerPatch; i++)
            {
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blade.transform.SetParent(patch.transform);
                blade.transform.localPosition = new Vector3(
                    Random.Range(-0.8f, 0.8f),
                    0.3f,
                    Random.Range(-0.8f, 0.8f)
                );
                blade.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                blade.transform.localScale = new Vector3(0.15f, 0.4f, 0.15f);

                Renderer renderer = blade.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(Shader.Find("Standard"));
                    float green = Random.Range(0.3f, 0.5f);
                    renderer.material.color = new Color(0.2f, green, 0.1f);
                }
            }

            return patch;
        }

        private Vector3? GetValidPosition(Vector2 area, bool allowCenter)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = Random.Range(-area.x / 2f, area.x / 2f);
                float z = Random.Range(-area.y / 2f, area.y / 2f);

                if (!allowCenter && Mathf.Abs(x) < m_minDistanceFromCenter && Mathf.Abs(z) < m_minDistanceFromCenter)
                {
                    continue;
                }

                if (m_avoidRoads && IsNearRoad(new Vector3(x, 0, z)))
                {
                    continue;
                }

                Vector3 pos = new Vector3(x, 0, z);

                if (m_groundLayer != 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out hit, 100f, m_groundLayer))
                    {
                        return hit.point;
                    }
                }
                else
                {
                    return pos;
                }
            }

            return null;
        }

        private bool IsNearRoad(Vector3 position)
        {
            if (m_roadColliders == null) return false;

            foreach (var collider in m_roadColliders)
            {
                if (collider != null)
                {
                    Bounds bounds = collider.bounds;
                    bounds.Expand(m_minDistanceFromRoads);
                    if (bounds.Contains(position))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ApplyBatching()
        {
            if (!m_batchingEnabled) return;

            StaticBatchingUtility.Combine(m_spawnedTrees.ToArray(), m_treesRoot.gameObject);
            StaticBatchingUtility.Combine(m_spawnedRocks.ToArray(), m_rocksRoot.gameObject);
        }

        public void SetRoadColliders(Collider[] colliders)
        {
            m_roadColliders = colliders;
        }

        public void RefreshNature()
        {
            GenerateNature();
        }

        public int GetTreeCount() => m_spawnedTrees.Count;
        public int GetRockCount() => m_spawnedRocks.Count;
        public int GetGrassPatchCount() => m_spawnedGrass.Count;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_treeAreaSize.x, 10f, m_treeAreaSize.y));

            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_rockAreaSize.x, 10f, m_rockAreaSize.y));

            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Gizmos.DrawWireCube(transform.position, new Vector3(m_grassAreaSize.x, 10f, m_grassAreaSize.y));
        }
    }
}
