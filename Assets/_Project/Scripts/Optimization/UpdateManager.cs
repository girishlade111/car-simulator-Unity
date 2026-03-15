using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Optimization
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private int m_defaultPoolSize = 10;
        [SerializeField] private bool m_allowGrowth = true;

        private Dictionary<string, Pool> m_pools = new Dictionary<string, Pool>();

        private class Pool
        {
            public GameObject prefab;
            public Queue<GameObject> available = new Queue<GameObject>();
            public List<GameObject> active = new List<GameObject>();
            public int maxSize;
            public bool allowGrowth;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RegisterPool(string poolId, GameObject prefab, int size = 10, bool allowGrowth = true)
        {
            if (m_pools.ContainsKey(poolId))
            {
                Debug.LogWarning($"[ObjectPooler] Pool {poolId} already exists");
                return;
            }

            Pool pool = new Pool
            {
                prefab = prefab,
                maxSize = size,
                allowGrowth = allowGrowth
            };

            for (int i = 0; i < size; i++)
            {
                GameObject obj = CreatePooledObject(pool);
                obj.SetActive(false);
                pool.available.Enqueue(obj);
            }

            m_pools[poolId] = pool;
        }

        private GameObject CreatePooledObject(Pool pool)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.transform.SetParent(transform);
            PooledObject pooled = obj.AddComponent<PooledObject>();
            pooled.PoolId = pool.prefab.name;
            return obj;
        }

        public GameObject Get(string poolId, Vector3 position, Quaternion rotation)
        {
            if (!m_pools.TryGetValue(poolId, out Pool pool))
            {
                Debug.LogWarning($"[ObjectPooler] Pool not found: {poolId}");
                return null;
            }

            GameObject obj;

            if (pool.available.Count > 0)
            {
                obj = pool.available.Dequeue();
            }
            else if (pool.allowGrowth && pool.active.Count < pool.maxSize)
            {
                obj = CreatePooledObject(pool);
            }
            else
            {
                return null;
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            pool.active.Add(obj);

            return obj;
        }

        public GameObject Get(string poolId)
        {
            return Get(poolId, Vector3.zero, Quaternion.identity);
        }

        public void Return(string poolId, GameObject obj)
        {
            if (!m_pools.TryGetValue(poolId, out Pool pool))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.active.Remove(obj);
            pool.available.Enqueue(obj);
        }

        public void ReturnAll(string poolId)
        {
            if (!m_pools.TryGetValue(poolId, out Pool pool)) return;

            for (int i = pool.active.Count - 1; i >= 0; i--)
            {
                GameObject obj = pool.active[i];
                obj.SetActive(false);
                pool.available.Enqueue(obj);
            }
            pool.active.Clear();
        }

        public void Clear(string poolId)
        {
            if (!m_pools.TryGetValue(poolId, out Pool pool)) return;

            foreach (var obj in pool.active)
            {
                if (obj != null) Destroy(obj);
            }
            foreach (var obj in pool.available)
            {
                if (obj != null) Destroy(obj);
            }

            pool.active.Clear();
            pool.available.Clear();
        }

        public int GetActiveCount(string poolId)
        {
            if (m_pools.TryGetValue(poolId, out Pool pool))
            {
                return pool.active.Count;
            }
            return 0;
        }

        public int GetAvailableCount(string poolId)
        {
            if (m_pools.TryGetValue(poolId, out Pool pool))
            {
                return pool.available.Count;
            }
            return 0;
        }
    }

    public class PooledObject : MonoBehaviour
    {
        public string PoolId { get; set; }

        public void ReturnToPool()
        {
            if (!string.IsNullOrEmpty(PoolId))
            {
                ObjectPooler.Instance?.Return(PoolId, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class UpdateManager : MonoBehaviour
    {
        public static UpdateManager Instance { get; private set; }

        private List<IUpdateable> m_updateables = new List<IUpdateable>();
        private List<IFixedUpdateable> m_fixedUpdateables = new List<IFixedUpdateable>();
        private List<ILateUpdateable> m_lateUpdateables = new List<ILateUpdateable>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Register(IUpdateable updateable)
        {
            if (!m_updateables.Contains(updateable))
            {
                m_updateables.Add(updateable);
            }
        }

        public void Unregister(IUpdateable updateable)
        {
            m_updateables.Remove(updateable);
        }

        public void RegisterFixed(IFixedUpdateable updateable)
        {
            if (!m_fixedUpdateables.Contains(updateable))
            {
                m_fixedUpdateables.Add(updateable);
            }
        }

        public void UnregisterFixed(IFixedUpdateable updateable)
        {
            m_fixedUpdateables.Remove(updateable);
        }

        public void RegisterLate(ILateUpdateable updateable)
        {
            if (!m_lateUpdateables.Contains(updateable))
            {
                m_lateUpdateables.Add(updateable);
            }
        }

        public void UnregisterLate(ILateUpdateable updateable)
        {
            m_lateUpdateables.Remove(updateable);
        }

        private void Update()
        {
            for (int i = m_updateables.Count - 1; i >= 0; i--)
            {
                if (m_updateables[i] != null)
                {
                    m_updateables[i].OnUpdate();
                }
            }
        }

        private void FixedUpdate()
        {
            for (int i = m_fixedUpdateables.Count - 1; i >= 0; i--)
            {
                if (m_fixedUpdateables[i] != null)
                {
                    m_fixedUpdateables[i].OnFixedUpdate();
                }
            }
        }

        private void LateUpdate()
        {
            for (int i = m_lateUpdateables.Count - 1; i >= 0; i--)
            {
                if (m_lateUpdateables[i] != null)
                {
                    m_lateUpdateables[i].OnLateUpdate();
                }
            }
        }

        public int GetUpdateableCount() => m_updateables.Count;
    }

    public interface IUpdateable
    {
        void OnUpdate();
    }

    public interface IFixedUpdateable
    {
        void OnFixedUpdate();
    }

    public interface ILateUpdateable
    {
        void OnLateUpdate();
    }
}
