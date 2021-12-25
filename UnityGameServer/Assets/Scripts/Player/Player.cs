using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;

public class Player : MonoBehaviour, IHealth
{
    [SerializeField] private float _health;

    public float Health => _health;
    
    public void AdjustHealth(float value)
    {
        _health += value;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
