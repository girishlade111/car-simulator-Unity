using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class BuildingRenovationSystem : MonoBehaviour
    {
        public static BuildingRenovationSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableRenovations = true;
        [SerializeField] private float m_baseRenovationCost = 500f;

        [Header("Renovation Categories")]
        [SerializeField] private bool m_enableExterior = true;
        [SerializeField] private bool m_enableInterior = true;
        [SerializeField] private bool m_enableAmenities = true;

        [Header("Prefabs")]
        [SerializeField] private GameObject m_renovationEffect;
        [SerializeField] private ParticleSystem m_celebrationEffect;

        [Header("Available Renovations")]
        [SerializeField] private List<RenovationPackage> m_availablePackages;

        private Dictionary<GameObject, List<AppliedRenovation>> m_buildingRenovations = new Dictionary<GameObject, List<AppliedRenovation>>();

        [System.Serializable]
        public class RenovationPackage
        {
            public string id;
            public string displayName;
            public string description;
            public RenovationCategory category;
            public float cost;
            public GameObject[] addOnPrefabs;
            public Material[] materialOverlays;
            public float valueMultiplier = 1.1f;
        }

        [System.Serializable]
        public class AppliedRenovation
        {
            public string packageId;
            public GameObject[] installedAddOns;
            public Material[] appliedMaterials;
            public float valueBonus;
        }

        public enum RenovationCategory
        {
            Exterior,
            Interior,
            Amenities,
            EcoUpgrade,
            Security,
            Aesthetic
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
            InitializeDefaultPackages();
        }

        private void InitializeDefaultPackages()
        {
            if (m_availablePackages == null)
            {
                m_availablePackages = new List<RenovationPackage>();
            }

            if (m_availablePackages.Count == 0)
            {
                m_availablePackages.Add(new RenovationPackage
                {
                    id = "rooftop_garden",
                    displayName = "Rooftop Garden",
                    description = "Add a beautiful green garden to the roof",
                    category = RenovationCategory.Amenities,
                    cost = 2000f,
                    valueMultiplier = 1.25f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "solar_panels",
                    displayName = "Solar Panels",
                    description = "Install eco-friendly solar panels",
                    category = RenovationCategory.EcoUpgrade,
                    cost = 1500f,
                    valueMultiplier = 1.2f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "modern_paint",
                    displayName = "Modern Paint Job",
                    description = "Fresh new exterior paint",
                    category = RenovationCategory.Exterior,
                    cost = 800f,
                    valueMultiplier = 1.15f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "security_system",
                    displayName = "Security System",
                    description = "Install cameras and alarm systems",
                    category = RenovationCategory.Security,
                    cost = 1200f,
                    valueMultiplier = 1.18f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "neon_signage",
                    displayName = "Neon Signage",
                    description = "Add eye-catching neon signs",
                    category = RenovationCategory.Aesthetic,
                    cost = 600f,
                    valueMultiplier = 1.12f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "lobby_upgrade",
                    displayName = "Lobby Upgrade",
                    description = "Modernize the building entrance",
                    category = RenovationCategory.Interior,
                    cost = 1000f,
                    valueMultiplier = 1.15f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "green_walls",
                    displayName = "Living Walls",
                    description = "Install vertical gardens on exterior",
                    category = RenovationCategory.EcoUpgrade,
                    cost = 1800f,
                    valueMultiplier = 1.22f
                });

                m_availablePackages.Add(new RenovationPackage
                {
                    id = "roof_deck",
                    displayName = "Rooftop Deck",
                    description = "Create an outdoor lounge area",
                    category = RenovationCategory.Amenities,
                    cost = 2500f,
                    valueMultiplier = 1.3f
                });
            }
        }

        public bool CanAffordRenovation(float cost)
        {
            int credits = PlayerPrefs.GetInt("PlayerCredits", 0);
            return credits >= cost;
        }

        public bool ApplyRenovation(GameObject building, RenovationPackage package)
        {
            if (!m_enableRenovations) return false;
            if (!CanAffordRenovation(package.cost)) return false;

            if (!m_buildingRenovations.ContainsKey(building))
            {
                m_buildingRenovations[building] = new List<AppliedRenovation>();
            }

            if (HasRenovation(building, package.id))
            {
                Debug.LogWarning($"[Renovation] Building already has {package.displayName}");
                return false;
            }

            DeductCredits((int)package.cost);

            AppliedRenovation renovation = new AppliedRenovation
            {
                packageId = package.id,
                installedAddOns = InstallAddOns(building, package),
                appliedMaterials = ApplyMaterials(building, package),
                valueBonus = package.valueMultiplier
            };

            m_buildingRenovations[building].Add(renovation);

            PlayRenovationEffect(building.transform.position);

            Debug.Log($"[Renovation] Applied {package.displayName} to building - Cost: ${package.cost}");

            return true;
        }

        private GameObject[] InstallAddOns(GameObject building, RenovationPackage package)
        {
            if (package.addOnPrefabs == null || package.addOnPrefabs.Length == 0)
                return null;

            List<GameObject> installed = new List<GameObject>();

            foreach (var prefab in package.addOnPrefabs)
            {
                if (prefab != null)
                {
                    GameObject addOn = Instantiate(prefab, building.transform);
                    addOn.transform.localPosition = Vector3.zero;
                    addOn.transform.localRotation = Quaternion.identity;
                    installed.Add(addOn);
                }
            }

            return installed.ToArray();
        }

        private Material[] ApplyMaterials(GameObject building, RenovationPackage package)
        {
            if (package.materialOverlays == null || package.materialOverlays.Length == 0)
                return null;

            List<Material> applied = new List<Material>();
            Renderer[] renderers = building.GetComponentsInChildren<Renderer>();

            int matIndex = 0;
            foreach (var renderer in renderers)
            {
                if (matIndex >= package.materialOverlays.Length) break;

                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length && matIndex < package.materialOverlays.Length; i++)
                {
                    if (mats[i] != null)
                    {
                        mats[i] = package.materialOverlays[matIndex];
                        applied.Add(mats[i]);
                        matIndex++;
                    }
                }
                renderer.materials = mats;
            }

            return applied.ToArray();
        }

        public bool HasRenovation(GameObject building, string packageId)
        {
            if (!m_buildingRenovations.ContainsKey(building))
                return false;

            return m_buildingRenovations[building].Exists(r => r.packageId == packageId);
        }

        public List<RenovationPackage> GetAvailablePackages(RenovationCategory category)
        {
            List<RenovationPackage> result = new List<RenovationPackage>();

            foreach (var package in m_availablePackages)
            {
                if (package.category == category)
                {
                    result.Add(package);
                }
            }

            return result;
        }

        public List<RenovationPackage> GetAllAvailablePackages()
        {
            return new List<RenovationPackage>(m_availablePackages);
        }

        public float GetBuildingValueMultiplier(GameObject building)
        {
            if (!m_buildingRenovations.ContainsKey(building))
                return 1f;

            float multiplier = 1f;
            foreach (var renovation in m_buildingRenovations[building])
            {
                multiplier *= renovation.valueBonus;
            }

            return multiplier;
        }

        public int GetRenovationCount(GameObject building)
        {
            if (!m_buildingRenovations.ContainsKey(building))
                return 0;

            return m_buildingRenovations[building].Count;
        }

        public void RemoveRenovation(GameObject building, string packageId)
        {
            if (!m_buildingRenovations.ContainsKey(building))
                return;

            AppliedRenovation renovation = m_buildingRenovations[building].Find(r => r.packageId == packageId);
            if (renovation == null)
                return;

            if (renovation.installedAddOns != null)
            {
                foreach (var addOn in renovation.installedAddOns)
                {
                    if (addOn != null)
                        Destroy(addOn);
                }
            }

            m_buildingRenovations[building].Remove(renovation);

            Debug.Log($"[Renovation] Removed {packageId} from building");
        }

        private void DeductCredits(int amount)
        {
            int current = PlayerPrefs.GetInt("PlayerCredits", 0);
            PlayerPrefs.SetInt("PlayerCredits", Mathf.Max(0, current - amount));
            PlayerPrefs.Save();
        }

        private void PlayRenovationEffect(Vector3 position)
        {
            if (m_renovationEffect != null)
            {
                GameObject effect = Instantiate(m_renovationEffect, position, Quaternion.identity);
                Destroy(effect, 3f);
            }

            if (m_celebrationEffect != null)
            {
                ParticleSystem celebration = Instantiate(m_celebrationEffect, position, Quaternion.identity);
                celebration.Play();
                Destroy(celebration.gameObject, 5f);
            }
        }

        public void SellBuilding(GameObject building)
        {
            float baseValue = 10000f;
            float valueMultiplier = GetBuildingValueMultiplier(building);

            int salePrice = Mathf.RoundToInt(baseValue * valueMultiplier);

            if (m_buildingRenovations.ContainsKey(building))
            {
                foreach (var renovation in m_buildingRenovations[building])
                {
                    if (renovation.installedAddOns != null)
                    {
                        foreach (var addOn in renovation.installedAddOns)
                        {
                            if (addOn != null)
                                Destroy(addOn);
                        }
                    }
                }
                m_buildingRenovations.Remove(building);
            }

            int current = PlayerPrefs.GetInt("PlayerCredits", 0);
            PlayerPrefs.SetInt("PlayerCredits", current + salePrice);
            PlayerPrefs.Save();

            Debug.Log($"[Renovation] Building sold for ${salePrice}");
        }
    }

    public class BuildingRenovationUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject m_panel;
        [SerializeField] private Transform m_packageListContainer;
        [SerializeField] private GameObject m_packageButtonPrefab;

        private GameObject m_selectedBuilding;
        private BuildingRenovationSystem m_renovationSystem;

        private void Start()
        {
            m_renovationSystem = BuildingRenovationSystem.Instance;
            if (m_panel != null)
            {
                m_panel.SetActive(false);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
            {
                TogglePanel();
            }
        }

        public void TogglePanel()
        {
            if (m_panel != null)
            {
                m_panel.SetActive(!m_panel.activeSelf);
                if (m_panel.activeSelf)
                {
                    RefreshPackageList();
                }
            }
        }

        public void ShowPanel(GameObject building)
        {
            m_selectedBuilding = building;
            if (m_panel != null)
            {
                m_panel.SetActive(true);
                RefreshPackageList();
            }
        }

        public void HidePanel()
        {
            if (m_panel != null)
            {
                m_panel.SetActive(false);
            }
        }

        private void RefreshPackageList()
        {
            if (m_packageListContainer == null || m_packageButtonPrefab == null) return;

            foreach (Transform child in m_packageListContainer)
            {
                Destroy(child.gameObject);
            }

            var packages = m_renovationSystem.GetAllAvailablePackages();

            foreach (var package in packages)
            {
                GameObject button = Instantiate(m_packageButtonPrefab, m_packageListContainer);
                var buttonScript = button.GetComponent<RenovationButton>();

                bool alreadyApplied = m_selectedBuilding != null && 
                    m_renovationSystem.HasRenovation(m_selectedBuilding, package.id);

                buttonScript.Setup(package, alreadyApplied, ApplyPackage);
            }
        }

        private void ApplyPackage(BuildingRenovationSystem.RenovationPackage package)
        {
            if (m_selectedBuilding == null) return;

            bool success = m_renovationSystem.ApplyRenovation(m_selectedBuilding, package);

            if (success)
            {
                RefreshPackageList();
            }
        }

        public void OnSellBuilding()
        {
            if (m_selectedBuilding == null) return;

            m_renovationSystem.SellBuilding(m_selectedBuilding);
            HidePanel();
        }
    }

    public class RenovationButton : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text m_nameText;
        [SerializeField] private UnityEngine.UI.Text m_costText;
        [SerializeField] private UnityEngine.UI.Text m_descriptionText;
        [SerializeField] private UnityEngine.UI.Button m_applyButton;

        private BuildingRenovationSystem.RenovationPackage m_package;
        private System.Action<BuildingRenovationSystem.RenovationPackage> m_onClick;

        public void Setup(BuildingRenovationSystem.RenovationPackage package, bool alreadyApplied, System.Action<BuildingRenovationSystem.RenovationPackage> onClick)
        {
            m_package = package;
            m_onClick = onClick;

            if (m_nameText != null)
                m_nameText.text = package.displayName;

            if (m_costText != null)
                m_costText.text = $"${package.cost}";

            if (m_descriptionText != null)
                m_descriptionText.text = package.description;

            if (m_applyButton != null)
            {
                m_applyButton.interactable = !alreadyApplied;
                m_applyButton.onClick.RemoveAllListeners();
                m_applyButton.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            m_onClick?.Invoke(m_package);
        }
    }
}
