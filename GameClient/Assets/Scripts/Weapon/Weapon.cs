using System;
using ObjectPooling;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon params")]
    [SerializeField] private Transform _firePoint;
    
    // TODO: replace with injection
    private BulletPooler _pooler;

    private void Awake()
    {
        _pooler = FindObjectOfType<BulletPooler>();
    }

    private void OnEnable()
    {
        ClientHandle.OnBulletSpawn += HandleOnBulletSpawn;
    }
    
    private void OnDisable()
    {
        ClientHandle.OnBulletSpawn -= HandleOnBulletSpawn;
    }

    private void HandleOnBulletSpawn(int bulletId, Vector3 position, Quaternion rotation)
    {
        var bullet = _pooler.SpawnFromPool(position, rotation);
        bullet.Init(bulletId);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        var position = _firePoint.position;
        var rotation = _firePoint.rotation;
        
        ClientSend.PlayerShoot(position, rotation);
    }
}
