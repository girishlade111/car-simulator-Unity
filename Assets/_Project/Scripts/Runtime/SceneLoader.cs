using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CarSimulator.Runtime
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader s_instance;
        private static SceneLoader Instance => s_instance ?? Create();

        private AsyncOperation m_currentOperation;
        private Action m_onSceneLoaded;

        public static bool IsLoading => Instance.m_currentOperation != null;

        public static void Load(string sceneName, Action onLoaded = null)
        {
            Instance.StartCoroutine(Instance.LoadInternal(sceneName, onLoaded));
        }

        public static void LoadAdditive(string sceneName, Action onLoaded = null)
        {
            Instance.StartCoroutine(Instance.LoadAdditiveInternal(sceneName, onLoaded));
        }

        public static void ReloadCurrent(Action onLoaded = null)
        {
            string current = SceneManager.GetActiveScene().name;
            Load(current, onLoaded);
        }

        private static SceneLoader Create()
        {
            GameObject go = new GameObject("[SceneLoader]");
            s_instance = go.AddComponent<SceneLoader>();
            DontDestroyOnLoad(go);
            return s_instance;
        }

        private IEnumerator LoadInternal(string sceneName, Action onLoaded)
        {
            m_onSceneLoaded = onLoaded;
            m_currentOperation = SceneManager.LoadSceneAsync(sceneName);

            yield return m_currentOperation;

            m_currentOperation = null;
            m_onSceneLoaded?.Invoke();
            m_onSceneLoaded = null;
        }

        private IEnumerator LoadAdditiveInternal(string sceneName, Action onLoaded)
        {
            m_onSceneLoaded = onLoaded;
            m_currentOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            yield return m_currentOperation;

            m_currentOperation = null;
            m_onSceneLoaded?.Invoke();
            m_onSceneLoaded = null;
        }
    }
}
