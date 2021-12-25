using System;
using System.Collections;
using System.Collections.Generic;
using ObjectPooling;
using UnityEngine;
using Weapon;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, IDamaging, IDestroyable
{
    [Header("Movement")]
    [SerializeField] private float _speed;

    [Header("Damage")] 
    [SerializeField] private float _damage;

    public float Damage => _damage;
    public int Id => _id;
    
    private int _id;
    
    private Vector3 _direction;
    
    private Coroutine _coroutine;
    private Rigidbody2D _rigidbody;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void Init(int id)
    {
        _id = id;
    }

    public void Launch(Vector3 direction)
    {
        _direction = direction;
        _coroutine = StartCoroutine(FlyCo());
    }

    private IEnumerator FlyCo()
    {
        while (true)
        {   
            Move();
            yield return new WaitForFixedUpdate();
        }
    }

    private void Move()
    {
        _rigidbody.MovePosition(transform.position + _direction * _speed * Time.fixedDeltaTime);
        ServerSend.BulletPosition(_id, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IHealth health = other.GetComponent<IHealth>();
        
        if (health != null)
        {
            DealDamage(health, Damage);
            Destroy();
        }
    }

    public void DealDamage(IHealth health, float damage)
    {
        health.AdjustHealth(-damage);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        ServerSend.BulletDestroy(Id);
    }
}
