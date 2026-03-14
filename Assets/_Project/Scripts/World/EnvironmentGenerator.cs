using UnityEngine;

namespace CarSimulator.World
{
    public class EnvironmentGenerator : MonoBehaviour
    {
        [Header("Ground")]
        [SerializeField] private Vector2 m_groundSize = new Vector2(500f, 500f);
        [SerializeField] private Material m_groundMaterial;

        [Header("Nature Props")]
        [SerializeField] private int m_treeCount = 50;
        [SerializeField] private int m_rockCount = 20;
        [SerializeField] private int m_grassClusterCount = 30;
        [SerializeField] private Vector2 m_natureAreaSize = new Vector2(200f, 200f);

        [Header("Spawn Areas")]
        [SerializeField] private bool m_spawnOnRoads = false;
        [SerializeField] private float m_minDistanceFromRoad = 5f;
        [SerializeField] private float m_minDistanceFromCenter = 15f;

        private GameObject m_groundObject;
        private Transform m_natureRoot;

        public void GenerateEnvironment()
        {
            CreateGround();
            CreateNatureArea();
        }

        public void ClearEnvironment()
        {
            if (m_groundObject != null)
            {
                DestroyImmediate(m_groundObject);
                m_groundObject = null;
            }

            if (m_natureRoot != null)
            {
                DestroyImmediate(m_natureRoot.gameObject);
                m_natureRoot = null;
            }
        }

        private void CreateGround()
        {
            m_groundObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            m_groundObject.name = "Ground";
            m_groundObject.transform.SetParent(transform);
            m_groundObject.transform.position = Vector3.zero;
            m_groundObject.transform.localScale = new Vector3(m_groundSize.x / 10f, 1f, m_groundSize.y / 10f);

            if (m_groundMaterial != null)
            {
                m_groundObject.GetComponent<Renderer>().material = m_groundMaterial;
            }

            m_groundObject.layer = LayerMask.NameToLayer("Ground");
        }

        private void CreateNatureArea()
        {
            m_natureRoot = new GameObject("NatureRoot").transform;
            m_natureRoot.SetParent(transform);

            for (int i = 0; i < m_treeCount; i++)
            {
                Vector3? pos = GetValidNaturePosition();
                if (pos.HasValue)
                {
                    CreateTree(pos.Value);
                }
            }

            for (int i = 0; i < m_rockCount; i++)
            {
                Vector3? pos = GetValidNaturePosition();
                if (pos.HasValue)
                {
                    CreateRock(pos.Value);
                }
            }

            for (int i = 0; i < m_grassClusterCount; i++)
            {
                Vector3? pos = GetValidNaturePosition();
                if (pos.HasValue)
                {
                    CreateGrassCluster(pos.Value);
                }
            }
        }

        private Vector3? GetValidNaturePosition()
        {
            for (int attempts = 0; attempts < 10; attempts++)
            {
                float x = Random.Range(-m_natureAreaSize.x / 2f, m_natureAreaSize.x / 2f);
                float z = Random.Range(-m_natureAreaSize.y / 2f, m_natureAreaSize.y / 2f);

                if (Mathf.Abs(x) < m_minDistanceFromCenter && Mathf.Abs(z) < m_minDistanceFromCenter)
                {
                    continue;
                }

                if (!m_spawnOnRoads)
                {
                    if (IsNearRoad(x, z))
                    {
                        continue;
                    }
                }

                return new Vector3(x, 0, z);
            }

            return null;
        }

        private bool IsNearRoad(float x, float z)
        {
            float roadWidth = 8f;
            float checkRoadX = 0f;
            float checkRoadZ = 0f;

            if (Mathf.Abs(x - checkRoadX) < roadWidth / 2f + m_minDistanceFromRoad ||
                Mathf.Abs(z - checkRoadZ) < roadWidth / 2f + m_minDistanceFromRoad)
            {
                return true;
            }

            return false;
        }

        private void CreateTree(Vector3 position)
        {
            GameObject tree = new GameObject($"Tree_{Random.Range(1, 1000)}");
            tree.transform.SetParent(m_natureRoot);
            tree.transform.position = position;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 2f, 0);
            trunk.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
            trunk.name = "Trunk";

            GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.transform.SetParent(tree.transform);
            leaves.transform.localPosition = new Vector3(0, 5f, 0);
            leaves.transform.localScale = new Vector3(3f, 4f, 3f);
            leaves.name = "Leaves";

            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        private void CreateRock(Vector3 position)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.transform.SetParent(m_natureRoot);
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
            rock.name = $"Rock_{Random.Range(1, 1000)}";
        }

        private void CreateGrassCluster(Vector3 position)
        {
            GameObject grass = new GameObject($"Grass_{Random.Range(1, 1000)}");
            grass.transform.SetParent(m_natureRoot);
            grass.transform.position = position;

            for (int i = 0; i < 5; i++)
            {
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blade.transform.SetParent(grass.transform);
                blade.transform.localPosition = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0.25f,
                    Random.Range(-0.5f, 0.5f)
                );
                blade.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                blade.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
            }
        }
    }
}
