using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MissionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject m_missionPanel;
    [SerializeField] private Text m_missionTitle;
    [SerializeField] private Text m_missionDescription;
    [SerializeField] private Text m_objectivesText;
    [SerializeField] private Text m_timerText;
    [SerializeField] private Text m_rewardText;

    [Header("Settings")]
    [SerializeField] private bool m_showTimer = true;
    [SerializeField] private bool m_showRewards = true;

    private MissionManager m_missionManager;
    private float m_elapsedTime;

    private void Start()
    {
        m_missionManager = MissionManager.Instance;

        if (m_missionPanel != null)
        {
            m_missionPanel.SetActive(false);
        }

        MissionEvents.OnMissionStarted += OnMissionStarted;
        MissionEvents.OnMissionCompleted += OnMissionCompleted;
        MissionEvents.OnMissionFailed += OnMissionFailed;
    }

    private void OnDestroy()
    {
        MissionEvents.OnMissionStarted -= OnMissionStarted;
        MissionEvents.OnMissionCompleted -= OnMissionCompleted;
        MissionEvents.OnMissionFailed -= OnMissionFailed;
    }

    private void Update()
    {
        if (m_missionManager != null && m_missionManager.IsMissionActive)
        {
            UpdateUI();
        }
    }

    private void OnMissionStarted(MissionData mission)
    {
        if (m_missionPanel != null)
        {
            m_missionPanel.SetActive(true);
        }

        if (m_missionTitle != null)
        {
            m_missionTitle.text = mission.MissionName;
        }

        if (m_missionDescription != null)
        {
            m_missionDescription.text = mission.Description;
        }

        if (m_rewardText != null)
        {
            m_rewardText.text = $"Reward: {mission.CurrencyReward} coins";
            m_rewardText.gameObject.SetActive(m_showRewards);
        }

        m_elapsedTime = 0f;
    }

    private void OnMissionCompleted(MissionData mission)
    {
        if (m_missionPanel != null)
        {
            m_missionPanel.SetActive(false);
        }

        Debug.Log($"[MissionUI] Mission completed: {mission.MissionName}");
    }

    private void OnMissionFailed(MissionData mission, string reason)
    {
        if (m_missionPanel != null)
        {
            m_missionPanel.SetActive(false);
        }

        Debug.Log($"[MissionUI] Mission failed: {mission.MissionName} - {reason}");
    }

    private void UpdateUI()
    {
        var mission = m_missionManager.ActiveMission;
        if (mission == null) return;

        m_elapsedTime += Time.deltaTime;

        if (m_objectivesText != null && mission.Data.Objectives != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Objectives:");

            for (int i = 0; i < mission.Data.Objectives.Length; i++)
            {
                var obj = mission.Data.Objectives[i];
                var progress = mission.GetOrCreateProgress(obj.objectiveId);

                string status = progress.IsComplete ? "[✓]" : "[ ]";
                string line = $"{status} {obj.description}";

                if (obj.type == ObjectiveData.ObjectiveType.DriveDistance ||
                    obj.type == ObjectiveData.ObjectiveType.CollectItems)
                {
                    line += $" ({progress.CurrentCount}/{obj.targetCount})";
                }

                sb.AppendLine(line);
            }

            m_objectivesText.text = sb.ToString();
        }

        if (m_timerText != null && m_showTimer)
        {
            float timeLimit = mission.Data.TimeLimit;
            if (timeLimit > 0)
            {
                float remaining = timeLimit - m_elapsedTime;
                m_timerText.text = $"Time: {remaining:F1}s";
                m_timerText.color = remaining < 10f ? Color.red : Color.white;
            }
            else
            {
                m_timerText.text = $"Time: {m_elapsedTime:F1}s";
            }
        }
    }

    public void ShowPanel(bool show)
    {
        if (m_missionPanel != null)
        {
            m_missionPanel.SetActive(show);
        }
    }
}
