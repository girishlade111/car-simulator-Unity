using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text m_speedText;
    [SerializeField] private Text m_controlsText;
    [SerializeField] private VehicleController m_vehicle;

    [Header("Settings")]
    [SerializeField] private bool m_showControls = true;

    private void Start()
    {
        if (m_vehicle == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_vehicle = player.GetComponent<VehicleController>();
            }
        }

        if (m_controlsText != null)
        {
            m_controlsText.gameObject.SetActive(m_showControls);
        }
    }

    private void Update()
    {
        if (m_vehicle != null && m_speedText != null)
        {
            float speed = m_vehicle.CurrentSpeed;
            m_speedText.text = $"{speed:F0} km/h";
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (m_controlsText != null)
            {
                m_controlsText.gameObject.SetActive(!m_controlsText.gameObject.activeSelf);
            }
        }
    }
}
