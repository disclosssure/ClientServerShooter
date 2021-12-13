using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> _executeOnMainThread = new List<Action>();
    private static readonly List<Action> _executeCopiedOnMainThread = new List<Action>();
    private static bool _actionToExecuteOnMainThread = false;

    private void Update()
    {
        UpdateMain();
    }

    public static void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        lock (_executeOnMainThread)
        {
            _executeOnMainThread.Add(_action);
            _actionToExecuteOnMainThread = true;
        }
    }

    public static void UpdateMain()
    {
        if (_actionToExecuteOnMainThread)
        {
            _executeCopiedOnMainThread.Clear();
            lock (_executeOnMainThread)
            {
                _executeCopiedOnMainThread.AddRange(_executeOnMainThread);
                _executeOnMainThread.Clear();
                _actionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < _executeCopiedOnMainThread.Count; i++)
            {
                _executeCopiedOnMainThread[i]();
            }
        }
    }
}