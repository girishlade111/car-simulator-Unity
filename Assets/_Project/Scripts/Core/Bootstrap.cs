using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CarSimulator.Utils;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private string m_firstScene = "MainMenu";
    [SerializeField] private string m_openWorldScene = "OpenWorld_TestDistrict";
    [SerializeField] private string m_garageScene = "Garage_Test";

    [Header("Persistent Systems")]
    [SerializeField] private bool m_autoCreateSystems = true;
    [SerializeField] private GameObject m_persistentSystemsPrefab;

    private bool m_isInitializing;
    private GameSaveData m_loadedSaveData;

    public static string OpenWorldScene => Instance != null ? Instance.m_openWorldScene : "OpenWorld_TestDistrict";
    public static string GarageScene => Instance != null ? Instance.m_garageScene : "Garage_Test";
    public GameSaveData LoadedSaveData => m_loadedSaveData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        m_isInitializing = true;

        if (m_autoCreateSystems)
        {
            CreatePersistentSystems();
        }

        if (m_persistentSystemsPrefab != null)
        {
            Instantiate(m_persistentSystemsPrefab);
        }

        yield return null;

        LoadScene(m_firstScene);
        m_isInitializing = false;
    }

    private void CreatePersistentSystems()
    {
        GameObject systemsRoot = new GameObject("[Systems]");
        DontDestroyOnLoad(systemsRoot);

        var eventSystem = systemsRoot.AddComponent<EventSystem>();
        var gameManager = systemsRoot.AddComponent<GameManager>();
        var saveManager = systemsRoot.AddComponent<SaveManager>();
        var settingsPersistence = systemsRoot.AddComponent<SettingsPersistence>();
        var profileManager = systemsRoot.AddComponent<ProfileManager>();
        var missionManager = systemsRoot.AddComponent<MissionManager>();
        var musicManager = systemsRoot.AddComponent<MusicManager>();
        var sfxManager = systemsRoot.AddComponent<SFXManager>();
        var mobileHelper = systemsRoot.AddComponent<MobileHelper>();

        Debug.Log("[Bootstrap] Persistent systems created");
    }

    public static void LoadScene(string sceneName)
    {
        if (Instance == null)
        {
            Debug.LogError("[Bootstrap] Instance not found!");
            return;
        }

        Instance.StartCoroutine(Instance.LoadSceneInternal(sceneName));
    }

    private IEnumerator LoadSceneInternal(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        OnSceneLoaded(sceneName);
        Debug.Log($"[Bootstrap] Loaded scene: {sceneName}");
    }

    private void OnSceneLoaded(string sceneName)
    {
        if (sceneName == m_openWorldScene && m_loadedSaveData != null)
        {
            ApplySaveData(m_loadedSaveData);
        }
    }

    private void ApplySaveData(GameSaveData saveData)
    {
        if (saveData == null) return;

        if (saveData.player != null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && saveData.player.lastPosition != default)
            {
                player.transform.position = saveData.player.lastPosition;
                player.transform.rotation = saveData.player.lastRotation;
            }

            if (ProfileManager.Instance != null)
            {
                ProfileManager.Instance.UpdatePlayerProgress(saveData.player);
            }
        }

        Debug.Log($"[Bootstrap] Applied save data: {saveData.saveName}");
    }

    public static void LoadGame(int slotIndex)
    {
        if (Instance == null) return;

        var saveData = SaveManager.Instance?.LoadGame(slotIndex);
        if (saveData != null)
        {
            Instance.m_loadedSaveData = saveData;
            LoadScene(Instance.m_openWorldScene);
        }
    }

    public static void NewGame()
    {
        Instance?.StartCoroutine(Instance.NewGameInternal());
    }

    private IEnumerator NewGameInternal()
    {
        m_loadedSaveData = null;

        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.LoadProfiles();
        }

        LoadScene(m_openWorldScene);
        yield break;
    }

    public static void SaveGame(int slotIndex, string saveName)
    {
        if (Instance == null || SaveManager.Instance == null) return;

        GameSaveData saveData = new GameSaveData
        {
            saveName = saveName,
            currentScene = SceneManager.GetActiveScene().name
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

        SaveManager.Instance.SaveGame(saveData, slotIndex);
    }

    public static void GoToMainMenu()
    {
        LoadScene("MainMenu");
    }

    public static void GoToOpenWorld()
    {
        LoadScene(OpenWorldScene);
    }

    public static void GoToGarage()
    {
        LoadScene(GarageScene);
    }

    public static void QuitGame()
    {
        Debug.Log("[Bootstrap] Quitting game...");

        if (SettingsPersistence.Instance != null)
        {
            SettingsPersistence.Instance.SaveSettings();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static bool IsInitializing()
    {
        return Instance != null && Instance.m_isInitializing;
    }
}
