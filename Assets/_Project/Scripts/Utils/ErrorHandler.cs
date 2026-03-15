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
                LogWarning(nameof(SafeGetComponent), $"GameObject is null when getting component {typeof(T).Name}");
                return null;
            }

            T component = go.GetComponent<T>();
            if (component == null)
            {
                LogWarning(nameof(SafeGetComponent), $"Component {typeof(T).Name} not found on {go.name}");
            }
            return component;
        }

        public static T[] SafeGetComponents<T>(GameObject go) where T : class
        {
            if (go == null)
            {
                LogWarning(nameof(SafeGetComponents), $"GameObject is null when getting components {typeof(T).Name}");
                return Array.Empty<T>();
            }

            return go.GetComponents<T>();
        }

        public static T SafeGetComponentInChildren<T>(GameObject go) where T : class
        {
            if (go == null)
            {
                LogWarning(nameof(SafeGetComponentInChildren), $"GameObject is null when getting child component {typeof(T).Name}");
                return null;
            }

            return go.GetComponentInChildren<T>();
        }

        public static bool SafeSetActive(GameObject go, bool active)
        {
            if (go == null)
            {
                LogWarning(nameof(SafeSetActive), "Cannot set active on null GameObject");
                return false;
            }

            go.SetActive(active);
            return true;
        }

        public static void SafeInvoke(Action action, string context = "callback")
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
                LogWarning(nameof(SafeIndex), "Array is null");
                return defaultValue;
            }

            if (index < 0 || index >= array.Length)
            {
                LogWarning(nameof(SafeIndex), $"Index {index} out of bounds for array length {array.Length}");
                return defaultValue;
            }

            return array[index];
        }

        public static bool Try<T>(Func<T> action, out T result, T defaultValue = default, string context = "operation")
        {
            try
            {
                result = action();
                return true;
            }
            catch (Exception e)
            {
                LogError(context, e);
                result = defaultValue;
                return false;
            }
        }

        public static bool Try(Action action, string context = "operation")
        {
            try
            {
                action();
                return true;
            }
            catch (Exception e)
            {
                LogError(context, e);
                return false;
            }
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
        protected virtual void SafeUpdate() { }
        protected virtual void SafeFixedUpdate() { }
        protected virtual void SafeLateUpdate() { }

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

        private void Update()
        {
            try
            {
                SafeUpdate();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError($"{GetType().Name}.Update", e);
            }
        }

        private void FixedUpdate()
        {
            try
            {
                SafeFixedUpdate();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError($"{GetType().Name}.FixedUpdate", e);
            }
        }

        private void LateUpdate()
        {
            try
            {
                SafeLateUpdate();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError($"{GetType().Name}.LateUpdate", e);
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

        protected void Log(string message) => Debug.Log($"[{GetType().Name}] {message}");
        protected void LogWarning(string message) => Debug.LogWarning($"[{GetType().Name}] {message}");
        protected void LogError(string message) => Debug.LogError($"[{GetType().Name}] {message}");
    }
}
