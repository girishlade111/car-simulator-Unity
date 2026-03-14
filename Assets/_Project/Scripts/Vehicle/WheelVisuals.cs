using UnityEngine;

public class WheelVisuals : MonoBehaviour
{
    [SerializeField] private WheelCollider m_wheelCollider;
    [SerializeField] private Transform m_wheelMesh;

    private void FixedUpdate()
    {
        if (m_wheelCollider == null || m_wheelMesh == null) return;

        Vector3 pos;
        Quaternion rot;
        m_wheelCollider.GetWorldPose(out pos, out rot);

        m_wheelMesh.position = pos;
        m_wheelMesh.rotation = rot;
    }
}
