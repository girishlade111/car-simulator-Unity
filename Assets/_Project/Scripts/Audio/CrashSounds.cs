using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Audio
{
    public class CrashSounds : MonoBehaviour
    {
        [Header("Crash Settings")]
        [SerializeField] private bool m_enableCrashSounds = true;
        [SerializeField] private float m_minImpactSpeed = 3f;
        [SerializeField] private float m_crashVolumeScale = 0.1f;

        [Header("Sound Variations")]
        [SerializeField] private AudioClip[] m_crashSounds;
        [SerializeField] private AudioClip[] m_scrapesSounds;
        [SerializeField] private AudioClip[] m_impactSounds;

        [Header("References")]
        [SerializeField] private AudioSource m_audioSource;

        private void Start()
        {
            SetupAudioSource();
            GenerateCrashSounds();
        }

        private void SetupAudioSource()
        {
            if (m_audioSource == null)
            {
                m_audioSource = gameObject.AddComponent<AudioSource>();
            }
            m_audioSource.spatialBlend = 1f;
            m_audioSource.minDistance = 2f;
            m_audioSource.maxDistance = 50f;
        }

        private void GenerateCrashSounds()
        {
            if (m_crashSounds == null || m_crashSounds.Length == 0)
            {
                m_crashSounds = new AudioClip[3];
                for (int i = 0; i < 3; i++)
                {
                    m_crashSounds[i] = GenerateCrashSound(200f + i * 100f, 0.5f);
                }
            }

            if (m_scrapesSounds == null || m_scrapesSounds.Length == 0)
            {
                m_scrapesSounds = new AudioClip[2];
                for (int i = 0; i < 2; i++)
                {
                    m_scrapesSounds[i] = GenerateCrashSound(800f + i * 200f, 0.3f);
                }
            }

            if (m_impactSounds == null || m_impactSounds.Length == 0)
            {
                m_impactSounds = new AudioClip[3];
                for (int i = 0; i < 3; i++)
                {
                    m_impactSounds[i] = GenerateImpactSound(100f + i * 50f, 0.4f);
                }
            }
        }

        private AudioClip GenerateCrashSound(float frequency, float duration)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float noise = Random.Range(-1f, 1f) * 0.3f;
                float tone = Mathf.Sin(2 * Mathf.PI * frequency * t) * Mathf.Exp(-t * 5f);
                data[i] = Mathf.Clamp(tone + noise, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("Crash", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateImpactSound(float frequency, float duration)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float noise = Random.Range(-1f, 1f) * 0.5f;
                float tone = Mathf.Sin(2 * Mathf.PI * frequency * t) * Mathf.Exp(-t * 10f);
                data[i] = Mathf.Clamp(tone + noise, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("Impact", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!m_enableCrashSounds) return;

            float impactSpeed = collision.relativeVelocity.magnitude * 3.6f;

            if (impactSpeed > m_minImpactSpeed)
            {
                PlayCrashSound(impactSpeed);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            // Scraping sound while dragging
        }

        public void PlayCrashSound(float impactSpeed)
        {
            if (m_audioSource == null) return;

            float volume = Mathf.Clamp01(impactSpeed * m_crashVolumeScale);
            m_audioSource.volume = volume;

            AudioClip[] sounds = GetRandomSoundSet();
            if (sounds != null && sounds.Length > 0)
            {
                m_audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
            }
        }

        private AudioClip[] GetRandomSoundSet()
        {
            int rand = Random.Range(0, 3);
            return rand switch
            {
                0 => m_crashSounds,
                1 => m_scrapesSounds,
                2 => m_impactSounds,
                _ => m_crashSounds
            };
        }

        public void PlayImpact()
        {
            if (m_impactSounds != null && m_impactSounds.Length > 0)
            {
                m_audioSource.PlayOneShot(m_impactSounds[Random.Range(0, m_impactSounds.Length)]);
            }
        }
    }
}
