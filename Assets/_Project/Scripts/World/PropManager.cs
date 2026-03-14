using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    public static PropManager Instance { get; private set; }

    [Header("Prop Categories")]
    [SerializeField] private PropCategory[] m_categories;

    private Dictionary<string, List<GameObject>> m_propPools;
    private Dictionary<string, GameObject> m_propPrefabs;

    [System.Serializable]
    public class PropCategory
    {
        public string name;
        public GameObject[] prefabs;
        public int initialPoolSize = 10;
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
        m_propPools = new Dictionary<string, List<GameObject>>();
        m_propPrefabs = new Dictionary<string, GameObject>();

        if (m_categories == null) return;

        for (int i = 0; i < m_categories.Length; i++)
        {
            var category = m_categories[i];
            if (category.prefabs == null || category.prefabs.Length == 0) continue;

            m_propPools[category.name] = new List<GameObject>();

            for (int j = 0; j < category.prefabs.Length; j++)
            {
                var prefab = category.prefabs[j];
                if (prefab != null)
                {
                    m_propPrefabs[$"{category.name}_{j}"] = prefab;
                }
            }
        }
    }

    public GameObject SpawnProp(string categoryName, Vector3 position, Quaternion rotation)
    {
        if (!m_propPools.ContainsKey(categoryName))
        {
            Debug.LogWarning($"[PropManager] Category not found: {categoryName}");
            return null;
        }

        List<GameObject> pool = m_propPools[categoryName];
        GameObject prop = null;

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeSelf)
            {
                prop = pool[i];
                break;
            }
        }

        if (prop == null)
        {
            int prefabIndex = Random.Range(0, m_categories.Length);
            var category = m_categories[prefabIndex];
            if (category.prefabs.Length > 0 && category.prefabs[0] != null)
            {
                prop = Instantiate(category.prefabs[0], position, rotation);
                pool.Add(prop);
            }
        }

        if (prop != null)
        {
            prop.transform.position = position;
            prop.transform.rotation = rotation;
            prop.SetActive(true);
        }

        return prop;
    }

    public GameObject SpawnProp(string categoryName, Vector3 position)
    {
        return SpawnProp(categoryName, position, Quaternion.identity);
    }

    public void DespawnProp(GameObject prop)
    {
        if (prop != null)
        {
            prop.SetActive(false);
        }
    }

    public void DespawnAllProps()
    {
        foreach (var pool in m_propPools.Values)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] != null)
                {
                    pool[i].SetActive(false);
                }
            }
        }
    }

    public string[] GetCategoryNames()
    {
        if (m_categories == null) return new string[0];

        string[] names = new string[m_categories.Length];
        for (int i = 0; i < m_categories.Length; i++)
        {
            names[i] = m_categories[i].name;
        }
        return names;
    }

    public PropCategory GetCategory(string name)
    {
        if (m_categories == null) return null;

        for (int i = 0; i < m_categories.Length; i++)
        {
            if (m_categories[i].name == name)
            {
                return m_categories[i];
            }
        }
        return null;
    }
}
