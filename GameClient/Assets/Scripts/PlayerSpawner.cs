using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _playerPrefab;

    public GameObject SpawnPlayer(Vector3 position = default, Quaternion rotation = default, bool isLocalPlayer = false)
    {
        var prefab = isLocalPlayer ? _localPlayerPrefab : _playerPrefab;
        return Instantiate(prefab, position, rotation);
    }
}
