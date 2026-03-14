using UnityEngine;
using System.Collections.Generic;

namespace CarSimulator.Audio
{
    public class CarHorn : MonoBehaviour
    {
        [Header("Horn Settings")]
        [SerializeField] private KeyCode m_hornKey = KeyCode.H;
        [SerializeField] private bool m_hornEnabled = true;

        [Header("Horn Types")]
        [SerializeField] private AudioClip[] m_hornSounds;
        [SerializeField] private int m_currentHornIndex;
        [SerializeField] private float m_hornVolume = 0.8f;

        [Header("References")]
        [SerializeField] private AudioSource m_audioSource;

        private float m_hornDuration;
        private bool m_isHonking;

        private void Start()
        {
            SetupAudioSource();
            
            if (m_hornSounds == null || m_hornSounds.Length == 0)
            {
                GenerateProceduralHorn();
            }
        }

        private void SetupAudioSource()
        {
            if (m_audioSource == null)
            {
                m_audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            m_audioSource.loop = true;
            m_audioSource.playOnAwake = false;
            m_audioSource.spatialBlend = 1f;
            m_audioSource.minDistance = 5f;
            m_audioSource.maxDistance = 100f;
            m_audioSource.volume = m_hornVolume;
        }

        private void GenerateProceduralHorn()
        {
            // Generate a simple horn tone
            m_hornSounds = new AudioClip[3];
            
            float[] frequencies = { 440f, 520f, 660f };
            string[] names = { "Low", "Mid", "High" };
            
            for (int i = 0; i < 3; i++)
            {
                m_hornSounds[i] = GenerateTone(frequencies[i], 1f, names[i]);
            }
        }

        private AudioClip GenerateTone(float frequency, float duration, string name)
        {
            int sampleRate = 44100;
            int sampleLength = (int)(sampleRate * duration);
            float[] samples = new float[sampleLength];
            
            for (int i = 0; i < sampleLength; i++)
            {
                float t = (float)i / sampleRate;
                samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * 0.5f;
                samples[i] += Mathf.Sin(2 * Mathf.PI * frequency * 1.5f * t) * 0.25f;
                samples[i] = Mathf.Clamp(samples[i], -1f, 1f);
            }
            
            AudioClip clip = AudioClip.Create(name, sampleLength, 1, sampleRate, false);
            clip.SetData(samples, 0);
            
            return clip;
        }

        private void Update()
        {
            if (!m_hornEnabled) return;

            if (Input.GetKeyDown(m_hornKey))
            {
                StartHonking();
            }
            else if (Input.GetKeyUp(m_hornKey))
            {
                StopHonking();
            }
        }

        public void StartHonking()
        {
            if (m_isHonking || m_hornSounds == null || m_hornSounds.Length == 0) return;

            m_isHonking = true;
            
            if (m_currentHornIndex < m_hornSounds.Length)
            {
                m_audioSource.clip = m_hornSounds[m_currentHornIndex];
            }
            
            m_audioSource.Play();
        }

        public void StopHonking()
        {
            if (!m_isHonking) return;

            m_isHonking = false;
            m_audioSource.Stop();
        }

        public void Honk()
        {
            StartHonking();
            Invoke(nameof(StopHonking), 0.3f);
        }

        public void SetHornType(int index)
        {
            if (index >= 0 && index < m_hornSounds.Length)
            {
                m_currentHornIndex = index;
            }
        }

        public void CycleHorn()
        {
            m_currentHornIndex = (m_currentHornIndex + 1) % m_hornSounds.Length;
        }

        public void SetVolume(float volume)
        {
            m_hornVolume = Mathf.Clamp01(volume);
            if (m_audioSource != null)
            {
                m_audioSource.volume = m_hornVolume;
            }
        }
    }
}
