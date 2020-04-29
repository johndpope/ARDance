using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Camera))]
public class VfxIntegretor2 : MonoBehaviour
{

    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private BackgroundPinner _backgroundPinner;
    [SerializeField] private RenderTexture _positionMap;
    [SerializeField] private RenderTexture _velocityMap;
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] protected VisualEffect _visualEffect;
    
    private RenderTexture _tempPositionMap;
    private RenderTexture _tempVelocityMap;
    private uint _threadSizeX, _threadSizeY, _threadSizeZ;
    private Camera _camera;
    private int _portraitKernel;
    private Matrix4x4 _viewportInv;
    
    // Compute Shader
    private readonly int PropertyID_CameraPos = Shader.PropertyToID("cameraPos");
    private readonly int PropertyID_Converter = Shader.PropertyToID("converter");
    private readonly int PropertyID_PositionMap = Shader.PropertyToID("positionMap");
    private readonly int PropertyID_VelocityMap = Shader.PropertyToID("velocityMap");
    private readonly int PropertyID_HumanDepthTexture = Shader.PropertyToID("humanDepthTexture");
    private readonly int PropertyID_FrameRate = Shader.PropertyToID("frameRate");
    private readonly int PropertyID_IsWide = Shader.PropertyToID("isWide");
    private readonly int PropertyID_UVMultiplierPortrait = Shader.PropertyToID("uVMultiplierPortrait");

    // Visual Effect Graph
    private readonly int Vfx_PropertyID_PositionMap = Shader.PropertyToID("PositionMap");
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _portraitKernel = _computeShader.FindKernel("Portrait");
        _computeShader.SetInt(PropertyID_IsWide, 0);
    }

    private void Update()
    {
        var humanDepthTexture = _arOcclusionManager.humanDepthTexture;
        if (humanDepthTexture)
        {
            if (_tempPositionMap && _tempVelocityMap)
            {
                _computeShader.SetVector(PropertyID_CameraPos, _camera.transform.position);
                _computeShader.SetMatrix(PropertyID_Converter, GetConverter());
                _computeShader.SetTexture(_portraitKernel, PropertyID_HumanDepthTexture, humanDepthTexture);
                _computeShader.SetFloat(PropertyID_FrameRate, 1f / Time.deltaTime);
                _computeShader.Dispatch(_portraitKernel, Screen.width / (int) _threadSizeX,
                    Screen.height / (int) _threadSizeY, (int) _threadSizeZ);
                Graphics.CopyTexture(_tempPositionMap, _positionMap);
                Graphics.CopyTexture(_tempVelocityMap, _velocityMap);
            }
            else
            {
                InitSetup(humanDepthTexture);
            }
        }
    }

    private void InitSetup(Texture humanDepthTexture)
    {
        _tempPositionMap = new RenderTexture(_positionMap.width, _positionMap.height, 0, _positionMap.format) {enableRandomWrite = true};
        _tempVelocityMap = new RenderTexture(_velocityMap.width, _velocityMap.height, 0, _velocityMap.format) {enableRandomWrite = true};
        _tempPositionMap.Create();
        _tempVelocityMap.Create();
        _computeShader.SetTexture(_portraitKernel, PropertyID_PositionMap, _tempPositionMap);
        _computeShader.SetTexture(_portraitKernel, PropertyID_VelocityMap, _tempVelocityMap);
        _computeShader.GetKernelThreadGroupSizes(_portraitKernel, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
        _computeShader.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(humanDepthTexture));
        SetViewPortInv();
        
        //_visualEffect.SetTexture(Vfx_PropertyID_PositionMap, _positionMap);
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
    
    public void TakeBackground()
    {
        _backgroundPinner.TakeBackground();
    }
}
