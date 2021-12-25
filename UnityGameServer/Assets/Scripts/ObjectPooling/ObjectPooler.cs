using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public class ObjectPooler<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected Pool<T> _pool;

        public Pool<T> Pool => _pool;

        protected Queue<T> _objectPool = new Queue<T>();

        public Action OnObjectSpawnedFromPool { get; set; }
        
        protected virtual void Start()
        {
            SpawnPool();
        }

        public virtual T SpawnFromPool(Vector3 spawnPosition, Quaternion spawnRotation = default)
        {
            if (_objectPool.Count != 0)
            {
                var objectToSpawn = _objectPool.Dequeue();

                objectToSpawn.gameObject.SetActive(true);
                
                var objectTransform = objectToSpawn.transform;
                objectTransform.position = spawnPosition;
                objectTransform.rotation = spawnRotation;

                _objectPool.Enqueue(objectToSpawn);

                OnObjectSpawnedFromPool?.Invoke();

                return objectToSpawn;
            }
            
            return null;
        }

        protected virtual void SpawnPool()
        {
            for (int i = 0; i < _pool.size; i++)
            {
                var poolObject = Instantiate(_pool.prefab);
                poolObject.gameObject.SetActive(false);
                _objectPool.Enqueue(poolObject);
            }
        }
    }
}