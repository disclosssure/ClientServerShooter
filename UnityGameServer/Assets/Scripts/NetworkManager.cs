using System;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private Player _playerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        StartServer();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer() => Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);

    private void StartServer()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        
        Server.Start(50, 26950);
    }
}