using System;
using UnityEngine;

public class BulletModel : MonoBehaviour
{
    private int _id;
    public int Id => _id;

    public void Init(int id)
    {
        _id = id;
    }

    private void OnEnable()
    {
        ClientHandle.OnBulletPositionChanged += HandleOnBulletPositionChanged;
        ClientHandle.OnBulletDestroyed += HandleOnBulletDestroyed;
    }

    private void OnDisable()
    {
        ClientHandle.OnBulletPositionChanged += HandleOnBulletPositionChanged;
        ClientHandle.OnBulletDestroyed -= HandleOnBulletDestroyed;
    }

    private void HandleOnBulletPositionChanged(int bulletId, Vector3 position)
    {
        if (Id == bulletId)
        {
            transform.position = position;
        }
    }
    
    private void HandleOnBulletDestroyed(int bulletId)
    {
        if (Id == bulletId)
        {
            gameObject.SetActive(false);
        }
    }
}
