using System;
using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] inputs = 
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D),
        };
        
        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        ClientSend.PlayerMovement(inputs, mousePosition);
    }
}
