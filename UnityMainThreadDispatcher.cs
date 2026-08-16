using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        EnsureExists();
        return _instance;
    }

    public static void Run(Action action)
    {
        Instance().Enqueue(action);
    }

    public void Enqueue(Action action)
    {
        if (action == null) return;
        _queue.Enqueue(action);
    }

    private void Update()
    {
        while (_queue.TryDequeue(out var action))
        {
            try { action?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
        }
    }

    private static void EnsureExists()
    {
        if (_instance != null) return;

        var go = new GameObject("UnityMainThreadDispatcher");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<UnityMainThreadDispatcher>();
    }
}