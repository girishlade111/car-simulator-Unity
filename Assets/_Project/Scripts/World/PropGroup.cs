using UnityEngine;

public class PropGroup : MonoBehaviour
{
    [Header("Group Settings")]
    [SerializeField] private string m_groupName;
    [SerializeField] private bool m_isStatic = true;

    [Header("Individual Props")]
    [SerializeField] private PropData[] m_props;

    [System.Serializable]
    public class PropData
    {
        public GameObject prefab;
        public Vector3 localPosition;
        public Vector3 localRotation;
        public Vector3 localScale = Vector3.one;
    }

    private GameObject[] m_spawnedProps;

    private void Start()
    {
        SpawnGroup();
    }

    public void SpawnGroup()
    {
        if (m_props == null || m_props.Length == 0) return;

        m_spawnedProps = new GameObject[m_props.Length];

        for (int i = 0; i < m_props.Length; i++)
        {
            var propData = m_props[i];
            if (propData.prefab == null) continue;

            GameObject prop = Instantiate(propData.prefab, transform);
            prop.transform.localPosition = propData.localPosition;
            prop.transform.localEulerAngles = propData.localRotation;
            prop.transform.localScale = propData.localScale;

            if (m_isStatic)
            {
                var renderer = prop.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Static;
                }
            }

            m_spawnedProps[i] = prop;
        }
    }

    public void ClearGroup()
    {
        if (m_spawnedProps == null) return;

        for (int i = 0; i < m_spawnedProps.Length; i++)
        {
            if (m_spawnedProps[i] != null)
            {
                Destroy(m_spawnedProps[i]);
            }
        }

        m_spawnedProps = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

        if (m_props == null) return;

        for (int i = 0; i < m_props.Length; i++)
        {
            var prop = m_props[i];
            Vector3 worldPos = transform.TransformPoint(prop.localPosition);
            Gizmos.DrawCube(worldPos, prop.localScale * 0.5f);
        }
    }
}
