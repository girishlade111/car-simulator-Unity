using UnityEngine;
using CarSimulator.Runtime;

namespace CarSimulator.Vehicle
{
    public class VehicleController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private VehicleInput m_input;
        [SerializeField] private VehiclePhysics m_physics;

        [Header("Respawn")]
        [SerializeField] private float m_respawnHeight = -10f;
        [SerializeField] private float m_resetDelay = 2f;

        private float m_respawnTimer;

        private void Update()
        {
            if (m_input == null) return;

            HandleReset();
            HandlePauseInput();
        }

        private void FixedUpdate()
        {
            if (m_physics == null) return;

            CheckRespawn();
        }

        private void HandleReset()
        {
            if (m_input.IsResetPressed && m_physics != null)
            {
                m_physics.ResetVehicle();
            }
        }

        private void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TogglePause();
                }
            }
        }

        private void CheckRespawn()
        {
            if (transform.position.y < m_respawnHeight)
            {
                m_respawnTimer += Time.deltaTime;
                if (m_respawnTimer >= m_resetDelay)
                {
                    m_physics.ResetVehicle();
                    m_respawnTimer = 0f;
                }
            }
            else
            {
                m_respawnTimer = 0f;
            }
        }
    }
}
