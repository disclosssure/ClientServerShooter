using System;
using ObjectPooling;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private BulletPooler _pooler;

    private void OnEnable()
    {
        ServerHandle.OnPlayerShoot += HandleOnBulletSpawned;
    }
    
    private void OnDisable()
    {
        ServerHandle.OnPlayerShoot -= HandleOnBulletSpawned;
    }

    private void HandleOnBulletSpawned(Vector3 position, Quaternion rotation)
    {
        var bullet = _pooler.SpawnFromPool(position, rotation);
        ServerSend.SpawnBullet(bullet.Id, position, rotation);
        bullet.Launch(bullet.transform.right);
    }
}