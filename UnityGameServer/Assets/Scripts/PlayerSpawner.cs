using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }
    
    [SerializeField] private PlayerModel _playerPrefab;

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
    
    public PlayerModel SpawnPlayer(Vector3 position = default, Quaternion rotation = default)
    {
        return Instantiate(_playerPrefab, position, rotation);
    }
}
