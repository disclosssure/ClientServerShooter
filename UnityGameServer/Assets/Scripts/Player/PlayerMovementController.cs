using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float _speed;
    
    [Header("Player Model")]
    [SerializeField] private PlayerModel _playerModel;

    public float Speed => _speed;
    
    public int Id => _playerModel.Id;

    private bool[] _inputs = new bool[4];
    private Vector3 _mousePosition;
    
    private Rigidbody2D _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        ServerHandle.OnPlayerInputChanged += OnPlayerInputChangedHandler;
    }

    private void OnDisable()
    {
        ServerHandle.OnPlayerInputChanged -= OnPlayerInputChangedHandler;
    }

    private void FixedUpdate()
    {
        Vector2 direction = Vector2.zero;

        if (_inputs[0]) // W
        {
            direction.y += 1;
        }

        if (_inputs[1]) // A
        {
            direction.x -= 1;
        }

        if (_inputs[2]) // S
        {
            direction.y -= 1;
        }

        if (_inputs[3]) // D
        {
            direction.x += 1;
        }

        Move(direction.normalized);
        Rotate();
    }

    public void AdjustSpeed(float value)
    {
        _speed += value;
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
    
    private void OnPlayerInputChangedHandler(int id, bool[] inputs, Vector3 mousePosition)
    {
        if (id == Id)
        {
            _inputs = inputs;
            _mousePosition = mousePosition;
        }
        else
        {
            _inputs = new bool[4];
            _mousePosition = Vector3.zero;
        }
    }
}