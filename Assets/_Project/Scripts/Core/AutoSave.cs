using UnityEngine;

public class AutoSave : MonoBehaviour
{
    [Header("Auto Save Settings")]
    [SerializeField] private bool m_enableAutoSave = true;
    [SerializeField] private float m_autoSaveInterval = 120f;
    [SerializeField] private int m_autoSaveSlot = 0;
    [SerializeField] private string m_autoSaveName = "Auto Save";

    [Header("Quick Save")]
    [SerializeField] private KeyCode m_quickSaveKey = KeyCode.F5;
    [SerializeField] private KeyCode m_quickLoadKey = KeyCode.F9;

    [Header("UI Feedback")]
    [SerializeField] private bool m_showSaveNotification = true;
    [SerializeField] private float m_notificationDuration = 2f;

    private float m_timer;
    private GameObject m_notificationUI;

    private void Start()
    {
        m_timer = m_autoSaveInterval;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_quickSaveKey))
        {
            QuickSave();
        }

        if (Input.GetKeyDown(m_quickLoadKey))
        {
            QuickLoad();
        }

        if (m_enableAutoSave)
        {
            m_timer -= Time.deltaTime;

            if (m_timer <= 0)
            {
                AutoSaveGame();
                m_timer = m_autoSaveInterval;
            }
        }
    }

    private void AutoSaveGame()
    {
        DoSave(m_autoSaveSlot, m_autoSaveName);
    }

    public void QuickSave()
    {
        DoSave(9, "Quick Save");
    }

    public void QuickLoad()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave(9))
        {
            Bootstrap.LoadGame(9);
        }
    }

    private void DoSave(int slotIndex, string saveName)
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[AutoSave] SaveManager not found!");
            return;
        }

        GameSaveData saveData = new GameSaveData
        {
            saveName = saveName,
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        };

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            saveData.player = new PlayerData
            {
                lastPosition = player.transform.position,
                lastRotation = player.transform.rotation
            };
        }

        if (ProfileManager.Instance != null && ProfileManager.Instance.ActiveProfile != null)
        {
            saveData.player = ProfileManager.Instance.ActiveProfile.PlayerData;
        }

        bool success = SaveManager.Instance.SaveGame(saveData, slotIndex);

        if (success && m_showSaveNotification)
        {
            ShowNotification($"Game saved: {saveName}");
        }

        Debug.Log($"[AutoSave] Saved to slot {slotIndex}: {saveName}");
    }

    private void ShowNotification(string message)
    {
        Debug.Log($"[AutoSave] {message}");
    }

    public void SetAutoSave(bool enabled)
    {
        m_enableAutoSave = enabled;
    }

    public void SetAutoSaveInterval(float interval)
    {
        m_autoSaveInterval = Mathf.Max(30f, interval);
    }
}
