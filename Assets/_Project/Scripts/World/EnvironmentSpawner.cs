using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private bool m_spawnOnStart = true;
    [SerializeField] private Vector2 m_areaSize = new Vector2(200f, 200f);

    [Header("Environment Elements")]
    [SerializeField] private int m_treeCount = 30;
    [SerializeField] private int m_rockCount = 20;
    [SerializeField] private int m_grassPatches = 10;

    [Header("Placement")]
    [SerializeField] private bool m_useRaycast = true;
    [SerializeField] private LayerMask m_groundLayer = -1;
    [SerializeField] private bool m_avoidRoads = true;
    [SerializeField] private Collider[] m_roadColliders;

    [Header("Prefab References (Optional)")]
    [SerializeField] private GameObject m_treePrefab;
    [SerializeField] private GameObject m_rockPrefab;
    [SerializeField] private GameObject m_grassPrefab;

    private void Start()
    {
        if (m_spawnOnStart)
        {
            SpawnEnvironment();
        }
    }

    public void SpawnEnvironment()
    {
        ClearEnvironment();

        SpawnTrees();
        SpawnRocks();
        SpawnGrass();

        Debug.Log("[EnvironmentSpawner] Environment spawned");
    }

    private void SpawnTrees()
    {
        for (int i = 0; i < m_treeCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            if (pos.y < -100f) continue;

            GameObject tree = CreatePlaceholderTree(pos);
            tree.transform.SetParent(transform);
            tree.name = $"Tree_{i}";
        }
    }

    private void SpawnRocks()
    {
        for (int i = 0; i < m_rockCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            if (pos.y < -100f) continue;

            GameObject rock = CreatePlaceholderRock(pos);
            rock.transform.SetParent(transform);
            rock.name = $"Rock_{i}";
        }
    }

    private void SpawnGrass()
    {
        for (int i = 0; i < m_grassPatches; i++)
        {
            Vector3 pos = GetRandomPosition();
            if (pos.y < -100f) continue;

            GameObject grass = CreatePlaceholderGrass(pos);
            grass.transform.SetParent(transform);
            grass.name = $"Grass_{i}";
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-m_areaSize.x / 2f, m_areaSize.x / 2f);
        float z = Random.Range(-m_areaSize.y / 2f, m_areaSize.y / 2f);
        Vector3 pos = transform.position + new Vector3(x, 50f, z);

        if (m_useRaycast)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 200f, m_groundLayer))
            {
                pos = hit.point;
            }
            else
            {
                return new Vector3(x, -1000f, z);
            }
        }

        if (m_avoidRoads && m_roadColliders != null)
        {
            for (int i = 0; i < m_roadColliders.Length; i++)
            {
                if (m_roadColliders[i] != null)
                {
                    Bounds bounds = m_roadColliders[i].bounds;
                    bounds.Expand(5f);
                    if (bounds.Contains(pos))
                    {
                        return new Vector3(x, -1000f, z);
                    }
                }
            }
        }

        return pos;
    }

    public GameObject CreatePlaceholderTree(Vector3 position)
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.position = position;

        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
        trunk.transform.localScale = new Vector3(0.4f, 1.5f, 0.4f);
        trunk.name = "Trunk";

        GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.transform.SetParent(tree.transform);
        foliage.transform.localPosition = new Vector3(0, 4f, 0);
        foliage.transform.localScale = new Vector3(3f, 3f, 3f);
        foliage.name = "Foliage";

        return tree;
    }

    public GameObject CreatePlaceholderRock(Vector3 position)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rock.transform.position = position;
        rock.transform.localScale = new Vector3(Random.Range(1f, 3f), Random.Range(0.5f, 1.5f), Random.Range(1f, 3f));
        rock.transform.rotation = Quaternion.Euler(Random.Range(0, 30), Random.Range(0, 360), Random.Range(0, 30));
        rock.name = "Rock";

        return rock;
    }

    public GameObject CreatePlaceholderGrass(Vector3 position)
    {
        GameObject grass = new GameObject("GrassPatch");
        grass.transform.position = position;

        for (int i = 0; i < 5; i++)
        {
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Quad);
            blade.transform.SetParent(grass.transform);
            blade.transform.localPosition = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
            blade.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            blade.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        return grass;
    }

    public void ClearEnvironment()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(m_areaSize.x, 1f, m_areaSize.y));
    }
}
