using UnityEngine;

namespace CarSimulator.Buildings
{
    public class BuildingLODSetup : MonoBehaviour
    {
        [Header("LOD Settings")]
        [SerializeField] private float m_lodDistance1 = 50f;
        [SerializeField] private float m_lodDistance2 = 100f;
        [SerializeField] private float m_lodDistance3 = 150f;

        [Header("LOD Models")]
        [SerializeField] private GameObject m_lod0Model;
        [SerializeField] private GameObject m_lod1Model;
        [SerializeField] private GameObject m_lod2Model;

        [Header("Materials")]
        [SerializeField] private Material[] m_lodMaterials;

        [Header("Auto Setup")]
        [SerializeField] private bool m_setupOnStart;

        private LODGroup m_lodGroup;

        private void Start()
        {
            if (m_setupOnStart)
            {
                SetupLOD();
            }
        }

        public void SetupLOD()
        {
            m_lodGroup = GetComponent<LODGroup>();
            if (m_lodGroup == null)
            {
                m_lodGroup = gameObject.AddComponent<LODGroup>();
            }

            Renderer[] lod0Renderers = GetLODRenderers(0);
            Renderer[] lod1Renderers = GetLODRenderers(1);
            Renderer[] lod2Renderers = GetLODRenderers(2);

            LOD[] lods = new LOD[3];

            lods[0] = new LOD(1f / m_lodDistance1, lod0Renderers);
            lods[1] = new LOD(1f / m_lodDistance2, lod1Renderers);
            lods[2] = new LOD(1f / m_lodDistance3, lod2Renderers);

            m_lodGroup.SetLODs(lods);
            m_lodGroup.RecalculateBounds();

            Debug.Log($"[BuildingLODSetup] LOD setup complete for {gameObject.name}");
        }

        private Renderer[] GetLODRenderers(int lodLevel)
        {
            GameObject model = null;

            switch (lodLevel)
            {
                case 0:
                    model = m_lod0Model;
                    break;
                case 1:
                    model = m_lod1Model;
                    break;
                case 2:
                    model = m_lod2Model;
                    break;
            }

            if (model != null)
            {
                return model.GetComponentsInChildren<Renderer>();
            }

            return GetComponentsInChildren<Renderer>();
        }

        public void SetLODDistances(float dist1, float dist2, float dist3)
        {
            m_lodDistance1 = dist1;
            m_lodDistance2 = dist2;
            m_lodDistance3 = dist3;

            if (m_lodGroup != null)
            {
                SetupLOD();
            }
        }

        public void SetLODModel(int level, GameObject model)
        {
            switch (level)
            {
                case 0:
                    m_lod0Model = model;
                    break;
                case 1:
                    m_lod1Model = model;
                    break;
                case 2:
                    m_lod2Model = model;
                    break;
            }
        }

        public void SetMaterials(Material[] materials)
        {
            m_lodMaterials = materials;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length && i < materials.Length; i++)
            {
                if (renderers[i] != null && materials[i] != null)
                {
                    renderers[i].material = materials[i];
                }
            }
        }
    }
}
