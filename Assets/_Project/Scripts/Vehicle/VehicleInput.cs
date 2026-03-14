using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class VehicleInput : MonoBehaviour
    {
        [Header("Input Axes")]
        [SerializeField] private string m_horizontalAxis = "Horizontal";
        [SerializeField] private string m_verticalAxis = "Vertical";
        [SerializeField] private KeyCode m_brakeKey = KeyCode.Space;
        [SerializeField] private KeyCode m_handbrakeKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode m_resetKey = KeyCode.R;

        public float SteerInput { get; private set; }
        public float ThrottleInput { get; private set; }
        public bool IsBraking { get; private set; }
        public bool IsHandbraking { get; private set; }
        public bool IsResetPressed { get; private set; }

        private void Update()
        {
            GatherInput();
        }

        private void GatherInput()
        {
            SteerInput = Input.GetAxisRaw(m_horizontalAxis);
            ThrottleInput = Input.GetAxisRaw(m_verticalAxis);
            IsBraking = Input.GetKey(m_brakeKey) || (ThrottleInput < 0f && GetSpeed() > 1f);
            IsHandbraking = Input.GetKey(m_handbrakeKey);
            IsResetPressed = Input.GetKeyDown(m_resetKey);
        }

        private float GetSpeed()
        {
            var rb = GetComponent<Rigidbody>();
            return rb != null ? rb.velocity.magnitude * 3.6f : 0f;
        }
    }
}
