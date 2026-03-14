using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class TestDistrictBuilder : MonoBehaviour
    {
        [Header("District Settings")]
        [SerializeField] private string m_districtName = "TestDistrict";
        [SerializeField] private Vector2 m_districtSize = new Vector2(300f, 300f);

        [Header("Road Layout")]
        [SerializeField] private bool m_createCrossRoads = true;
        [SerializeField] private int m_mainRoadLength = 100;
        [SerializeField] private int m_sideRoadCount = 2;

        [Header("Environment")]
        [SerializeField] private bool m_generateNature = true;
        [SerializeField] private bool m_addBoundary = true;
        [SerializeField] private bool m_addSpawnPoint = true;

        [Header("Prefabs")]
        [SerializeField] private Material m_roadMaterial;
        [SerializeField] private Material m_groundMaterial;

        private Transform m_districtRoot;

        [ContextMenu("Build Test District")]
        public void BuildTestDistrict()
        {
            ClearDistrict();
            CreateDistrictRoot();
            CreateGround();
            CreateRoads();
            
            if (m_generateNature)
            {
                CreateNature();
            }

            if (m_addBoundary)
            {
                CreateBoundary();
            }

            if (m_addSpawnPoint)
            {
                CreateSpawnPoint();
            }

            CreateManagers();
            
            Debug.Log($"[TestDistrictBuilder] Built district: {m_districtName}");
        }

        [ContextMenu("Clear District")]
        public void ClearDistrict()
        {
            if (m_districtRoot != null)
            {
                DestroyImmediate(m_districtRoot.gameObject);
            }
        }

        private void CreateDistrictRoot()
        {
            GameObject root = new GameObject(m_districtName);
            root.transform.SetParent(transform);
            m_districtRoot = root.transform;
        }

        private void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(m_districtRoot);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(m_districtSize.x / 10f, 1f, m_districtSize.y / 10f);

            if (m_groundMaterial != null)
            {
                ground.GetComponent<Renderer>().material = m_groundMaterial;
            }

            ground.layer = LayerMask.NameToLayer("Ground");
        }

        private void CreateRoads()
        {
            GameObject roadRoot = new GameObject("Roads");
            roadRoot.transform.SetParent(m_districtRoot);

            if (m_createCrossRoads)
            {
                CreateStraightRoad(roadRoot.transform, Vector3.zero, m_mainRoadLength, 0);
                CreateStraightRoad(roadRoot.transform, Vector3.zero, m_mainRoadLength, 90);
            }

            for (int i = 0; i < m_sideRoadCount; i++)
            {
                float offset = (i + 1) * (m_mainRoadLength / (m_sideRoadCount + 1));
                CreateStraightRoad(roadRoot.transform, new Vector3(offset, 0, 0), m_mainRoadLength / 2f, 0);
                CreateStraightRoad(roadRoot.transform, new Vector3(-offset, 0, 0), m_mainRoadLength / 2f, 0);
                CreateStraightRoad(roadRoot.transform, new Vector3(0, 0, offset), m_mainRoadLength / 2f, 90);
                CreateStraightRoad(roadRoot.transform, new Vector3(0, 0, -offset), m_mainRoadLength / 2f, 90);
            }
        }

        private void CreateStraightRoad(Transform parent, Vector3 position, float length, float rotationY)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Road_Straight";
            road.transform.SetParent(parent);
            road.transform.position = position + Vector3.up * 0.01f;
            road.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            road.transform.localScale = new Vector3(8f, 0.02f, length);

            if (m_roadMaterial != null)
            {
                road.GetComponent<Renderer>().material = m_roadMaterial;
            }

            road.layer = LayerMask.NameToLayer("Ground");

            RoadSegment roadSegment = road.AddComponent<RoadSegment>();
            roadSegment.m_length = length;
            roadSegment.m_width = 8f;
            roadSegment.m_connectNorth = true;
            roadSegment.m_connectSouth = true;
            roadSegment.m_connectEast = true;
            roadSegment.m_connectWest = true;
        }

        private void CreateNature()
        {
            GameObject natureRoot = new GameObject("Nature");
            natureRoot.transform.SetParent(m_districtRoot);

            float areaSize = Mathf.Min(m_districtSize.x, m_districtSize.y) * 0.4f;

            for (int i = 0; i < 30; i++)
            {
                Vector3 pos = GetRandomNaturePosition(areaSize);
                CreateTree(natureRoot.transform, pos);
            }

            for (int i = 0; i < 15; i++)
            {
                Vector3 pos = GetRandomNaturePosition(areaSize);
                CreateRock(natureRoot.transform, pos);
            }
        }

        private Vector3 GetRandomNaturePosition(float areaSize)
        {
            float minDist = 20f;
            
            for (int attempts = 0; attempts < 10; attempts++)
            {
                float x = Random.Range(-areaSize, areaSize);
                float z = Random.Range(-areaSize, areaSize);

                if (Mathf.Abs(x) < minDist || Mathf.Abs(z) < minDist)
                {
                    continue;
                }

                return new Vector3(x, 0, z);
            }

            return Vector3.zero;
        }

        private void CreateTree(Transform parent, Vector3 position)
        {
            GameObject tree = new GameObject("Tree");
            tree.transform.SetParent(parent);
            tree.transform.position = position;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 2f, 0);
            trunk.transform.localScale = new Vector3(0.5f, 2f, 0.5f);

            GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.transform.SetParent(tree.transform);
            leaves.transform.localPosition = new Vector3(0, 5f, 0);
            leaves.transform.localScale = new Vector3(3f, 4f, 3f);

            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        private void CreateRock(Transform parent, Vector3 position)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.transform.SetParent(parent);
            rock.transform.position = position + Vector3.up * 0.5f;
            rock.transform.localScale = new Vector3(
                Random.Range(1f, 2f),
                Random.Range(0.5f, 1f),
                Random.Range(1f, 2f)
            );
            rock.transform.rotation = Quaternion.Euler(
                Random.Range(0, 30),
                Random.Range(0, 360),
                Random.Range(0, 30)
            );
        }

        private void CreateBoundary()
        {
            GameObject boundary = new GameObject("DistrictBoundary");
            boundary.transform.SetParent(m_districtRoot);
            boundary.transform.position = Vector3.zero;

            DistrictBoundary boundaryComp = boundary.AddComponent<DistrictBoundary>();
            boundaryComp.m_size = m_districtSize;
            boundaryComp.m_wallHeight = 20f;
            boundaryComp.m_warningDistance = 30f;
            boundaryComp.m_showWarning = true;
        }

        private void CreateSpawnPoint()
        {
            GameObject spawnPoint = new GameObject("PlayerSpawnPoint");
            spawnPoint.transform.SetParent(m_districtRoot);
            spawnPoint.transform.position = new Vector3(0, 0.5f, 0);

            SpawnPoint spawnComp = spawnPoint.AddComponent<SpawnPoint>();
            spawnComp.m_isDefaultSpawn = true;
            spawnComp.m_spawnPointName = "MainSpawn";
        }

        private void CreateManagers()
        {
            GameObject districtManagerObj = new GameObject("DistrictManager");
            districtManagerObj.transform.SetParent(m_districtRoot);
            districtManagerObj.transform.position = Vector3.zero;

            DistrictManager districtManager = districtManagerObj.AddComponent<DistrictManager>();
            districtManager.m_districtSize = m_districtSize;

            GameObject spawnManagerObj = new GameObject("SpawnManager");
            spawnManagerObj.transform.SetParent(m_districtRoot);
            spawnManagerObj.transform.position = Vector3.zero;
            spawnManagerObj.AddComponent<SpawnManager>();
        }
    }
}

#if UNITY_EDITOR
namespace CarSimulator.World
{
    [CustomEditor(typeof(TestDistrictBuilder))]
    public class TestDistrictBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TestDistrictBuilder builder = (TestDistrictBuilder)target;

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Build Test District"))
            {
                builder.BuildTestDistrict();
            }

            if (GUILayout.Button("Clear District"))
            {
                builder.ClearDistrict();
            }
        }
    }
}
#endif
