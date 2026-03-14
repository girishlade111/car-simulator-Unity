using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class NaturePrefabGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private bool m_generateOnStart = false;
        [SerializeField] private string m_outputFolder = "Assets/_Project/Prefabs/Environment";

        [Header("Tree Settings")]
        [SerializeField] private int m_treeVariations = 3;
        [SerializeField] private Vector2 m_treeHeightRange = new Vector2(4f, 8f);
        [SerializeField] private Vector2 m_trunkThicknessRange = new Vector2(0.2f, 0.5f);

        [Header("Rock Settings")]
        [SerializeField] private int m_rockVariations = 3;
        [SerializeField] private Vector2 m_rockSizeRange = new Vector2(0.5f, 2f);

        [Header("Grass Settings")]
        [SerializeField] private int m_grassVariations = 2;
        [SerializeField] private Vector2 m_grassHeightRange = new Vector2(0.3f, 0.8f);

        private List<GameObject> m_generatedTrees = new List<GameObject>();
        private List<GameObject> m_generatedRocks = new List<GameObject>();
        private List<GameObject> m_generatedGrass = new List<GameObject>();

        private void Start()
        {
            if (m_generateOnStart)
            {
                GenerateAll();
            }
        }

        public void GenerateAll()
        {
            GenerateTrees();
            GenerateRocks();
            GenerateGrass();
            Debug.Log($"[NaturePrefabGenerator] Generated: {m_generatedTrees.Count} trees, {m_generatedRocks.Count} rocks, {m_generatedGrass.Count} grass");
        }

        public List<GameObject> GenerateTrees()
        {
            m_generatedTrees.Clear();

            for (int i = 0; i < m_treeVariations; i++)
            {
                GameObject tree = CreateTree(i);
                m_generatedTrees.Add(tree);
            }

            return m_generatedTrees;
        }

        private GameObject CreateTree(int variation)
        {
            GameObject tree = new GameObject($"Tree_variation_{variation}");
            tree.transform.position = Vector3.zero;

            float height = Random.Range(m_treeHeightRange.x, m_treeHeightRange.y);
            float trunkThickness = Random.Range(m_trunkThicknessRange.x, m_trunkThicknessRange.y);
            float foliageSize = height * 0.5f;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, height * 0.3f, 0);
            trunk.transform.localScale = new Vector3(trunkThickness, height * 0.4f, trunkThickness);
            trunk.GetComponent<Renderer>().material = CreateBarkMaterial(variation);

            int foliageLayers = variation + 2;
            for (int i = 0; i < foliageLayers; i++)
            {
                GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                foliage.name = $"Foliage_{i}";
                foliage.transform.SetParent(tree.transform);
                
                float layerHeight = height * (0.4f + i * 0.2f);
                float layerSize = foliageSize * (1f - i * 0.2f);
                foliage.transform.localPosition = new Vector3(0, layerHeight, 0);
                foliage.transform.localScale = new Vector3(layerSize, layerSize * 0.8f, layerSize);
                foliage.GetComponent<Renderer>().material = CreateFoliageMaterial(variation);
            }

            AddCollider(tree);

            return tree;
        }

        public List<GameObject> GenerateRocks()
        {
            m_generatedRocks.Clear();

            for (int i = 0; i < m_rockVariations; i++)
            {
                GameObject rock = CreateRock(i);
                m_generatedRocks.Add(rock);
            }

            return m_generatedRocks;
        }

        private GameObject CreateRock(int variation)
        {
            GameObject rock = new GameObject($"Rock_variation_{variation}");
            rock.transform.position = Vector3.zero;

            float baseSize = Random.Range(m_rockSizeRange.x, m_rockSizeRange.y);
            
            int segments = Random.Range(3, 6);
            for (int i = 0; i < segments; i++)
            {
                GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                segment.name = $"Segment_{i}";
                segment.transform.SetParent(rock.transform);

                float segSize = baseSize * Random.Range(0.5f, 1f);
                segment.transform.localPosition = Random.insideUnitSphere * baseSize * 0.5f;
                segment.transform.localScale = Vector3.one * segSize;
                segment.transform.rotation = Random.rotation;
                
                segment.GetComponent<Renderer>().material = CreateRockMaterial(variation);
            }

            AddCollider(rock);

            return rock;
        }

        public List<GameObject> GenerateGrass()
        {
            m_generatedGrass.Clear();

            for (int i = 0; i < m_grassVariations; i++)
            {
                GameObject grass = CreateGrassPatch(i);
                m_generatedGrass.Add(grass);
            }

            return m_generatedGrass;
        }

        private GameObject CreateGrassPatch(int variation)
        {
            GameObject grass = new GameObject($"Grass_variation_{variation}");
            grass.transform.position = Vector3.zero;

            float height = Random.Range(m_grassHeightRange.x, m_grassHeightRange.y);
            int blades = Random.Range(3, 7);

            for (int i = 0; i < blades; i++)
            {
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blade.name = $"Blade_{i}";
                blade.transform.SetParent(grass.transform);

                blade.transform.localPosition = Random.insideUnitCircle * 0.3f;
                blade.transform.localPosition += Vector3.up * height * 0.5f;
                blade.transform.localScale = new Vector3(0.1f, height, 0.1f);
                blade.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                
                blade.GetComponent<Renderer>().material = CreateGrassMaterial(variation);
            }

            return grass;
        }

        private Material CreateBarkMaterial(int variation)
        {
            Material mat = new Material(Shader.Find("Standard"));
            float brown = Random.Range(0.3f, 0.5f);
            mat.color = new Color(brown, brown * 0.8f, brown * 0.6f);
            return mat;
        }

        private Material CreateFoliageMaterial(int variation)
        {
            Material mat = new Material(Shader.Find("Standard"));
            float green = Random.Range(0.3f, 0.5f);
            mat.color = new Color(green * 0.4f, green, green * 0.3f);
            return mat;
        }

        private Material CreateRockMaterial(int variation)
        {
            Material mat = new Material(Shader.Find("Standard"));
            float gray = Random.Range(0.4f, 0.7f);
            mat.color = new Color(gray, gray * 0.95f, gray * 0.9f);
            return mat;
        }

        private Material CreateGrassMaterial(int variation)
        {
            Material mat = new Material(Shader.Find("Standard"));
            float green = Random.Range(0.4f, 0.6f);
            mat.color = new Color(green * 0.3f, green, green * 0.2f);
            mat.SetFloat("_Mode", 3);
            return mat;
        }

        private void AddCollider(GameObject obj)
        {
            MeshCollider collider = obj.AddComponent<MeshCollider>();
            collider.convex = true;
        }

        public List<GameObject> GetGeneratedTrees() => m_generatedTrees;
        public List<GameObject> GetGeneratedRocks() => m_generatedRocks;
        public List<GameObject> GetGeneratedGrass() => m_generatedGrass;
    }
}
