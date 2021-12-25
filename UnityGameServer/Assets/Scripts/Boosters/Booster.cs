using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boosters
{
    public enum BoosterType
    {
        Speed = 0,
        Health = 1,
    }

    public abstract class Booster : MonoBehaviour
    {
        [SerializeField] private BoosterType _boosterType;
        public BoosterType BoosterType => _boosterType;

        public int Id => _id;

        private int _id;

        public void Init(int id)
        {
            _id = id;
        }

        public Action OnUse;

        public void Use()
        {
            ServerSend.BoosterUse(Id);
            OnUse?.Invoke();
        }
    }
}