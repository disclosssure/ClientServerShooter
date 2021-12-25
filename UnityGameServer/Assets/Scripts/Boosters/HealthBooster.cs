using UnityEngine;

namespace Boosters
{
    public class HealthBooster : Booster
    {
        [SerializeField] private float _value;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var aliveCreature = other.GetComponent<AliveCreature>();
            if (aliveCreature)
            {
                aliveCreature.AdjustHealth(_value);
            }
        }
    }
}