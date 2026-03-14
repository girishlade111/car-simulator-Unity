using UnityEngine;

public class DistrictManager : MonoBehaviour
{
    public static DistrictManager Instance { get; private set; }

    [Header("District Settings")]
    [SerializeField] private Vector2 m_districtSize = new Vector2(500f, 500f);
    [SerializeField] private int m_chunkGridSize = 100;

    [Header("References")]
    [SerializeField] private Transform m_districtRoot;

    private Vector2[] m_chunkOffsets;
    private int m_chunksX;
    private int m_chunksZ;

    public Vector2 DistrictSize => m_districtSize;
    public Vector2 DistrictCenter => new Vector2(transform.position.x, transform.position.z);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (m_districtRoot == null)
        {
            m_districtRoot = transform;
        }

        CalculateChunks();
    }

    private void CalculateChunks()
    {
        m_chunksX = Mathf.CeilToInt(m_districtSize.x / m_chunkGridSize);
        m_chunksZ = Mathf.CeilToInt(m_districtSize.y / m_chunkGridSize);

        m_chunkOffsets = new Vector2[m_chunksX * m_chunksZ];

        for (int x = 0; x < m_chunksX; x++)
        {
            for (int z = 0; z < m_chunksZ; z++)
            {
                int index = x * m_chunksZ + z;
                float offsetX = (x * m_chunkGridSize) - (m_districtSize.x / 2f);
                float offsetZ = (z * m_chunkGridSize) - (m_districtSize.y / 2f);
                m_chunkOffsets[index] = new Vector2(offsetX, offsetZ);
            }
        }
    }

    public Vector2 GetRandomPositionInDistrict()
    {
        float x = Random.Range(-m_districtSize.x / 2f, m_districtSize.x / 2f);
        float z = Random.Range(-m_districtSize.y / 2f, m_districtSize.y / 2f);
        return new Vector2(x, z);
    }

    public Vector2 GetRandomPositionInChunk(int chunkIndex)
    {
        if (chunkIndex < 0 || chunkIndex >= m_chunkOffsets.Length)
        {
            return GetRandomPositionInDistrict();
        }

        Vector2 offset = m_chunkOffsets[chunkIndex];
        float x = offset.x + Random.Range(0, m_chunkGridSize);
        float z = offset.y + Random.Range(0, m_chunkGridSize);
        return new Vector2(x, z);
    }

    public int GetChunkCount()
    {
        return m_chunkOffsets.Length;
    }

    public bool IsPositionInDistrict(Vector2 position)
    {
        float halfX = m_districtSize.x / 2f;
        float halfZ = m_districtSize.y / 2f;

        return position.x >= -halfX && position.x <= halfX &&
               position.y >= -halfZ && position.y <= halfZ;
    }

    public int GetChunkIndex(Vector2 position)
    {
        float halfX = m_districtSize.x / 2f;
        float halfZ = m_districtSize.y / 2f;

        int x = Mathf.FloorToInt((position.x + halfX) / m_chunkGridSize);
        int z = Mathf.FloorToInt((position.y + halfZ) / m_chunkGridSize);

        x = Mathf.Clamp(x, 0, m_chunksX - 1);
        z = Mathf.Clamp(z, 0, m_chunksZ - 1);

        return x * m_chunksZ + z;
    }

    public void DrawDebugGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_districtSize.x, 10f, m_districtSize.y));

        if (m_chunkOffsets != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < m_chunkOffsets.Length; i++)
            {
                Vector2 offset = m_chunkOffsets[i];
                Vector3 center = transform.position + new Vector3(offset.x + m_chunkGridSize / 2f, 0, offset.y + m_chunkGridSize / 2f);
                Gizmos.DrawWireCube(center, new Vector3(m_chunkGridSize, 1f, m_chunkGridSize));
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawDebugGizmos();
    }
}
