using UnityEngine;

namespace CarSimulator.Vehicle
{
    public class VehicleMirror : MonoBehaviour
    {
        [Header("Mirror Settings")]
        [SerializeField] private MirrorType m_mirrorType = MirrorType.Side;
        [SerializeField] private float m_reflectionDistance = 50f;
        [SerializeField] private float m_fieldOfView = 60f;

        [Header("Render Texture")]
        [SerializeField] private RenderTexture m_renderTexture;
        [SerializeField] private int m_textureWidth = 512;
        [SerializeField] private int m_textureHeight = 256;

        [Header("References")]
        [SerializeField] private Camera m_mirrorCamera;
        [SerializeField] private Renderer m_mirrorRenderer;

        public enum MirrorType
        {
            Side,
            Rear,
            Front
        }

        private void Start()
        {
            SetupMirror();
        }

        private void SetupMirror()
        {
            if (m_renderTexture == null)
            {
                m_renderTexture = new RenderTexture(m_textureWidth, m_textureHeight, 16);
                m_renderTexture.Create();
            }

            if (m_mirrorCamera == null)
            {
                CreateMirrorCamera();
            }

            if (m_mirrorRenderer == null)
            {
                m_mirrorRenderer = GetComponent<Renderer>();
            }

            if (m_mirrorRenderer != null && m_renderTexture != null)
            {
                m_mirrorRenderer.material.mainTexture = m_renderTexture;
            }
        }

        private void CreateMirrorCamera()
        {
            GameObject cameraObj = new GameObject($"MirrorCamera_{m_mirrorType}");
            cameraObj.transform.SetParent(transform);
            cameraObj.transform.localPosition = Vector3.zero;
            cameraObj.transform.localRotation = Quaternion.identity;

            m_mirrorCamera = cameraObj.AddComponent<Camera>();
            m_mirrorCamera.targetTexture = m_renderTexture;
            m_mirrorCamera.fieldOfView = m_fieldOfView;
            m_mirrorCamera.nearClipPlane = 0.1f;
            m_mirrorCamera.farClipPlane = m_reflectionDistance;
            m_mirrorCamera.enabled = false;

            SetMirrorRotation();
        }

        private void SetMirrorRotation()
        {
            switch (m_mirrorType)
            {
                case MirrorType.Side:
                    m_mirrorCamera.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case MirrorType.Rear:
                    m_mirrorCamera.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    break;
                case MirrorType.Front:
                    m_mirrorCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }

        private void LateUpdate()
        {
            if (m_mirrorCamera == null) return;

            Transform vehicleTransform = transform;
            while (vehicleTransform.parent != null && vehicleTransform.parent.GetComponent<VehiclePhysics>() == null)
            {
                vehicleTransform = vehicleTransform.parent;
            }

            Vector3 mirrorPos = vehicleTransform.TransformPoint(transform.localPosition);
            Vector3 forwardDir = transform.forward;
            Vector3 upDir = transform.up;

            m_mirrorCamera.transform.position = mirrorPos;
            
            Vector3 lookDir = vehicleTransform.forward;
            if (m_mirrorType == MirrorType.Side)
            {
                lookDir = -vehicleTransform.right;
            }
            else if (m_mirrorType == MirrorType.Rear)
            {
                lookDir = -vehicleTransform.forward;
            }

            m_mirrorCamera.transform.rotation = Quaternion.LookRotation(lookDir, upDir);
            m_mirrorCamera.transform.Rotate(0, 180, 0);
        }

        public void SetMirrorType(MirrorType type)
        {
            m_mirrorType = type;
            SetMirrorRotation();
        }

        public void SetFOV(float fov)
        {
            m_fieldOfView = fov;
            if (m_mirrorCamera != null)
            {
                m_mirrorCamera.fieldOfView = fov;
            }
        }
    }
}
