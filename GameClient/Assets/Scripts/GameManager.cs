using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private PlayerSpawner _playerSpawner;

    public static readonly Dictionary<int, ClientModel> Players = new Dictionary<int, ClientModel>();

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

    public void InitPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        bool isLocalPlayer = id == Client.Instance.Id;
        GameObject player = _playerSpawner.SpawnPlayer(position, rotation, isLocalPlayer);

        var playerManager = player.GetComponent<ClientModel>();
        if (playerManager)
        {
            playerManager.Init(id, username);

            Players.Add(id, playerManager);

            var mainCamera = Camera.main;
            if (mainCamera)
            {
                var cameraController = mainCamera.GetComponent<CameraPositionController>();
                if (cameraController)
                {
                    cameraController.Init();
                }
            }
        }
    }
}