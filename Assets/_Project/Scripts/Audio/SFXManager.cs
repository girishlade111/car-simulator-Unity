using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int m_poolSize = 20;
    [SerializeField] private float m_defaultVolume = 1f;

    [Header("SFX Library")]
    [SerializeField] private SFXEntry[] m_sfxLibrary;

    private Dictionary<string, AudioSource> m_playingSFX;
    private List<AudioSource> m_sourcePool;
    private float m_volume = 1f;

    [System.Serializable]
    public class SFXEntry
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool loop = false;
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

        m_playingSFX = new Dictionary<string, AudioSource>();
        InitializePool();
    }

    private void InitializePool()
    {
        m_sourcePool = new List<AudioSource>();

        for (int i = 0; i < m_poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.volume = m_defaultVolume;
            source.enabled = false;
            m_sourcePool.Add(source);
        }
    }

    private AudioSource GetSourceFromPool()
    {
        for (int i = 0; i < m_sourcePool.Count; i++)
        {
            if (!m_sourcePool[i].enabled || !m_sourcePool[i].isPlaying)
            {
                m_sourcePool[i].enabled = true;
                return m_sourcePool[i];
            }
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.spatialBlend = 0f;
        m_sourcePool.Add(newSource);
        return newSource;
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
        for (int i = 0; i < m_sfxLibrary.Length; i++)
        {
            if (m_sfxLibrary[i].name == sfxName)
            {
                entry = m_sfxLibrary[i];
                break;
            }
        }

        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[SFXManager] SFX not found: {sfxName}");
            return;
        }

        AudioSource source = GetSourceFromPool();
        source.clip = entry.clip;
        source.loop = entry.loop;
        source.volume = m_volume * entry.volume * volumeMultiplier;
        source.transform.position = position;

        if (position != Vector3.zero)
        {
            source.spatialBlend = 1f;
            source.minDistance = 1f;
            source.maxDistance = 50f;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        source.Play();

        if (!entry.loop)
        {
            StartCoroutine(DisableSourceAfterPlay(source, entry.clip.length));
        }
    }

    private System.Collections.IEnumerator DisableSourceAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.enabled = false;
    }

    public void PlaySFXOneShot(string sfxName)
    {
        PlaySFXOneShot(sfxName, 1f);
    }

    public void PlaySFXOneShot(string sfxName, float volumeMultiplier)
    {
        if (m_sfxLibrary == null) return;

        for (int i = 0; i < m_sfxLibrary.Length; i++)
        {
            if (m_sfxLibrary[i].name == sfxName && m_sfxLibrary[i].clip != null)
            {
                AudioSource.PlayClipAtPoint(
                    m_sfxLibrary[i].clip, 
                    Vector3.zero, 
                    m_volume * m_sfxLibrary[i].volume * volumeMultiplier
                );
                return;
            }
        }

        Debug.LogWarning($"[SFXManager] SFX not found: {sfxName}");
    }

    public void PlaySFXAtPosition(string sfxName, Vector3 position)
    {
        PlaySFXAtPosition(sfxName, position, 1f);
    }

    public void PlaySFXAtPosition(string sfxName, Vector3 position, float volumeMultiplier)
    {
        if (m_sfxLibrary == null) return;

        for (int i = 0; i < m_sfxLibrary.Length; i++)
        {
            if (m_sfxLibrary[i].name == sfxName && m_sfxLibrary[i].clip != null)
            {
                AudioSource.PlayClipAtPoint(
                    m_sfxLibrary[i].clip,
                    position,
                    m_volume * m_sfxLibrary[i].volume * volumeMultiplier
                );
                return;
            }
        }

        Debug.LogWarning($"[SFXManager] SFX not found: {sfxName}");
    }

    public void StopSFX(string sfxName)
    {
        if (m_playingSFX.ContainsKey(sfxName))
        {
            m_playingSFX[sfxName].Stop();
            m_playingSFX[sfxName].enabled = false;
            m_playingSFX.Remove(sfxName);
        }
    }

    public void StopAllSFX()
    {
        for (int i = 0; i < m_sourcePool.Count; i++)
        {
            m_sourcePool[i].Stop();
            m_sourcePool[i].enabled = false;
        }
        m_playingSFX.Clear();
    }

    public void SetVolume(float volume)
    {
        m_volume = Mathf.Clamp01(volume);
    }

    public float Volume => m_volume;

    public void SetSFXVolume(string sfxName, float volume)
    {
        if (m_sfxLibrary == null) return;

        for (int i = 0; i < m_sfxLibrary.Length; i++)
        {
            if (m_sfxLibrary[i].name == sfxName)
            {
                m_sfxLibrary[i].volume = Mathf.Clamp01(volume);
                return;
            }
        }
    }

    public bool HasSFX(string sfxName)
    {
        if (m_sfxLibrary == null) return false;

        for (int i = 0; i < m_sfxLibrary.Length; i++)
        {
            if (m_sfxLibrary[i].name == sfxName)
            {
                return true;
            }
        }
        return false;
    }
}
