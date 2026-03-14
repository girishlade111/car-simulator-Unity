using UnityEngine;
using UnityEngine.UI;

namespace CarSimulator.UI
{
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [Header("Entry UI")]
        [SerializeField] private Text m_rankText;
        [SerializeField] private Text m_nameText;
        [SerializeField] private Text m_valueText;
        [SerializeField] private Image m_background;
        [SerializeField] private Color m_firstPlaceColor = new Color(1f, 0.84f, 0f);
        [SerializeField] private Color m_secondPlaceColor = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color m_thirdPlaceColor = new Color(0.8f, 0.5f, 0.2f);

        public void Setup(int rank, string name, string value)
        {
            if (m_rankText != null)
            {
                m_rankText.text = rank.ToString();
                
                if (rank == 1)
                    m_rankText.color = m_firstPlaceColor;
                else if (rank == 2)
                    m_rankText.color = m_secondPlaceColor;
                else if (rank == 3)
                    m_rankText.color = m_thirdPlaceColor;
            }

            if (m_nameText != null)
                m_nameText.text = name;

            if (m_valueText != null)
                m_valueText.text = value;

            if (m_background != null)
            {
                if (rank == 1)
                    m_background.color = new Color(1f, 0.84f, 0f, 0.2f);
                else if (rank % 2 == 0)
                    m_background.color = new Color(0, 0, 0, 0.1f);
                else
                    m_background.color = new Color(0, 0, 0, 0.05f);
            }
        }
    }
}
