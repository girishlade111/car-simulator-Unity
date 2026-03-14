using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class RimShop : MonoBehaviour
    {
        [Header("Shop UI")]
        [SerializeField] private GameObject m_shopPanel;
        [SerializeField] private Text m_creditsText;

        [Header("Rim Categories")]
        [SerializeField] private GameObject m_stockRimsPanel;
        [SerializeField] private GameObject m_sportRimsPanel;
        [SerializeField] private GameObject m_racingRimsPanel;
        [SerializeField] private GameObject m_luxuryRimsPanel;

        [Header("Preview")]
        [SerializeField] private Image m_previewImage;
        [SerializeField] private Text m_previewName;
        [SerializeField] private Text m_previewPrice;

        [Header("References")]
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private int m_currentCredits = 10000;
        private List<RimData> m_availableRims = new List<RimData>();
        private RimData m_selectedRim;

        [System.Serializable]
        public class RimData
        {
            public string name;
            public string category;
            public int price;
            public Color rimColor = Color.gray;
            public float rimSize = 0.35f;
            public int spokeCount = 5;
            public bool isChrome;
        }

        private void Start()
        {
            InitializeRimCatalog();
            CloseShop();
        }

        private void InitializeRimCatalog()
        {
            m_availableRims = new List<RimData>
            {
                // Stock
                new RimData { name = "Stock Steel", category = "Stock", price = 0, rimColor = Color.gray, spokeCount = 5 },
                new RimData { name = "Stock Alloy", category = "Stock", price = 200, rimColor = new Color(0.7f, 0.7f, 0.7f), spokeCount = 5 },

                // Sport
                new RimData { name = "Sport 5-Spoke", category = "Sport", price = 600, rimColor = Color.black, spokeCount = 5 },
                new RimData { name = "Sport Multi-Spoke", category = "Sport", price = 750, rimColor = Color.black, spokeCount = 10 },
                new RimData { name = "Sport Mesh", category = "Sport", price = 900, rimColor = new Color(0.6f, 0.6f, 0.6f), spokeCount = 12 },

                // Racing
                new RimData { name = "Racing BBS", category = "Racing", price = 1500, rimColor = Color.black, spokeCount = 7 },
                new RimData { name = "Racing OZ Racing", category = "Racing", price = 1800, rimColor = new Color(0.3f, 0.3f, 0.3f), spokeCount = 10 },
                new RimData { name = "Racing Enkei", category = "Racing", price = 1600, rimColor = Color.black, spokeCount = 6 },

                // Luxury
                new RimData { name = "Luxury Chrome", category = "Luxury", price = 2500, rimColor = new Color(0.85f, 0.85f, 0.9f), spokeCount = 7, isChrome = true },
                new RimData { name = "Luxury Gold", category = "Luxury", price = 3000, rimColor = new Color(0.85f, 0.7f, 0.2f), spokeCount = 7 },
                new RimData { name = "Luxury Black Chrome", category = "Luxury", price = 2800, rimColor = new Color(0.1f, 0.1f, 0.15f), spokeCount = 10, isChrome = true }
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
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

        public void SelectRim(int index)
        {
            if (index >= 0 && index < m_availableRims.Count)
            {
                m_selectedRim = m_availableRims[index];
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            if (m_selectedRim == null) return;

            if (m_previewName != null)
                m_previewName.text = m_selectedRim.name;

            if (m_previewPrice != null)
                m_previewPrice.text = $"${m_selectedRim.price}";

            if (m_previewImage != null && m_previewImage.color != null)
                m_previewImage.color = m_selectedRim.rimColor;
        }

        public void BuySelectedRim()
        {
            if (m_selectedRim == null)
            {
                Debug.LogWarning("[RimShop] No rim selected!");
                return;
            }

            if (m_currentCredits >= m_selectedRim.price)
            {
                m_currentCredits -= m_selectedRim.price;
                UpdateCreditsDisplay();
                ApplyRimToVehicle(m_selectedRim);
                Debug.Log($"[RimShop] Purchased {m_selectedRim.name} for ${m_selectedRim.price}");
            }
            else
            {
                Debug.LogWarning("[RimShop] Not enough credits!");
            }
        }

        private void ApplyRimToVehicle(RimData rim)
        {
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                var wheelCustomizer = spawner.CurrentVehicle.GetComponent<WheelCustomizer>();
                if (wheelCustomizer != null)
                {
                    wheelCustomizer.SetRimColor(rim.rimColor);
                    wheelCustomizer.SetRimSize(rim.rimSize);
                }
            }
        }

        private void UpdateCreditsDisplay()
        {
            if (m_creditsText != null)
                m_creditsText.text = $"Credits: {m_currentCredits}";
        }

        public void ShowStockRims() => TogglePanel(m_stockRimsPanel);
        public void ShowSportRims() => TogglePanel(m_sportRimsPanel);
        public void ShowRacingRims() => TogglePanel(m_racingRimsPanel);
        public void ShowLuxuryRims() => TogglePanel(m_luxuryRimsPanel);

        private void TogglePanel(GameObject panel)
        {
            if (m_stockRimsPanel != null) m_stockRimsPanel.SetActive(false);
            if (m_sportRimsPanel != null) m_sportRimsPanel.SetActive(false);
            if (m_racingRimsPanel != null) m_racingRimsPanel.SetActive(false);
            if (m_luxuryRimsPanel != null) m_luxuryRimsPanel.SetActive(false);

            if (panel != null) panel.SetActive(true);
        }

        public void AddCredits(int amount)
        {
            m_currentCredits += amount;
            UpdateCreditsDisplay();
        }
    }
}
