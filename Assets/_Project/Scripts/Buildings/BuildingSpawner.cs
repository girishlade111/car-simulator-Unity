using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public static BuildingSpawner Instance { get; private set; }

    [Header("Building Types")]
    [SerializeField] private BuildingData[] m_buildingTypes;

    [Header("Spawn Settings")]
    [SerializeField] private int m_maxBuildings = 20;
    [SerializeField] private Vector2 m_spawnArea = new Vector2(200f, 200f);
    [SerializeField] private float m_minDistanceBetween = 20f;
    [SerializeField] private bool m_useRaycast = true;
    [SerializeField] private LayerMask m_groundLayer = -1;

    [Header("References")]
    [SerializeField] private Transform m_buildingRoot;

    private List<BuildingSpawnData> m_spawnedBuildings;

    [System.Serializable]
    public class BuildingSpawnData
    {
        public GameObject building;
        public BuildingData data;
        public Vector3 position;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (m_buildingRoot == null)
        {
            m_buildingRoot = transform;
        }

        m_spawnedBuildings = new List<BuildingSpawnData>();
    }

    private void Start()
    {
        SpawnBuildings();
    }

    public void SpawnBuildings()
    {
        ClearBuildings();

        if (m_buildingTypes == null || m_buildingTypes.Length == 0)
        {
            Debug.LogWarning("[BuildingSpawner] No building types assigned!");
            return;
        }

        int attempts = 0;
        int maxAttempts = m_maxBuildings * 10;

        while (m_spawnedBuildings.Count < m_maxBuildings && attempts < maxAttempts)
        {
            attempts++;

            Vector3 position = GetRandomPosition();

            if (m_useRaycast)
            {
                position = GetGroundPosition(position);
                if (position.y < -100f) continue;
            }

            if (!IsValidPosition(position)) continue;

            BuildingData buildingType = GetRandomBuildingType();
            if (buildingType == null || buildingType.prefab == null) continue;

            GameObject building = Instantiate(buildingType.prefab, position, Quaternion.identity);
            building.transform.SetParent(m_buildingRoot);
            building.transform.rotation = GetBuildingRotation(position);

            BuildingSpawnData spawnData = new BuildingSpawnData
            {
                building = building,
                data = buildingType,
                position = position
            };

            m_spawnedBuildings.Add(spawnData);
        }

        Debug.Log($"[BuildingSpawner] Spawned {m_spawnedBuildings.Count} buildings");
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-m_spawnArea.x / 2f, m_spawnArea.x / 2f);
        float z = Random.Range(-m_spawnArea.y / 2f, m_spawnArea.y / 2f);

        return transform.position + new Vector3(x, 50f, z);
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

    private bool IsValidPosition(Vector3 position)
    {
        for (int i = 0; i < m_spawnedBuildings.Count; i++)
        {
            float distance = Vector3.Distance(position, m_spawnedBuildings[i].position);
            if (distance < m_minDistanceBetween)
            {
                return false;
            }
        }

        return true;
    }

    private BuildingData GetRandomBuildingType()
    {
        return m_buildingTypes[Random.Range(0, m_buildingTypes.Length)];
    }

    private Quaternion GetBuildingRotation(Vector3 position)
    {
        float[] rotations = { 0f, 90f, 180f, 270f };
        return Quaternion.Euler(0f, rotations[Random.Range(0, rotations.Length)], 0f);
    }

    public void ClearBuildings()
    {
        for (int i = 0; i < m_spawnedBuildings.Count; i++)
        {
            if (m_spawnedBuildings[i].building != null)
            {
                Destroy(m_spawnedBuildings[i].building);
            }
        }

        m_spawnedBuildings.Clear();
    }

    public List<BuildingSpawnData> GetSpawnedBuildings()
    {
        return m_spawnedBuildings;
    }

    public BuildingData[] GetBuildingTypes()
    {
        return m_buildingTypes;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(m_spawnArea.x, 1f, m_spawnArea.y));
    }
}
