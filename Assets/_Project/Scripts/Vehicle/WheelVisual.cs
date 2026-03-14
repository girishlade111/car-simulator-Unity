using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class WheelVisual : MonoBehaviour
    {
        [SerializeField] private WheelCollider m_wheelCollider;
        [SerializeField] private Transform m_wheelMesh;

        private void FixedUpdate()
        {
            if (m_wheelCollider == null || m_wheelMesh == null) return;

            Vector3 position;
            Quaternion rotation;
            m_wheelCollider.GetWorldPose(out position, out rotation);

            m_wheelMesh.position = position;
            m_wheelMesh.rotation = rotation;
        }
    }
}
