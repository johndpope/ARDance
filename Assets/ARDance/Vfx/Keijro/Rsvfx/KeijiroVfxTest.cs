using UnityEngine;

public class KeijiroVfxTest : MonoBehaviour
{
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private RenderTexture _positionMapPortrait;
    [SerializeField] private Texture _humanDepth;
    private Camera _camera;
    private RenderTexture _renderTexture;
    private uint _threadSizeX, _threadSizeY, _threadSizeZ;
    private int _portraitKernel;
    private Matrix4x4 _viewportInv;
    
    // Compute Shader
    private readonly int PropertyID_CameraPos = Shader.PropertyToID("cameraPos");
    private readonly int PropertyID_Converter = Shader.PropertyToID("converter");
    private readonly int PropertyID_Target = Shader.PropertyToID("target");
    private readonly int PropertyID_Origin = Shader.PropertyToID("origin");

    void Start()
    {
        _camera = GetComponent<Camera>();
        _portraitKernel = _computeShader.FindKernel("Portrait");
        InitSetup();
    }

    private void Update()
    {
        if (_renderTexture)
        {
            _computeShader.SetVector(PropertyID_CameraPos, _camera.transform.position);
            _computeShader.SetMatrix(PropertyID_Converter, GetConverter());
            _computeShader.SetTexture(_portraitKernel, PropertyID_Origin, _humanDepth); 
            _computeShader.Dispatch(_portraitKernel, Screen.width / (int) _threadSizeX, Screen.height / (int) _threadSizeY, (int) _threadSizeZ);
            Graphics.CopyTexture(_renderTexture, _positionMapPortrait);
        }
    }

    private void InitSetup()
    {
        _renderTexture = new RenderTexture(_positionMapPortrait.width, _positionMapPortrait.height, 0, _positionMapPortrait.format) {enableRandomWrite = true};
        _renderTexture.Create();
        _computeShader.SetTexture(_portraitKernel, PropertyID_Target, _renderTexture);
        _computeShader.GetKernelThreadGroupSizes(_portraitKernel, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
        SetViewPortInv();
    }
    
    private void SetViewPortInv()
    {
        _viewportInv = Matrix4x4.identity;
        _viewportInv.m00 = _viewportInv.m03 = Screen.width / 2f;
        _viewportInv.m11 = Screen.height / 2f;
        _viewportInv.m13 = Screen.height / 2f;
        _viewportInv.m22 = (_camera.farClipPlane - _camera.nearClipPlane) / 2f;
        _viewportInv.m23 = (_camera.farClipPlane + _camera.nearClipPlane) / 2f;
        _viewportInv = _viewportInv.inverse;
    }

    private Matrix4x4 GetConverter()
    {
        Matrix4x4 viewMatInv = _camera.worldToCameraMatrix.inverse;
        Matrix4x4 projMatInv = _camera.projectionMatrix.inverse;
        return viewMatInv * projMatInv * _viewportInv;
    }
}
