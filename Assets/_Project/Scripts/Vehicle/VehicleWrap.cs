using UnityEngine;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class VehicleWrap : MonoBehaviour
    {
        [Header("Wrap Settings")]
        [SerializeField] private bool m_enableWraps = true;
        [SerializeField] private Color m_currentWrapColor = Color.red;
        [SerializeField] private WrapPattern m_currentPattern = WrapPattern.Solid;
        [SerializeField] private int m_currentDecal;

        [Header("Decals")]
        [SerializeField] private GameObject[] m_decalPrefabs;
        [SerializeField] private Transform[] m_decalMountPoints;

        [Header("References")]
        [SerializeField] private Renderer[] m_vehicleRenderers;

        public enum WrapPattern
        {
            Solid,
            Matte,
            Metallic,
            Chrome,
            Carbon,
            Flame,
            RacingStripes,
            Gradient
        }

        private Dictionary<Renderer, Material> m_originalMaterials = new Dictionary<Renderer, Material>();
        private List<GameObject> m_activeDecals = new List<GameObject>();

        private void Start()
        {
            if (m_enableWraps)
            {
                SetupDecalMountPoints();
                ApplyWrap();
            }
        }

        private void SetupDecalMountPoints()
        {
            if (m_decalMountPoints == null || m_decalMountPoints.Length == 0)
            {
                m_decalMountPoints = new Transform[5];
                
                m_decalMountPoints[0] = CreateMountPoint("Hood", new Vector3(0, 0.6f, 1.5f));
                m_decalMountPoints[1] = CreateMountPoint("Roof", new Vector3(0, 1.5f, -0.5f));
                m_decalMountPoints[2] = CreateMountPoint("Door_L", new Vector3(-0.9f, 0.8f, 0f));
                m_decalMountPoints[3] = CreateMountPoint("Door_R", new Vector3(0.9f, 0.8f, 0f));
                m_decalMountPoints[4] = CreateMountPoint("Rear", new Vector3(0, 0.8f, -1.8f));
            }
        }

        private Transform CreateMountPoint(string name, Vector3 localPos)
        {
            GameObject mount = new GameObject($"DecalMount_{name}");
            mount.transform.SetParent(transform);
            mount.transform.localPosition = localPos;
            return mount.transform;
        }

        public void SetWrapColor(Color color)
        {
            m_currentWrapColor = color;
            ApplyWrap();
        }

        public void SetWrapPattern(WrapPattern pattern)
        {
            m_currentPattern = pattern;
            ApplyWrap();
        }

        public void ApplyWrap()
        {
            FindVehicleRenderers();

            foreach (var renderer in m_vehicleRenderers)
            {
                if (renderer == null) continue;

                Material wrapMaterial = CreateWrapMaterial();
                
                if (!m_originalMaterials.ContainsKey(renderer))
                {
                    m_originalMaterials[renderer] = renderer.material;
                }

                renderer.material = wrapMaterial;
            }
        }

        private Material CreateWrapMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));
            
            switch (m_currentPattern)
            {
                case WrapPattern.Solid:
                    mat.color = m_currentWrapColor;
                    mat.SetFloat("_Glossiness", 0.5f);
                    break;

                case WrapPattern.Matte:
                    mat.color = m_currentWrapColor;
                    mat.SetFloat("_Glossiness", 0f);
                    break;

                case WrapPattern.Metallic:
                    mat.color = m_currentWrapColor;
                    mat.SetFloat("_Glossiness", 0.8f);
                    mat.SetFloat("_Metallic", 0.8f);
                    break;

                case WrapPattern.Chrome:
                    mat.color = new Color(0.8f, 0.8f, 0.8f);
                    mat.SetFloat("_Glossiness", 1f);
                    mat.SetFloat("_Metallic", 1f);
                    break;

                case WrapPattern.Carbon:
                    mat.color = new Color(0.1f, 0.1f, 0.1f);
                    mat.SetFloat("_Glossiness", 0.7f);
                    break;

                case WrapPattern.Flame:
                    mat = CreateFlameMaterial();
                    break;

                case WrapPattern.RacingStripes:
                    mat = CreateStripeMaterial();
                    break;

                case WrapPattern.Gradient:
                    mat = CreateGradientMaterial();
                    break;
            }

            return mat;
        }

        private Material CreateFlameMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = m_currentWrapColor;
            mat.SetFloat("_Glossiness", 0.6f);
            return mat;
        }

        private Material CreateStripeMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = m_currentWrapColor;
            mat.SetFloat("_Glossiness", 0.5f);
            return mat;
        }

        private Material CreateGradientMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = m_currentWrapColor;
            mat.SetFloat("_Glossiness", 0.4f);
            return mat;
        }

        public void ApplyDecal(int decalIndex)
        {
            if (decalIndex < 0 || decalIndex >= m_decalMountPoints.Length) return;

            ClearDecals();

            if (m_decalPrefabs != null && decalIndex < m_decalPrefabs.Length && m_decalPrefabs[decalIndex] != null)
            {
                GameObject decal = Instantiate(m_decalPrefabs[decalIndex], m_decalMountPoints[decalIndex]);
                decal.transform.SetParent(m_decalMountPoints[decalIndex]);
                decal.transform.localPosition = Vector3.zero;
                decal.transform.localRotation = Quaternion.identity;
                m_activeDecals.Add(decal);
                m_currentDecal = decalIndex;
            }
        }

        public void ClearDecals()
        {
            foreach (var decal in m_activeDecals)
            {
                if (decal != null)
                {
                    Destroy(decal);
                }
            }
            m_activeDecals.Clear();
        }

        public void RemoveDecal()
        {
            if (m_activeDecals.Count > 0)
            {
                GameObject decal = m_activeDecals[m_activeDecals.Count - 1];
                m_activeDecals.RemoveAt(m_activeDecals.Count - 1);
                if (decal != null)
                {
                    Destroy(decal);
                }
            }
        }

        private void FindVehicleRenderers()
        {
            if (m_vehicleRenderers == null || m_vehicleRenderers.Length == 0)
            {
                m_vehicleRenderers = GetComponentsInChildren<Renderer>();
            }
        }

        public void ResetToDefault()
        {
            foreach (var kvp in m_originalMaterials)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.material = kvp.Value;
                }
            }

            ClearDecals();
            m_currentWrapColor = Color.red;
            m_currentPattern = WrapPattern.Solid;
        }

        public Color GetCurrentColor() => m_currentWrapColor;
        public WrapPattern GetCurrentPattern() => m_currentPattern;
    }
}
