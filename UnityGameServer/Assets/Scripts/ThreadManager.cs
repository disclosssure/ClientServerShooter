using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> _executeOnMainThread = new List<Action>();
    private static readonly List<Action> _executeCopiedOnMainThread = new List<Action>();
    private static bool _actionToExecuteOnMainThread;

    private void FixedUpdate()
    {
        UpdateMain();
    }

    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            Console.WriteLine("No action to execute on main thread!");
            return;
        }

        lock (_executeOnMainThread)
        {
            _executeOnMainThread.Add(action);
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
            
            _executeCopiedOnMainThread.ForEach(action => action.Invoke());
        }
    }
}
