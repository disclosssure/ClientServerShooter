using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _playerPrefab;
    
    public static readonly Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();
    
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

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.Instance.Id)
        {
            player = Instantiate(_localPlayerPrefab, position, rotation);
        }
        else
        {
            player = Instantiate(_playerPrefab, position, rotation);
        }

        var playerManager = player.GetComponent<PlayerManager>();
        playerManager.Id = id;
        playerManager.Username = username;
        
        Players.Add(id, playerManager);
    }
}