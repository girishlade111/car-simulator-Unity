using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.World
{
    public class PropSpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_spawnOnStart = true;
        [SerializeField] private bool m_usePool = true;

        [Header("Prop Prefabs")]
        [SerializeField] private GameObject[] m_streetProps;
        [SerializeField] private GameObject[] m_natureProps;
        [SerializeField] private GameObject[] m_buildingProps;
        [SerializeField] private GameObject[] m_vehicleProps;
        [SerializeField] private GameObject[] m_decorativeProps;

        private List<PropSpawnAnchor> m_anchors = new List<PropSpawnAnchor>();
        private List<GameObject> m_spawnedProps = new List<GameObject>();

        private void Start()
        {
            if (m_spawnOnStart)
            {
                SpawnAllProps();
            }
        }

        public void RegisterAnchor(PropSpawnAnchor anchor)
        {
            if (!m_anchors.Contains(anchor))
            {
                m_anchors.Add(anchor);
            }
        }

        public void UnregisterAnchor(PropSpawnAnchor anchor)
        {
            m_anchors.Remove(anchor);
        }

        public void SpawnAllProps()
        {
            ClearSpawnedProps();

            foreach (var anchor in m_anchors)
            {
                if (anchor.IsActive)
                {
                    SpawnProp(anchor);
                }
            }
        }

        public GameObject SpawnProp(PropSpawnAnchor anchor)
        {
            GameObject[] props = GetPropsForCategory(anchor.Category);
            if (props == null || props.Length == 0) return null;

            GameObject prefab = props[Random.Range(0, props.Length)];
            if (prefab == null) return null;

            Vector3 position = anchor.GetSpawnPosition();
            Quaternion rotation = anchor.GetSpawnRotation();
            Vector3 scale = anchor.GetSpawnScale();

            GameObject prop = Instantiate(prefab, position, rotation);
            prop.transform.localScale = scale;
            prop.transform.SetParent(transform);

            m_spawnedProps.Add(prop);
            return prop;
        }

        public void ClearSpawnedProps()
        {
            foreach (var prop in m_spawnedProps)
            {
                if (prop != null)
                {
                    Destroy(prop);
                }
            }
            m_spawnedProps.Clear();
        }

        private GameObject[] GetPropsForCategory(PropCategory category)
        {
            switch (category)
            {
                case PropCategory.Street:
                    return m_streetProps;
                case PropCategory.Nature:
                    return m_natureProps;
                case PropCategory.Building:
                    return m_buildingProps;
                case PropCategory.Vehicle:
                    return m_vehicleProps;
                case PropCategory.Decorative:
                    return m_decorativeProps;
                default:
                    return null;
            }
        }

        public void FindAnchorsInScene()
        {
            var anchors = FindObjectsOfType<PropSpawnAnchor>();
            m_anchors.AddRange(anchors);
        }
    }
}
