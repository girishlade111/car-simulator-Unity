using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Vehicle
{
    public class WindowTint : MonoBehaviour
    {
        [Header("Window Tint Settings")]
        [SerializeField] private bool m_enableTinting = true;
        [SerializeField] private TintLevel m_currentTint = TintLevel.Light;
        [SerializeField] private Color m_tintColor = new Color(0.1f, 0.1f, 0.1f);

        [Header("Windows")]
        [SerializeField] private GameObject m_windshield;
        [SerializeField] private GameObject m_rearWindow;
        [SerializeField] private GameObject[] m_sideWindows;

        public enum TintLevel
        {
            None,
            Light,
            Medium,
            Dark,
            Black
        }

        private void Start()
        {
            if (m_enableTinting)
            {
                CreateWindows();
                ApplyTint(m_currentTint);
            }
        }

        private void CreateWindows()
        {
            if (m_windshield == null)
            {
                m_windshield = CreateWindow("Windshield", new Vector3(0, 1.4f, 0.5f), 
                    new Vector3(1.8f, 0.8f, 0.05f), Quaternion.Euler(30, 0, 0));
            }

            if (m_rearWindow == null)
            {
                m_rearWindow = CreateWindow("RearWindow", new Vector3(0, 1.3f, -1.2f), 
                    new Vector3(1.6f, 0.6f, 0.05f), Quaternion.Euler(-30, 0, 0));
            }

            if (m_sideWindows == null || m_sideWindows.Length == 0)
            {
                m_sideWindows = new GameObject[4];
                m_sideWindows[0] = CreateWindow("SideWindow_L", new Vector3(-0.9f, 1.3f, 0f), 
                    new Vector3(0.05f, 0.5f, 1.2f), Quaternion.identity);
                m_sideWindows[1] = CreateWindow("SideWindow_R", new Vector3(0.9f, 1.3f, 0f), 
                    new Vector3(0.05f, 0.5f, 1.2f), Quaternion.identity);
            }
        }

        private GameObject CreateWindow(string name, Vector3 localPos, Vector3 scale, Quaternion rotation)
        {
            GameObject window = GameObject.CreatePrimitive(PrimitiveType.Quad);
            window.name = name;
            window.transform.SetParent(transform);
            window.transform.localPosition = localPos;
            window.transform.localScale = scale;
            window.transform.localRotation = rotation;

            Renderer renderer = window.GetComponent<Renderer>();
            Material windowMat = new Material(Shader.Find("Standard"));
            windowMat.SetFloat("_Mode", 3);
            windowMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            windowMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            windowMat.SetInt("_ZWrite", 0);
            windowMat.DisableKeyword("_ALPHATEST_ON");
            windowMat.EnableKeyword("_ALPHABLEND_ON");
            windowMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            windowMat.renderQueue = 3000;
            renderer.material = windowMat;

            return window;
        }

        public void SetTintLevel(TintLevel level)
        {
            m_currentTint = level;
            ApplyTint(level);
        }

        public void ApplyTint(TintLevel level)
        {
            float alpha = GetAlphaForLevel(level);
            Color tintColor = new Color(m_tintColor.r, m_tintColor.g, m_tintColor.b, alpha);

            ApplyTintToWindow(m_windshield, tintColor);
            ApplyTintToWindow(m_rearWindow, tintColor);

            if (m_sideWindows != null)
            {
                foreach (var window in m_sideWindows)
                {
                    ApplyTintToWindow(window, tintColor);
                }
            }
        }

        private void ApplyTintToWindow(GameObject window, Color tint)
        {
            if (window == null) return;

            Renderer renderer = window.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = tint;
            }
        }

        private float GetAlphaForLevel(TintLevel level)
        {
            return level switch
            {
                TintLevel.None => 0f,
                TintLevel.Light => 0.15f,
                TintLevel.Medium => 0.35f,
                TintLevel.Dark => 0.6f,
                TintLevel.Black => 0.85f,
                _ => 0f
            };
        }

        public void CycleTint()
        {
            int currentIndex = (int)m_currentTint;
            int nextIndex = (currentIndex + 1) % System.Enum.GetValues(typeof(TintLevel)).Length;
            SetTintLevel((TintLevel)nextIndex);
        }

        public void RemoveTint()
        {
            SetTintLevel(TintLevel.None);
        }

        public TintLevel GetCurrentTint() => m_currentTint;
    }
}
