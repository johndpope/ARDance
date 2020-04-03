using UnityEngine;

public class Rotater : MonoBehaviour
{
    private Vector3 _center = new Vector3(0, 0, 1);
    private float _radius = 0.7f;
    private float _angle;
    private float _speed = 16f;
    private bool _isRotating;
    
    public void StartRotate()
    {
        _isRotating = true;
    }

    public void StopRotate()
    {
        _isRotating = false;
    }

    private void Start()
    {
        StartRotate();
    }

    private void Update()
    {
        if (_isRotating)
        {
            _angle += 360f / _speed * Time.deltaTime;
            transform.position = _center + new Vector3
            (
                _radius * Mathf.Cos(_angle * Mathf.Deg2Rad),
                0f,
                _radius * Mathf.Sin(_angle * Mathf.Deg2Rad)
            );
        }
    }
}
