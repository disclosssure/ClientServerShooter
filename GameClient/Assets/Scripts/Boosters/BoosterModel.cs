using System;
using UnityEngine;

namespace Boosters
{
    public class BoosterModel : MonoBehaviour
    {
        public int Id { get; private set; }

        public void Init(int id)
        {
            Id = id;
        }

        private void OnEnable()
        {
            ClientHandle.OnBoosterUsed += HandleOnBoosterUsed;
        }

        private void OnDisable()
        {
            ClientHandle.OnBoosterUsed -= HandleOnBoosterUsed;
        }

        private void HandleOnBoosterUsed(int boosterId)
        {
            if (Id == boosterId)
            {
                gameObject.SetActive(false);
            }
        }
    }
}