using UnityEngine;

namespace CarSimulator.Audio
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("Audio Source")]
        [SerializeField] private AudioSource m_musicSource;

        [Header("Tracks")]
        [SerializeField] private MusicTrack[] m_tracks;

        [Header("Settings")]
        [SerializeField] private float m_volume = 0.8f;
        [SerializeField] private bool m_loop = true;

        private int m_currentTrackIndex = -1;

        [System.Serializable]
        public class MusicTrack
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

            if (m_musicSource == null)
            {
                m_musicSource = gameObject.AddComponent<AudioSource>();
                m_musicSource.playOnAwake = false;
                m_musicSource.loop = m_loop;
                m_musicSource.spatialBlend = 0f;
            }
        }

        private void Start()
        {
            if (m_tracks != null && m_tracks.Length > 0)
            {
                PlayTrack(0);
            }
        }

        public void PlayTrack(int index)
        {
            if (m_tracks == null || index < 0 || index >= m_tracks.Length) return;
            if (m_tracks[index].clip == null) return;

            m_currentTrackIndex = index;
            m_musicSource.clip = m_tracks[index].clip;
            m_musicSource.volume = m_volume;
            m_musicSource.Play();
        }

        public void PlayTrack(string trackName)
        {
            if (m_tracks == null) return;

            for (int i = 0; i < m_tracks.Length; i++)
            {
                if (m_tracks[i].name == trackName)
                {
                    PlayTrack(i);
                    return;
                }
            }
        }

        public void Stop()
        {
            m_musicSource.Stop();
        }

        public void Pause()
        {
            m_musicSource.Pause();
        }

        public void Resume()
        {
            m_musicSource.UnPause();
        }

        public void SetVolume(float volume)
        {
            m_volume = Mathf.Clamp01(volume);
            m_musicSource.volume = m_volume;
        }

        public float Volume => m_volume;
    }
}
