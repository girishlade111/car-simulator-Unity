using UnityEngine;
using UnityEngine.UI;
using CarSimulator.Vehicle;

namespace CarSimulator.World
{
    public class VehicleRestoration : MonoBehaviour
    {
        [Header("Restoration Settings")]
        [SerializeField] private float m_restorationCost = 5000f;
        [SerializeField] private float m_damageThreshold = 30f;

        [Header("UI References")]
        [SerializeField] private GameObject m_restorationPanel;
        [SerializeField] private Text m_costText;
        [SerializeField] private Text m_statusText;
        [SerializeField] private Button m_restoreButton;
        [SerializeField] private Slider m_restorationProgress;

        [Header("References")]
        [SerializeField] private VehicleDamage m_vehicleDamage;
        [SerializeField] private VehicleSpawner m_vehicleSpawner;

        private bool m_isRestoring;
        private float m_restorationProgress;
        private int m_currentCredits = 10000;

        public enum RestorationStage
        {
            None,
            BodyWork,
            Paint,
            Engine,
            Interior,
            FinalDetail,
            Complete
        }

        private RestorationStage m_currentStage = RestorationStage.None;

        private void Start()
        {
            FindVehicleComponents();
            ClosePanel();
        }

        private void FindVehicleComponents()
        {
            if (m_vehicleDamage == null)
            {
                var spawner = FindObjectOfType<VehicleSpawner>();
                if (spawner?.CurrentVehicle != null)
                {
                    m_vehicleDamage = spawner.CurrentVehicle.GetComponent<VehicleDamage>();
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                TogglePanel();
            }

            if (m_isRestoring)
            {
                UpdateRestoration();
            }
        }

        public void TogglePanel()
        {
            if (m_restorationPanel != null)
            {
                bool isOpen = m_restorationPanel.activeSelf;
                m_restorationPanel.SetActive(!isOpen);
                
                if (!isOpen)
                {
                    UpdatePanel();
                }
            }
        }

        public void OpenPanel()
        {
            if (m_restorationPanel != null)
            {
                m_restorationPanel.SetActive(true);
                UpdatePanel();
            }
        }

        public void ClosePanel()
        {
            if (m_restorationPanel != null)
            {
                m_restorationPanel.SetActive(false);
            }
        }

        private void UpdatePanel()
        {
            FindVehicleComponents();

            bool needsRestoration = false;
            string status = "Vehicle is in perfect condition!";

            if (m_vehicleDamage != null)
            {
                float healthPercent = m_vehicleDamage.HealthPercent;
                
                if (healthPercent < 0.5f)
                {
                    needsRestoration = true;
                    status = "Critical damage detected!";
                }
                else if (healthPercent < 0.8f)
                {
                    needsRestoration = true;
                    status = "Moderate damage detected";
                }
                else if (healthPercent < 1f)
                {
                    needsRestoration = true;
                    status = "Minor damage detected";
                }
            }

            if (m_costText != null)
            {
                m_costText.text = $"Restoration Cost: ${m_restorationCost}";
            }

            if (m_statusText != null)
            {
                m_statusText.text = status;
            }

            if (m_restoreButton != null)
            {
                m_restoreButton.interactable = needsRestoration && m_currentCredits >= m_restorationCost;
            }
        }

        public void StartRestoration()
        {
            if (m_currentCredits < m_restorationCost)
            {
                Debug.LogWarning("[Restoration] Not enough credits!");
                return;
            }

            m_currentCredits -= (int)m_restorationCost;
            m_isRestoring = true;
            m_restorationProgress = 0f;
            m_currentStage = RestorationStage.BodyWork;

            Debug.Log("[Restoration] Starting vehicle restoration...");
        }

        private void UpdateRestoration()
        {
            m_restorationProgress += Time.deltaTime * 0.25f;

            if (m_restorationProgress > 1f)
            {
                AdvanceStage();
                m_restorationProgress = 0f;
            }

            if (m_restorationProgress != null && m_restorationProgress != null)
            {
                m_restorationProgress = Mathf.Clamp01(m_restorationProgress);
            }

            UpdateProgressUI();

            if (m_currentStage == RestorationStage.Complete)
            {
                CompleteRestoration();
            }
        }

        private void AdvanceStage()
        {
            switch (m_currentStage)
            {
                case RestorationStage.BodyWork:
                    m_currentStage = RestorationStage.Paint;
                    break;
                case RestorationStage.Paint:
                    m_currentStage = RestorationStage.Engine;
                    break;
                case RestorationStage.Engine:
                    m_currentStage = RestorationStage.Interior;
                    break;
                case RestorationStage.Interior:
                    m_currentStage = RestorationStage.FinalDetail;
                    break;
                case RestorationStage.FinalDetail:
                    m_currentStage = RestorationStage.Complete;
                    break;
            }
        }

        private void UpdateProgressUI()
        {
            if (m_restorationProgress != null)
            {
                m_restorationProgress = m_restorationProgress;
            }

            if (m_statusText != null)
            {
                string stageName = m_currentStage.ToString().Replace("_", " ");
                m_statusText.text = $"Restoring: {stageName}... {(int)(m_restorationProgress * 100)}%";
            }
        }

        private void CompleteRestoration()
        {
            m_isRestoring = false;

            if (m_vehicleDamage != null)
            {
                m_vehicleDamage.RepairFull();
            }

            var spawner = FindObjectOfType<VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                var wrap = spawner.CurrentVehicle.GetComponent<VehicleWrap>();
                if (wrap != null)
                {
                    wrap.ResetToDefault();
                }

                var tint = spawner.CurrentVehicle.GetComponent<WindowTint>();
                if (tint != null)
                {
                    tint.SetTintLevel(WindowTint.TintLevel.None);
                }

                var neon = spawner.CurrentVehicle.GetComponent<NeonUnderglow>();
                if (neon != null)
                {
                    neon.SetNeonEnabled(false);
                }
            }

            Debug.Log("[Restoration] Vehicle fully restored!");
            ClosePanel();
        }

        public void AddCredits(int amount)
        {
            m_currentCredits += amount;
        }
    }
}
