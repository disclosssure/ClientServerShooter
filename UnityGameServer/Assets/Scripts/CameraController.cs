using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Player _player;

    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothness;

    private readonly Vector3 _offset = new Vector3(0, 0, -10);

    private void FixedUpdate()
    {
        // transform.position = Vector3.Lerp(transform.position, _target.position + _offset, _smoothness * Time.deltaTime);
        // ServerSend.CameraPosition(_player.Id, transform.position);
    }
}