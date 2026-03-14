using UnityEngine;

public class MobileHelper : MonoBehaviour
{
    public static MobileHelper Instance { get; private set; }

    [Header("Mobile Detection")]
    [SerializeField] private bool m_isMobile;
    [SerializeField] private bool m_autoDetect = true;

    [Header("Mobile Settings")]
    [SerializeField] private int m_targetFrameRate = 30;
    [SerializeField] private bool m_reduceParticles = true;
    [SerializeField] private bool m_simplifyShadows = true;

    public bool IsMobile => m_isMobile;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (m_autoDetect)
        {
            DetectMobile();
        }

        ApplyMobileSettings();
    }

    private void DetectMobile()
    {
        m_isMobile = Application.isMobilePlatform;

#if UNITY_ANDROID || UNITY_IOS
        m_isMobile = true;
#endif

        Debug.Log($"[MobileHelper] Mobile detected: {m_isMobile}");
    }

    private void ApplyMobileSettings()
    {
        if (m_isMobile)
        {
            Application.targetFrameRate = m_targetFrameRate;

            if (m_reduceParticles)
            {
                ReduceParticles();
            }

            if (m_simplifyShadows)
            {
                SimplifyShadows();
            }

            Debug.Log("[MobileHelper] Mobile settings applied");
        }
    }

    private void ReduceParticles()
    {
        var particles = FindObjectsOfType<ParticleSystem>();
        for (int i = 0; i < particles.Length; i++)
        {
            var main = particles[i].main;
            main.maxParticles = Mathf.Min(main.maxParticles, 50);
        }
    }

    private void SimplifyShadows()
    {
        QualitySettings.shadowQuality = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = ShadowResolution.Low;
        QualitySettings.shadowDistance = 30f;
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
    }

    public int GetQualityLevel()
    {
        return QualitySettings.GetQualityLevel();
    }

    public void EnableVSync(bool enable)
    {
        QualitySettings.vSyncCount = enable ? 1 : 0;
    }

    public void SetTargetFrameRate(int fps)
    {
        m_targetFrameRate = fps;
        Application.targetFrameRate = fps;
    }

    public int GetTargetFrameRate()
    {
        return m_targetFrameRate;
    }

    public bool IsLowMemoryDevice()
    {
#if UNITY_ANDROID
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var context = activity.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            var activityManager = context.Call<AndroidJavaObject>("getSystemService", "activity");
            var memInfo = activityManager.Call<AndroidJavaObject>("getMemoryInfo");
            long totalMemory = memInfo.Get<long>("totalMem");
            long threshold = memInfo.Get<long>("threshold");
            return totalMemory < threshold;
        }
#elif UNITY_IOS
        return false;
#else
        return false;
#endif
    }

    public long GetAvailableMemory()
    {
#if UNITY_ANDROID
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var context = activity.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            var activityManager = context.Call<AndroidJavaObject>("getSystemService", "activity");
            var memInfo = activityManager.Call<AndroidJavaObject>("getMemoryInfo");
            return memInfo.Get<long>("availMem");
        }
#else
        return System.GC.GetTotalMemory(false);
#endif
    }

    public void OptimizeForMobile()
    {
        m_isMobile = true;
        ApplyMobileSettings();
    }

    public void ResetToDefaults()
    {
        Application.targetFrameRate = 60;
        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
        QualitySettings.shadowQuality = ShadowQuality.All;
        QualitySettings.shadowDistance = 50f;
    }
}

public class PerformanceMonitor : MonoBehaviour
{
    public static PerformanceMonitor Instance { get; private set; }

    [Header("Monitoring")]
    [SerializeField] private bool m_logPerformance = false;
    [SerializeField] private float m_logInterval = 5f;

    private float m_timer;
    private int m_frameCount;
    private float m_fps;

    public float FPS => m_fps;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        m_frameCount++;
        m_timer += Time.deltaTime;

        if (m_timer >= m_logInterval)
        {
            m_fps = m_frameCount / m_timer;
            m_frameCount = 0;
            m_timer = 0;

            if (m_logPerformance)
            {
                Debug.Log($"[PerformanceMonitor] FPS: {m_fps:F1}");
            }
        }
    }

    public float GetAverageFPS()
    {
        return m_fps;
    }

    public bool IsPerformanceGood()
    {
        return m_fps >= 30f;
    }
}
