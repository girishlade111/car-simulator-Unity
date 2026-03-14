using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private GameObject[] m_buildingPrefabs;
    [SerializeField] private Vector2 m_gridSize = new Vector2(50f, 50f);
    [SerializeField] private int m_gridCellsX = 4;
    [SerializeField] private int m_gridCellsZ = 4;
    [SerializeField] private bool m_randomRotation = true;
    [SerializeField] private bool m_avoidOverlaps = true;

    [Header("Building Spacing")]
    [SerializeField] private Vector2 m_buildingSize = new Vector2(15f, 15f);
    [SerializeField] private float m_streetWidth = 10f;

    public void PlaceBuildings()
    {
        ClearBuildings();

        float cellWidth = (m_gridSize.x - (m_streetWidth * (m_gridCellsX + 1))) / m_gridCellsX;
        float cellLength = (m_gridSize.y - (m_streetWidth * (m_gridCellsZ + 1))) / m_gridCellsZ;

        for (int x = 0; x < m_gridCellsX; x++)
        {
            for (int z = 0; z < m_gridCellsZ; z++)
            {
                if (m_buildingPrefabs == null || m_buildingPrefabs.Length == 0) continue;

                GameObject prefab = m_buildingPrefabs[Random.Range(0, m_buildingPrefabs.Length)];
                if (prefab == null) continue;

                float posX = m_streetWidth + (x * (cellWidth + m_streetWidth)) + (cellWidth / 2f) - (m_gridSize.x / 2f);
                float posZ = m_streetWidth + (z * (cellLength + m_streetWidth)) + (cellLength / 2f) - (m_gridSize.y / 2f);

                Vector3 position = new Vector3(posX, 0, posZ);
                position.y = GetGroundY(position);

                Quaternion rotation = Quaternion.identity;
                if (m_randomRotation)
                {
                    rotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);
                }

                GameObject building = Instantiate(prefab, position, rotation, transform);
                building.name = $"Building_{x}_{z}";
            }
        }

        Debug.Log($"[BuildingPlacer] Placed {m_gridCellsX * m_gridCellsZ} buildings");
    }

    private float GetGroundY(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hit, 100f))
        {
            return hit.point.y;
        }
        return 0f;
    }

    public void ClearBuildings()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(m_gridSize.x, 1f, m_gridSize.y));
    }
}
