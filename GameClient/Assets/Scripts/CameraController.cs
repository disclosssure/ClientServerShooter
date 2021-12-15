using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerManager _player;
    
    [Header("Camera settings")]
    [SerializeField] private float _sensitivity;
    
    [Range(0, 90)]
    [SerializeField] private float _clampAngle;

    private float _verticalRotation;
    private float _horizontalRotation;

    private void Start()
    {
        _verticalRotation = transform.localEulerAngles.x;
        _horizontalRotation = _player.transform.eulerAngles.y;
    }

    private void Update()
    {
        Look();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float verticalInput = -Input.GetAxis("Mouse Y");
        float horizontalInput = Input.GetAxis("Mouse X");

        _verticalRotation += verticalInput * _sensitivity * Time.deltaTime;
        _horizontalRotation += horizontalInput * _sensitivity * Time.deltaTime;

        _verticalRotation = Mathf.Clamp(_verticalRotation, -_clampAngle, _clampAngle);
        
        transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        _player.transform.rotation = Quaternion.Euler(0f, _horizontalRotation, 0f);
    }
}
