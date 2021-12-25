using UnityEngine;

namespace ObjectPooling
{
    public class BulletPooler : ObjectPooler<Bullet>
    {
        public override Bullet SpawnFromPool(Vector3 spawnPosition, Quaternion spawnRotation = default)
        {
            if (_objectPool.Count != 0)
            {
                var objectToSpawn = _objectPool.Dequeue();

                objectToSpawn.gameObject.SetActive(true);
                
                var objectTransform = objectToSpawn.transform;
                objectTransform.position = spawnPosition;
                objectTransform.rotation = spawnRotation;

                _objectPool.Enqueue(objectToSpawn);
                
                ServerSend.SpawnBullet(objectToSpawn.Id, spawnPosition, spawnRotation);
                OnObjectSpawnedFromPool?.Invoke();

                return objectToSpawn;
            }
            
            return null;
        }

        protected override void SpawnPool()
        {
            for (int i = 0; i < _pool.size; i++)
            {
                var poolObject = Instantiate(_pool.prefab);
                poolObject.GetComponent<Bullet>().Init(i);
                poolObject.gameObject.SetActive(false);
                _objectPool.Enqueue(poolObject);
            }
        }
    }
}