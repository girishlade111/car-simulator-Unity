using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace CarSimulator.UI
{
    public class MissionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject m_missionPanel;
        [SerializeField] private Text m_titleText;
        [SerializeField] private Text m_objectivesText;
        [SerializeField] private Text m_timerText;

        private MissionManager m_missionManager;

        private void Start()
        {
            m_missionManager = MissionManager.Instance;

            if (m_missionPanel != null)
                m_missionPanel.SetActive(false);
        }

        private void Update()
        {
            if (m_missionManager != null && m_missionManager.IsMissionActive)
            {
                UpdateUI();
            }
            else
            {
                if (m_missionPanel != null)
                    m_missionPanel.SetActive(false);
            }
        }

        private void UpdateUI()
        {
            var mission = m_missionManager.ActiveMission;
            if (mission == null) return;

            if (m_missionPanel != null)
                m_missionPanel.SetActive(true);

            if (m_titleText != null)
                m_titleText.text = mission.Data.missionName;

            if (m_objectivesText != null && mission.Data.objectives != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Objectives:");

                foreach (var obj in mission.Data.objectives)
                {
                    var progress = mission.GetOrCreateProgress(obj.objectiveId);
                    string status = progress.IsComplete ? "[✓]" : "[ ]";
                    string line = $"{status} {obj.description}";

                    if (obj.type == Missions.ObjectiveData.ObjectiveType.DriveDistance)
                    {
                        line += $" ({progress.CurrentCount:F0}/{obj.targetCount})";
                    }

                    sb.AppendLine(line);
                }

                m_objectivesText.text = sb.ToString();
            }

            if (m_timerText != null && mission.Data.timeLimit > 0)
            {
                float remaining = mission.Data.timeLimit - mission.ElapsedTime;
                m_timerText.text = $"Time: {remaining:F1}s";
                m_timerText.color = remaining < 10f ? Color.red : Color.white;
            }
        }

        public void ShowPanel(bool show)
        {
            if (m_missionPanel != null)
                m_missionPanel.SetActive(show);
        }
    }
}
