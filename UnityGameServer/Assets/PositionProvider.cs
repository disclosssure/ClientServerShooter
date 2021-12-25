using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class PositionProvider : MonoBehaviour
{
    [SerializeField] private List<SpawnPoint> _spawnPoints;

    public bool TryGetPosition(out SpawnPoint spawnPoint)
    {
        var freePositions = _spawnPoints.Where(x => !x.IsHasItem);

        if (freePositions.Count() >= 1)
        {
            var random = new Random();
            int randomIndex = random.Next(0, freePositions.Count());

            spawnPoint = freePositions.ElementAt(randomIndex);
            spawnPoint.SetIsHasItem(true);
            return true;
        }

        spawnPoint = null;
        return false;
    }
}