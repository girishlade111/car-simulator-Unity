using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarSimulator.Runtime
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator s_instance;
        private static ServiceLocator Instance => s_instance ?? Create();

        private readonly Dictionary<Type, object> m_services = new Dictionary<Type, object>();

        public static T Get<T>() where T : class
        {
            if (Instance.m_services.TryGetValue(typeof(T), out object service))
            {
                return service as T;
            }
            return null;
        }

        public static void Register<T>(T service) where T : class
        {
            Instance.m_services[typeof(T)] = service;
        }

        public static void Unregister<T>() where T : class
        {
            Instance.m_services.Remove(typeof(T));
        }

        private static ServiceLocator Create()
        {
            GameObject go = new GameObject("[ServiceLocator]");
            s_instance = go.AddComponent<ServiceLocator>();
            DontDestroyOnLoad(go);
            return s_instance;
        }

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (s_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public class ServiceAttribute : Attribute
    {
    }
}
