using UnityEngine;

public class PropScatter : MonoBehaviour
{
    [Header("Scatter Settings")]
    [SerializeField] private string m_categoryName = "Trees";
    [SerializeField] private int m_count = 20;
    [SerializeField] private Vector2 m_scatterArea = new Vector2(100f, 100f);
    [SerializeField] private Vector2 m_minMaxDistance = new Vector2(5f, 20f);

    [Header("Placement")]
    [SerializeField] private bool m_useRaycast = true;
    [SerializeField] private LayerMask m_groundLayer = -1;
    [SerializeField] private float m_yOffset;

    [Header("Variation")]
    [SerializeField] private bool m_randomRotation = true;
    [SerializeField] private float m_randomScaleMin = 0.8f;
    [SerializeField] private float m_randomScaleMax = 1.2f;

    [Header("Exclusions")]
    [SerializeField] private bool m_avoidRoads = true;
    [SerializeField] private Collider[] m_roadColliders;

    private GameObject[] m_spawnedProps;

    public string CategoryName => m_categoryName;
    public int Count => m_count;

    private void Start()
    {
        Scatter();
    }

    public void Scatter()
    {
        if (PropManager.Instance == null)
        {
            Debug.LogWarning("[PropScatter] PropManager not found!");
            return;
        }

        Clear();

        m_spawnedProps = new GameObject[m_count];

        int attempts = 0;
        int maxAttempts = m_count * 10;
        int spawned = 0;

        while (spawned < m_count && attempts < maxAttempts)
        {
            attempts++;

            Vector3 position = GetRandomPosition();

            if (m_useRaycast)
            {
                position = GetGroundPosition(position);
                if (position.y < -100f)
                {
                    continue;
                }
            }

            if (m_avoidRoads && m_roadColliders != null)
            {
                if (IsNearRoad(position))
                {
                    continue;
                }
            }

            position.y += m_yOffset;

            Quaternion rotation = Quaternion.identity;
            if (m_randomRotation)
            {
                rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }

            GameObject prop = PropManager.Instance.SpawnProp(m_categoryName, position, rotation);

            if (prop != null)
            {
                if (m_randomScaleMin != 1f || m_randomScaleMax != 1f)
                {
                    float scale = Random.Range(m_randomScaleMin, m_randomScaleMax);
                    prop.transform.localScale = Vector3.one * scale;
                }

                prop.transform.SetParent(transform);
                m_spawnedProps[spawned] = prop;
                spawned++;
            }
        }

        Debug.Log($"[PropScatter] Spawned {spawned} {m_categoryName} props");
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-m_scatterArea.x / 2f, m_scatterArea.x / 2f);
        float z = Random.Range(-m_scatterArea.y / 2f, m_scatterArea.y / 2f);

        Vector3 basePosition = transform.position;
        return basePosition + new Vector3(x, 50f, z);
    }

    private Vector3 GetGroundPosition(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 200f, m_groundLayer))
        {
            return hit.point;
        }

        return new Vector3(position.x, -1000f, position.z);
    }

    private bool IsNearRoad(Vector3 position)
    {
        if (m_roadColliders == null) return false;

        for (int i = 0; i < m_roadColliders.Length; i++)
        {
            if (m_roadColliders[i] == null) continue;

            Bounds bounds = m_roadColliders[i].bounds;
            bounds.Expand(5f);

            if (bounds.Contains(position))
            {
                return true;
            }
        }

        return false;
    }

    public void Clear()
    {
        if (m_spawnedProps == null) return;

        for (int i = 0; i < m_spawnedProps.Length; i++)
        {
            if (m_spawnedProps[i] != null)
            {
                PropManager.Instance.DespawnProp(m_spawnedProps[i]);
            }
        }

        m_spawnedProps = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(m_scatterArea.x, 1f, m_scatterArea.y));
    }
}
