using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class EnvironmentOrganizer : MonoBehaviour
    {
        public static EnvironmentOrganizer Instance { get; private set; }

        [Header("Root Containers")]
        [SerializeField] private Transform m_terrainRoot;
        [SerializeField] private Transform m_roadsRoot;
        [SerializeField] private Transform m_buildingsRoot;
        [SerializeField] private Transform m_natureRoot;
        [SerializeField] private Transform m_propsRoot;
        [SerializeField] private Transform m_vehiclesRoot;
        [SerializeField] private Transform m_effectsRoot;

        [Header("Cleanup Settings")]
        [SerializeField] private bool m_autoCleanupOnPlay = false;
        [SerializeField] private bool m_mergeMeshesOnBuild = true;
        [SerializeField] private float m_maxObjectDistance = 500f;

        [Header("Performance")]
        [SerializeField] private bool m_enableCulling = true;
        [SerializeField] private bool m_optimizeMaterials = true;

        private List<Transform> m_allRoots = new List<Transform>();
        private Dictionary<string, List<GameObject>> m_objectsByCategory = new Dictionary<string, List<GameObject>>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateRoots();

            if (m_autoCleanupOnPlay)
            {
                CleanupHierarchy();
            }
        }

        private void CreateRoots()
        {
            m_allRoots.Clear();

            if (m_terrainRoot == null)
            {
                m_terrainRoot = CreateRoot("Terrain");
            }
            if (m_roadsRoot == null)
            {
                m_roadsRoot = CreateRoot("Roads");
            }
            if (m_buildingsRoot == null)
            {
                m_buildingsRoot = CreateRoot("Buildings");
            }
            if (m_natureRoot == null)
            {
                m_natureRoot = CreateRoot("Nature");
            }
            if (m_propsRoot == null)
            {
                m_propsRoot = CreateRoot("Props");
            }
            if (m_vehiclesRoot == null)
            {
                m_vehiclesRoot = CreateRoot("Vehicles");
            }
            if (m_effectsRoot == null)
            {
                m_effectsRoot = CreateRoot("Effects");
            }

            m_allRoots.AddRange(new[] { m_terrainRoot, m_roadsRoot, m_buildingsRoot, m_natureRoot, m_propsRoot, m_vehiclesRoot, m_effectsRoot });

            foreach (var root in m_allRoots)
            {
                if (root != null)
                {
                    root.SetParent(transform);
                }
            }
        }

        private Transform CreateRoot(string name)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(transform);
            return root.transform;
        }

        public void RegisterObject(GameObject obj, string category)
        {
            if (!m_objectsByCategory.ContainsKey(category))
            {
                m_objectsByCategory[category] = new List<GameObject>();
            }

            if (!m_objectsByCategory[category].Contains(obj))
            {
                m_objectsByCategory[category].Add(obj);
            }
        }

        public void UnregisterObject(GameObject obj, string category)
        {
            if (m_objectsByCategory.ContainsKey(category))
            {
                m_objectsByCategory[category].Remove(obj);
            }
        }

        public Transform GetRootForCategory(string category)
        {
            switch (category.ToLower())
            {
                case "terrain":
                    return m_terrainRoot;
                case "road":
                case "roads":
                    return m_roadsRoot;
                case "building":
                case "buildings":
                    return m_buildingsRoot;
                case "nature":
                case "tree":
                case "rock":
                case "grass":
                    return m_natureRoot;
                case "prop":
                case "props":
                    return m_propsRoot;
                case "vehicle":
                case "vehicles":
                    return m_vehiclesRoot;
                case "effect":
                case "effects":
                    return m_effectsRoot;
                default:
                    return m_propsRoot;
            }
        }

        public void OrganizeScene()
        {
            CleanupHierarchy();
            CategorizeAllObjects();
            OptimizeTransforms();
        }

        private void CleanupHierarchy()
        {
            foreach (var root in m_allRoots)
            {
                if (root == null) continue;

                for (int i = root.childCount - 1; i >= 0; i--)
                {
                    Transform child = root.GetChild(i);
                    if (child == null) continue;

                    if (child.childCount == 0 && child.GetComponents<Component>().Length <= 1)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
        }

        private void CategorizeAllObjects()
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (var obj in allObjects)
            {
                if (obj.transform.parent == null || IsRoot(obj.transform))
                {
                    continue;
                }

                string category = DetermineCategory(obj);
                Transform targetRoot = GetRootForCategory(category);

                if (obj.transform.parent != targetRoot)
                {
                    obj.transform.SetParent(targetRoot);
                }

                RegisterObject(obj, category);
            }
        }

        private string DetermineCategory(GameObject obj)
        {
            if (obj.CompareTag("Player") || obj.CompareTag("Vehicle"))
            {
                return "vehicle";
            }

            string name = obj.name.ToLower();

            if (name.Contains("tree") || name.Contains("rock") || name.Contains("grass") || name.Contains("foliage"))
            {
                return "nature";
            }

            if (name.Contains("road") || name.Contains("street") || name.Contains("asphalt"))
            {
                return "roads";
            }

            if (name.Contains("building") || name.Contains("house") || name.Contains("apartment"))
            {
                return "buildings";
            }

            if (name.Contains("prop") || name.Contains("lamp") || name.Contains("bench") || name.Contains("sign"))
            {
                return "props";
            }

            if (name.Contains("effect") || name.Contains("particle") || name.Contains("light"))
            {
                return "effects";
            }

            if (obj.GetComponent<Terrain>() != null)
            {
                return "terrain";
            }

            return "props";
        }

        private bool IsRoot(Transform t)
        {
            foreach (var root in m_allRoots)
            {
                if (t == root) return true;
            }
            return false;
        }

        private void OptimizeTransforms()
        {
            foreach (var root in m_allRoots)
            {
                if (root == null) continue;

                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    child.hasChanged = false;
                }
            }
        }

        public void EnableCulling()
        {
            if (!m_enableCulling) return;

            foreach (var kvp in m_objectsByCategory)
            {
                foreach (var obj in kvp.Value)
                {
                    if (obj == null) continue;

                    float distance = Vector3.Distance(obj.transform.position, Camera.main.transform.position);
                    bool shouldBeActive = distance < m_maxObjectDistance;

                    if (obj.activeSelf != shouldBeActive)
                    {
                        obj.SetActive(shouldBeActive);
                    }
                }
            }
        }

        public void OptimizeMaterials()
        {
            if (!m_optimizeMaterials) return;

            Renderer[] renderers = FindObjectsOfType<Renderer>();

            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length > 1)
                {
                    // TODO: When replacing placeholders, use material combining for better performance
                }
            }
        }

        public void MergeStaticMeshes()
        {
            if (!m_mergeMeshesOnBuild) return;

            foreach (var root in m_allRoots)
            {
                if (root == null || root.name == "Nature" || root.name == "Effects") continue;

                List<GameObject> children = new List<GameObject>();
                for (int i = 0; i < root.childCount; i++)
                {
                    children.Add(root.GetChild(i).gameObject);
                }

                // TODO: Use CombineMeshes for static geometry when artist assets are ready
            }
        }

        public Transform TerrainRoot => m_terrainRoot;
        public Transform RoadsRoot => m_roadsRoot;
        public Transform BuildingsRoot => m_buildingsRoot;
        public Transform NatureRoot => m_natureRoot;
        public Transform PropsRoot => m_propsRoot;
        public Transform VehiclesRoot => m_vehiclesRoot;
        public Transform EffectsRoot => m_effectsRoot;

        public int GetObjectCount(string category)
        {
            if (m_objectsByCategory.ContainsKey(category))
            {
                return m_objectsByCategory[category].Count;
            }
            return 0;
        }

        public Dictionary<string, int> GetAllCounts()
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (var kvp in m_objectsByCategory)
            {
                counts[kvp.Key] = kvp.Value.Count;
            }
            return counts;
        }
    }
}
