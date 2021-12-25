using System;
using System.Collections;
using System.Collections.Generic;
using Boosters;
using UnityEngine;

public class SpeedBooster : Booster
{
    [SerializeField] private float _value;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovementController>();

        if (player)
        {
            player.AdjustSpeed(_value);
            Use();
        }
    }
}
