using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPositionController : MonoBehaviour
{
    private bool IsInited { get; set; }

    public void Init()
    {
        IsInited = true;
    }

    private void OnEnable()
    {
        ClientHandle.OnCameraPositionChanged += HandleOnCameraPositionChanged;
    }

    private void OnDisable()
    {
        ClientHandle.OnCameraPositionChanged -= HandleOnCameraPositionChanged;
        IsInited = false;
    }

    private void HandleOnCameraPositionChanged(Vector3 position)
    {
        if (IsInited)
        {
            transform.position = position;
        }
    }
}