using System.Collections.Generic;
using Boosters;
using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
    [SerializeField] private List<BoosterModel> _boosters;
    
    public enum BoosterType
    {
        Speed = 0,
        Health = 1,
    }
    
    private void OnEnable()
    {
        ClientHandle.OnBoosterSpawn += HandleOnBoosterSpawn;
    }

    private void OnDisable()
    {
        ClientHandle.OnBoosterSpawn -= HandleOnBoosterSpawn;
    }
    
    private void HandleOnBoosterSpawn(int boosterId, int boosterType, Vector3 position, Quaternion rotation)
    {
        var booster = Instantiate(_boosters[boosterType], position, rotation);
        booster.Init(boosterId);
    }
}
