using UnityEngine;
using UnityEngine.UI;

namespace CarSimulator.UI
{
    public class SpeedDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehicle;
        [SerializeField] private Text m_speedText;

        [Header("Settings")]
        [SerializeField] private string m_format = "{0:F0} km/h";
        [SerializeField] private bool m_showWhenNoVehicle = true;

        private void Start()
        {
            if (m_vehicle == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    m_vehicle = player.GetComponent<VehiclePhysics>();
                }
            }
        }

        private void Update()
        {
            if (m_vehicle != null && m_speedText != null)
            {
                m_speedText.text = string.Format(m_format, m_vehicle.CurrentSpeed);
            }
            else if (m_showWhenNoVehicle && m_speedText != null)
            {
                m_speedText.text = string.Format(m_format, 0f);
            }
        }
    }
}
