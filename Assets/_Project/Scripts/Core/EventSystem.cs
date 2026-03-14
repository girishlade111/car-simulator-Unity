using System;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

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

    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        // Simple implementation - in production use a proper event bus
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : struct
    {
    }

    public static void Publish<T>(T eventData) where T : struct
    {
        // Placeholder for event publishing
    }
}
