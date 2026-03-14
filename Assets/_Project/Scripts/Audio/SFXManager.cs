using UnityEngine;

namespace CarSimulator.Audio
{
    public class SFXManager : MonoBehaviour
    {
        public static SFXManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float m_volume = 1f;
        [SerializeField] private int m_poolSize = 20;

        [Header("SFX Library")]
        [SerializeField] private SFXEntry[] m_sfxLibrary;

        private AudioSource[] m_sourcePool;
        private int m_currentSource;

        [System.Serializable]
        public class SFXEntry
        {
            public string name;
            public AudioClip clip;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePool();
        }

        private void InitializePool()
        {
            m_sourcePool = new AudioSource[m_poolSize];
            for (int i = 0; i < m_poolSize; i++)
            {
                m_sourcePool[i] = gameObject.AddComponent<AudioSource>();
                m_sourcePool[i].playOnAwake = false;
                m_sourcePool[i].spatialBlend = 0f;
            }
        }

        public void PlaySFX(string sfxName)
        {
            PlaySFX(sfxName, Vector3.zero, 1f);
        }

        public void PlaySFX(string sfxName, float volumeMultiplier)
        {
            PlaySFX(sfxName, Vector3.zero, volumeMultiplier);
        }

        public void PlaySFX(string sfxName, Vector3 position)
        {
            PlaySFX(sfxName, position, 1f);
        }

        public void PlaySFX(string sfxName, Vector3 position, float volumeMultiplier)
        {
            if (m_sfxLibrary == null) return;

            SFXEntry entry = null;
            foreach (var sfx in m_sfxLibrary)
            {
                if (sfx.name == sfxName)
                {
                    entry = sfx;
                    break;
                }
            }

            if (entry == null || entry.clip == null) return;

            AudioSource source = GetNextSource();
            source.clip = entry.clip;
            source.volume = m_volume * volumeMultiplier;
            source.transform.position = position;
            source.spatialBlend = position != Vector3.zero ? 1f : 0f;
            source.Play();
        }

        private AudioSource GetNextSource()
        {
            m_currentSource = (m_currentSource + 1) % m_poolSize;
            return m_sourcePool[m_currentSource];
        }

        public void SetVolume(float volume)
        {
            m_volume = Mathf.Clamp01(volume);
        }

        public float Volume => m_volume;
    }
}
