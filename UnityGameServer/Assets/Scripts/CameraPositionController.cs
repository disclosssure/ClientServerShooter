using UnityEngine;

public class CameraPositionController : MonoBehaviour
{
    private bool IsInited { get; set; }
    
    private int _playerId;
    private Transform _target;
    
    private readonly Vector3 _offset = new Vector3(0, 0, -10);
    private readonly float _smoothness = 0.85f;

    private Vector3 _currentPosition;
    
    public void Init(int playerId, Transform target)
    {
        _playerId = playerId;
        _target = target;

        _currentPosition = target.position;
        IsInited = true;
    }

    private void FixedUpdate()
    {
        if (IsInited)
        {
            _currentPosition = Vector3.Lerp(_currentPosition, _target.position + _offset, _smoothness * Time.deltaTime);
            ServerSend.CameraPosition(_playerId, _currentPosition);
        }
    }
}