using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CarSimulator.UI
{
    public class EmergencyDispatchUI : MonoBehaviour
    {
        public static EmergencyDispatchUI Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject m_mainPanel;
        [SerializeField] private GameObject m_callPanel;
        [SerializeField] private GameObject m_activeCallsPanel;

        [Header("Buttons")]
        [SerializeField] private Button m_fireButton;
        [SerializeField] private Button m_medicalButton;
        [SerializeField] private Button m_policeButton;
        [SerializeField] private Button m_accidentButton;
        [SerializeField] private Button m_closeButton;

        [Header("Info Display")]
        [SerializeField] private Text m_statusText;
        [SerializeField] private Text m_activeUnitsText;
        [SerializeField] private Text m_pendingCallsText;

        [Header("Map")]
        [SerializeField] private Image m_mapImage;
        [SerializeField] private Transform m_playerMarker;

        [Header("Settings")]
        [SerializeField] private KeyCode m_dispatchKey = KeyCode.K;

        private bool m_isOpen;

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
            SetupButtons();
            HidePanel();
        }

        private void SetupButtons()
        {
            if (m_fireButton != null)
                m_fireButton.onClick.AddListener(OnFireClicked);
            if (m_medicalButton != null)
                m_medicalButton.onClick.AddListener(OnMedicalClicked);
            if (m_policeButton != null)
                m_policeButton.onClick.AddListener(OnPoliceClicked);
            if (m_accidentButton != null)
                m_accidentButton.onClick.AddListener(OnAccidentClicked);
            if (m_closeButton != null)
                m_closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void Update()
        {
            if (Input.GetKeyDown(m_dispatchKey))
            {
                TogglePanel();
            }

            UpdateStatusDisplay();
        }

        private void TogglePanel()
        {
            if (m_isOpen)
                HidePanel();
            else
                ShowPanel();
        }

        public void ShowPanel()
        {
            if (m_mainPanel != null)
            {
                m_mainPanel.SetActive(true);
                m_isOpen = true;
            }
        }

        public void HidePanel()
        {
            if (m_mainPanel != null)
            {
                m_mainPanel.SetActive(false);
                m_isOpen = false;
            }
        }

        private void OnFireClicked()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var emergency = FindObjectOfType<AI.EmergencyResponseSystem>();
                emergency?.RegisterFireAt(player.transform.position);
                ShowCallConfirmation("Fire department dispatched!");
            }
        }

        private void OnMedicalClicked()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var emergency = FindObjectOfType<AI.EmergencyResponseSystem>();
                emergency?.RegisterMedicalEmergency(player.transform.position);
                ShowCallConfirmation("Ambulance dispatched!");
            }
        }

        private void OnPoliceClicked()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var emergency = FindObjectOfType<AI.EmergencyResponseSystem>();
                emergency?.RegisterCrime(player.transform.position);
                ShowCallConfirmation("Police dispatched!");
            }
        }

        private void OnAccidentClicked()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var emergency = FindObjectOfType<AI.EmergencyResponseSystem>();
                emergency?.RegisterAccident(player.transform.position);
                ShowCallConfirmation("Emergency services dispatched!");
            }
        }

        private void OnCloseClicked()
        {
            HidePanel();
        }

        private void ShowCallConfirmation(string message)
        {
            if (m_statusText != null)
            {
                m_statusText.text = message;
            }
        }

        private void UpdateStatusDisplay()
        {
            var emergency = FindObjectOfType<AI.EmergencyResponseSystem>();

            if (m_activeUnitsText != null)
            {
                int activeUnits = emergency != null ? emergency.GetActiveUnitCount() : 0;
                m_activeUnitsText.text = $"Active Units: {activeUnits}";
            }

            if (m_pendingCallsText != null)
            {
                int pendingCalls = emergency != null ? emergency.GetPendingCallCount() : 0;
                m_pendingCallsText.text = $"Pending Calls: {pendingCalls}";
            }
        }

        public void SetStatus(string status)
        {
            if (m_statusText != null)
            {
                m_statusText.text = status;
            }
        }
    }

    public class EmergencyHotline : MonoBehaviour
    {
        [Header("Hotline Settings")]
        [SerializeField] private KeyCode m_callKey = KeyCode.H;
        [SerializeField] private bool m_showPrompt = true;

        [Header("UI")]
        [SerializeField] private GameObject m_promptPanel;
        [SerializeField] private Text m_promptText;

        private bool m_isNearPhone;

        private void Start()
        {
            if (m_promptPanel != null)
                m_promptPanel.SetActive(false);
        }

        private void Update()
        {
            if (m_isNearPhone && Input.GetKeyDown(m_callKey))
            {
                OpenDispatchUI();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_isNearPhone = true;
                ShowPrompt();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                m_isNearPhone = false;
                HidePrompt();
            }
        }

        private void ShowPrompt()
        {
            if (m_showPrompt && m_promptPanel != null)
            {
                m_promptPanel.SetActive(true);
                if (m_promptText != null)
                {
                    m_promptText.text = "Press H to call emergency services";
                }
            }
        }

        private void HidePrompt()
        {
            if (m_promptPanel != null)
            {
                m_promptPanel.SetActive(false);
            }
        }

        private void OpenDispatchUI()
        {
            EmergencyDispatchUI.Instance?.ShowPanel();
        }
    }
}
