using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private int m_initialSize = 10;
    [SerializeField] private int m_maxSize = 50;
    [SerializeField] private bool m_autoExpand = true;

    private Queue<GameObject> m_availableObjects;
    private List<GameObject> m_activeObjects;

    public int ActiveCount => m_activeObjects.Count;
    public int AvailableCount => m_availableObjects.Count;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        m_availableObjects = new Queue<GameObject>();
        m_activeObjects = new List<GameObject>();

        InitializePool();
    }

    private void InitializePool()
    {
        if (m_prefab == null)
        {
            Debug.LogWarning("[ObjectPool] No prefab assigned!");
            return;
        }

        for (int i = 0; i < m_initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        if (m_availableObjects.Count + m_activeObjects.Count >= m_maxSize)
        {
            return null;
        }

        GameObject obj = Instantiate(m_prefab, transform);
        obj.SetActive(false);
        m_availableObjects.Enqueue(obj);
        return obj;
    }

    public GameObject Get()
    {
        return Get(Vector3.zero, Quaternion.identity);
    }

    public GameObject Get(Vector3 position)
    {
        return Get(position, Quaternion.identity);
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;

        if (m_availableObjects.Count > 0)
        {
            obj = m_availableObjects.Dequeue();
        }
        else if (m_autoExpand)
        {
            obj = CreateNewObject();
        }

        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            m_activeObjects.Add(obj);
        }

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        m_activeObjects.Remove(obj);
        m_availableObjects.Enqueue(obj);
    }

    public void ReturnAll()
    {
        for (int i = m_activeObjects.Count - 1; i >= 0; i--)
        {
            Return(m_activeObjects[i]);
        }
    }

    public void Clear()
    {
        ReturnAll();

        while (m_availableObjects.Count > 0)
        {
            GameObject obj = m_availableObjects.Dequeue();
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pools")]
    [SerializeField] private PoolData[] m_pools;

    private Dictionary<string, ObjectPool> m_poolDict;

    [System.Serializable]
    public class PoolData
    {
        public string poolName;
        public GameObject prefab;
        public int initialSize = 10;
        public int maxSize = 50;
        public bool autoExpand = true;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        m_poolDict = new Dictionary<string, ObjectPool>();

        for (int i = 0; i < m_pools.Length; i++)
        {
            var poolData = m_pools[i];
            if (poolData.prefab == null) continue;

            GameObject poolObj = new GameObject($"Pool_{poolData.poolName}");
            poolObj.transform.SetParent(transform);

            ObjectPool pool = poolObj.AddComponent<ObjectPool>();
            var field = typeof(ObjectPool).GetField("m_prefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(pool, poolData.prefab);

            m_poolDict[poolData.poolName] = pool;
        }
    }

    public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
    {
        if (m_poolDict.ContainsKey(poolName))
        {
            return m_poolDict[poolName].Get(position, rotation);
        }

        Debug.LogWarning($"[ObjectPoolManager] Pool not found: {poolName}");
        return null;
    }

    public void Despawn(string poolName, GameObject obj)
    {
        if (m_poolDict.ContainsKey(poolName))
        {
            m_poolDict[poolName].Return(obj);
        }
    }

    public void DespawnAll(string poolName)
    {
        if (m_poolDict.ContainsKey(poolName))
        {
            m_poolDict[poolName].ReturnAll();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
