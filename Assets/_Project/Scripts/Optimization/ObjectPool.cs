using System.Collections.Generic;
using UnityEngine;

namespace CarSimulator.Optimization
{
    public class ObjectPool : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject m_prefab;
        [SerializeField] private int m_initialSize = 10;
        [SerializeField] private int m_maxSize = 50;
        [SerializeField] private bool m_autoExpand = true;

        private Queue<GameObject> m_available = new Queue<GameObject>();
        private List<GameObject> m_active = new List<GameObject>();

        public int ActiveCount => m_active.Count;
        public int AvailableCount => m_available.Count;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (m_prefab == null) return;

            for (int i = 0; i < m_initialSize; i++)
            {
                CreateObject();
            }
        }

        private GameObject CreateObject()
        {
            GameObject obj = Instantiate(m_prefab, transform);
            obj.SetActive(false);
            m_available.Enqueue(obj);
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

            if (m_available.Count > 0)
            {
                obj = m_available.Dequeue();
            }
            else if (m_autoExpand && m_active.Count + m_available.Count < m_maxSize)
            {
                obj = CreateObject();
            }

            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                m_active.Add(obj);
            }

            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            m_active.Remove(obj);
            m_available.Enqueue(obj);
        }

        public void ReturnAll()
        {
            for (int i = m_active.Count - 1; i >= 0; i--)
            {
                Return(m_active[i]);
            }
        }
    }

    public class PoolManager : MonoBehaviour
    {
        private static PoolManager s_instance;
        public static PoolManager Instance => s_instance;

        private Dictionary<string, ObjectPool> m_pools = new Dictionary<string, ObjectPool>();

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterPool(string name, ObjectPool pool)
        {
            m_pools[name] = pool;
        }

        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
        {
            if (m_pools.TryGetValue(poolName, out ObjectPool pool))
            {
                return pool.Get(position, rotation);
            }
            Debug.LogWarning($"[PoolManager] Pool not found: {poolName}");
            return null;
        }

        public void Despawn(string poolName, GameObject obj)
        {
            if (m_pools.TryGetValue(poolName, out ObjectPool pool))
            {
                pool.Return(obj);
            }
        }
    }
}
