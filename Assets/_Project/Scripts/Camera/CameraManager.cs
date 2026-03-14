using UnityEngine;

namespace CarSimulator.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public enum CameraView
        {
            ThirdPerson,
            FirstPerson,
            Interior,
            Hood,
            Orbit
        }

        public static CameraManager Instance { get; private set; }

        [Header("Cameras")]
        [SerializeField] private FollowCamera m_thirdPersonCamera;
        [SerializeField] private Camera m_firstPersonCamera;
        [SerializeField] private InteriorCamera m_interiorCamera;

        [Header("Settings")]
        [SerializeField] private CameraView m_currentView = CameraView.ThirdPerson;
        [SerializeField] private KeyCode m_switchKey = KeyCode.C;

        private Camera m_activeCamera;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SetupCameras();
            SetCameraView(m_currentView);
        }

        private void SetupCameras()
        {
            if (m_thirdPersonCamera == null)
            {
                var existing = FindObjectOfType<FollowCamera>();
                if (existing != null)
                {
                    m_thirdPersonCamera = existing;
                }
            }

            if (m_firstPersonCamera == null)
            {
                GameObject fpCamObj = new GameObject("FirstPersonCamera");
                fpCamObj.transform.SetParent(transform);
                m_firstPersonCamera = fpCamObj.AddComponent<Camera>();
                m_firstPersonCamera.enabled = false;
            }

            if (m_interiorCamera == null)
            {
                GameObject intCamObj = new GameObject("InteriorCamera");
                intCamObj.transform.SetParent(transform);
                m_interiorCamera = intCamObj.AddComponent<InteriorCamera>();
                m_interiorCamera.enabled = false;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(m_switchKey))
            {
                CycleCameraView();
            }
        }

        public void SetCameraView(CameraView view)
        {
            DisableAllCameras();
            m_currentView = view;

            switch (view)
            {
                case CameraView.ThirdPerson:
                    if (m_thirdPersonCamera != null)
                    {
                        m_thirdPersonCamera.enabled = true;
                        m_activeCamera = m_thirdPersonCamera.GetComponent<Camera>();
                    }
                    break;

                case CameraView.FirstPerson:
                    if (m_firstPersonCamera != null)
                    {
                        m_firstPersonCamera.enabled = true;
                        m_activeCamera = m_firstPersonCamera;
                        SetupFirstPersonCamera();
                    }
                    break;

                case CameraView.Interior:
                    if (m_interiorCamera != null)
                    {
                        m_interiorCamera.enabled = true;
                        m_activeCamera = m_interiorCamera.GetComponent<Camera>();
                    }
                    break;

                case CameraView.Hood:
                    SetupHoodCamera();
                    break;

                case CameraView.Orbit:
                    if (m_thirdPersonCamera != null)
                    {
                        m_thirdPersonCamera.enabled = true;
                        m_activeCamera = m_thirdPersonCamera.GetComponent<Camera>();
                    }
                    break;
            }
        }

        private void SetupFirstPersonCamera()
        {
            var spawner = FindObjectOfType<Vehicle.VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                Transform vehicle = spawner.CurrentVehicle.transform;
                m_firstPersonCamera.transform.SetParent(vehicle);
                m_firstPersonCamera.transform.localPosition = new Vector3(0, 1.3f, 0.5f);
                m_firstPersonCamera.transform.localRotation = Quaternion.identity;
                m_firstPersonCamera.fieldOfView = 75f;
            }
        }

        private void SetupHoodCamera()
        {
            var spawner = FindObjectOfType<Vehicle.VehicleSpawner>();
            if (spawner?.CurrentVehicle != null)
            {
                Transform vehicle = spawner.CurrentVehicle.transform;
                m_firstPersonCamera.transform.SetParent(vehicle);
                m_firstPersonCamera.transform.localPosition = new Vector3(0, 1.5f, 1.2f);
                m_firstPersonCamera.transform.localRotation = Quaternion.Euler(10, 0, 0);
                m_firstPersonCamera.enabled = true;
                m_activeCamera = m_firstPersonCamera;
            }
        }

        private void DisableAllCameras()
        {
            if (m_thirdPersonCamera != null)
                m_thirdPersonCamera.enabled = false;
            if (m_firstPersonCamera != null)
                m_firstPersonCamera.enabled = false;
            if (m_interiorCamera != null)
                m_interiorCamera.enabled = false;
        }

        public void CycleCameraView()
        {
            int nextIndex = ((int)m_currentView + 1) % System.Enum.GetValues(typeof(CameraView)).Length;
            SetCameraView((CameraView)nextIndex);
            Debug.Log($"[CameraManager] Switched to {m_currentView} view");
        }

        public CameraView CurrentView => m_currentView;
    }
}
