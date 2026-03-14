using System.Collections.Generic;
using UnityEngine;

namespace CarSimulator.Utils
{
    public class DebugLogger : MonoBehaviour
    {
        private static DebugLogger s_instance;
        public static DebugLogger Instance => s_instance;

        [SerializeField] private bool m_enableLogging = true;
        [SerializeField] private bool m_logToScreen = false;
        [SerializeField] private int m_screenLogCount = 10;

        private readonly Queue<string> m_screenLogs = new Queue<string>();

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else if (s_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static void Log(string message, string context = null)
        {
            if (s_instance == null || !s_instance.m_enableLogging) return;

            string formatted = context != null ? $"[{context}] {message}" : message;
            Debug.Log(formatted);
            s_instance.AddScreenLog(formatted);
        }

        public static void LogWarning(string message, string context = null)
        {
            if (s_instance == null || !s_instance.m_enableLogging) return;

            string formatted = context != null ? $"[{context}] {message}" : message;
            Debug.LogWarning(formatted);
        }

        public static void LogError(string message, string context = null)
        {
            if (s_instance == null || !s_instance.m_enableLogging) return;

            string formatted = context != null ? $"[{context}] {message}" : message;
            Debug.LogError(formatted);
        }

        private void AddScreenLog(string log)
        {
            if (!m_logToScreen) return;

            m_screenLogs.Enqueue(log);
            while (m_screenLogs.Count > m_screenLogCount)
            {
                m_screenLogs.Dequeue();
            }
        }

        private void OnGUI()
        {
            if (!m_logToScreen) return;

            int y = 10;
            foreach (string log in m_screenLogs)
            {
                GUI.Label(new Rect(10, y, Screen.width - 20, 20), log);
                y += 20;
            }
        }
    }
}
