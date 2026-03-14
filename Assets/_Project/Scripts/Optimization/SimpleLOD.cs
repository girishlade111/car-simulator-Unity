using UnityEngine;

namespace CarSimulator.Optimization
{
    public class SimpleLOD : MonoBehaviour
    {
        [Header("LOD Levels")]
        [SerializeField] private LODLevel[] m_levels;

        [Header("Distances")]
        [SerializeField] private float[] m_distances = new float[] { 50f, 100f, 200f };

        private Camera m_mainCamera;
        private int m_currentLOD;

        [System.Serializable]
        public class LODLevel
        {
            public GameObject highDetail;
            public GameObject mediumDetail;
            public GameObject lowDetail;
        }

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
            if (m_mainCamera == null || m_levels == null || m_levels.Length == 0) return;

            float distance = Vector3.Distance(transform.position, m_mainCamera.transform.position);
            int newLOD = GetLODLevel(distance);

            if (newLOD != m_currentLOD)
            {
                SetLOD(newLOD);
            }
        }

        private int GetLODLevel(float distance)
        {
            for (int i = 0; i < m_distances.Length; i++)
            {
                if (distance < m_distances[i])
                    return i;
            }
            return m_distances.Length;
        }

        private void SetLOD(int lod)
        {
            m_currentLOD = lod;

            for (int i = 0; i < m_levels.Length; i++)
            {
                var level = m_levels[i];
                if (level.highDetail) level.highDetail.SetActive(false);
                if (level.mediumDetail) level.mediumDetail.SetActive(false);
                if (level.lowDetail) level.lowDetail.SetActive(false);
            }

            if (m_currentLOD < m_levels.Length)
            {
                var activeLevel = m_levels[m_currentLOD];
                if (activeLevel.highDetail) activeLevel.highDetail.SetActive(true);
                if (activeLevel.mediumDetail) activeLevel.mediumDetail.SetActive(true);
                if (activeLevel.lowDetail) activeLevel.lowDetail.SetActive(true);
            }
        }
    }
}
