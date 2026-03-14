using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class CustomizationShop : MonoBehaviour
    {
        [Header("Shop UI")]
        [SerializeField] private GameObject m_shopPanel;
        [SerializeField] private Canvas m_shopCanvas;
        [SerializeField] private Text m_creditsText;

        [Header("Categories")]
        [SerializeField] private GameObject m_paintPanel;
        [SerializeField] private GameObject m_partsPanel;
        [SerializeField] private GameObject m_upgradesPanel;

        [Header("Items")]
        [SerializeField] private Button[] m_paintButtons;
        [SerializeField] private Button[] m_wheelButtons;
        [SerializeField] private Button[] m_upgradeButtons;

        [Header("Preview")]
        [SerializeField] private Transform m_vehiclePreview;
        [SerializeField] private float m_previewRotationSpeed = 30f;

        [Header("References")]
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private int m_currentCredits = 10000;
        private bool m_isOpen;
        private Dictionary<string, int> m_prices = new Dictionary<string, int>();

        private void Start()
        {
            InitializeShop();
            CloseShop();
        }

        private void InitializeShop()
        {
            SetupPrices();
            UpdateCreditsDisplay();

            if (m_shopPanel != null)
                m_shopPanel.SetActive(false);
        }

        private void SetupPrices()
        {
            m_prices["Paint_Red"] = 500;
            m_prices["Paint_Blue"] = 500;
            m_prices["Paint_Green"] = 500;
            m_prices["Paint_Black"] = 750;
            m_prices["Paint_White"] = 500;
            m_prices["Paint_Gold"] = 1500;
            m_prices["Paint_Chrome"] = 2000;

            m_prices["Wheels_Sport"] = 1000;
            m_prices["Wheels_Racing"] = 2500;
            m_prices["Wheels_Offroad"] = 1500;

            m_prices["Upgrade_Engine"] = 3000;
            m_prices["Upgrade_Brakes"] = 2000;
            m_prices["Upgrade_Suspension"] = 2500;
            m_prices["Upgrade_Turbo"] = 4000;
        }

        private void Update()
        {
            if (m_isOpen && m_vehiclePreview != null)
            {
                m_vehiclePreview.Rotate(Vector3.up, m_previewRotationSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                ToggleShop();
            }
        }

        public void ToggleShop()
        {
            if (m_isOpen)
                CloseShop();
            else
                OpenShop();
        }

        public void OpenShop()
        {
            if (m_shopPanel != null)
            {
                m_shopPanel.SetActive(true);
                m_isOpen = true;
            }
        }

        public void CloseShop()
        {
            if (m_shopPanel != null)
            {
                m_shopPanel.SetActive(false);
                m_isOpen = false;
            }
        }

        private void UpdateCreditsDisplay()
        {
            if (m_creditsText != null)
            {
                m_creditsText.text = $"Credits: {m_currentCredits}";
            }
        }

        public void BuyPaint(Color color, int price)
        {
            if (m_currentCredits >= price)
            {
                m_currentCredits -= price;
                UpdateCreditsDisplay();

                if (m_vehicleSpawner != null)
                {
                    m_vehicleSpawner.SetVehicleColor(color);
                }

                Debug.Log($"[Shop] Purchased paint for {price} credits");
            }
            else
            {
                Debug.LogWarning("[Shop] Not enough credits!");
            }
        }

        public void BuyWheels(string wheelType, int price)
        {
            if (m_currentCredits >= price)
            {
                m_currentCredits -= price;
                UpdateCreditsDisplay();
                Debug.Log($"[Shop] Purchased {wheelType} wheels for {price} credits");
            }
            else
            {
                Debug.LogWarning("[Shop] Not enough credits!");
            }
        }

        public void BuyUpgrade(UpgradeType type, int price)
        {
            if (m_currentCredits >= price)
            {
                m_currentCredits -= price;
                UpdateCreditsDisplay();

                var spawner = FindObjectOfType<VehicleSpawner>();
                if (spawner?.CurrentVehicle != null)
                {
                    var upgrades = spawner.CurrentVehicle.GetComponent<PerformanceUpgrades>();
                    if (upgrades != null)
                    {
                        upgrades.ApplyUpgrade(type);
                    }
                }

                Debug.Log($"[Shop] Purchased {type} upgrade for {price} credits");
            }
            else
            {
                Debug.LogWarning("[Shop] Not enough credits!");
            }
        }

        public void AddCredits(int amount)
        {
            m_currentCredits += amount;
            UpdateCreditsDisplay();
        }

        public void SelectPaint(int index)
        {
            Color[] colors = { Color.red, Color.blue, Color.green, Color.black, Color.white, Color.yellow, Color.cyan };
            int[] prices = { 500, 500, 500, 750, 500, 600, 800 };

            if (index < colors.Length && index < prices.Length)
            {
                BuyPaint(colors[index], prices[index]);
            }
        }

        public void SelectWheels(int index)
        {
            string[] types = { "Sport", "Racing", "Offroad" };
            int[] prices = { 1000, 2500, 1500 };

            if (index < types.Length && index < prices.Length)
            {
                BuyWheels(types[index], prices[index]);
            }
        }

        public void SelectUpgrade(int index)
        {
            UpgradeType[] types = { UpgradeType.Engine, UpgradeType.Brakes, UpgradeType.Suspension, UpgradeType.Turbo };
            int[] prices = { 3000, 2000, 2500, 4000 };

            if (index < types.Length && index < prices.Length)
            {
                BuyUpgrade(types[index], prices[index]);
            }
        }

        public void ShowPaint() => TogglePanel(m_paintPanel);
        public void ShowParts() => TogglePanel(m_partsPanel);
        public void ShowUpgrades() => TogglePanel(m_upgradesPanel);

        private void TogglePanel(GameObject panel)
        {
            if (m_paintPanel != null) m_paintPanel.SetActive(false);
            if (m_partsPanel != null) m_partsPanel.SetActive(false);
            if (m_upgradesPanel != null) m_upgradesPanel.SetActive(false);

            if (panel != null) panel.SetActive(true);
        }
    }
}
