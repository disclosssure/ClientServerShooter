using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private string _username;

    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb;
    
    public int Id => _id;
    public string Username => _username;

    private bool[] _inputs;
    private Vector3 _mousePosition;

    public void Init(int id, string username)
    {
        _id = id;
        _username = username;

        _inputs = new bool[4];
    }

    public void FixedUpdate()
    {
        Vector2 direction = Vector2.zero;

        if (_inputs[0])     // W
        {
            direction.y += 1;
        }
        if (_inputs[1])     // A
        {
            direction.x -= 1;
        }
        if (_inputs[2])     // S
        {
            direction.y -= 1;
        }
        if (_inputs[3])     // D
        {
            direction.x += 1;
        }

        Move(direction.normalized);
        Rotate();
    }

    private void Move(Vector2 direction)
    {
        _rb.MovePosition(transform.position + (Vector3)direction * _speed * Time.fixedDeltaTime);

        ServerSend.PlayerPosition(this);
    }

    private void Rotate()
    {
        Vector2 lookDirection = (Vector2)_mousePosition - _rb.position;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        _rb.rotation = angle;
        
        ServerSend.PlayerRotation(this);
    }

    public void HandleInput(bool[] inputs, Vector3 mousePosition)
    {
        _inputs = inputs;
        _mousePosition = mousePosition;
    }
}
