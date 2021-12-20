using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // [SerializeField] private Player _playerPrefab;
    //
    // private void OnEnable()
    // {
    //     ServerHandle.OnPlayerConnected += HandleOnPlayerConnected;
    // }
    //
    // private void OnDisable()
    // {
    //     ServerHandle.OnPlayerConnected -= HandleOnPlayerConnected;
    // }
    //
    // private void HandleOnPlayerConnected(int id, string username)
    // {
    //     SpawnPlayer(arg1, arg2);
    // }
    //
    // private void SpawnPlayer(int id, string username)
    // {
    //     var player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
    //     player.Init(id, username);
    //     
    //     var cameraController = player.transform.gameObject.AddComponent<CameraPositionController>();
    //     cameraController.Init(player, player.transform);
    // }
}
