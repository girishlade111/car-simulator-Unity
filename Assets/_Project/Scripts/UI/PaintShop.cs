using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class PaintShop : MonoBehaviour
    {
        [Header("Shop UI")]
        [SerializeField] private GameObject m_shopPanel;
        [SerializeField] private Text m_creditsText;

        [Header("Paint Categories")]
        [SerializeField] private GameObject m_solidPanel;
        [SerializeField] private GameObject m_metallicPanel;
        [SerializeField] private GameObject m_pearlPanel;
        [SerializeField] private GameObject m_mattePanel;
        [SerializeField] private GameObject m_chromePanel;

        [Header("Paint Preview")]
        [SerializeField] private Image m_previewColor;
        [SerializeField] private Slider m_glossSlider;
        [SerializeField] private Slider m_metallicSlider;
        [SerializeField] private Text m_previewName;

        [Header("References")]
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private int m_currentCredits = 10000;
        private PaintData m_selectedPaint;
        private List<PaintData> m_availablePaints = new List<PaintData>();

        [System.Serializable]
        public class PaintData
        {
            public string name;
            public Color color;
            public PaintCategory category;
            public int price;
            public float glossiness;
            public float metallic;
        }

        public enum PaintCategory
        {
            Solid,
            Metallic,
            Pearl,
            Matte,
            Chrome
        }

        private void Start()
        {
            InitializePaintCatalog();
            CloseShop();
        }

        private void InitializePaintCatalog()
        {
            m_availablePaints = new List<PaintData>
            {
                // Solid Colors
                new PaintData { name = "Racing Red", color = Color.red, category = PaintCategory.Solid, price = 500, glossiness = 0.6f, metallic = 0f },
                new PaintData { name = "Midnight Blue", color = Color.blue, category = PaintCategory.Solid, price = 500, glossiness = 0.6f, metallic = 0f },
                new PaintData { name = "Forest Green", color = Color.green, category = PaintCategory.Solid, price = 500, glossiness = 0.6f, metallic = 0f },
                new PaintData { name = "Pearl White", color = new Color(0.95f, 0.95f, 0.95f), category = PaintCategory.Solid, price = 600, glossiness = 0.7f, metallic = 0.1f },
                new PaintData { name = "Sunset Orange", color = new Color(1f, 0.5f, 0f), category = PaintCategory.Solid, price = 550, glossiness = 0.6f, metallic = 0f },

                // Metallic
                new PaintData { name = "Gunmetal", color = new Color(0.3f, 0.3f, 0.35f), category = PaintCategory.Metallic, price = 800, glossiness = 0.8f, metallic = 0.6f },
                new PaintData { name = "Titanium Silver", color = new Color(0.7f, 0.7f, 0.75f), category = PaintCategory.Metallic, price = 850, glossiness = 0.85f, metallic = 0.7f },
                new PaintData { name = "Midnight Purple", color = new Color(0.3f, 0.1f, 0.4f), category = PaintCategory.Metallic, price = 900, glossiness = 0.8f, metallic = 0.5f },
                new PaintData { name = "British Racing Green", color = new Color(0f, 0.3f, 0.15f), category = PaintCategory.Metallic, price = 900, glossiness = 0.8f, metallic = 0.6f },

                // Pearl
                new PaintData { name = "Pearl White", color = new Color(0.98f, 0.98f, 1f), category = PaintCategory.Pearl, price = 1200, glossiness = 0.9f, metallic = 0.3f },
                new PaintData { name = "Pearl Black", color = new Color(0.1f, 0.1f, 0.15f), category = PaintCategory.Pearl, price = 1300, glossiness = 0.9f, metallic = 0.4f },
                new PaintData { name = "Rainbow Pearl", color = new Color(0.5f, 0f, 0.5f), category = PaintCategory.Pearl, price = 1500, glossiness = 0.9f, metallic = 0.5f },

                // Matte
                new PaintData { name = "Matte Black", color = new Color(0.15f, 0.15f, 0.15f), category = PaintCategory.Matte, price = 1000, glossiness = 0.05f, metallic = 0f },
                new PaintData { name = "Matte Gray", color = new Color(0.4f, 0.4f, 0.4f), category = PaintCategory.Matte, price = 1000, glossiness = 0.05f, metallic = 0f },
                new PaintData { name = "Matte Blue", color = new Color(0.2f, 0.3f, 0.8f), category = PaintCategory.Matte, price = 1100, glossiness = 0.05f, metallic = 0f },

                // Chrome
                new PaintData { name = "Pure Chrome", color = new Color(0.85f, 0.85f, 0.9f), category = PaintCategory.Chrome, price = 2500, glossiness = 1f, metallic = 1f },
                new PaintData { name = "Gold Chrome", color = new Color(0.85f, 0.7f, 0.2f), category = PaintCategory.Chrome, price = 2800, glossiness = 1f, metallic = 1f },
                new PaintData { name = "Blue Chrome", color = new Color(0.3f, 0.5f, 0.9f), category = PaintCategory.Chrome, price = 2700, glossiness = 1f, metallic = 1f }
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                ToggleShop();
            }
        }

        public void ToggleShop()
        {
            if (m_shopPanel != null)
            {
                m_shopPanel.SetActive(!m_shopPanel.activeSelf);
            }
        }

        public void OpenShop()
        {
            if (m_shopPanel != null)
            {
                m_shopPanel.SetActive(true);
            }
        }

        public void CloseShop()
        {
            if (m_shopPanel != null)
            {
                m_shopPanel.SetActive(false);
            }
        }

        public void SelectPaint(int index)
        {
            if (index >= 0 && index < m_availablePaints.Count)
            {
                m_selectedPaint = m_availablePaints[index];
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            if (m_selectedPaint == null) return;

            if (m_previewColor != null)
            {
                m_previewColor.color = m_selectedPaint.color;
            }

            if (m_previewName != null)
            {
                m_previewName.text = m_selectedPaint.name;
            }

            if (m_glossSlider != null)
            {
                m_glossSlider.value = m_selectedPaint.glossiness;
            }

            if (m_metallicSlider != null)
            {
                m_metallicSlider.value = m_selectedPaint.metallic;
            }
        }

        public void BuySelectedPaint()
        {
            if (m_selectedPaint == null)
            {
                Debug.LogWarning("[PaintShop] No paint selected!");
                return;
            }

            if (m_currentCredits >= m_selectedPaint.price)
            {
                m_currentCredits -= m_selectedPaint.price;
                UpdateCreditsDisplay();
                ApplyPaintToVehicle(m_selectedPaint);
                Debug.Log($"[PaintShop] Purchased {m_selectedPaint.name} for ${m_selectedPaint.price}");
            }
            else
            {
                Debug.LogWarning("[PaintShop] Not enough credits!");
            }
        }

        private void ApplyPaintToVehicle(PaintData paint)
        {
            if (m_vehicleSpawner == null)
            {
                m_vehicleSpawner = FindObjectOfType<VehicleSpawner>();
            }

            if (m_vehicleSpawner?.CurrentVehicle != null)
            {
                var wrap = m_vehicleSpawner.CurrentVehicle.GetComponent<VehicleWrap>();
                if (wrap != null)
                {
                    wrap.SetWrapColor(paint.color);
                }

                m_vehicleSpawner.SetVehicleColor(paint.color);
            }
        }

        private void UpdateCreditsDisplay()
        {
            if (m_creditsText != null)
            {
                m_creditsText.text = $"Credits: {m_currentCredits}";
            }
        }

        public void ShowSolidPaints() => TogglePanel(m_solidPanel);
        public void ShowMetallicPaints() => TogglePanel(m_metallicPanel);
        public void ShowPearlPaints() => TogglePanel(m_pearlPanel);
        public void ShowMattePaints() => TogglePanel(m_mattePanel);
        public void ShowChromePaints() => TogglePanel(m_chromePanel);

        private void TogglePanel(GameObject panel)
        {
            if (m_solidPanel != null) m_solidPanel.SetActive(false);
            if (m_metallicPanel != null) m_metallicPanel.SetActive(false);
            if (m_pearlPanel != null) m_pearlPanel.SetActive(false);
            if (m_mattePanel != null) m_mattePanel.SetActive(false);
            if (m_chromePanel != null) m_chromePanel.SetActive(false);

            if (panel != null) panel.SetActive(true);
        }

        public void AddCredits(int amount)
        {
            m_currentCredits += amount;
            UpdateCreditsDisplay();
        }
    }
}
