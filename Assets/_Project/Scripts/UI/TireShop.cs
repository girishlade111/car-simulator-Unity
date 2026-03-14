using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class TireShop : MonoBehaviour
    {
        [Header("Shop UI")]
        [SerializeField] private GameObject m_shopPanel;
        [SerializeField] private Text m_creditsText;

        [Header("Tire Categories")]
        [SerializeField] private GameObject m_streetTiresPanel;
        [SerializeField] private GameObject m_racingTiresPanel;
        [SerializeField] private GameObject m_offroadTiresPanel;
        [SerializeField] private GameObject m_winterTiresPanel;

        [Header("Tire Items")]
        [SerializeField] private Button[] m_tireButtons;
        [SerializeField] private Text[] m_tirePriceTexts;

        [Header("Current Selection")]
        [SerializeField] private TireData m_selectedTire;
        [SerializeField] private int m_selectedPrice;

        [Header("References")]
        [SerializeField] private CustomizationShop m_customizationShop;

        private int m_currentCredits = 10000;
        private List<TireData> m_availableTires = new List<TireData>();

        [System.Serializable]
        public class TireData
        {
            public string name;
            public string category;
            public int price;
            public float gripModifier = 1f;
            public float durabilityModifier = 1f;
            public float speedModifier = 1f;
            public Color tireColor = Color.black;
        }

        private void Start()
        {
            InitializeTireCatalog();
            SetupButtons();
            CloseShop();
        }

        private void InitializeTireCatalog()
        {
            m_availableTires = new List<TireData>
            {
                // Street Tires
                new TireData { name = "Street Basic", category = "Street", price = 300, gripModifier = 1.0f, durabilityModifier = 1.2f },
                new TireData { name = "Street Sport", category = "Street", price = 600, gripModifier = 1.2f, durabilityModifier = 1.0f },
                new TireData { name = "Street Performance", category = "Street", price = 900, gripModifier = 1.3f, durabilityModifier = 0.9f },

                // Racing Tires
                new TireData { name = "Racing Slick", category = "Racing", price = 1500, gripModifier = 1.5f, durabilityModifier = 0.5f },
                new TireData { name = "Racing Semi-Slick", category = "Racing", price = 1200, gripModifier = 1.4f, durabilityModifier = 0.7f },
                new TireData { name = "Racing Wet", category = "Racing", price = 1100, gripModifier = 1.3f, durabilityModifier = 0.8f },

                // Offroad Tires
                new TireData { name = "All-Terrain", category = "Offroad", price = 800, gripModifier = 1.1f, durabilityModifier = 1.5f },
                new TireData { name = "Mud Terrain", category = "Offroad", price = 1000, gripModifier = 1.3f, durabilityModifier = 1.3f },
                new TireData { name = "Rock Crawler", category = "Offroad", price = 1400, gripModifier = 1.4f, durabilityModifier = 1.6f },

                // Winter Tires
                new TireData { name = "Winter Standard", category = "Winter", price = 500, gripModifier = 1.2f, durabilityModifier = 1.1f },
                new TireData { name = "Winter Premium", category = "Winter", price = 750, gripModifier = 1.4f, durabilityModifier = 1.0f }
            };

            UpdateTireDisplay();
        }

        private void SetupButtons()
        {
            if (m_tireButtons != null)
            {
                for (int i = 0; i < m_tireButtons.Length; i++)
                {
                    int tireIndex = i;
                    m_tireButtons[i].onClick.AddListener(() => SelectTire(tireIndex));
                }
            }
        }

        private void UpdateTireDisplay()
        {
            if (m_tirePriceTexts != null)
            {
                for (int i = 0; i < m_tirePriceTexts.Length && i < m_availableTires.Count; i++)
                {
                    TireData tire = m_availableTires[i];
                    m_tirePriceTexts[i].text = $"{tire.name}\n${tire.price}";
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleShop();
            }
        }

        public void ToggleShop()
        {
            if (m_shopPanel != null)
            {
                bool isOpen = m_shopPanel.activeSelf;
                m_shopPanel.SetActive(!isOpen);
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

        public void SelectTire(int index)
        {
            if (index >= 0 && index < m_availableTires.Count)
            {
                m_selectedTire = m_availableTires[index];
                m_selectedPrice = m_selectedTire.price;
                Debug.Log($"[TireShop] Selected: {m_selectedTire.name} - ${m_selectedPrice}");
            }
        }

        public void BuySelectedTire()
        {
            if (m_selectedTire == null)
            {
                Debug.LogWarning("[TireShop] No tire selected!");
                return;
            }

            if (m_currentCredits >= m_selectedPrice)
            {
                m_currentCredits -= m_selectedPrice;
                UpdateCreditsDisplay();
                ApplyTireToVehicle(m_selectedTire);
                Debug.Log($"[TireShop] Purchased {m_selectedTire.name} for ${m_selectedPrice}");
            }
            else
            {
                Debug.LogWarning("[TireShop] Not enough credits!");
            }
        }

        private void ApplyTireToVehicle(TireData tire)
        {
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                var wheelCustomizer = spawner.CurrentVehicle.GetComponent<WheelCustomizer>();
                if (wheelCustomizer != null)
                {
                    wheelCustomizer.SetRimSize(tire.gripModifier);
                }
            }
        }

        private void UpdateCreditsDisplay()
        {
            if (m_creditsText != null)
            {
                m_creditsText.text = $"Credits: {m_currentCredits}";
            }
        }

        public void ShowStreetTires() => TogglePanel(m_streetTiresPanel);
        public void ShowRacingTires() => TogglePanel(m_racingTiresPanel);
        public void ShowOffroadTires() => TogglePanel(m_offroadTiresPanel);
        public void ShowWinterTires() => TogglePanel(m_winterTiresPanel);

        private void TogglePanel(GameObject panel)
        {
            if (m_streetTiresPanel != null) m_streetTiresPanel.SetActive(false);
            if (m_racingTiresPanel != null) m_racingTiresPanel.SetActive(false);
            if (m_offroadTiresPanel != null) m_offroadTiresPanel.SetActive(false);
            if (m_winterTiresPanel != null) m_winterTiresPanel.SetActive(false);

            if (panel != null) panel.SetActive(true);
        }

        public void AddCredits(int amount)
        {
            m_currentCredits += amount;
            UpdateCreditsDisplay();
        }
    }
}
