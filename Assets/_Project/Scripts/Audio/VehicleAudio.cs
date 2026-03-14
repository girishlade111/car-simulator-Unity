using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VehicleAudio : MonoBehaviour
{
    [Header("Engine Sound")]
    [SerializeField] private AudioSource m_engineSource;
    [SerializeField] private AudioClip m_engineClip;
    [SerializeField] private float m_minEnginePitch = 0.5f;
    [SerializeField] private float m_maxEnginePitch = 2f;
    [SerializeField] private float m_engineVolume = 0.8f;

    [Header("Tire Sound")]
    [SerializeField] private AudioSource m_tireSource;
    [SerializeField] private AudioClip m_tireScreechClip;
    [SerializeField] private float m_tireVolume = 0.5f;
    [SerializeField] private float m_screechThreshold = 0.8f;

    [Header("Collision Sound")]
    [SerializeField] private AudioSource m_collisionSource;
    [SerializeField] private AudioClip[] m_collisionClips;
    [SerializeField] private float m_collisionVolume = 1f;
    [SerializeField] private float m_minCollisionSpeed = 5f;

    [Header("Horn Sound")]
    [SerializeField] private AudioSource m_hornSource;
    [SerializeField] private AudioClip m_hornClip;
    [SerializeField] private float m_hornVolume = 0.7f;

    private VehicleController m_vehicle;
    private float m_currentSpeed;
    private float m_targetPitch;
    private bool m_isScreeching;
    private float m_initialEnginePitch;

    private void Awake()
    {
        m_vehicle = GetComponent<VehicleController>();
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
            m_engineSource.Play();
        }

        if (m_tireSource == null)
        {
            m_tireSource = CreateAudioSource("TireSource");
            m_tireSource.clip = m_tireScreechClip;
            m_tireSource.loop = true;
            m_tireSource.volume = 0f;
        }

        if (m_collisionSource == null)
        {
            m_collisionSource = CreateAudioSource("CollisionSource");
            m_collisionSource.spatialBlend = 1f;
        }

        if (m_hornSource == null)
        {
            m_hornSource = CreateAudioSource("HornSource");
            m_hornSource.clip = m_hornClip;
            m_hornSource.loop = true;
            m_hornSource.volume = m_hornVolume;
        }

        m_initialEnginePitch = m_engineSource.pitch;
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
        if (m_vehicle == null) return;

        m_currentSpeed = m_vehicle.CurrentSpeed;
        UpdateEngineSound();
        UpdateTireSound();
        HandleHornInput();
    }

    private void UpdateEngineSound()
    {
        if (m_engineSource == null || m_engineClip == null) return;

        float speedRatio = m_currentSpeed / m_vehicle.MaxSpeed;
        speedRatio = Mathf.Clamp01(speedRatio);

        m_targetPitch = Mathf.Lerp(m_minEnginePitch, m_maxEnginePitch, speedRatio);

        m_engineSource.pitch = Mathf.Lerp(m_engineSource.pitch, m_targetPitch, Time.deltaTime * 2f);
        m_engineSource.volume = m_engineVolume;
    }

    private void UpdateTireSound()
    {
        if (m_tireSource == null || m_tireScreechClip == null) return;

        bool shouldScreech = m_currentSpeed > 10f && 
                             (Mathf.Abs(Input.GetAxis("Horizontal")) > m_screechThreshold || 
                              Input.GetKey(KeyCode.LeftShift) && m_currentSpeed > 5f);

        if (shouldScreech && !m_isScreeching)
        {
            m_tireSource.Play();
            m_isScreeching = true;
        }
        else if (!shouldScreech && m_isScreeching)
        {
            m_tireSource.Stop();
            m_isScreeching = false;
        }

        float targetVolume = shouldScreech ? m_tireVolume : 0f;
        m_tireSource.volume = Mathf.Lerp(m_tireSource.volume, targetVolume, Time.deltaTime * 5f);
    }

    private void HandleHornInput()
    {
        if (Input.GetKeyDown(KeyCode.H) && m_hornSource != null && m_hornClip != null)
        {
            if (!m_hornSource.isPlaying)
            {
                m_hornSource.Play();
            }
            else
            {
                m_hornSource.Stop();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_collisionSource == null || m_collisionClips == null || m_collisionClips.Length == 0) return;

        if (m_currentSpeed < m_minCollisionSpeed) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Vehicle")) return;

        AudioClip clip = m_collisionClips[Random.Range(0, m_collisionClips.Length)];
        if (clip != null)
        {
            m_collisionSource.PlayOneShot(clip, m_collisionVolume);
        }
    }

    public void SetEngineVolume(float volume)
    {
        m_engineVolume = Mathf.Clamp01(volume);
        if (m_engineSource != null)
        {
            m_engineSource.volume = m_engineVolume;
        }
    }

    public void SetTireVolume(float volume)
    {
        m_tireVolume = Mathf.Clamp01(volume);
    }

    public void SetHornVolume(float volume)
    {
        m_hornVolume = Mathf.Clamp01(volume);
        if (m_hornSource != null)
        {
            m_hornSource.volume = m_hornVolume;
        }
    }

    public void ToggleEngineSound(bool enabled)
    {
        if (m_engineSource != null)
        {
            if (enabled && !m_engineSource.isPlaying)
            {
                m_engineSource.Play();
            }
            else if (!enabled && m_engineSource.isPlaying)
            {
                m_engineSource.Stop();
            }
        }
    }
}
