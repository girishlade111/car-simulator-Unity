using UnityEngine;

namespace CarSimulator.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class EngineAudio : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private bool m_enableEngineSound = true;
        [SerializeField] private float m_basePitch = 0.5f;
        [SerializeField] private float m_maxPitch = 2f;
        [SerializeField] private float m_pitchMultiplier = 1.5f;
        [SerializeField] private float m_volumeMultiplier = 1f;

        [Header("RPM Settings")]
        [SerializeField] private float m_idleRPM = 800f;
        [SerializeField] private float m_maxRPM = 7000f;

        [Header("References")]
        [SerializeField] private Vehicle.VehiclePhysics m_vehiclePhysics;

        private AudioSource m_audioSource;
        private float m_currentRPM;
        private float m_targetPitch;

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
            m_audioSource.maxDistance = 50f;
        }

        private void Start()
        {
            if (m_vehiclePhysics == null)
            {
                m_vehiclePhysics = GetComponent<Vehicle.VehiclePhysics>();
            }

            if (m_enableEngineSound && m_audioSource != null)
            {
                m_audioSource.Play();
            }
        }

        private void Update()
        {
            if (m_vehiclePhysics == null || !m_enableEngineSound) return;

            UpdateRPM();
            UpdateEngineSound();
        }

        private void UpdateRPM()
        {
            float speed = m_vehiclePhysics.CurrentSpeed;
            float speedRatio = speed / m_vehiclePhysics.MaxSpeed;

            float targetRPM = Mathf.Lerp(m_idleRPM, m_maxRPM, speedRatio);
            m_currentRPM = Mathf.Lerp(m_currentRPM, targetRPM, Time.deltaTime * 5f);
        }

        private void UpdateEngineSound()
        {
            if (m_audioSource == null) return;

            float speed = m_vehiclePhysics != null ? m_vehiclePhysics.CurrentSpeed : 0f;
            float speedRatio = speed / 180f;

            m_targetPitch = Mathf.Lerp(m_basePitch, m_maxPitch, speedRatio * m_pitchMultiplier);
            m_targetPitch = Mathf.Clamp(m_targetPitch, m_basePitch, m_maxPitch);

            m_audioSource.pitch = Mathf.Lerp(m_audioSource.pitch, m_targetPitch, Time.deltaTime * 10f);

            float targetVolume = Mathf.Lerp(0.1f, 0.5f, speedRatio) * m_volumeMultiplier;
            m_audioSource.volume = Mathf.Lerp(m_audioSource.volume, targetVolume, Time.deltaTime * 5f);
        }

        public void SetEngineSoundEnabled(bool enabled)
        {
            m_enableEngineSound = enabled;
            
            if (m_audioSource != null)
            {
                if (enabled)
                {
                    if (!m_audioSource.isPlaying)
                    {
                        m_audioSource.Play();
                    }
                }
                else
                {
                    m_audioSource.Stop();
                }
            }
        }

        public void SetVolume(float volume)
        {
            m_volumeMultiplier = Mathf.Clamp01(volume);
        }

        public float CurrentRPM => m_currentRPM;
    }
}
