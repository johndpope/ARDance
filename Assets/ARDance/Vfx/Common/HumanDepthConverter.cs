using UnityEngine;

public class HumanDepthConverter
{
    public HumanDepthConverter(Camera camera, ComputeShader computeShader)
    {
        _camera = camera;
        _computeShader = computeShader;
        _portraitKernel = _computeShader.FindKernel("Portrait");
        _landscapeKernel = _computeShader.FindKernel("Landscape");
    }
    
    private ComputeShader _computeShader;
    private Camera _camera;
    private RenderTexture _renderTexture;
    private uint _threadSizeX, _threadSizeY, _threadSizeZ;
    private int _portraitKernel, _landscapeKernel;
    private Matrix4x4 _viewportInv;
    
    private readonly int PropertyID_CameraPos = Shader.PropertyToID("cameraPos");
    private readonly int PropertyID_Converter = Shader.PropertyToID("converter");
    private readonly int PropertyID_Target = Shader.PropertyToID("target");
    private readonly int PropertyID_Origin = Shader.PropertyToID("origin");
    private readonly int PropertyID_IsWide = Shader.PropertyToID("isWide");
    private readonly int PropertyID_UVFlip = Shader.PropertyToID("uVFlip");
    private readonly int PropertyID_UVMultiplierPortrait = Shader.PropertyToID("uVMultiplierPortrait");
    private readonly int PropertyID_UVMultiplierLandScape = Shader.PropertyToID("uVMultiplierLandScape");
    
    public void InitSetup(RenderTexture positionMap)
    {
        _renderTexture = new RenderTexture(positionMap.width, positionMap.height, 0, positionMap.format) {enableRandomWrite = true};
        _renderTexture.Create();
        if (Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            _computeShader.SetTexture(_portraitKernel, PropertyID_Target, _renderTexture);
            _computeShader.GetKernelThreadGroupSizes(_portraitKernel, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
            _computeShader.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait());
            _computeShader.SetInt(PropertyID_IsWide, 0);
        }
        else
        {
            _computeShader.SetTexture(_landscapeKernel, PropertyID_Target, _renderTexture);
            _computeShader.GetKernelThreadGroupSizes(_landscapeKernel, out _threadSizeX, out _threadSizeY, out _threadSizeZ);
            _computeShader.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape());
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
        }
        SetViewPortInv();
    }

    public void ConvertDepth(Texture humanDepthTexture, RenderTexture positionMap, bool isPortrait)
    {
        if (_renderTexture)
        {
            _computeShader.SetVector(PropertyID_CameraPos, _camera.transform.position);
            _computeShader.SetMatrix(PropertyID_Converter, GetConverter());
            var kernel = isPortrait ? _portraitKernel : _landscapeKernel;
            _computeShader.SetTexture(kernel, PropertyID_Origin, humanDepthTexture); 
            _computeShader.Dispatch(kernel, Screen.width / (int) _threadSizeX, 
                Screen.height / (int) _threadSizeY, (int) _threadSizeZ);
            Graphics.CopyTexture(_renderTexture, positionMap);
        }
    }

    private float CalculateUVMultiplierLandScape(Texture textureFromAROcclusionManager = null)
    {
        float screenAspect = (float) Screen.width / Screen.height;
        float cameraTextureAspect = 4f / 3f;
        if (textureFromAROcclusionManager)
        {
            cameraTextureAspect = (float) textureFromAROcclusionManager.width / textureFromAROcclusionManager.height;
        }
        return screenAspect / cameraTextureAspect;
    }
    
    private float CalculateUVMultiplierPortrait(Texture textureFromAROcclusionManager = null)
    {
        float screenAspect = (float) Screen.height / Screen.width;
        float cameraTextureAspect = 4f / 3f;
        if (textureFromAROcclusionManager)
        {
            cameraTextureAspect = (float) textureFromAROcclusionManager.width / textureFromAROcclusionManager.height;
        }
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
}
