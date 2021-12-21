using System;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    private void Start()
    {
        StartServer();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void StartServer()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        
        Server.Start(50, 26950);
    }
}