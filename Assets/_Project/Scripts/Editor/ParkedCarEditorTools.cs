using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CarSimulator.Editor
{
    public class ParkedCarEditorTools : EditorWindow
    {
        [MenuItem("Car Simulator/Parked Car Tools")]
        public static void ShowWindow()
        {
            GetWindow<ParkedCarEditorTools>("Parked Car Tools");
        }

        [Header("Quick Spawn")]
        [SerializeField] private GameObject m_carPrefab;
        [SerializeField] private int m_spawnCount = 1;
        [SerializeField] private float m_spacing = 8f;
        [SerializeField] private float m_rotation = 0f;

        [Header("Batch Operations")]
        [SerializeField] private bool m_randomizeColors = true;
        [SerializeField] private bool m_alignToGround = true;

        private void OnGUI()
        {
            GUILayout.Label("Parked Car Placement Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            GUILayout.Label("Quick Spawn", EditorStyles.boldLabel);
            m_carPrefab = (GameObject)EditorGUILayout.ObjectField("Car Prefab", m_carPrefab, typeof(GameObject), false);
            m_spawnCount = EditorGUILayout.IntSlider("Count", m_spawnCount, 1, 20);
            m_spacing = EditorGUILayout.FloatField("Spacing", m_spacing);
            m_rotation = EditorGUILayout.FloatField("Rotation (Y)", m_rotation);

            if (GUILayout.Button("Spawn Parked Cars"))
            {
                SpawnParkedCars();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Batch Operations", EditorStyles.boldLabel);
            m_randomizeColors = EditorGUILayout.Toggle("Randomize Colors", m_randomizeColors);
            m_alignToGround = EditorGUILayout.Toggle("Align to Ground", m_alignToGround);

            if (GUILayout.Button("Randomize All Colors"))
            {
                RandomizeAllColors();
            }

            if (GUILayout.Button("Align Selected to Ground"))
            {
                AlignSelectedToGround();
            }

            if (GUILayout.Button("Find All Parked Cars"))
            {
                FindAllParkedCars();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Utilities", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Parked Car Spawn Point"))
            {
                CreateSpawnPoint();
            }

            if (GUILayout.Button("Create Parked Car Group"))
            {
                CreateCarGroup();
            }

            if (GUILayout.Button("Create RoadSide Spawner"))
            {
                CreateRoadSideSpawner();
            }
        }

        private void SpawnParkedCars()
        {
            if (m_carPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a car prefab", "OK");
                return;
            }

            GameObject parent = new GameObject($"ParkedCars_Batch_{System.DateTime.Now.Ticks}");
            parent.transform.position = Vector3.zero;

            for (int i = 0; i < m_spawnCount; i++)
            {
                Vector3 position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 5f;
                position += Vector3.right * (i * m_spacing);

                GameObject car = Instantiate(m_carPrefab, position, Quaternion.Euler(0, m_rotation, 0));
                car.transform.SetParent(parent.transform);
                car.name = $"ParkedCar_{i}";

                if (m_randomizeColors)
                {
                    var parkedCar = car.GetComponent<World.ParkedCar>();
                    if (parkedCar == null)
                    {
                        parkedCar = car.AddComponent<World.ParkedCar>();
                    }
                    parkedCar.SetBodyColor(GetRandomColor());
                }
            }

            Selection.activeGameObject = parent;
            Debug.Log($"[ParkedCarTools] Spawned {m_spawnCount} parked cars");
        }

        private void RandomizeAllColors()
        {
            var parkedCars = FindObjectsOfType<World.ParkedCar>();
            int count = 0;

            foreach (var car in parkedCars)
            {
                car.SetBodyColor(GetRandomColor());
                count++;
            }

            Debug.Log($"[ParkedCarTools] Randomized colors for {count} parked cars");
        }

        private void AlignSelectedToGround()
        {
            var selected = Selection.gameObjects;
            int count = 0;

            foreach (var obj in selected)
            {
                Ray ray = new Ray(obj.transform.position + Vector3.up * 50f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    obj.transform.position = hit.point;
                    count++;
                }
            }

            Debug.Log($"[ParkedCarTools] Aligned {count} objects to ground");
        }

        private void FindAllParkedCars()
        {
            var parkedCars = FindObjectsOfType<World.ParkedCar>();
            var spawnPoints = FindObjectsOfType<World.ParkedCarSpawnPoint>();
            var groups = FindObjectsOfType<World.ParkedCarGroup>();
            var roadside = FindObjectsOfType<World.RoadSideSpawner>();

            Debug.Log($"[ParkedCarTools] Found:\n" +
                     $"  ParkedCars: {parkedCars.Length}\n" +
                     $"  SpawnPoints: {spawnPoints.Length}\n" +
                     $"  Groups: {groups.Length}\n" +
                     $"  RoadSideSpawners: {roadside.Length}");

            List<GameObject> allObjects = new List<GameObject>();
            
            foreach (var car in parkedCars) allObjects.Add(car.gameObject);
            foreach (var sp in spawnPoints) allObjects.Add(sp.gameObject);
            foreach (var gr in groups) allObjects.Add(gr.gameObject);
            foreach (var rs in roadside) allObjects.Add(rs.gameObject);

            if (allObjects.Count > 0)
            {
                Selection.objects = allObjects.ToArray();
            }
        }

        private void CreateSpawnPoint()
        {
            GameObject obj = new GameObject("ParkedCarSpawnPoint");
            obj.AddComponent<World.ParkedCarSpawnPoint>();
            obj.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 5f;
            Selection.activeGameObject = obj;
        }

        private void CreateCarGroup()
        {
            GameObject obj = new GameObject("ParkedCarGroup");
            obj.AddComponent<World.ParkedCarGroup>();
            obj.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 5f;
            Selection.activeGameObject = obj;
        }

        private void CreateRoadSideSpawner()
        {
            GameObject obj = new GameObject("RoadSideSpawner");
            obj.AddComponent<World.RoadSideSpawner>();
            obj.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 5f;
            Selection.activeGameObject = obj;
        }

        private Color GetRandomColor()
        {
            Color[] colors = {
                Color.white,
                Color.black,
                new Color(0.8f, 0.2f, 0.2f),
                new Color(0.2f, 0.3f, 0.8f),
                new Color(0.9f, 0.9f, 0.2f),
                new Color(0.2f, 0.7f, 0.3f),
                new Color(0.6f, 0.3f, 0.7f),
                new Color(0.9f, 0.5f, 0.2f),
                new Color(0.3f, 0.3f, 0.3f),
                new Color(0.7f, 0.7f, 0.7f)
            };

            return colors[Random.Range(0, colors.Length)];
        }
    }
}
