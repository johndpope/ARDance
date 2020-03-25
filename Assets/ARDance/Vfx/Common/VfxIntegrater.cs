using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Camera))]
public class VfxIntegrater : MonoBehaviour, IScreenRTBlit
{
    public RenderTexture LatestCameraFeedBuffer
    {
        get
        {
            if (_lastDeviceOrientation == DeviceOrientation.Portrait)
            {
                return _colorMapPortrait;
            }
            return _colorMapLandscape;
        }
    }
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private RenderTexture _positionMapPortrait;
    [SerializeField] private RenderTexture _positionMapLandscape;
    [SerializeField] private RenderTexture _colorMapPortrait;
    [SerializeField] private RenderTexture _colorMapLandscape;
    [SerializeField] protected VisualEffect _visualEffect;

    private RenderTexture _renderTexture;
    private uint _threadSizeX, _threadSizeY, _threadSizeZ;
    private Camera _camera;
    private DeviceOrientation _lastDeviceOrientation;
    private int _portraitKernel, _landscapeKernel;
    private Matrix4x4 _viewportInv;
    private ComputeBuffer _buffer;
    private float[] _data;

    // Compute Shader
    private readonly int PropertyID_CameraPos = Shader.PropertyToID("cameraPos");
    private readonly int PropertyID_Converter = Shader.PropertyToID("converter");
    private readonly int PropertyID_Target = Shader.PropertyToID("target");
    private readonly int PropertyID_Origin = Shader.PropertyToID("origin");
    private readonly int PropertyID_IsWide = Shader.PropertyToID("isWide");
    private readonly int PropertyID_UVFlip = Shader.PropertyToID("uVFlip");
    private readonly int PropertyID_UVMultiplierPortrait = Shader.PropertyToID("uVMultiplierPortrait");
    private readonly int PropertyID_UVMultiplierLandScape = Shader.PropertyToID("uVMultiplierLandScape");

    // Visual Effect Graph
    private readonly int PropertyID_PositionMap = Shader.PropertyToID("PositionMap");
    private readonly int PropertyID_ColorMap = Shader.PropertyToID("ColorMap");

    void Start()
    {
        _camera = GetComponent<Camera>();

        _portraitKernel = _computeShader.FindKernel("Portrait");
        _landscapeKernel = _computeShader.FindKernel("Landscape");
        
        // Init Portrait at first
        _lastDeviceOrientation = DeviceOrientation.Portrait;
        
        _computeShader.SetInt(PropertyID_IsWide, 0);
        _buffer = new ComputeBuffer(3, sizeof(float));
        _computeShader.SetBuffer(2, "sample", _buffer);
        _data = new float[3];
    }
    
    void Update()
    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);
//
//            if (touch.phase == TouchPhase.Began)
//            {
//                _computeShader.SetVector("screenPosition", touch.position);
//                _computeShader.Dispatch(2, 1, 1, 1);
//                _buffer.GetData(_data);
//                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                sphere.transform.localScale = Vector3.one * 0.1f;
//                sphere.transform.position = new Vector3(_data[0], _data[1], _data[2]);
//            }
//        }
        
        var humanDepthTexture = _arOcclusionManager.humanDepthTexture;
        if (humanDepthTexture)
        {
            if (_lastDeviceOrientation != Input.deviceOrientation)
            {
                if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                {
                    _computeShader.SetFloat(PropertyID_UVFlip, 0);
                    _computeShader.SetInt(PropertyID_IsWide, 1);
                }
                else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
                {
                    _computeShader.SetFloat(PropertyID_UVFlip, 1);
                    _computeShader.SetInt(PropertyID_IsWide, 1);
                }
                else
                {
                    _computeShader.SetInt(PropertyID_IsWide, 0);
                }
                _lastDeviceOrientation = Input.deviceOrientation;
                InitSetup(humanDepthTexture);
            }
            
            if (_renderTexture)
            {
                _computeShader.SetVector(PropertyID_CameraPos, _camera.transform.position);
                _computeShader.SetMatrix(PropertyID_Converter, GetConverter());
                
                if (_lastDeviceOrientation == DeviceOrientation.Portrait)
                {
                    _computeShader.SetTexture(_portraitKernel, PropertyID_Origin, humanDepthTexture);
                    _computeShader.Dispatch(_portraitKernel, Screen.width / (int) _threadSizeX,
                        Screen.height / (int) _threadSizeY, (int) _threadSizeZ);
                    Graphics.CopyTexture(_renderTexture, _positionMapPortrait);
                }
                else
                {
                    _computeShader.SetTexture(_landscapeKernel, PropertyID_Origin, humanDepthTexture);
                    _computeShader.Dispatch(_landscapeKernel, Screen.width / (int) _threadSizeX,
                        Screen.height / (int) _threadSizeY, (int) _threadSizeZ);
                    Graphics.CopyTexture(_renderTexture, _positionMapLandscape);
                }
            }
            else
            {
                InitSetup(humanDepthTexture);
            }
        }
    }

    private void InitSetup(Texture humanDepthTexture)
    {
        if (_lastDeviceOrientation == DeviceOrientation.Portrait)
        {
            _renderTexture = new RenderTexture(_positionMapPortrait.width, _positionMapPortrait.height, 0, _positionMapPortrait.format) {enableRandomWrite = true};
            _renderTexture.Create();
            _computeShader.SetTexture(_portraitKernel, PropertyID_Target, _renderTexture);
            _visualEffect.SetTexture(PropertyID_PositionMap, _positionMapPortrait);
            _visualEffect.SetTexture(PropertyID_ColorMap, _colorMapPortrait);
            _computeShader.GetKernelThreadGroupSizes(_portraitKernel, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
            _computeShader.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(humanDepthTexture));
        }
        else
        {
            _renderTexture = new RenderTexture(_positionMapLandscape.width, _positionMapLandscape.height, 0, _positionMapLandscape.format) {enableRandomWrite = true};
            _renderTexture.Create();
            _computeShader.SetTexture(_landscapeKernel, PropertyID_Target, _renderTexture);
            _visualEffect.SetTexture(PropertyID_PositionMap, _positionMapLandscape);
            _visualEffect.SetTexture(PropertyID_ColorMap, _colorMapLandscape);
            _computeShader.GetKernelThreadGroupSizes(_landscapeKernel, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
            _computeShader.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanDepthTexture));
        }

        SetViewPortInv();
    }

    private float CalculateUVMultiplierLandScape(Texture textureFromAROcclusionManager)
    {
        float screenAspect = (float) Screen.width / Screen.height;
        float cameraTextureAspect = (float) textureFromAROcclusionManager.width / textureFromAROcclusionManager.height;
        return screenAspect / cameraTextureAspect;
    }
    
    private float CalculateUVMultiplierPortrait(Texture textureFromAROcclusionManager)
    {
        float screenAspect = (float) Screen.height / Screen.width;
        float cameraTextureAspect = (float) textureFromAROcclusionManager.width / textureFromAROcclusionManager.height;
        return screenAspect / cameraTextureAspect;
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

    #region Debug

    [SerializeField] private Text debug;

    public void DebugLog(string log)
    {
        debug.text += log;
    }

    private IEnumerator ClearText()
    {
        yield return new WaitForSeconds(5);
        debug.text = "";
    }

    #endregion
}
