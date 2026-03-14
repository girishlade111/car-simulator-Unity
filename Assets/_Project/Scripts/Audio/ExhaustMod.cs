using UnityEngine;
using CarSimulator.Vehicle;

namespace CarSimulator.Audio
{
    public class ExhaustMod : MonoBehaviour
    {
        [Header("Exhaust Settings")]
        [SerializeField] private bool m_enableExhaust = true;
        [SerializeField] private ExhaustType m_currentExhaust = ExhaustType.Stock;

        [Header("Audio")]
        [SerializeField] private AudioSource m_exhaustSource;
        [SerializeField] private AudioSource m_popSource;

        [Header("References")]
        [SerializeField] private VehiclePhysics m_vehiclePhysics;

        public enum ExhaustType
        {
            Stock,
            Sport,
            Performance,
            Racing,
            Titanium,
            TurboBack
        }

        private Dictionary<ExhaustType, ExhaustData> m_exhaustData = new Dictionary<ExhaustType, ExhaustData>();

        [System.Serializable]
        public class ExhaustData
        {
            public string name;
            public int price;
            public float basePitch;
            public float pitchMultiplier;
            public float volume;
            public float popIntensity;
            public bool hasAfterfire;
        }

        private void Start()
        {
            InitializeExhausts();
            SetupAudioSources();
            
            if (m_vehiclePhysics == null)
                m_vehiclePhysics = GetComponent<VehiclePhysics>();
        }

        private void InitializeExhausts()
        {
            m_exhaustData = new Dictionary<ExhaustType, ExhaustData>
            {
                { ExhaustType.Stock, new ExhaustData { name = "Stock", price = 0, basePitch = 1f, pitchMultiplier = 1f, volume = 0.3f, popIntensity = 0f, hasAfterfire = false } },
                { ExhaustType.Sport, new ExhaustData { name = "Sport", price = 800, basePitch = 1.2f, pitchMultiplier = 1.2f, volume = 0.5f, popIntensity = 0.2f, hasAfterfire = false } },
                { ExhaustType.Performance, new ExhaustData { name = "Performance", price = 1500, basePitch = 1.4f, pitchMultiplier = 1.4f, volume = 0.6f, popIntensity = 0.4f, hasAfterfire = true } },
                { ExhaustType.Racing, new ExhaustData { name = "Racing", price = 2500, basePitch = 1.6f, pitchMultiplier = 1.6f, volume = 0.7f, popIntensity = 0.6f, hasAfterfire = true } },
                { ExhaustType.Titanium, new ExhaustData { name = "Titanium", price = 3500, basePitch = 1.8f, pitchMultiplier = 1.8f, volume = 0.75f, popIntensity = 0.8f, hasAfterfire = true } },
                { ExhaustType.TurboBack, new ExhaustData { name = "Turbo Back", price = 4500, basePitch = 2f, pitchMultiplier = 2f, volume = 0.8f, popIntensity = 1f, hasAfterfire = true } }
            };
        }

        private void SetupAudioSources()
        {
            if (m_exhaustSource == null)
            {
                m_exhaustSource = gameObject.AddComponent<AudioSource>();
                m_exhaustSource.loop = true;
                m_exhaustSource.playOnAwake = false;
                m_exhaustSource.spatialBlend = 1f;
                m_exhaustSource.minDistance = 2f;
                m_exhaustSource.maxDistance = 30f;
            }

            if (m_popSource == null)
            {
                m_popSource = gameObject.AddComponent<AudioSource>();
                m_popSource.loop = false;
                m_popSource.playOnAwake = false;
                m_popSource.spatialBlend = 1f;
                m_popSource.minDistance = 2f;
                m_popSource.maxDistance = 20f;
            }
        }

        private void Update()
        {
            if (!m_enableExhaust || m_vehiclePhysics == null) return;

            UpdateExhaustSound();
            HandlePopSounds();
        }

        private void UpdateExhaustSound()
        {
            float speed = m_vehiclePhysics.CurrentSpeed;
            float speedRatio = speed / m_vehiclePhysics.MaxSpeed;

            ExhaustData data = GetExhaustData(m_currentExhaust);
            if (data == null) return;

            float targetPitch = data.basePitch + (speedRatio * data.pitchMultiplier);
            float targetVolume = data.volume * Mathf.Clamp01(speedRatio + 0.2f);

            if (m_exhaustSource != null)
            {
                m_exhaustSource.pitch = targetPitch;
                m_exhaustSource.volume = targetVolume;

                if (!m_exhaustSource.isPlaying)
                {
                    m_exhaustSource.Play();
                }
            }
        }

        private void HandlePopSounds()
        {
            ExhaustData data = GetExhaustData(m_currentExhaust);
            if (data == null || data.popIntensity <= 0) return;

            var input = GetComponent<VehicleInput>();
            if (input == null) return;

            bool throttleRelease = input.ThrottleInput < -0.5f && m_vehiclePhysics.CurrentSpeed > 20f;
            
            if (throttleRelease && Random.value < data.popIntensity * 0.1f)
            {
                PlayPopSound();
            }
        }

        private void PlayPopSound()
        {
            if (m_popSource != null)
            {
                m_popSource.pitch = Random.Range(0.8f, 1.2f);
                m_popSource.volume = Random.Range(0.3f, 0.6f);
                m_popSource.PlayOneShot(GeneratePopSound());
            }
        }

        private AudioClip GeneratePopSound()
        {
            int sampleRate = 44100;
            float duration = 0.15f;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float noise = Random.Range(-1f, 1f) * 0.3f;
                float pop = Mathf.Sin(2 * Mathf.PI * 150f * t) * Mathf.Exp(-t * 20f);
                data[i] = Mathf.Clamp(pop + noise, -1f, 1f);
            }

            AudioClip clip = AudioClip.Create("Pop", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        public void SetExhaust(ExhaustType type)
        {
            m_currentExhaust = type;
            Debug.Log($"[ExhaustMod] Equipped: {GetExhaustData(type)?.name}");
        }

        private ExhaustData GetExhaustData(ExhaustType type)
        {
            return m_exhaustData.ContainsKey(type) ? m_exhaustData[type] : null;
        }

        public ExhaustData GetCurrentExhaustData() => GetExhaustData(m_currentExhaust);
        public ExhaustType GetCurrentExhaust() => m_currentExhaust;

        public void CycleExhaust()
        {
            int currentIndex = (int)m_currentExhaust;
            int nextIndex = (currentIndex + 1) % System.Enum.GetValues(typeof(ExhaustType)).Length;
            SetExhaust((ExhaustType)nextIndex);
        }
    }
}
