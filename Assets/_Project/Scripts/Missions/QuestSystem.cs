using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Missions
{
    [System.Serializable]
    public class QuestData
    {
        public string questId;
        public string questTitle;
        [TextArea] public string questDescription;
        public QuestType type;
        public QuestCategory category;
        public int difficulty;

        [Header("Objectives")]
        public QuestObjective[] objectives;
        public bool isMainQuest;

        [Header("Rewards")]
        public int creditsReward;
        public int experienceReward;
        public string itemReward;

        [Header("Progress")]
        public bool isCompleted;
        public bool isActive;
        public float progress;

        [Header("Timing")]
        public float timeLimit;
        public float timeRemaining;
        public bool hasTimeLimit;

        public enum QuestType
        {
            Talk,
            Gather,
            Escort,
            Defeat,
            Explore,
            Transport,
            Stunt,
            Survival
        }

        public enum QuestCategory
        {
            Story,
            Side,
            Character,
            Location,
            Event
        }
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveId;
        public string description;
        public ObjectiveType type;
        public int targetCount;
        public int currentCount;
        public bool isOptional;

        [Header("Target")]
        public string targetTag;
        public Vector3 targetLocation;
        public float targetRadius;

        public enum ObjectiveType
        {
            ReachLocation,
            TalkToNPC,
            CollectItem,
            DefeatTarget,
            EscortTarget,
            PerformStunt,
            SurviveTime,
            DriveDistance
        }
    }

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool m_enableQuests = true;
        [SerializeField] private int m_maxActiveQuests = 5;

        [Header("Quest Database")]
        [SerializeField] private QuestData[] m_availableQuests;
        [SerializeField] private QuestData[] m_dailyQuests;

        [Header("Active Quests")]
        [SerializeField] private List<QuestData> m_activeQuests = new List<QuestData>();
        [SerializeField] private QuestData m_currentQuest;

        [Header("Quest Progress")]
        [SerializeField] private List<string> m_completedQuests = new List<string>();

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
            if (!m_enableQuests) return;
            UpdateQuestTimers();
            CheckQuestProgress();
        }

        public void StartQuest(QuestData quest)
        {
            if (m_activeQuests.Count >= m_maxActiveQuests)
            {
                Debug.Log("[QuestManager] Max active quests reached");
                return;
            }

            quest.isActive = true;
            quest.progress = 0f;
            quest.timeRemaining = quest.timeLimit;

            m_activeQuests.Add(quest);

            if (m_currentQuest == null)
            {
                m_currentQuest = quest;
            }

            Debug.Log($"[QuestManager] Started quest: {quest.questTitle}");
        }

        public void CompleteQuest(QuestData quest)
        {
            if (!quest.isCompleted)
            {
                quest.isCompleted = true;
                quest.isActive = false;
                m_completedQuests.Add(quest.questId);

                int credits = PlayerPrefs.GetInt("PlayerCredits", 0);
                PlayerPrefs.SetInt("PlayerCredits", credits + quest.creditsReward);
                PlayerPrefs.Save();

                Debug.Log($"[QuestManager] Completed quest: {quest.questTitle} - Reward: ${quest.creditsReward}");

                m_activeQuests.Remove(quest);

                if (m_currentQuest == quest)
                {
                    m_currentQuest = m_activeQuests.Count > 0 ? m_activeQuests[0] : null;
                }
            }
        }

        public void AbandonQuest(QuestData quest)
        {
            quest.isActive = false;
            m_activeQuests.Remove(quest);

            if (m_currentQuest == quest)
            {
                m_currentQuest = m_activeQuests.Count > 0 ? m_activeQuests[0] : null;
            }

            Debug.Log($"[QuestManager] Abandoned quest: {quest.questTitle}");
        }

        private void UpdateQuestTimers()
        {
            foreach (var quest in m_activeQuests)
            {
                if (quest.hasTimeLimit && quest.timeRemaining > 0)
                {
                    quest.timeRemaining -= Time.deltaTime;

                    if (quest.timeRemaining <= 0)
                    {
                        FailQuest(quest);
                    }
                }
            }
        }

        private void FailQuest(QuestData quest)
        {
            Debug.Log($"[QuestManager] Quest failed: {quest.questTitle}");
            quest.isActive = false;
            m_activeQuests.Remove(quest);
        }

        private void CheckQuestProgress()
        {
            foreach (var quest in m_activeQuests)
            {
                if (quest.objectives == null) continue;

                int completedObjectives = 0;

                foreach (var objective in quest.objectives)
                {
                    if (objective.currentCount >= objective.targetCount)
                    {
                        completedObjectives++;
                    }
                }

                quest.progress = (float)completedObjectives / quest.objectives.Length;

                if (quest.progress >= 1f)
                {
                    CompleteQuest(quest);
                }
            }
        }

        public void UpdateObjectiveProgress(string objectiveId, int amount = 1)
        {
            foreach (var quest in m_activeQuests)
            {
                if (quest.objectives == null) continue;

                foreach (var objective in quest.objectives)
                {
                    if (objective.objectiveId == objectiveId)
                    {
                        objective.currentCount = Mathf.Min(objective.currentCount + amount, objective.targetCount);
                        Debug.Log($"[QuestManager] Objective {objectiveId}: {objective.currentCount}/{objective.targetCount}");
                    }
                }
            }
        }

        public void UpdateObjectiveByType(QuestObjective.ObjectiveType type, Vector3 location)
        {
            foreach (var quest in m_activeQuests)
            {
                if (quest.objectives == null) continue;

                foreach (var objective in quest.objectives)
                {
                    if (objective.type == type)
                    {
                        if (type == QuestObjective.ObjectiveType.ReachLocation)
                        {
                            float dist = Vector3.Distance(location, objective.targetLocation);
                            if (dist <= objective.targetRadius)
                            {
                                objective.currentCount = Mathf.Min(objective.currentCount + 1, objective.targetCount);
                            }
                        }
                    }
                }
            }
        }

        public QuestData[] GetAvailableQuests()
        {
            return m_availableQuests;
        }

        public QuestData[] GetActiveQuests()
        {
            return m_activeQuests.ToArray();
        }

        public QuestData GetCurrentQuest() => m_currentQuest;

        public bool HasCompletedQuest(string questId)
        {
            return m_completedQuests.Contains(questId);
        }
    }

    public class QuestGiver : MonoBehaviour
    {
        [Header("Quest")]
        [SerializeField] private QuestData[] m_availableQuests;
        [SerializeField] private bool m_giveOnInteract = true;

        [Header("Interaction")]
        [SerializeField] private string m_greetingText = "Can you help me?";
        [SerializeField] private string m_completionText = "Thank you!";
        [SerializeField] private float m_interactionRange = 3f;

        [Header("Visual")]
        [SerializeField] private GameObject m_questMarker;
        [SerializeField] private Color m_availableColor = Color.yellow;
        [SerializeField] private Color m_completedColor = Color.green;

        private Transform m_playerTransform;
        private bool m_hasQuestAvailable;

        private void Start()
        {
            FindPlayer();
            UpdateQuestMarker();
        }

        private void Update()
        {
            if (m_playerTransform == null) return;

            float dist = Vector3.Distance(transform.position, m_playerTransform.position);
            if (dist <= m_interactionRange && m_hasQuestAvailable && m_giveOnInteract)
            {
                ShowQuestPrompt();
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

        private void ShowQuestPrompt()
        {
            Debug.Log($"[QuestGiver] {m_greetingText}");
        }

        private void UpdateQuestMarker()
        {
            if (m_questMarker == null) return;
            if (QuestManager.Instance == null) return;

            foreach (var quest in m_availableQuests)
            {
                if (!QuestManager.Instance.HasCompletedQuest(quest.questId))
                {
                    m_hasQuestAvailable = true;
                    m_questMarker.SetActive(true);
                    return;
                }
            }

            m_hasQuestAvailable = false;
            m_questMarker.SetActive(false);
        }

        public void Interact()
        {
            if (QuestManager.Instance == null) return;

            foreach (var quest in m_availableQuests)
            {
                if (!QuestManager.Instance.HasCompletedQuest(quest.questId) && !quest.isActive)
                {
                    QuestManager.Instance.StartQuest(quest);
                    Debug.Log($"[QuestGiver] Given quest: {quest.questTitle}");
                    return;
                }
            }

            Debug.Log($"[QuestGiver] {m_completionText}");
        }
    }
}
