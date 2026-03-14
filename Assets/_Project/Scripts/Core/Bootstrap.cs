using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance { get; private set; }

    [SerializeField] private string m_firstScene = "MainMenu";
    [SerializeField] private GameObject m_persistentSystemsPrefab;

    private bool m_isInitializing;

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

        if (m_persistentSystemsPrefab != null)
        {
            Instantiate(m_persistentSystemsPrefab);
        }

        yield return null;

        LoadScene(m_firstScene);
        m_isInitializing = false;
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

        Debug.Log($"[Bootstrap] Loaded scene: {sceneName}");
    }

    public static bool IsInitializing()
    {
        return Instance != null && Instance.m_isInitializing;
    }
}
