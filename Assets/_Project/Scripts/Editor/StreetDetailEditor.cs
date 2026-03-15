using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CarSimulator.Editor
{
    public class StreetDetailEditor : EditorWindow
    {
        [MenuItem("Car Simulator/Street Details")]
        public static void ShowWindow()
        {
            GetWindow<StreetDetailEditor>("Street Details");
        }

        [SerializeField] private StreetDetailPlaceholder.DetailType m_selectedType = StreetDetailPlaceholder.DetailType.Bench;
        [SerializeField] private int m_spawnCount = 1;
        [SerializeField] private float m_spacing = 5f;
        [SerializeField] private bool m_randomRotation = true;
        [SerializeField] private bool m_alignToGround = true;
        [SerializeField] private float m_groundOffset = 0f;

        private void OnGUI()
        {
            GUILayout.Label("Street Detail Placement Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            m_selectedType = (StreetDetailPlaceholder.DetailType)EditorGUILayout.EnumPopup("Detail Type", m_selectedType);
            m_spawnCount = EditorGUILayout.IntSlider("Count", m_spawnCount, 1, 20);
            m_spacing = EditorGUILayout.FloatField("Spacing", m_spacing);
            m_randomRotation = EditorGUILayout.Toggle("Random Rotation", m_randomRotation);
            m_alignToGround = EditorGUILayout.Toggle("Align to Ground", m_alignToGround);
            m_groundOffset = EditorGUILayout.FloatField("Ground Offset", m_groundOffset);

            EditorGUILayout.Space();

            if (GUILayout.Button("Spawn Details"))
            {
                SpawnDetails();
            }

            if (GUILayout.Button("Spawn Along Road"))
            {
                SpawnAlongRoad();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Quick Categories", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Benches"))
            {
                SpawnCategory(StreetDetailPlaceholder.DetailType.Bench, 5);
            }
            if (GUILayout.Button("Lamps"))
            {
                SpawnCategory(StreetDetailPlaceholder.DetailType.StreetLamp, 5);
            }
            if (GUILayout.Button("Trash Bins"))
            {
                SpawnCategory(StreetDetailPlaceholder.DetailType.TrashBin, 5);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Signs"))
            {
                SpawnCategory(StreetDetailPlaceholder.DetailType.StopSign, 5);
            }
            if (GUILayout.Button("Barriers"))
            {
                SpawnCategory(StreetDetailPlaceholder.DetailType.Barrier, 5);
            }
            if (GUILayout.Button("Plants"))
            {
                SpawnCategory(StreetDetailPlaceholder.DetailType.Plant, 5);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Find All Details"))
            {
                FindAllDetails();
            }

            if (GUILayout.Button("Create Detail Parent"))
            {
                CreateDetailParent();
            }
        }

        private void SpawnDetails()
        {
            GameObject parent = new GameObject($"StreetDetails_{m_selectedType}");
            parent.transform.position = Vector3.zero;

            for (int i = 0; i < m_spawnCount; i++)
            {
                Vector3 position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 5f;
                position += Vector3.right * (i * m_spacing);

                if (m_alignToGround)
                {
                    Ray ray = new Ray(position + Vector3.up * 20f, Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                    {
                        position = hit.point + Vector3.up * m_groundOffset;
                    }
                }

                GameObject detail = StreetDetailPlaceholder.CreatePlaceholder(m_selectedType, $"{m_selectedType}_{i}");
                detail.transform.position = position;

                if (m_randomRotation)
                {
                    detail.transform.Rotate(Vector3.up, Random.Range(0, 360));
                }

                detail.transform.SetParent(parent.transform);
            }

            Selection.activeGameObject = parent;
            Debug.Log($"[StreetDetailEditor] Spawned {m_spawnCount} {m_selectedType} details");
        }

        private void SpawnAlongRoad()
        {
            var roads = FindObjectsOfType<RoadSegment>();
            if (roads.Length == 0)
            {
                EditorUtility.DisplayDialog("No Roads", "No RoadSegment found in scene", "OK");
                return;
            }

            GameObject parent = new GameObject($"RoadDetails_{m_selectedType}");

            foreach (var road in roads)
            {
                Vector3 direction = road.transform.forward;
                float length = road.Length;

                int count = Mathf.RoundToInt(length / m_spacing);

                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / count;
                    Vector3 pos = road.transform.position + direction * (t * length - length / 2f);

                    Vector3 offset = Vector3.Cross(direction, Vector3.up).normalized * 5f;
                    pos += offset;

                    if (m_alignToGround)
                    {
                        Ray ray = new Ray(pos + Vector3.up * 20f, Vector3.down);
                        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                        {
                            pos = hit.point + Vector3.up * m_groundOffset;
                        }
                    }

                    GameObject detail = StreetDetailPlaceholder.CreatePlaceholder(m_selectedType, $"{m_selectedType}_{i}");
                    detail.transform.position = pos;
                    detail.transform.rotation = Quaternion.LookRotation(direction);

                    if (m_randomRotation)
                    {
                        detail.transform.Rotate(Vector3.up, Random.Range(-15f, 15f));
                    }

                    detail.transform.SetParent(parent.transform);
                }
            }

            Selection.activeGameObject = parent;
            Debug.Log($"[StreetDetailEditor] Spawned along roads");
        }

        private void SpawnCategory(StreetDetailPlaceholder.DetailType type, int count)
        {
            m_selectedType = type;
            m_spawnCount = count;
            SpawnDetails();
        }

        private void FindAllDetails()
        {
            var details = FindObjectsOfType<StreetDetailPlaceholder>();
            var spawnAnchors = FindObjectsOfType<PropSpawnAnchor>();
            var debrisScatters = FindObjectsOfType<DebrisScatter>();
            var propGroups = FindObjectsOfType<PropGroupContainer>();

            Debug.Log($"[StreetDetailEditor] Found:\n" +
                     $"  Street Details: {details.Length}\n" +
                     $"  Prop Anchors: {spawnAnchors.Length}\n" +
                     $"  Debris Scatters: {debrisScatters.Length}\n" +
                     $"  Prop Groups: {propGroups.Length}");

            List<GameObject> allObjects = new List<GameObject>();
            foreach (var d in details) allObjects.Add(d.gameObject);
            foreach (var a in spawnAnchors) allObjects.Add(a.gameObject);
            foreach (var d in debrisScatters) allObjects.Add(d.gameObject);
            foreach (var p in propGroups) allObjects.Add(p.gameObject);

            if (allObjects.Count > 0)
            {
                Selection.objects = allObjects.ToArray();
            }
        }

        private void CreateDetailParent()
        {
            GameObject parent = new GameObject("StreetDetails_Container");
            parent.transform.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 10f;

            PropGroupContainer group = parent.AddComponent<PropGroupContainer>();
            group.m_groupName = "New Prop Group";
            group.m_groupType = PropGroupContainer.PropGroupType.Street;

            Selection.activeGameObject = parent;
        }
    }
}
