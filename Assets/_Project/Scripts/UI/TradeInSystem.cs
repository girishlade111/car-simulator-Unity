using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.UI
{
    public class TradeInSystem : MonoBehaviour
    {
        [Header("Trade In UI")]
        [SerializeField] private GameObject m_tradePanel;
        [SerializeField] private Text m_creditsText;
        [SerializeField] private Text m_currentCarValueText;

        [Header("Trade Cars")]
        [SerializeField] private Button[] m_carButtons;
        [SerializeField] private Text[] m_carNames;
        [SerializeField] private Text[] m_carValues;

        [Header("References")]
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private int m_currentCredits = 10000;
        private List<VehicleData> m_availableVehicles = new List<VehicleData>();
        private int m_selectedCarIndex;

        [System.Serializable]
        public class VehicleData
        {
            public string name;
            public int value;
            public Color bodyColor;
            public VehicleTuningPresets.TuningType tuningType;
            public Sprite previewImage;
        }

        private void Start()
        {
            InitializeVehicleCatalog();
            CloseTradePanel();
        }

        private void InitializeVehicleCatalog()
        {
            m_availableVehicles = new List<VehicleData>
            {
                new VehicleData { name = "Sedan Base", value = 5000, bodyColor = Color.white, tuningType = VehicleTuningPresets.TuningType.Default },
                new VehicleData { name = "Sports Car", value = 15000, bodyColor = Color.red, tuningType = VehicleTuningPresets.TuningType.Sport },
                new VehicleData { name = "Drift King", value = 20000, bodyColor = new Color(1f, 0.5f, 0f), tuningType = VehicleTuningPresets.TuningType.Drift },
                new VehicleData { name = "Offroad Beast", value = 18000, bodyColor = Color.green, tuningType = VehicleTuningPresets.TuningType.Offroad },
                new VehicleData { name = "Luxury Sedan", value = 25000, bodyColor = Color.black, tuningType = VehicleTuningPresets.TuningType.Default },
                new VehicleData { name = "Super Car", value = 50000, bodyColor = Color.yellow, tuningType = VehicleTuningPresets.TuningType.Sport }
            };

            UpdateCarList();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleTradePanel();
            }
        }

        public void ToggleTradePanel()
        {
            if (m_tradePanel != null)
            {
                m_tradePanel.SetActive(!m_tradePanel.activeSelf);
            }
        }

        public void OpenTradePanel()
        {
            if (m_tradePanel != null)
            {
                m_tradePanel.SetActive(true);
                UpdateCarList();
            }
        }

        public void CloseTradePanel()
        {
            if (m_tradePanel != null)
            {
                m_tradePanel.SetActive(false);
            }
        }

        private void UpdateCarList()
        {
            if (m_carNames != null && m_carValues != null)
            {
                for (int i = 0; i < m_carNames.Length && i < m_availableVehicles.Count; i++)
                {
                    m_carNames[i].text = m_availableVehicles[i].name;
                    m_carValues[i].text = $"${m_availableVehicles[i].value}";
                }
            }

            UpdateCurrentCarValue();
            UpdateCreditsDisplay();
        }

        private void UpdateCurrentCarValue()
        {
            int currentValue = 5000;
            
            if (m_vehicleSpawner?.CurrentVehicle != null)
            {
                currentValue = CalculateCurrentCarValue();
            }

            if (m_currentCarValueText != null)
                m_currentCarValueText.text = $"Current Car Value: ${currentValue}";
        }

        private int CalculateCurrentCarValue()
        {
            int baseValue = 5000;
            
            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                var upgrades = spawner.CurrentVehicle.GetComponent<PerformanceUpgrades>();
                if (upgrades != null)
                {
                    int upgradeBonus = upgrades.GetTotalUpgradeLevel() * 1000;
                    baseValue += upgradeBonus;
                }
            }

            return baseValue;
        }

        public void SelectCar(int index)
        {
            if (index >= 0 && index < m_availableVehicles.Count)
            {
                m_selectedCarIndex = index;
                Debug.Log($"[TradeIn] Selected: {m_availableVehicles[index].name}");
            }
        }

        public void TradeInCurrentCar()
        {
            int tradeInValue = CalculateCurrentCarValue();
            m_currentCredits += tradeInValue;
            
            UpdateCreditsDisplay();
            Debug.Log($"[TradeIn] Traded car for ${tradeInValue}");
        }

        public void BuySelectedCar()
        {
            if (m_selectedCarIndex < 0 || m_selectedCarIndex >= m_availableVehicles.Count)
            {
                Debug.LogWarning("[TradeIn] No car selected!");
                return;
            }

            VehicleData selectedCar = m_availableVehicles[m_selectedCarIndex];

            if (m_currentCredits >= selectedCar.value)
            {
                m_currentCredits -= selectedCar.value;
                UpdateCreditsDisplay();
                SpawnNewCar(selectedCar);
                Debug.Log($"[TradeIn] Purchased {selectedCar.name} for ${selectedCar.value}");
            }
            else
            {
                Debug.LogWarning("[TradeIn] Not enough credits!");
            }
        }

        private void SpawnNewCar(VehicleData carData)
        {
            if (m_vehicleSpawner == null)
                m_vehicleSpawner = FindObjectOfType<VehicleSpawner>();

            if (m_vehicleSpawner != null)
            {
                m_vehicleSpawner.SetVehicleColor(carData.bodyColor);
                m_vehicleSpawner.SetTuningType(carData.tuningType);
                m_vehicleSpawner.RespawnVehicle();
            }
        }

        private void UpdateCreditsDisplay()
        {
            if (m_creditsText != null)
                m_creditsText.text = $"Credits: {m_currentCredits}";
        }

        public void AddCredits(int amount)
        {
            m_currentCredits += amount;
            UpdateCreditsDisplay();
        }
    }
}
