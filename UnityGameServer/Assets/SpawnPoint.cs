using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private bool _isHasItem;

    public bool IsHasItem => _isHasItem;

    public void SetIsHasItem(bool value) => _isHasItem = value;
}
