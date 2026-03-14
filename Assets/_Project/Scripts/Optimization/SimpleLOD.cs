using UnityEngine;

public class SimpleLOD : MonoBehaviour
{
    [Header("LOD Settings")]
    [SerializeField] private LODLevel[] m_lodLevels;
    [SerializeField] private float[] m_lodDistances = new float[] { 50f, 100f, 200f };

    [System.Serializable]
    public class LODLevel
    {
        public GameObject highDetail;
        public GameObject mediumDetail;
        public GameObject lowDetail;
    }

    private Camera m_mainCamera;
    private int m_currentLOD;

    private void Start()
    {
        m_mainCamera = Camera.main;
        UpdateLOD();
    }

    private void Update()
    {
        UpdateLOD();
    }

    private void UpdateLOD()
    {
        if (m_mainCamera == null || m_lodLevels == null || m_lodLevels.Length == 0) return;

        float distance = Vector3.Distance(transform.position, m_mainCamera.transform.position);
        int newLOD = GetLODLevel(distance);

        if (newLOD != m_currentLOD)
        {
            SetLOD(newLOD);
        }
    }

    private int GetLODLevel(float distance)
    {
        for (int i = 0; i < m_lodDistances.Length; i++)
        {
            if (distance < m_lodDistances[i])
            {
                return i;
            }
        }
        return m_lodDistances.Length;
    }

    private void SetLOD(int lod)
    {
        m_currentLOD = lod;

        for (int i = 0; i < m_lodLevels.Length; i++)
        {
            var level = m_lodLevels[i];
            if (level.highDetail != null) level.highDetail.SetActive(i == 0 && lod == 0);
            if (level.mediumDetail != null) level.mediumDetail.SetActive(i == 0 && lod == 1);
            if (level.lowDetail != null) level.lowDetail.SetActive(i == 0 && lod >= 2);
        }
    }

    private void OnEnable()
    {
        UpdateLOD();
    }

    private void OnDisable()
    {
        for (int i = 0; i < m_lodLevels.Length; i++)
        {
            var level = m_lodLevels[i];
            if (level.highDetail != null) level.highDetail.SetActive(false);
            if (level.mediumDetail != null) level.mediumDetail.SetActive(false);
            if (level.lowDetail != null) level.lowDetail.SetActive(false);
        }
    }
}
