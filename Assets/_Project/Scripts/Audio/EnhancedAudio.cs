using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Audio
{
    public class DynamicMusicSystem : MonoBehaviour
    {
        public static DynamicMusicSystem Instance { get; private set; }

        [Header("Music Layers")]
        [SerializeField] private AudioSource m_baseLayer;
        [SerializeField] private AudioSource m_actionLayer;
        [SerializeField] private AudioSource m_ambientLayer;
        [SerializeField] private AudioSource m_cityLayer;

        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_transitionSpeed = 2f;
        [SerializeField] private float m_actionThreshold = 0.7f;

        [Header("Clips")]
        [SerializeField] private AudioClip m_calmMusic;
        [SerializeField] private AudioClip m_actionMusic;
        [SerializeField] private AudioClip m_cityAmbient;
        [SerializeField] private AudioClip m_natureAmbient;

        private float m_currentIntensity;
        private Transform m_playerTransform;
        private float m_playerSpeed;

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
            FindPlayer();
            InitializeAudioSources();
            PlayMusic();
        }

        private void Update()
        {
            if (!m_enabled) return;

            UpdatePlayerSpeed();
            UpdateMusicIntensity();
            UpdateAudioLayers();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void InitializeAudioSources()
        {
            if (m_baseLayer == null) m_baseLayer = CreateAudioSource("BaseLayer");
            if (m_actionLayer == null) m_actionLayer = CreateAudioSource("ActionLayer");
            if (m_ambientLayer == null) m_ambientLayer = CreateAudioSource("AmbientLayer");
            if (m_cityLayer == null) m_cityLayer = CreateAudioSource("CityLayer");

            m_baseLayer.loop = true;
            m_actionLayer.loop = true;
            m_ambientLayer.loop = true;
            m_cityLayer.loop = true;
        }

        private AudioSource CreateAudioSource(string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform);
            return obj.AddComponent<AudioSource>();
        }

        private void PlayMusic()
        {
            if (m_calmMusic != null)
            {
                m_baseLayer.clip = m_calmMusic;
                m_baseLayer.Play();
            }

            if (m_natureAmbient != null)
            {
                m_ambientLayer.clip = m_natureAmbient;
                m_ambientLayer.Play();
            }

            if (m_cityAmbient != null)
            {
                m_cityLayer.clip = m_cityAmbient;
                m_cityLayer.Play();
            }
        }

        private void UpdatePlayerSpeed()
        {
            if (m_playerTransform == null) return;

            var vehicle = m_playerTransform.GetComponent<Vehicle.VehicleController>();
            if (vehicle != null)
            {
                m_playerSpeed = vehicle.GetCurrentSpeed() / 200f;
            }
        }

        private void UpdateMusicIntensity()
        {
            m_currentIntensity = Mathf.Lerp(m_currentIntensity, m_playerSpeed, Time.deltaTime * m_transitionSpeed);
            m_currentIntensity = Mathf.Clamp01(m_currentIntensity);
        }

        private void UpdateAudioLayers()
        {
            float baseVolume = 1f - m_currentIntensity;
            float actionVolume = m_currentIntensity;

            if (m_currentIntensity > m_actionThreshold && !m_actionLayer.isPlaying)
            {
                if (m_actionMusic != null)
                {
                    m_actionLayer.clip = m_actionMusic;
                    m_actionLayer.Play();
                }
            }
            else if (m_currentIntensity < m_actionThreshold && m_actionLayer.isPlaying)
            {
                m_actionLayer.Stop();
            }

            m_baseLayer.volume = Mathf.Lerp(m_baseLayer.volume, baseVolume, Time.deltaTime * m_transitionSpeed);
            m_actionLayer.volume = Mathf.Lerp(m_actionLayer.volume, actionVolume, Time.deltaTime * m_transitionSpeed);
        }

        public void SetIntensity(float intensity)
        {
            m_currentIntensity = Mathf.Clamp01(intensity);
        }

        public void TriggerActionMusic()
        {
            m_currentIntensity = 1f;
        }

        public void TriggerCalmMusic()
        {
            m_currentIntensity = 0f;
        }
    }

    public class WeatherAudio : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource m_rainSource;
        [SerializeField] private AudioSource m_windSource;
        [SerializeField] private AudioSource m_thunderSource;

        [Header("Clips")]
        [SerializeField] private AudioClip m_lightRain;
        [SerializeField] private AudioClip m_heavyRain;
        [SerializeField] private AudioClip m_wind;
        [SerializeField] private AudioClip m_thunder;

        [Header("Settings")]
        [SerializeField] private float m_transitionSpeed = 1f;

        private float m_currentRainIntensity;
        private float m_targetRainIntensity;

        private void Start()
        {
            InitializeSources();
        }

        private void InitializeSources()
        {
            if (m_rainSource == null)
            {
                m_rainSource = CreateAudioSource("Rain");
                m_rainSource.loop = true;
            }

            if (m_windSource == null)
            {
                m_windSource = CreateAudioSource("Wind");
                m_windSource.loop = true;
            }

            if (m_thunderSource == null)
            {
                m_thunderSource = CreateAudioSource("Thunder");
            }

            if (m_lightRain != null)
            {
                m_rainSource.clip = m_lightRain;
                m_rainSource.Play();
            }

            if (m_wind != null)
            {
                m_windSource.clip = m_wind;
                m_windSource.Play();
            }
        }

        private AudioSource CreateAudioSource(string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform);
            return obj.AddComponent<AudioSource>();
        }

        private void Update()
        {
            UpdateWeatherAudio();
        }

        private void UpdateWeatherAudio()
        {
            m_currentRainIntensity = Mathf.Lerp(m_currentRainIntensity, m_targetRainIntensity, Time.deltaTime * m_transitionSpeed);

            m_rainSource.volume = m_currentRainIntensity;
            m_windSource.volume = 0.3f + m_currentRainIntensity * 0.5f;
        }

        public void SetRainIntensity(float intensity)
        {
            m_targetRainIntensity = Mathf.Clamp01(intensity);

            if (intensity > 0.7f && m_heavyRain != null && m_rainSource.clip != m_heavyRain)
            {
                m_rainSource.clip = m_heavyRain;
            }
            else if (intensity <= 0.7f && m_lightRain != null && m_rainSource.clip != m_lightRain)
            {
                m_rainSource.clip = m_lightRain;
            }
        }

        public void PlayThunder()
        {
            if (m_thunder != null)
            {
                m_thunderSource.PlayOneShot(m_thunder);
            }
        }
    }

    public class FootstepAudio : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private float m_stepInterval = 0.5f;
        [SerializeField] private float m_runInterval = 0.3f;

        [Header("Audio")]
        [SerializeField] private AudioSource m_footstepSource;
        [SerializeField] private AudioClip[] m_footstepClips;
        [SerializeField] private AudioClip[] m_runClips;

        [Header("Surface Detection")]
        [SerializeField] private LayerMask m_groundLayer;

        private float m_stepTimer;
        private bool m_isMoving;
        private bool m_isRunning;
        private Transform m_playerTransform;

        private void Start()
        {
            FindPlayer();

            if (m_footstepSource == null)
            {
                m_footstepSource = gameObject.AddComponent<AudioSource>();
                m_footstepSource.spatialBlend = 1f;
            }
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (!m_enabled || m_playerTransform == null) return;

            UpdateMovementState();

            if (m_isMoving)
            {
                m_stepTimer += Time.deltaTime;

                float interval = m_isRunning ? m_runInterval : m_stepInterval;

                if (m_stepTimer >= interval)
                {
                    m_stepTimer = 0;
                    PlayFootstep();
                }
            }
        }

        private void UpdateMovementState()
        {
            var vehicle = m_playerTransform.GetComponent<Vehicle.VehicleController>();
            if (vehicle != null)
            {
                m_isMoving = vehicle.GetCurrentSpeed() > 5f;
                m_isRunning = vehicle.GetCurrentSpeed() > 100f;
            }
        }

        private void PlayFootstep()
        {
            AudioClip[] clips = m_isRunning ? m_runClips : m_footstepClips;

            if (clips == null || clips.Length == 0) return;

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            m_footstepSource.PlayOneShot(clip);
        }
    }

    public class AmbientWorldAudio : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource m_daySource;
        [SerializeField] private AudioSource m_nightSource;

        [Header("Clips")]
        [SerializeField] private AudioClip m_dayAmbient;
        [SerializeField] private AudioClip m_nightAmbient;

        [Header("Settings")]
        [SerializeField] private float m_transitionSpeed = 0.5f;

        private float m_currentDayAmount = 1f;

        private void Start()
        {
            InitializeSources();
        }

        private void InitializeSources()
        {
            if (m_daySource == null)
            {
                m_daySource = CreateAudioSource("DayAmbient");
                m_daySource.loop = true;
            }

            if (m_nightSource == null)
            {
                m_nightSource = CreateAudioSource("NightAmbient");
                m_nightSource.loop = true;
            }

            if (m_dayAmbient != null)
            {
                m_daySource.clip = m_dayAmbient;
                m_daySource.Play();
            }

            if (m_nightAmbient != null)
            {
                m_nightSource.clip = m_nightAmbient;
                m_nightSource.Play();
            }
        }

        private AudioSource CreateAudioSource(string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(transform);
            return obj.AddComponent<AudioSource>();
        }

        private void Update()
        {
            UpdateDayNightAudio();
        }

        private void UpdateDayNightAudio()
        {
            var timeOfDay = FindObjectOfType<World.EnhancedTimeOfDay>();
            if (timeOfDay == null) return;

            float currentTime = timeOfDay.CurrentTime;
            float targetDayAmount = (currentTime > 6f && currentTime < 20f) ? 1f : 0f;

            m_currentDayAmount = Mathf.Lerp(m_currentDayAmount, targetDayAmount, Time.deltaTime * m_transitionSpeed);

            m_daySource.volume = m_currentDayAmount;
            m_nightSource.volume = 1f - m_currentDayAmount;
        }
    }
}
