using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioSource m_fadeSource;

    [Header("Music Tracks")]
    [SerializeField] private MusicTrack[] m_tracks;

    [Header("Settings")]
    [SerializeField] private float m_crossfadeDuration = 2f;
    [SerializeField] private bool m_shuffle = false;

    private int m_currentTrackIndex = -1;
    private float m_volume = 0.8f;
    private bool m_isFading;

    [System.Serializable]
    public class MusicTrack
    {
        public string name;
        public AudioClip clip;
        public bool loop = true;
        [Range(0f, 1f)] public float volume = 1f;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        if (m_musicSource == null)
        {
            m_musicSource = gameObject.AddComponent<AudioSource>();
            m_musicSource.playOnAwake = false;
            m_musicSource.loop = false;
            m_musicSource.spatialBlend = 0f;
        }

        if (m_fadeSource == null)
        {
            m_fadeSource = gameObject.AddComponent<AudioSource>();
            m_fadeSource.playOnAwake = false;
            m_fadeSource.loop = false;
            m_fadeSource.spatialBlend = 0f;
        }
    }

    private void Start()
    {
        if (m_tracks != null && m_tracks.Length > 0)
        {
            PlayTrack(0);
        }
    }

    private void Update()
    {
        if (m_musicSource != null && !m_musicSource.isPlaying && !m_isFading)
        {
            PlayNextTrack();
        }
    }

    public void PlayTrack(int index)
    {
        if (m_tracks == null || index < 0 || index >= m_tracks.Length) return;

        MusicTrack track = m_tracks[index];
        if (track.clip == null) return;

        m_currentTrackIndex = index;

        m_musicSource.clip = track.clip;
        m_musicSource.loop = track.loop;
        m_musicSource.volume = m_volume * track.volume;
        m_musicSource.Play();

        Debug.Log($"[MusicManager] Playing: {track.name}");
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

        Debug.LogWarning($"[MusicManager] Track not found: {trackName}");
    }

    public void PlayNextTrack()
    {
        if (m_tracks == null || m_tracks.Length == 0) return;

        int nextIndex;
        if (m_shuffle)
        {
            nextIndex = Random.Range(0, m_tracks.Length);
        }
        else
        {
            nextIndex = (m_currentTrackIndex + 1) % m_tracks.Length;
        }

        PlayTrack(nextIndex);
    }

    public void CrossfadeTo(int index)
    {
        if (m_tracks == null || index < 0 || index >= m_tracks.Length || m_isFading) return;

        StartCoroutine(Crossfade(index));
    }

    private System.Collections.IEnumerator Crossfade(int index)
    {
        m_isFading = true;

        MusicTrack newTrack = m_tracks[index];
        m_fadeSource.clip = newTrack.clip;
        m_fadeSource.loop = newTrack.loop;
        m_fadeSource.volume = 0f;
        m_fadeSource.Play();

        float timer = 0f;
        float startVolume = m_musicSource.volume;

        while (timer < m_crossfadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / m_crossfadeDuration;

            m_musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
            m_fadeSource.volume = Mathf.Lerp(0f, m_volume * newTrack.volume, t);

            yield return null;
        }

        m_musicSource.Stop();
        m_currentTrackIndex = index;

        AudioSource temp = m_musicSource;
        m_musicSource = m_fadeSource;
        m_fadeSource = temp;

        m_musicSource.volume = m_volume * newTrack.volume;
        m_isFading = false;
    }

    public void Stop()
    {
        m_musicSource.Stop();
        m_fadeSource.Stop();
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
        if (m_musicSource != null && !m_isFading)
        {
            m_musicSource.volume = m_volume;
        }
    }

    public float Volume => m_volume;

    public int CurrentTrackIndex => m_currentTrackIndex;

    public string CurrentTrackName
    {
        get
        {
            if (m_tracks != null && m_currentTrackIndex >= 0 && m_currentTrackIndex < m_tracks.Length)
            {
                return m_tracks[m_currentTrackIndex].name;
            }
            return "";
        }
    }
}
