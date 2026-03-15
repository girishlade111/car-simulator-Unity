using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class PublicBuilding : MonoBehaviour
    {
        [Header("Building Info")]
        [SerializeField] private string m_buildingName;
        [Header("Capacity")]
        [SerializeField] private int m_capacity;
        [SerializeField] private int m_staffCount;

        [Header("Components")]
        [SerializeField] private Renderer[] m_wallRenderers;
        [SerializeField] private Renderer[] m_accentRenderers;

        [Header("Features")]
        [SerializeField] private bool m_hasPlayground;
        [SerializeField] private bool m_hasParking;
        [SerializeField] private bool m_hasSportsField;
        [SerializeField] private bool m_hasGarden;

        [Header("Parking")]
        [SerializeField] private Transform m_parkingArea;
        [SerializeField] private List<Transform> m_parkingSpots = new List<Transform>();

        private PublicBuildingData m_buildingData;

        public string BuildingName => m_buildingName;
        public int Capacity => m_capacity;
        public int StaffCount => m_staffCount;

        public void Initialize(PublicBuildingData data)
        {
            m_buildingData = data;
            m_buildingName = data.BuildingName;
            m_capacity = data.Capacity;
            m_staffCount = data.StaffCount;
            m_hasPlayground = data.HasPlayground;
            m_hasParking = data.HasParking;
            m_hasSportsField = data.HasSportsField;
            m_hasGarden = data.HasGarden;

            ApplyColors();
            GenerateFeatures();
        }

        private void ApplyColors()
        {
            if (m_buildingData == null) return;

            if (m_wallRenderers != null)
            {
                foreach (var renderer in m_wallRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_buildingData.PrimaryColor;
                    }
                }
            }

            if (m_accentRenderers != null)
            {
                foreach (var renderer in m_accentRenderers)
                {
                    if (renderer != null && renderer.material != null)
                    {
                        renderer.material.color = m_buildingData.AccentColor;
                    }
                }
            }
        }

        private void GenerateFeatures()
        {
            if (m_hasPlayground) CreatePlayground();
            if (m_hasSportsField) CreateSportsField();
            if (m_hasGarden) CreateGarden();
            if (m_hasParking) GenerateParking();
        }

        private void CreatePlayground()
        {
            GameObject playground = new GameObject("Playground");
            playground.transform.SetParent(transform);
            playground.transform.localPosition = new Vector3(10f, 0, 10f);

            GameObject slide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slide.name = "Slide";
            slide.transform.SetParent(playground.transform);
            slide.transform.localPosition = new Vector3(0, 1f, 0);
            slide.transform.localScale = new Vector3(1f, 2f, 3f);

            GameObject swing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            swing.name = "Swing";
            swing.transform.SetParent(playground.transform);
            swing.transform.localPosition = new Vector3(-3f, 1.5f, 0);
            swing.transform.localScale = new Vector3(0.2f, 2f, 0.2f);

            GameObject sandbox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sandbox.name = "Sandbox";
            sandbox.transform.SetParent(playground.transform);
            sandbox.transform.localPosition = new Vector3(3f, 0.2f, 2f);
            sandbox.transform.localScale = new Vector3(2f, 0.4f, 2f);

            Renderer renderer = sandbox.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.8f, 0.7f, 0.5f);
            }
        }

        private void CreateSportsField()
        {
            GameObject field = new GameObject("SportsField");
            field.transform.SetParent(transform);
            field.transform.localPosition = new Vector3(15f, 0.01f, 0);

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
            ground.transform.SetParent(field.transform);
            ground.transform.localPosition = Vector3.zero;
            ground.transform.localScale = new Vector3(20f, 15f, 1f);
            ground.transform.rotation = Quaternion.Euler(90, 0, 0);

            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.2f, 0.6f, 0.2f);
            }

            GameObject goals = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goals.name = "Goal1";
            goals.transform.SetParent(field.transform);
            goals.transform.localPosition = new Vector3(0, 1.5f, 7f);
            goals.transform.localScale = new Vector3(4f, 2.5f, 0.3f);

            GameObject goals2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goals2.name = "Goal2";
            goals2.transform.SetParent(field.transform);
            goals2.transform.localPosition = new Vector3(0, 1.5f, -7f);
            goals2.transform.localScale = new Vector3(4f, 2.5f, 0.3f);
        }

        private void CreateGarden()
        {
            GameObject garden = new GameObject("Garden");
            garden.transform.SetParent(transform);
            garden.transform.localPosition = new Vector3(-10f, 0, 8f);

            for (int i = 0; i < 5; i++)
            {
                GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tree.name = $"Tree_{i}";
                tree.transform.SetParent(garden.transform);
                tree.transform.localPosition = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
                tree.transform.localScale = new Vector3(0.3f, 2f, 0.3f);

                Renderer renderer = tree.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.4f, 0.3f, 0.2f);
                }

                GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leaves.name = $"Leaves_{i}";
                leaves.transform.SetParent(tree.transform);
                leaves.transform.localPosition = new Vector3(0, 2f, 0);
                leaves.transform.localScale = new Vector3(2f, 1.5f, 2f);

                Renderer leavesRenderer = leaves.GetComponent<Renderer>();
                if (leavesRenderer != null)
                {
                    leavesRenderer.material.color = new Color(0.2f, 0.5f, 0.2f);
                }
            }
        }

        private void GenerateParking()
        {
            if (m_parkingArea == null) return;

            m_parkingSpots.Clear();

            int spotsX = 4;
            int spotsZ = 3;

            for (int x = 0; x < spotsX; x++)
            {
                for (int z = 0; z < spotsZ; z++)
                {
                    GameObject spot = new GameObject($"ParkingSpot_{x}_{z}");
                    spot.transform.SetParent(m_parkingArea);

                    float xPos = (x - spotsX / 2f) * 3f;
                    float zPos = (z - spotsZ / 2f) * 5f;
                    spot.transform.localPosition = new Vector3(xPos, 0, zPos);

                    m_parkingSpots.Add(spot.transform);
                }
            }
        }

        public Transform GetFreeParkingSpot()
        {
            if (m_parkingSpots.Count == 0) return null;
            return m_parkingSpots[Random.Range(0, m_parkingSpots.Count)];
        }
    }
}
