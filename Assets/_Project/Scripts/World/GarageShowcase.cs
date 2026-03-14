using UnityEngine;
using System.Collections.Generic;
using CarSimulator.Vehicle;

namespace CarSimulator.World
{
    public class GarageShowcase : MonoBehaviour
    {
        [Header("Showcase Settings")]
        [SerializeField] private bool m_showcaseMode;
        [SerializeField] private float m_rotationSpeed = 20f;
        [SerializeField] private float m_hoverHeight = 0.5f;
        [SerializeField] private float m_hoverSpeed = 2f;

        [Header("Showcase Elements")]
        [SerializeField] private Light m_showcaseLight;
        [SerializeField] private ParticleSystem m_sparkleParticles;
        [SerializeField] private GameObject m_platform;

        [Header("Camera")]
        [SerializeField] private Camera m_showcaseCamera;
        [SerializeField] private Vector3 m_cameraOffset = new Vector3(0, 2, -6);
        [SerializeField] private float m_cameraRotationSpeed = 30f;
        [SerializeField] private float m_zoomSpeed = 5f;
        [SerializeField] private float m_minZoom = 3f;
        [SerializeField] private float m_maxZoom = 15f;
        [SerializeField] private float m_smoothTime = 0.1f;

        [Header("Camera Presets")]
        [SerializeField] private Vector3 m_frontViewOffset = new Vector3(0, 1.5f, -4f);
        [SerializeField] private Vector3 m_sideViewOffset = new Vector3(-4f, 1.5f, 0);
        [SerializeField] private Vector3 m_rearViewOffset = new Vector3(0, 1.5f, 4f);
        [SerializeField] private Vector3 m_topViewOffset = new Vector3(0, 5f, 0);
        [SerializeField] private Vector3 m_driftViewOffset = new Vector3(-3f, 2f, -3f);

        [Header("Camera Shake")]
        [SerializeField] private float m_shakeIntensity = 0.1f;
        [SerializeField] private float m_shakeDuration = 0.2f;

        [Header("References")]
        [SerializeField] private Transform m_vehicleDisplay;

        private bool m_isShowcaseMode;
        private float m_rotationTimer;

        private float m_currentZoom = 6f;
        private float m_targetZoom = 6f;
        private float m_currentYaw;
        private float m_targetYaw;
        private float m_currentPitch;
        private float m_targetPitch;
        private Vector3 m_cameraVelocity;
        private bool m_isDragging;
        private Vector3 m_lastMousePosition;
        private int m_currentPreset = -1;
        private bool m_isTransitioning;
        private Vector3 m_shakeOffset;

        private enum CameraPreset
        {
            Free,
            Front,
            Side,
            Rear,
            Top,
            Drift
        }

        private CameraPreset m_activePreset = CameraPreset.Free;

        private void Start()
        {
            SetupShowcase();
        }

        private void SetupShowcase()
        {
            if (m_platform == null)
            {
                m_platform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                m_platform.name = "ShowcasePlatform";
                m_platform.transform.position = Vector3.zero;
                m_platform.transform.localScale = new Vector3(5f, 0.2f, 5f);
            }

            if (m_showcaseLight == null)
            {
                GameObject lightObj = new GameObject("ShowcaseLight");
                lightObj.transform.position = new Vector3(0, 8, 0);
                lightObj.transform.LookAt(Vector3.zero);

                m_showcaseLight = lightObj.AddComponent<Light>();
                m_showcaseLight.type = LightType.Spot;
                m_showcaseLight.color = Color.white;
                m_showcaseLight.intensity = 2f;
                m_showcaseLight.range = 15f;
                m_showcaseLight.spotAngle = 45f;
            }

            if (m_sparkleParticles == null)
            {
                CreateSparkles();
            }
        }

        private void CreateSparkles()
        {
            GameObject sparkleObj = new GameObject("Sparkles");
            sparkleObj.transform.position = Vector3.up * 3f;

            m_sparkleParticles = sparkleObj.AddComponent<ParticleSystem>();
            
            var main = m_sparkleParticles.main;
            main.startLifetime = 2f;
            main.startSpeed = 0.5f;
            main.startSize = 0.1f;
            main.maxParticles = 50;

            var emission = m_sparkleParticles.emission;
            emission.rateOverTime = 10;
        }

        private void Update()
        {
            HandleInput();

            if (m_isShowcaseMode)
            {
                UpdateShowcase();
                UpdateCamera();
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_isDragging = true;
                m_lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_isDragging = false;
            }

            if (m_isDragging && !m_isTransitioning)
            {
                Vector3 delta = Input.mousePosition - m_lastMousePosition;
                m_targetYaw += delta.x * 0.5f;
                m_targetPitch = Mathf.Clamp(m_targetPitch - delta.y * 0.3f, -30f, 60f);
                m_lastMousePosition = Input.mousePosition;
                m_activePreset = CameraPreset.Free;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                m_targetZoom = Mathf.Clamp(m_targetZoom - scroll * m_zoomSpeed, m_minZoom, m_maxZoom);
                m_activePreset = CameraPreset.Free;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetCameraPreset(CameraPreset.Front);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SetCameraPreset(CameraPreset.Side);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SetCameraPreset(CameraPreset.Rear);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                SetCameraPreset(CameraPreset.Top);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                SetCameraPreset(CameraPreset.Drift);
            else if (Input.GetKeyDown(KeyCode.Alpha0))
                SetCameraPreset(CameraPreset.Free);
        }

        private void SetCameraPreset(CameraPreset preset)
        {
            m_activePreset = preset;
            m_currentPreset = (int)preset;
            m_isTransitioning = true;

            switch (preset)
            {
                case CameraPreset.Front:
                    m_targetYaw = 0;
                    m_targetPitch = 10;
                    m_targetZoom = 5f;
                    break;
                case CameraPreset.Side:
                    m_targetYaw = 90;
                    m_targetPitch = 5;
                    m_targetZoom = 5f;
                    break;
                case CameraPreset.Rear:
                    m_targetYaw = 180;
                    m_targetPitch = 10;
                    m_targetZoom = 5f;
                    break;
                case CameraPreset.Top:
                    m_targetYaw = 45;
                    m_targetPitch = 70;
                    m_targetZoom = 8f;
                    break;
                case CameraPreset.Drift:
                    m_targetYaw = -45;
                    m_targetPitch = 20;
                    m_targetZoom = 6f;
                    break;
                case CameraPreset.Free:
                    break;
            }

            Invoke(nameof(EndTransition), 0.5f);
        }

        private void EndTransition()
        {
            m_isTransitioning = false;
        }

        private void UpdateCamera()
        {
            if (m_showcaseCamera == null || m_vehicleDisplay == null) return;

            m_currentZoom = Mathf.Lerp(m_currentZoom, m_targetZoom, Time.deltaTime * 5f);
            m_currentYaw = Mathf.Lerp(m_currentYaw, m_targetYaw, Time.deltaTime * 5f);
            m_currentPitch = Mathf.Lerp(m_currentPitch, m_targetPitch, Time.deltaTime * 5f);

            Vector3 baseOffset = m_cameraOffset.normalized * m_currentZoom;

            Quaternion rotation = Quaternion.Euler(m_currentPitch, m_currentYaw, 0);
            Vector3 rotatedOffset = rotation * baseOffset;

            Vector3 targetPos = m_vehicleDisplay.position + rotatedOffset + Vector3.up * m_currentPitch * 0.05f;

            if (m_shakeOffset != Vector3.zero)
            {
                targetPos += m_shakeOffset;
                m_shakeOffset = Vector3.Lerp(m_shakeOffset, Vector3.zero, Time.deltaTime * 10f);
            }

            m_showcaseCamera.transform.position = Vector3.SmoothDamp(
                m_showcaseCamera.transform.position,
                targetPos,
                ref m_cameraVelocity,
                m_smoothTime
            );

            m_showcaseCamera.transform.LookAt(m_vehicleDisplay.position + Vector3.up * 0.5f);
        }

        public void ShakeCamera()
        {
            m_shakeOffset = Random.insideUnitSphere * m_shakeIntensity;
            Invoke(nameof(ClearShake), m_shakeDuration);
        }

        private void ClearShake()
        {
            m_shakeOffset = Vector3.zero;
        }

        public void SetCameraPresetByIndex(int index)
        {
            if (index >= 0 && index <= 5)
            {
                SetCameraPreset((CameraPreset)index);
            }
        }

        public void ToggleShowcaseMode()
        {
            m_isShowcaseMode = !m_isShowcaseMode;

            if (m_isShowcaseMode)
            {
                EnterShowcase();
            }
            else
            {
                ExitShowcase();
            }
        }

        private void EnterShowcase()
        {
            m_rotationTimer = 0f;
            
            if (m_showcaseLight != null)
            {
                m_showcaseLight.intensity = 3f;
            }

            if (m_sparkleParticles != null)
            {
                m_sparkleParticles.Play();
            }

            Debug.Log("[Showcase] Entered showcase mode");
        }

        private void ExitShowcase()
        {
            if (m_showcaseLight != null)
            {
                m_showcaseLight.intensity = 2f;
            }

            if (m_sparkleParticles != null)
            {
                m_sparkleParticles.Stop();
            }

            Debug.Log("[Showcase] Exited showcase mode");
        }

        private void UpdateShowcase()
        {
            if (m_vehicleDisplay == null) return;

            if (m_rotationSpeed > 0 && m_activePreset == CameraPreset.Free)
            {
                m_rotationTimer += Time.deltaTime;
                m_targetYaw += m_rotationSpeed * Time.deltaTime;
            }

            float hover = Mathf.Sin(Time.time * m_hoverSpeed) * m_hoverHeight;
            Vector3 pos = m_vehicleDisplay.position;
            pos.y = hover;
            m_vehicleDisplay.position = pos;
        }

        public void SetDisplayVehicle(Transform vehicle)
        {
            m_vehicleDisplay = vehicle;
            m_targetYaw = 0;
            m_targetPitch = 15;
            m_targetZoom = 6f;
        }

        public void SetShowcaseAngle(float angle)
        {
            m_rotationSpeed = angle;
        }

        public void ToggleAutoRotate()
        {
            m_rotationSpeed = m_rotationSpeed > 0 ? 0 : 20f;
            if (m_rotationSpeed > 0)
            {
                m_activePreset = CameraPreset.Free;
            }
        }

        public void ZoomIn()
        {
            m_targetZoom = Mathf.Clamp(m_targetZoom - 1f, m_minZoom, m_maxZoom);
            m_activePreset = CameraPreset.Free;
        }

        public void ZoomOut()
        {
            m_targetZoom = Mathf.Clamp(m_targetZoom + 1f, m_minZoom, m_maxZoom);
            m_activePreset = CameraPreset.Free;
        }

        public void ResetCamera()
        {
            SetCameraPreset(CameraPreset.Free);
            m_targetYaw = 0;
            m_targetPitch = 15;
            m_targetZoom = 6f;
            m_rotationSpeed = 20f;
        }

        public float GetCurrentZoom() => m_currentZoom;
        public int GetCurrentPreset() => m_currentPreset;
        public bool IsAutoRotating() => m_rotationSpeed > 0;

        public bool IsShowcaseMode => m_isShowcaseMode;
    }
}
