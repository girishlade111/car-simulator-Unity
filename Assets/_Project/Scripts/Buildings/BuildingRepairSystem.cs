using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Buildings
{
    public class BuildingRepairSystem : MonoBehaviour
    {
        public static BuildingRepairSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableRepairs = true;
        [SerializeField] private float m_baseRepairCost = 100f;
        [SerializeField] private float m_repairSpeed = 10f;

        [Header("Repair Types")]
        [SerializeField] private bool m_canRepairPartial = true;
        [SerializeField] private bool m_canRepairFull = true;
        [SerializeField] private bool m_canReplaceParts = true;

        [Header("Resources")]
        [SerializeField] private int m_materials = 100;
        [SerializeField] private int m_maxMaterials = 500;

        [Header("Prefabs")]
        [SerializeField] private GameObject m_repairEffect;
        [SerializeField] private ParticleSystem m_sparkleEffect;

        private List<RepairJob> m_activeRepairs = new List<RepairJob>();

        [System.Serializable]
        public class RepairJob
        {
            public GameObject building;
            public EnhancedBuildingDestruction destruction;
            public float repairAmount;
            public float repairCost;
            public bool isPartial;
            public float progress;
            public bool isComplete;
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

        private void Update()
        {
            UpdateRepairs();
        }

        private void UpdateRepairs()
        {
            for (int i = m_activeRepairs.Count - 1; i >= 0; i--)
            {
                RepairJob job = m_activeRepairs[i];

                if (job.isComplete || job.building == null)
                {
                    m_activeRepairs.RemoveAt(i);
                    continue;
                }

                job.progress += m_repairSpeed * Time.deltaTime;

                if (job.progress >= 1f)
                {
                    CompleteRepair(job);
                }
            }
        }

        public bool CanAffordRepair(float cost)
        {
            int credits = PlayerPrefs.GetInt("PlayerCredits", 0);
            return credits >= cost;
        }

        public RepairJob StartRepair(GameObject building, float amount, bool isPartial = false)
        {
            if (!m_enableRepairs) return null;

            var destruction = building.GetComponent<EnhancedBuildingDestruction>();
            if (destruction == null)
            {
                Debug.LogWarning("[BuildingRepair] No destruction component found");
                return null;
            }

            float healthPercent = destruction.GetHealthPercentage();
            float missingHealth = (1f - healthPercent) * destruction.MaxHealth;
            float repairHealth = amount > 0 ? amount : missingHealth;

            float cost = CalculateRepairCost(repairHealth);

            if (!CanAffordRepair(cost))
            {
                Debug.LogWarning("[BuildingRepair] Not enough credits");
                return null;
            }

            DeductCredits(cost);

            RepairJob job = new RepairJob
            {
                building = building,
                destruction = destruction,
                repairAmount = repairHealth,
                repairCost = cost,
                isPartial = isPartial,
                progress = 0
            };

            m_activeRepairs.Add(job);
            PlayRepairEffect(building.transform.position);

            Debug.Log($"[BuildingRepair] Started repair for {repairHealth} HP - Cost: ${cost}");

            return job;
        }

        public RepairJob StartFullRepair(GameObject building)
        {
            if (!m_canRepairFull) return null;

            var destruction = building.GetComponent<EnhancedBuildingDestruction>();
            if (destruction == null) return null;

            float missingHealth = (1f - destruction.GetHealthPercentage()) * destruction.MaxHealth;
            return StartRepair(building, missingHealth, false);
        }

        public RepairJob StartPartialRepair(GameObject building)
        {
            if (!m_canRepairPartial) return null;

            var destruction = building.GetComponent<EnhancedBuildingDestruction>();
            if (destruction == null) return null;

            float missingHealth = (1f - destruction.GetHealthPercentage()) * destruction.MaxHealth;
            float partialAmount = missingHealth * 0.25f;

            return StartRepair(building, partialAmount, true);
        }

        private void CompleteRepair(RepairJob job)
        {
            if (job.destruction != null)
            {
                job.destruction.Repair(job.repairAmount);
            }

            job.isComplete = true;
            Debug.Log($"[BuildingRepair] Completed repair - restored {job.repairAmount} HP");
        }

        private float CalculateRepairCost(float healthAmount)
        {
            return healthAmount * m_baseRepairCost / 100f;
        }

        private void DeductCredits(int amount)
        {
            int current = PlayerPrefs.GetInt("PlayerCredits", 0);
            PlayerPrefs.SetInt("PlayerCredits", Mathf.Max(0, current - amount));
            PlayerPrefs.Save();
        }

        private void PlayRepairEffect(Vector3 position)
        {
            if (m_sparkleEffect != null)
            {
                ParticleSystem sparkle = Instantiate(m_sparkleEffect, position, Quaternion.identity);
                sparkle.Play();
                Destroy(sparkle.gameObject, 3f);
            }
        }

        public bool IsRepairing(GameObject building)
        {
            return m_activeRepairs.Exists(j => j.building == building && !j.isComplete);
        }

        public float GetRepairProgress(GameObject building)
        {
            RepairJob job = m_activeRepairs.Find(j => j.building == building);
            return job != null ? job.progress : 0;
        }

        public void CancelRepair(GameObject building)
        {
            RepairJob job = m_activeRepairs.Find(j => j.building == building);
            if (job != null)
            {
                m_activeRepairs.Remove(job);
                Debug.Log("[BuildingRepair] Repair cancelled");
            }
        }

        public void AddMaterials(int amount)
        {
            m_materials = Mathf.Min(m_materials + amount, m_maxMaterials);
        }

        public int GetMaterials() => m_materials;
    }

    public class BuildingMaintenance : MonoBehaviour
    {
        [Header("Maintenance Settings")]
        [SerializeField] private bool m_autoRepair = false;
        [SerializeField] private float m_autoRepairThreshold = 0.7f;
        [SerializeField] private float m_checkInterval = 30f;

        [Header("Destructible Components")]
        [SerializeField] private EnhancedBuildingDestruction m_destruction;
        [SerializeField] private DestructibleComponent[] m_damagedParts;

        private float m_checkTimer;

        private void Start()
        {
            m_destruction = GetComponent<EnhancedBuildingDestruction>();
            m_damagedParts = GetComponentsInChildren<DestructibleComponent>();
        }

        private void Update()
        {
            if (m_autoRepair)
            {
                m_checkTimer += Time.deltaTime;

                if (m_checkTimer >= m_checkInterval)
                {
                    m_checkTimer = 0;
                    CheckAndRepair();
                }
            }
        }

        private void CheckAndRepair()
        {
            if (m_destruction != null && m_destruction.GetHealthPercentage() < m_autoRepairThreshold)
            {
                BuildingRepairSystem.Instance?.StartFullRepair(gameObject);
            }

            foreach (var part in m_damagedParts)
            {
                if (part == null) continue;
            }
        }

        public void RequestInspection()
        {
            Debug.Log("[Maintenance] Building inspection requested");

            if (m_destruction != null)
            {
                float health = m_destruction.GetHealthPercentage();
                Debug.Log($"[Maintenance] Building health: {health * 100}%");

                if (health < 0.5f)
                {
                    Debug.Log("[Maintenance] WARNING: Building requires repairs!");
                }
            }
        }

        public void ReplaceDamagedPart(DestructibleComponent part, GameObject newPart)
        {
            if (part == null || newPart == null) return;

            GameObject.Destroy(part.gameObject);

            Instantiate(newPart, part.transform.position, part.transform.rotation, part.transform.parent);
        }
    }
}
