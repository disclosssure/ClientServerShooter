using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private string _username;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private CharacterController _controller;
    
    public int Id => _id;
    public string Username => _username;

    private const float k_gravity = -9.81f;
    
    private bool[] _inputs; 

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
            direction.x += 1;
        }
        if (_inputs[2])     // S
        {
            direction.y -= 1;
        }
        if (_inputs[3])     // D
        {
            direction.x -= 1;
        }

        Move(direction);
    }

    private void Move(Vector2 direction)
    {
        Vector3 moveDirection = transform.right * direction.x + transform.forward * direction.y;

        _controller.Move(moveDirection * _moveSpeed * Time.fixedDeltaTime);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        _inputs = inputs;
        transform.rotation = rotation;
    }
}
