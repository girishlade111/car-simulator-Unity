using UnityEngine;

namespace CarSimulator.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class TireScreechAudio : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private bool m_enableScreech = true;
        [SerializeField] private float m_screechThreshold = 0.4f;
        [SerializeField] private float m_volumeMultiplier = 1f;
        [SerializeField] private float m_fadeSpeed = 5f;

        [Header("References")]
        [SerializeField] private Vehicle.VehiclePhysics m_vehiclePhysics;

        private AudioSource m_audioSource;
        private float m_currentVolume;
        private bool m_isScreeching;

        private void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
            SetupAudioSource();
        }

        private void SetupAudioSource()
        {
            m_audioSource.playOnAwake = false;
            m_audioSource.loop = true;
            m_audioSource.spatialBlend = 1f;
            m_audioSource.minDistance = 2f;
            m_audioSource.maxDistance = 30f;
            m_audioSource.volume = 0f;
        }

        private void Start()
        {
            if (m_vehiclePhysics == null)
            {
                m_vehiclePhysics = GetComponent<Vehicle.VehiclePhysics>();
            }

            if (m_enableScreech && m_audioSource != null)
            {
                m_audioSource.Play();
            }
        }

        private void Update()
        {
            if (m_vehiclePhysics == null || !m_enableScreech) return;

            UpdateScreech();
        }

        private void UpdateScreech()
        {
            bool shouldScreech = m_vehiclePhysics.IsDrifting || IsWheelSlipping();

            float targetVolume = shouldScreech ? 0.6f * m_volumeMultiplier : 0f;
            m_currentVolume = Mathf.Lerp(m_currentVolume, targetVolume, Time.deltaTime * m_fadeSpeed);

            if (m_audioSource != null)
            {
                m_audioSource.volume = m_currentVolume;
            }

            m_isScreeching = shouldScreech;
        }

        private bool IsWheelSlipping()
        {
            var wheels = GetComponentsInChildren<WheelCollider>();
            foreach (var wheel in wheels)
            {
                WheelHit hit;
                if (wheel.GetGroundHit(out hit))
                {
                    float slip = Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip);
                    if (slip > m_screechThreshold)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsScreeching => m_isScreeching;
        public float CurrentVolume => m_currentVolume;

        public void SetScreechEnabled(bool enabled)
        {
            m_enableScreech = enabled;
            if (!enabled && m_audioSource != null)
            {
                m_audioSource.Stop();
            }
        }
    }
}
