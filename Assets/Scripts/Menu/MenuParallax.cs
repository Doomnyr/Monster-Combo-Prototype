using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallax : MonoBehaviour
{
    public float parallaxEffectMultiplier = 0.1f;
    public float smoothTime = 0.3f;

    private Vector2 _startPosition;
    private Vector3 _velocity;

    private Vector2 _mousePosition;
    void Start()
    {
        _startPosition = transform.position;
    }

    
    void Update()
    {
        Vector2 _mousePosition = Mouse.current.position.ReadValue();
        Vector2 offset = Camera.main.ScreenToViewportPoint(_mousePosition);
        transform.position = Vector3.SmoothDamp(transform.position, _startPosition + (offset * parallaxEffectMultiplier), ref _velocity, smoothTime);
    }
}
