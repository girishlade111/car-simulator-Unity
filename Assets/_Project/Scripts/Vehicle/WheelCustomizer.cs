using UnityEngine;

namespace CarSimulator.Vehicle
{
    [System.Serializable]
    public class WheelStyle
    {
        public string name;
        public Material wheelMaterial;
        public float rimSize = 0.35f;
        public float rimWidth = 0.25f;
        public int spokeCount = 5;
    }

    public class WheelCustomizer : MonoBehaviour
    {
        [Header("Wheel Styles")]
        [SerializeField] private WheelStyle[] m_wheelStyles;
        [SerializeField] private int m_currentStyleIndex;

        [Header("Current Settings")]
        [SerializeField] private Color m_rimColor = Color.gray;
        [SerializeField] private float m_rimSizeMultiplier = 1f;

        private WheelCollider[] m_wheelColliders;
        private Transform[] m_wheelMeshes;

        private void Start()
        {
            FindWheels();
            ApplyCurrentStyle();
        }

        private void FindWheels()
        {
            m_wheelColliders = GetComponentsInChildren<WheelCollider>();
            
            m_wheelMeshes = new Transform[4];
            Transform[] wheelParents = new Transform[4];
            
            for (int i = 0; i < m_wheelColliders.Length && i < 4; i++)
            {
                wheelParents[i] = m_wheelColliders[i].transform;
            }
        }

        public void SetWheelStyle(int styleIndex)
        {
            if (styleIndex < 0 || styleIndex >= m_wheelStyles.Length) return;
            
            m_currentStyleIndex = styleIndex;
            ApplyCurrentStyle();
        }

        public void SetRimColor(Color color)
        {
            m_rimColor = color;
            ApplyRimColor();
        }

        public void SetRimSize(float sizeMultiplier)
        {
            m_rimSizeMultiplier = Mathf.Clamp(sizeMultiplier, 0.8f, 1.2f);
            ApplyRimSize();
        }

        private void ApplyCurrentStyle()
        {
            if (m_wheelStyles == null || m_wheelStyles.Length == 0) return;
            
            ApplyRimColor();
            ApplyRimSize();
        }

        private void ApplyRimColor()
        {
            // Apply to wheel mesh materials
        }

        private void ApplyRimSize()
        {
            if (m_wheelColliders == null) return;

            float baseRadius = 0.35f;
            
            foreach (var collider in m_wheelColliders)
            {
                collider.radius = baseRadius * m_rimSizeMultiplier;
            }
        }

        public WheelStyle GetCurrentStyle()
        {
            if (m_wheelStyles == null || m_wheelStyles.Length == 0 || m_currentStyleIndex >= m_wheelStyles.Length)
                return null;
            
            return m_wheelStyles[m_currentStyleIndex];
        }

        public int GetStyleCount()
        {
            return m_wheelStyles != null ? m_wheelStyles.Length : 0;
        }
    }
}
