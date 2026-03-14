using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Audio
{
    [RequireComponent(typeof(VehiclePhysics))]
    public class VehicleAudio : MonoBehaviour
    {
        [Header("Engine")]
        [SerializeField] private AudioSource m_engineSource;
        [SerializeField] private AudioClip m_engineClip;
        [SerializeField] private float m_minEnginePitch = 0.5f;
        [SerializeField] private float m_maxEnginePitch = 2f;
        [SerializeField] private float m_engineVolume = 0.8f;

        [Header("Tire")]
        [SerializeField] private AudioSource m_tireSource;
        [SerializeField] private AudioClip m_tireClip;
        [SerializeField] private float m_tireVolume = 0.5f;

        [Header("Horn")]
        [SerializeField] private AudioSource m_hornSource;
        [SerializeField] private AudioClip m_hornClip;
        [SerializeField] private float m_hornVolume = 0.7f;

        private VehiclePhysics m_physics;
        private float m_currentSpeed;

        private void Awake()
        {
            m_physics = GetComponent<VehiclePhysics>();
            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            if (m_engineSource == null)
            {
                m_engineSource = CreateAudioSource("EngineSource");
                m_engineSource.clip = m_engineClip;
                m_engineSource.loop = true;
                m_engineSource.playOnAwake = true;
                if (m_engineClip != null) m_engineSource.Play();
            }

            if (m_tireSource == null)
            {
                m_tireSource = CreateAudioSource("TireSource");
                m_tireSource.clip = m_tireClip;
                m_tireSource.loop = true;
                m_tireSource.volume = 0f;
            }

            if (m_hornSource == null)
            {
                m_hornSource = CreateAudioSource("HornSource");
                m_hornSource.clip = m_hornClip;
                m_hornSource.loop = true;
                m_hornSource.volume = m_hornVolume;
            }
        }

        private AudioSource CreateAudioSource(string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.spatialBlend = 0f;
            return source;
        }

        private void Update()
        {
            if (m_physics == null) return;

            m_currentSpeed = m_physics.CurrentSpeed;
            UpdateEngineSound();
            UpdateTireSound();
            HandleHornInput();
        }

        private void UpdateEngineSound()
        {
            if (m_engineSource == null || m_engineClip == null) return;

            float speedRatio = m_currentSpeed / m_physics.MaxSpeed;
            float targetPitch = Mathf.Lerp(m_minEnginePitch, m_maxEnginePitch, speedRatio);
            m_engineSource.pitch = Mathf.Lerp(m_engineSource.pitch, targetPitch, Time.deltaTime * 2f);
            m_engineSource.volume = m_engineVolume;
        }

        private void UpdateTireSound()
        {
            if (m_tireSource == null || m_tireClip == null) return;

            bool shouldPlay = m_currentSpeed > 10f && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            float targetVolume = shouldPlay ? m_tireVolume : 0f;
            m_tireSource.volume = Mathf.Lerp(m_tireSource.volume, targetVolume, Time.deltaTime * 5f);

            if (shouldPlay && !m_tireSource.isPlaying)
            {
                m_tireSource.Play();
            }
            else if (!shouldPlay && m_tireSource.isPlaying)
            {
                m_tireSource.Stop();
            }
        }

        private void HandleHornInput()
        {
            if (Input.GetKeyDown(KeyCode.H) && m_hornSource != null && m_hornClip != null)
            {
                if (m_hornSource.isPlaying)
                    m_hornSource.Stop();
                else
                    m_hornSource.Play();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_currentSpeed < 5f) return;
            // Play collision SFX if available
        }

        public void SetEngineVolume(float volume) => m_engineVolume = Mathf.Clamp01(volume);
        public void SetTireVolume(float volume) => m_tireVolume = Mathf.Clamp01(volume);
        public void SetHornVolume(float volume) => m_hornVolume = Mathf.Clamp01(volume);
    }
}
