using System;
using UnityEngine;

namespace ObjectPooling
{
    [Serializable]
    public class Pool<T> where T : MonoBehaviour
    {
        public T prefab;
        public int size;
    }
}