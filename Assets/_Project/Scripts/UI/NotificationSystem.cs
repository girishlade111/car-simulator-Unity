using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CarSimulator.UI
{
    public class NotificationSystem : MonoBehaviour
    {
        public static NotificationSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float m_displayDuration = 3f;
        [SerializeField] private float m_fadeSpeed = 2f;
        [SerializeField] private int m_maxNotifications = 5;

        [Header("UI References")]
        [SerializeField] private Transform m_notificationContainer;
        [SerializeField] private GameObject m_notificationPrefab;

        [Header("Colors")]
        [SerializeField] private Color m_infoColor = Color.white;
        [SerializeField] private Color m_successColor = Color.green;
        [SerializeField] private Color m_warningColor = Color.yellow;
        [SerializeField] private Color m_errorColor = Color.red;
        [SerializeField] private Color m_questColor = new Color(0.8f, 0.6f, 0.2f);

        private List<Notification> m_activeNotifications = new List<Notification>();

        private class Notification
        {
            public GameObject gameObject;
            public Text text;
            public Image background;
            public float timer;
            public float fadeTimer;
            public NotificationType type;
        }

        public enum NotificationType
        {
            Info,
            Success,
            Warning,
            Error,
            Quest
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
            UpdateNotifications();
        }

        public void ShowNotification(string message, NotificationType type = NotificationType.Info)
        {
            if (m_activeNotifications.Count >= m_maxNotifications)
            {
                RemoveOldestNotification();
            }

            GameObject notifObj = Instantiate(m_notificationPrefab, m_notificationContainer);
            notifObj.transform.SetAsFirstSibling();

            Notification notification = new Notification
            {
                gameObject = notifObj,
                text = notifObj.GetComponentInChildren<Text>(),
                background = notifObj.GetComponentInChildren<Image>(),
                timer = m_displayDuration,
                type = type
            };

            if (notification.text != null)
            {
                notification.text.text = message;
                notification.text.color = GetColorForType(type);
            }

            if (notification.background != null)
            {
                notification.background.color = GetColorForType(type) * 0.3f;
            }

            m_activeNotifications.Add(notification);
        }

        public void ShowInfo(string message) => ShowNotification(message, NotificationType.Info);
        public void ShowSuccess(string message) => ShowNotification(message, NotificationType.Success);
        public void ShowWarning(string message) => ShowNotification(message, NotificationType.Warning);
        public void ShowError(string message) => ShowNotification(message, NotificationType.Error);
        public void ShowQuest(string message) => ShowNotification(message, NotificationType.Quest);

        private void UpdateNotifications()
        {
            for (int i = m_activeNotifications.Count - 1; i >= 0; i--)
            {
                Notification notif = m_activeNotifications[i];

                notif.timer -= Time.deltaTime;

                if (notif.timer <= 1f)
                {
                    float alpha = notif.timer;
                    if (notif.text != null)
                    {
                        Color c = notif.text.color;
                        c.a = alpha;
                        notif.text.color = c;
                    }
                    if (notif.background != null)
                    {
                        Color c = notif.background.color;
                        c.a = alpha * 0.3f;
                        notif.background.color = c;
                    }
                }

                if (notif.timer <= 0)
                {
                    Destroy(notif.gameObject);
                    m_activeNotifications.RemoveAt(i);
                }
            }
        }

        private void RemoveOldestNotification()
        {
            if (m_activeNotifications.Count > 0)
            {
                Destroy(m_activeNotifications[0].gameObject);
                m_activeNotifications.RemoveAt(0);
            }
        }

        private Color GetColorForType(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Success: return m_successColor;
                case NotificationType.Warning: return m_warningColor;
                case NotificationType.Error: return m_errorColor;
                case NotificationType.Quest: return m_questColor;
                default: return m_infoColor;
            }
        }
    }

    public class MinimapController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_mapSize = 100f;
        [SerializeField] private float m_updateRate = 0.1f;

        [Header("References")]
        [SerializeField] private RawImage m_mapImage;
        [SerializeField] private Transform m_playerMarker;
        [SerializeField] private Transform m_mapCamera;

        [Header("Markers")]
        [SerializeField] private GameObject[] m_waypointMarkers;
        [SerializeField] private GameObject[] m_buildingMarkers;

        private float m_updateTimer;
        private Transform m_playerTransform;

        private void Start()
        {
            FindPlayer();
        }

        private void Update()
        {
            if (!m_enabled) return;

            m_updateTimer += Time.deltaTime;
            if (m_updateTimer >= m_updateRate)
            {
                m_updateTimer = 0;
                UpdateMap();
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void UpdateMap()
        {
            if (m_playerTransform == null || m_mapCamera == null) return;

            m_mapCamera.position = new Vector3(
                m_playerTransform.position.x,
                m_mapSize,
                m_playerTransform.position.z
            );

            m_mapCamera.rotation = Quaternion.Euler(90, 0, 0);
        }

        public void SetMapSize(float size)
        {
            m_mapSize = size;
        }

        public void ToggleMinimap()
        {
            m_enabled = !m_enabled;
            m_mapImage?.gameObject.SetActive(m_enabled);
        }
    }

    public class DialogueUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject m_panel;
        [SerializeField] private Text m_npcNameText;
        [SerializeField] private Text m_dialogueText;
        [SerializeField] private Button m_continueButton;
        [SerializeField] private Transform m_optionsContainer;

        [Header("Settings")]
        [SerializeField] private float m_textSpeed = 0.05f;

        private string[] m_dialogueLines;
        private int m_currentLine;
        private bool m_isTyping;
        private System.Action m_onComplete;

        private void Start()
        {
            if (m_panel != null)
            {
                m_panel.SetActive(false);
            }
        }

        public void StartDialogue(string npcName, string[] lines, System.Action onComplete = null)
        {
            m_dialogueLines = lines;
            m_currentLine = 0;
            m_onComplete = onComplete;

            if (m_npcNameText != null)
            {
                m_npcNameText.text = npcName;
            }

            ShowCurrentLine();

            if (m_panel != null)
            {
                m_panel.SetActive(true);
            }
        }

        private void ShowCurrentLine()
        {
            if (m_currentLine >= m_dialogueLines.Length)
            {
                EndDialogue();
                return;
            }

            StopAllCoroutines();
            StartCoroutine(TypeText(m_dialogueLines[m_currentLine]));
        }

        private System.Collections.IEnumerator TypeText(string text)
        {
            m_isTyping = true;

            if (m_dialogueText != null)
            {
                m_dialogueText.text = "";

                foreach (char c in text.ToCharArray())
                {
                    m_dialogueText.text += c;
                    yield return new WaitForSeconds(m_textSpeed);
                }
            }

            m_isTyping = false;
        }

        public void OnContinueClicked()
        {
            if (m_isTyping)
            {
                if (m_dialogueText != null)
                {
                    m_dialogueText.text = m_dialogueLines[m_currentLine];
                }
                m_isTyping = false;
                StopAllCoroutines();
                return;
            }

            m_currentLine++;
            ShowCurrentLine();
        }

        private void EndDialogue()
        {
            if (m_panel != null)
            {
                m_panel.SetActive(false);
            }

            m_onComplete?.Invoke();
        }

        public void CloseDialogue()
        {
            m_currentLine = m_dialogueLines.Length;
            EndDialogue();
        }
    }

    public class InteractionPrompt : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_showDistance = 3f;
        [SerializeField] private float m_fadeSpeed = 5f;

        [Header("UI References")]
        [SerializeField] private GameObject m_promptPanel;
        [SerializeField] private Text m_promptText;
        [SerializeField] private Image m_promptIcon;

        [Header("Default Prompt")]
        [SerializeField] private string m_defaultPrompt = "Press E to interact";

        private Transform m_playerTransform;
        private Transform m_interactableTarget;
        private string m_currentPrompt;
        private CanvasGroup m_canvasGroup;

        private void Start()
        {
            FindPlayer();
            m_canvasGroup = m_promptPanel?.GetComponent<CanvasGroup>();

            if (m_promptPanel != null)
            {
                m_promptPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (!m_enabled) return;

            UpdatePrompt();
            HandleInput();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void UpdatePrompt()
        {
            if (m_playerTransform == null) return;

            FindNearestInteractable();

            if (m_interactableTarget != null)
            {
                float dist = Vector3.Distance(transform.position, m_interactableTarget.position);

                if (dist <= m_showDistance)
                {
                    ShowPrompt();
                    return;
                }
            }

            HidePrompt();
        }

        private void FindNearestInteractable()
        {
            var interactables = FindObjectsOfType<Interactable>();
            float nearestDist = float.MaxValue;
            Interactable nearest = null;

            foreach (var interactable in interactables)
            {
                float dist = Vector3.Distance(transform.position, interactable.transform.position);
                if (dist < nearestDist && dist <= m_showDistance * 2)
                {
                    nearestDist = dist;
                    nearest = interactable;
                }
            }

            m_interactableTarget = nearest?.transform;
            m_currentPrompt = nearest != null ? nearest.GetInteractionPrompt() : m_defaultPrompt;
        }

        private void HandleInput()
        {
            if (m_interactableTarget != null && Input.GetKeyDown(KeyCode.E))
            {
                var interactable = m_interactableTarget.GetComponent<Interactable>();
                interactable?.Interact();
            }
        }

        private void ShowPrompt()
        {
            if (m_promptPanel != null && !m_promptPanel.activeSelf)
            {
                m_promptPanel.SetActive(true);
            }

            if (m_promptText != null)
            {
                m_promptText.text = m_currentPrompt;
            }

            if (m_canvasGroup != null)
            {
                m_canvasGroup.alpha = Mathf.Lerp(m_canvasGroup.alpha, 1f, Time.deltaTime * m_fadeSpeed);
            }
        }

        private void HidePrompt()
        {
            if (m_canvasGroup != null)
            {
                m_canvasGroup.alpha = Mathf.Lerp(m_canvasGroup.alpha, 0f, Time.deltaTime * m_fadeSpeed);

                if (m_canvasGroup.alpha < 0.01f && m_promptPanel?.activeSelf == true)
                {
                    m_promptPanel.SetActive(false);
                }
            }
        }
    }

    public class Interactable : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private string m_interactionPrompt = "Press E to interact";
        [SerializeField] private float m_interactionRange = 3f;
        [SerializeField] private bool m_canInteract = true;

        [Header("Events")]
        [SerializeField] private UnityEngine.Events.UnityEvent m_onInteract;

        public void Interact()
        {
            if (m_canInteract)
            {
                m_onInteract?.Invoke();
                Debug.Log($"[Interactable] Interacted with {gameObject.name}");
            }
        }

        public string GetInteractionPrompt() => m_interactionPrompt;
        public float GetInteractionRange() => m_interactionRange;
    }
}
