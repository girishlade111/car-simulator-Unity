using UnityEngine;
using CarSimulator.Camera;

namespace CarSimulator.Camera
{
    public class InteriorCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform m_target;
        [SerializeField] private Vector3 m_interiorOffset = new Vector3(0, 1.2f, 0.3f);
        [SerializeField] private float m_rotationSpeed = 5f;

        [Header("View Settings")]
        [SerializeField] private float m_fov = 75f;
        [SerializeField] private bool m_allowLookAround = true;

        [Header("References")]
        [SerializeField] private Camera m_camera;

        private float m_mouseX;
        private float m_mouseY;
        private Vector3 m_currentRotation;

        public Transform Target => m_target;
        public float FOV => m_fov;

        private void Start()
        {
            SetupCamera();
            FindTarget();
        }

        private void SetupCamera()
        {
            if (m_camera == null)
            {
                m_camera = GetComponent<Camera>();
                if (m_camera == null)
                {
                    m_camera = gameObject.AddComponent<Camera>();
                }
            }

            m_camera.fieldOfView = m_fov;
            m_camera.nearClipPlane = 0.1f;
        }

        private void FindTarget()
        {
            if (m_target == null)
            {
                var spawner = FindObjectOfType<Vehicle.VehicleSpawner>();
                if (spawner?.CurrentVehicle != null)
                {
                    m_target = spawner.CurrentVehicle.transform;
                }
            }

            if (m_target != null)
            {
                transform.SetParent(m_target);
                transform.localPosition = m_interiorOffset;
                transform.localRotation = Quaternion.identity;
            }
        }

        private void Update()
        {
            if (!m_allowLookAround) return;

            HandleLookAround();
        }

        private void HandleLookAround()
        {
            if (Input.GetMouseButton(1))
            {
                m_mouseX += Input.GetAxis("Mouse X") * m_rotationSpeed;
                m_mouseY -= Input.GetAxis("Mouse Y") * m_rotationSpeed;
                m_mouseY = Mathf.Clamp(m_mouseY, -60f, 60f);
            }
            else
            {
                m_mouseX = Mathf.Lerp(m_mouseX, 0f, Time.deltaTime * 3f);
                m_mouseY = Mathf.Lerp(m_mouseY, 0f, Time.deltaTime * 3f);
            }
        }

        private void LateUpdate()
        {
            if (m_target == null)
            {
                FindTarget();
                return;
            }

            UpdateCameraTransform();
        }

        private void UpdateCameraTransform()
        {
            Vector3 baseRotation = m_target.eulerAngles;
            
            transform.localPosition = m_interiorOffset;
            
            Quaternion lookRotation = Quaternion.Euler(baseRotation.x + m_mouseY, baseRotation.y + m_mouseX, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        public void SetFOV(float fov)
        {
            m_fov = fov;
            if (m_camera != null)
            {
                m_camera.fieldOfView = fov;
            }
        }

        public void SetTarget(Transform target)
        {
            m_target = target;
            if (target != null)
            {
                transform.SetParent(target);
                transform.localPosition = m_interiorOffset;
            }
        }

        public void ResetLook()
        {
            m_mouseX = 0f;
            m_mouseY = 0f;
        }
    }
}
