using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boosters;
using UnityEngine;

public class BoosterSpawner : MonoBehaviour
{
    [Header("Spawn settings")] 
    [SerializeField] private float _spawnFrequency;

    [Header("Boosters")] 
    [SerializeField] private List<Booster> _boosterPrefabs;

    [Header("Other components")] 
    [SerializeField] private PositionProvider _positionProvider;

    private Coroutine _coroutine;

    private int _boosterId = 0;
    
    private void OnEnable()
    {
        _coroutine = StartCoroutine(SpawnBoostersProcess());
    }

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator SpawnBoostersProcess()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnFrequency);
            SpawnRandomBooster();
        }
    }

    private void SpawnRandomBooster()
    {
        Booster boosterInstance;
        
        if (_positionProvider.TryGetPosition(out var point))
        {
            var random = new System.Random();
            int randomIndex = random.Next(0, _boosterPrefabs.Count);
            var prefab = _boosterPrefabs[randomIndex];

            var position = point.transform.position;
            
            boosterInstance = Instantiate(prefab, position, Quaternion.identity);
            boosterInstance.Init(_boosterId);
            
            ServerSend.BoosterSpawn(boosterInstance.Id, boosterInstance.BoosterType, position, Quaternion.identity);
            boosterInstance.OnUse += HandleOnUse;
        }
        
        void HandleOnUse()
        {
            boosterInstance.OnUse -= HandleOnUse;
            point.SetIsHasItem(false);
        }
    }
}