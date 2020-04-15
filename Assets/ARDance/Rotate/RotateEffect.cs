using UnityEngine;
using UnityEngine.UI;

public class RotateEffect : MonoBehaviour, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => true;

    [SerializeField] private Material _material;
    [SerializeField] private Vector2 _radius = new Vector2(0.3F, 0.3F);
    [SerializeField] [Range(0.0f, 360.0f)] private float _angle = 50;
    [SerializeField] private Vector2 _center = new Vector2(0.5F, 0.5F);
    private float _aspect;

    private readonly int PropertyID_RotationMatrix = Shader.PropertyToID("_RotationMatrix");
    private readonly int PropertyID_CenterRadius = Shader.PropertyToID("_CenterRadius");

    private void Start()
    {
        _aspect = (float) Screen.width / Screen.height;
        _radius = new Vector2(_radius.x, _radius.x * _aspect);
    }

    private void Update()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, _angle), Vector3.one);

        _material.SetMatrix(PropertyID_RotationMatrix, rotationMatrix);
        _material.SetVector(PropertyID_CenterRadius, new Vector4(_center.x, _center.y, _radius.x, _radius.y));
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                var screenPos = touch.position;
                _center = new Vector2(screenPos.x/Screen.width, screenPos.y/Screen.height);
            }
        }
    }

    public void ChangeRadius(Slider slider)
    {
        _radius = new Vector2(slider.value, slider.value * _aspect);
    }

    public void ChangeAngle(Slider slider)
    {
        _angle = slider.value;
    }
}