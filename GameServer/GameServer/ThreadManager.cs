using System;
using System.Collections.Generic;

public class ThreadManager
{
    private static readonly List<Action> _executeOnMainThread = new();
    private static readonly List<Action> _executeCopiedOnMainThread = new();
    private static bool _actionToExecuteOnMainThread;
    
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