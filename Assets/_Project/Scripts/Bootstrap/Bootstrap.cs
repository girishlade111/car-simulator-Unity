using System.Collections;
using UnityEngine;
using CarSimulator.Core;

namespace CarSimulator.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string m_firstScene = GameConstants.DEFAULT_SCENE;
        [SerializeField] private bool m_persistBetweenScenes = true;

        private void Awake()
        {
            if (m_persistBetweenScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            Debug.Log("[Bootstrap] Starting initialization...");

            InitializeManagers();
            yield return null;

            Debug.Log($"[Bootstrap] Loading first scene: {m_firstScene}");
            SceneLoader.Load(m_firstScene);
        }

        private void InitializeManagers()
        {
            GameObject managers = new GameObject("[Managers]");
            if (m_persistBetweenScenes)
            {
                DontDestroyOnLoad(managers);
            }

            managers.AddComponent<GameManager>();
            managers.AddComponent<DebugLogger>();
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
