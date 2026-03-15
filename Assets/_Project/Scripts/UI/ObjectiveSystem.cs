using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CarSimulator.UI
{
    public class ObjectiveSystem : MonoBehaviour
    {
        public static ObjectiveSystem Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject m_objectivePanel;
        [SerializeField] private Text m_objectiveText;
        [SerializeField] private Text m_timerText;
        [SerializeField] private Slider m_progressSlider;

        [Header("Settings")]
        [SerializeField] private bool m_showObjectives = true;
        [SerializeField] private float m_displayDuration = 5f;
        [SerializeField] private float m_fadeSpeed = 2f;

        [Header("Current Objective")]
        [SerializeField] private string m_currentObjective = "";
        [SerializeField] private string m_subObjective = "";
        [SerializeField] private float m_timer;
        [SerializeField] private float m_progress;

        private bool m_isVisible;
        private float m_displayTimer;

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
            if (m_objectivePanel != null)
            {
                m_objectivePanel.SetActive(false);
            }
        }

        private void Update()
        {
            UpdateTimer();
            UpdateVisibility();
            UpdateUI();
        }

        public void SetObjective(string objective)
        {
            m_currentObjective = objective;
            m_subObjective = "";
            m_displayTimer = m_displayDuration;
            ShowObjective();
        }

        public void SetObjective(string mainObjective, string subObjective)
        {
            m_currentObjective = mainObjective;
            m_subObjective = subObjective;
            m_displayTimer = m_displayDuration;
            ShowObjective();
        }

        public void SetObjectiveWithTimer(string objective, float timer)
        {
            m_currentObjective = objective;
            m_timer = timer;
            m_displayTimer = m_displayDuration;
            ShowObjective();
        }

        public void SetObjectiveWithProgress(string objective, float progress)
        {
            m_currentObjective = objective;
            m_progress = progress;
            m_displayTimer = m_displayDuration;
            ShowObjective();
        }

        public void SetProgress(float progress)
        {
            m_progress = Mathf.Clamp01(progress);
        }

        public void SetTimer(float time)
        {
            m_timer = time;
        }

        public void ClearObjective()
        {
            m_currentObjective = "";
            m_subObjective = "";
            m_progress = 0;
            m_timer = 0;
            HideObjective();
        }

        private void ShowObjective()
        {
            if (!m_showObjectives) return;

            m_isVisible = true;

            if (m_objectivePanel != null)
            {
                m_objectivePanel.SetActive(true);
            }
        }

        private void HideObjective()
        {
            m_isVisible = false;

            if (m_objectivePanel != null)
            {
                m_objectivePanel.SetActive(false);
            }
        }

        private void UpdateTimer()
        {
            if (m_timer > 0)
            {
                m_timer -= Time.deltaTime;
                m_displayTimer = m_displayDuration;

                if (m_timer <= 0)
                {
                    OnTimerExpired();
                }
            }
        }

        private void OnTimerExpired()
        {
            Debug.Log("[ObjectiveSystem] Timer expired!");
            NotificationSystem.Instance?.ShowWarning("Objective failed!");
        }

        private void UpdateVisibility()
        {
            if (string.IsNullOrEmpty(m_currentObjective)) return;

            if (m_displayTimer > 0)
            {
                m_displayTimer -= Time.deltaTime;
            }
            else if (m_isVisible)
            {
                HideObjective();
            }
        }

        private void UpdateUI()
        {
            if (m_objectiveText != null)
            {
                string text = m_currentObjective;
                if (!string.IsNullOrEmpty(m_subObjective))
                {
                    text += $"\n<size=80%>{m_subObjective}</size>";
                }
                m_objectiveText.text = text;
            }

            if (m_timerText != null)
            {
                if (m_timer > 0)
                {
                    m_timerText.gameObject.SetActive(true);
                    int minutes = Mathf.FloorToInt(m_timer / 60f);
                    int seconds = Mathf.FloorToInt(m_timer % 60f);
                    m_timerText.text = $"{minutes:00}:{seconds:00}";
                }
                else
                {
                    m_timerText.gameObject.SetActive(false);
                }
            }

            if (m_progressSlider != null)
            {
                if (m_progress > 0)
                {
                    m_progressSlider.gameObject.SetActive(true);
                    m_progressSlider.value = m_progress;
                }
                else
                {
                    m_progressSlider.gameObject.SetActive(false);
                }
            }
        }

        public void CompleteObjective()
        {
            Debug.Log($"[ObjectiveSystem] Completed: {m_currentObjective}");
            NotificationSystem.Instance?.ShowSuccess($"Objective Complete: {m_currentObjective}");
            ClearObjective();
        }
    }

    public class InteractionPromptUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject m_promptPanel;
        [SerializeField] private Text m_promptText;
        [SerializeField] private Image m_keyIcon;

        [Header("Settings")]
        [SerializeField] private KeyCode m_interactionKey = KeyCode.E;
        [SerializeField] private float m_fadeSpeed = 5f;

        [Header("Animation")]
        [SerializeField] private bool m_pulseAnimation = true;

        private CanvasGroup m_canvasGroup;
        private float m_targetAlpha = 0f;

        private void Start()
        {
            m_canvasGroup = m_promptPanel?.GetComponent<CanvasGroup>();
            
            if (m_promptPanel != null)
            {
                m_promptPanel.SetActive(false);
            }
        }

        private void Update()
        {
            UpdatePrompt();
            UpdateAnimation();
        }

        private void UpdatePrompt()
        {
            var interactionManager = World.WorldInteractionManager.Instance;
            if (interactionManager == null) return;

            var closest = interactionManager.GetClosestInteractable();
            
            if (closest != null)
            {
                ShowPrompt(closest.GetInteractionPrompt());
            }
            else
            {
                HidePrompt();
            }
        }

        private void UpdateAnimation()
        {
            if (m_canvasGroup == null) return;

            m_canvasGroup.alpha = Mathf.Lerp(m_canvasGroup.alpha, m_targetAlpha, Time.deltaTime * m_fadeSpeed);

            if (m_pulseAnimation && m_targetAlpha > 0.5f)
            {
                float pulse = Mathf.Sin(Time.time * 3f) * 0.1f + 1f;
                if (m_keyIcon != null)
                {
                    m_keyIcon.transform.localScale = Vector3.one * pulse;
                }
            }
        }

        public void ShowPrompt(string text)
        {
            if (m_promptPanel != null && !m_promptPanel.activeSelf)
            {
                m_promptPanel.SetActive(true);
            }

            if (m_promptText != null)
            {
                m_promptText.text = $"[{m_interactionKey}] {text}";
            }

            m_targetAlpha = 1f;
        }

        public void HidePrompt()
        {
            m_targetAlpha = 0f;

            if (m_canvasGroup != null && m_canvasGroup.alpha < 0.01f && m_promptPanel?.activeSelf == true)
            {
                m_promptPanel.SetActive(false);
            }
        }
    }
}
