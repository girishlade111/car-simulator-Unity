using UnityEngine;
using System;

namespace CarSimulator.Utils
{
    public static class ErrorHandler
    {
        public static void LogError(string context, Exception e)
        {
            Debug.LogError($"[Error] {context}: {e.Message}");
            if (e.StackTrace != null)
            {
                Debug.LogError($"[StackTrace] {e.StackTrace}");
            }
        }

        public static void LogWarning(string context, string message)
        {
            Debug.LogWarning($"[Warning] {context}: {message}");
        }

        public static T SafeGetComponent<T>(GameObject go) where T : class
        {
            if (go == null)
            {
                Debug.LogWarning($"[ErrorHandler] GameObject is null when getting component {typeof(T).Name}");
                return null;
            }

            T component = go.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"[ErrorHandler] Component {typeof(T).Name} not found on {go.name}");
            }
            return component;
        }

        public static T[] SafeGetComponents<T>(GameObject go) where T : class
        {
            if (go == null)
            {
                Debug.LogWarning($"[ErrorHandler] GameObject is null when getting components {typeof(T).Name}");
                return Array.Empty<T>();
            }

            return go.GetComponents<T>();
        }

        public static T SafeGetComponentInChildren<T>(GameObject go) where T : class
        {
            if (go == null)
            {
                Debug.LogWarning($"[ErrorHandler] GameObject is null when getting child component {typeof(T).Name}");
                return null;
            }

            return go.GetComponentInChildren<T>();
        }

        public static bool SafeSetActive(GameObject go, bool active)
        {
            if (go == null)
            {
                Debug.LogWarning("[ErrorHandler] Cannot set active on null GameObject");
                return false;
            }

            go.SetActive(active);
            return true;
        }

        public static void SafeInvoke(System.Action action, string context = "callback")
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                LogError(context, e);
            }
        }

        public static T SafeIndex<T>(T[] array, int index, T defaultValue = default)
        {
            if (array == null)
            {
                Debug.LogWarning("[ErrorHandler] Array is null");
                return defaultValue;
            }

            if (index < 0 || index >= array.Length)
            {
                Debug.LogWarning($"[ErrorHandler] Index {index} out of bounds for array length {array.Length}");
                return defaultValue;
            }

            return array[index];
        }
    }

    public abstract class MonoBehaviourSafe : MonoBehaviour
    {
        protected T GetCachedComponent<T>(ref T cached, bool required = true) where T : class
        {
            if (cached != null) return cached;

            cached = GetComponent<T>();
            if (cached == null && required)
            {
                Debug.LogError($"[{GetType().Name}] Required component {typeof(T).Name} not found!");
            }
            return cached;
        }

        protected void RequireComponent<T>(bool logError = true) where T : class
        {
            if (GetComponent<T>() == null && logError)
            {
                Debug.LogError($"[{GetType().Name}] Missing required component {typeof(T).Name}!");
            }
        }

        protected virtual void SafeAwake() { }
        protected virtual void SafeStart() { }
        protected virtual void SafeOnDestroy() { }

        private void Awake()
        {
            try
            {
                SafeAwake();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError($"{GetType().Name}.Awake", e);
            }
        }

        private void Start()
        {
            try
            {
                SafeStart();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError($"{GetType().Name}.Start", e);
            }
        }

        private void OnDestroy()
        {
            try
            {
                SafeOnDestroy();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError($"{GetType().Name}.OnDestroy", e);
            }
        }
    }
}
