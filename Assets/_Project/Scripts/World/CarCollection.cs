using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.World
{
    public class CarCollection : MonoBehaviour
    {
        public static CarCollection Instance { get; private set; }

        [Header("Collection UI")]
        [SerializeField] private GameObject m_collectionPanel;
        [SerializeField] private Text m_collectionCountText;
        [SerializeField] private GridLayoutGroup m_carGrid;

        [Header("Collection Items")]
        [SerializeField] private GameObject m_carItemPrefab;
        [SerializeField] private Image[] m_carSlots;

        private List<CollectedCar> m_collection = new List<CollectedCar>();
        private CollectedCar m_currentCar;

        [System.Serializable]
        public class CollectedCar
        {
            public string name;
            public int purchasePrice;
            public Color bodyColor;
            public VehicleTuningPresets.TuningType tuningType;
            public bool isOwned;
            public float totalDistance;
            public int raceWins;
            public int driftScore;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeCollection();
            LoadCollection();
        }

        private void InitializeCollection()
        {
            m_collection = new List<CollectedCar>
            {
                CreateCar("Starter Sedan", 0, Color.white, VehicleTuningPresets.TuningType.Default),
                CreateCar("Sport Coupe", 15000, Color.red, VehicleTuningPresets.TuningType.Sport),
                CreateCar("Drift Machine", 20000, new Color(1f, 0.5f, 0f), VehicleTuningPresets.TuningType.Drift),
                CreateCar("Offroad Truck", 18000, Color.green, VehicleTuningPresets.TuningType.Offroad),
                CreateCar("Luxury Sedan", 25000, Color.black, VehicleTuningPresets.TuningType.Default),
                CreateCar("Racing Beast", 35000, Color.yellow, VehicleTuningPresets.TuningType.Sport),
                CreateCar("Night Rider", 28000, new Color(0.2f, 0.1f, 0.4f), VehicleTuningPresets.TuningType.Drift),
                CreateCar("Mountain King", 22000, Color.blue, VehicleTuningPresets.TuningType.Offroad),
                CreateCar("Hyper Car", 80000, Color.cyan, VehicleTuningPresets.TuningType.Sport),
                CreateCar("Classic", 30000, new Color(0.6f, 0.3f, 0.1f), VehicleTuningPresets.TuningType.Default)
            };

            m_collection[0].isOwned = true;
        }

        private CollectedCar CreateCar(string name, int price, Color color, VehicleTuningPresets.TuningType tuning)
        {
            return new CollectedCar
            {
                name = name,
                purchasePrice = price,
                bodyColor = color,
                tuningType = tuning,
                isOwned = false,
                totalDistance = 0,
                raceWins = 0,
                driftScore = 0
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleCollection();
            }
        }

        public void ToggleCollection()
        {
            if (m_collectionPanel != null)
            {
                m_collectionPanel.SetActive(!m_collectionPanel.activeSelf);
                
                if (m_collectionPanel.activeSelf)
                {
                    UpdateCollectionDisplay();
                }
            }
        }

        public void OpenCollection()
        {
            if (m_collectionPanel != null)
            {
                m_collectionPanel.SetActive(true);
                UpdateCollectionDisplay();
            }
        }

        public void CloseCollection()
        {
            if (m_collectionPanel != null)
            {
                m_collectionPanel.SetActive(false);
            }
        }

        private void UpdateCollectionDisplay()
        {
            int ownedCount = 0;
            foreach (var car in m_collection)
            {
                if (car.isOwned) ownedCount++;
            }

            if (m_collectionCountText != null)
            {
                m_collectionCountText.text = $"Collection: {ownedCount}/{m_collection.Count}";
            }
        }

        public void AddCarToCollection(string name, int price, Color color, VehicleTuningPresets.TuningType tuning)
        {
            var existingCar = m_collection.Find(c => c.name == name);
            if (existingCar != null)
            {
                existingCar.isOwned = true;
            }
            else
            {
                CollectedCar newCar = CreateCar(name, price, color, tuning);
                newCar.isOwned = true;
                m_collection.Add(newCar);
            }

            SaveCollection();
            UpdateCollectionDisplay();
        }

        public void SelectCar(int index)
        {
            if (index >= 0 && index < m_collection.Count)
            {
                m_currentCar = m_collection[index];
                
                if (!m_currentCar.isOwned)
                {
                    Debug.LogWarning($"[Collection] You don't own {m_currentCar.name}!");
                    return;
                }

                EquipCar(m_currentCar);
            }
        }

        public void EquipCar(CollectedCar car)
        {
            if (!car.isOwned) return;

            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner != null)
            {
                spawner.SetVehicleColor(car.bodyColor);
                spawner.SetTuningType(car.tuningType);
                spawner.RespawnVehicle();
            }

            Debug.Log($"[Collection] Equipped: {car.name}");
        }

        public CollectedCar GetCurrentCar()
        {
            return m_currentCar;
        }

        public void UpdateCarStats(string carName, float distance, int wins, int drift)
        {
            var car = m_collection.Find(c => c.name == carName);
            if (car != null)
            {
                car.totalDistance += distance;
                car.raceWins += wins;
                car.driftScore += drift;
                SaveCollection();
            }
        }

        public int GetTotalCarsOwned()
        {
            int count = 0;
            foreach (var car in m_collection)
            {
                if (car.isOwned) count++;
            }
            return count;
        }

        public List<CollectedCar> GetOwnedCars()
        {
            List<CollectedCar> owned = new List<CollectedCar>();
            foreach (var car in m_collection)
            {
                if (car.isOwned) owned.Add(car);
            }
            return owned;
        }

        public void SaveCollection()
        {
            // Save to PlayerPrefs
            for (int i = 0; i < m_collection.Count; i++)
            {
                PlayerPrefs.SetInt($"Car_{i}_Owned", m_collection[i].isOwned ? 1 : 0);
                PlayerPrefs.SetFloat($"Car_{i}_Distance", m_collection[i].totalDistance);
                PlayerPrefs.SetInt($"Car_{i}_Wins", m_collection[i].raceWins);
                PlayerPrefs.SetInt($"Car_{i}_Drift", m_collection[i].driftScore);
            }
            PlayerPrefs.Save();
        }

        private void LoadCollection()
        {
            for (int i = 0; i < m_collection.Count; i++)
            {
                if (PlayerPrefs.HasKey($"Car_{i}_Owned"))
                {
                    m_collection[i].isOwned = PlayerPrefs.GetInt($"Car_{i}_Owned") == 1;
                    m_collection[i].totalDistance = PlayerPrefs.GetFloat($"Car_{i}_Distance");
                    m_collection[i].raceWins = PlayerPrefs.GetInt($"Car_{i}_Wins");
                    m_collection[i].driftScore = PlayerPrefs.GetInt($"Car_{i}_Drift");
                }
            }
        }

        public void ClearCollection()
        {
            for (int i = 0; i < m_collection.Count; i++)
            {
                m_collection[i].isOwned = i == 0;
                m_collection[i].totalDistance = 0;
                m_collection[i].raceWins = 0;
                m_collection[i].driftScore = 0;
            }
            SaveCollection();
            UpdateCollectionDisplay();
        }
    }
}
