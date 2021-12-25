using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

public class AliveCreature : MonoBehaviour, IHealth
{
    [SerializeField] private float _health;

    public float Health => _health;
    public void AdjustHealth(float value)
    {
        _health += value;
        if (_health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
