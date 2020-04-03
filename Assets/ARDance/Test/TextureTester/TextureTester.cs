using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TextureTester : MonoBehaviour, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _sceneDepth;
    public bool IsEnable => true;
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private RawImage _hsRawImage;
    [SerializeField] private RawImage _hdRawImage;
    [SerializeField] private RawImage _cfRawImage;
    [SerializeField] private Material _humanStencil;
    [SerializeField] private Material _humanDepth;
    [SerializeField] private Material _cameraFeed;
    [SerializeField] private Material _sceneDepth;
    
    private Texture2D _cameraFeedBuffer;
    
    private int PropertyID_UVMultiplierLandScape;
    private int PropertyID_UVMultiplierPortrait;
    private int PropertyID_UVFlip;
    private int PropertyID_OnWide;
    private int PropertyID_CameraFeed;
    
    void Awake()
    {
        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
        PropertyID_CameraFeed = Shader.PropertyToID("_CameraFeed");
    }
    
    private void OnEnable()
    {
        _arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        _arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void Update()
    {
        var humanStencil = _arOcclusionManager.humanStencilTexture;
        if (humanStencil)
        {
            SetMaterialPropertyHS(humanStencil);
        }
        
        var humanDepth = _arOcclusionManager.humanDepthTexture;
        if (humanDepth)
        {
            SetMaterialPropertyHD(humanDepth);
        }
    }
    
    private void SetMaterialPropertyHS(Texture texture)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            _humanStencil.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            _humanStencil.SetFloat(PropertyID_UVFlip, 0);
            _humanStencil.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            _humanStencil.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            _humanStencil.SetFloat(PropertyID_UVFlip, 1);
            _humanStencil.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            _humanStencil.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(texture));
            _humanStencil.SetInt(PropertyID_OnWide, 0);
        }

        _hsRawImage.texture = texture;
    }
    
    private void SetMaterialPropertyHD(Texture texture)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            _humanDepth.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            _humanDepth.SetFloat(PropertyID_UVFlip, 0);
            _humanDepth.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            _humanDepth.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            _humanDepth.SetFloat(PropertyID_UVFlip, 1);
            _humanDepth.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            _humanDepth.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(texture));
            _humanDepth.SetInt(PropertyID_OnWide, 0);
        }

        _hdRawImage.texture = texture;
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
    
    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        RefreshCameraFeedTexture();
    }

    private void RefreshCameraFeedTexture()
    {
        XRCameraImage cameraImage;
        _arCameraManager.TryGetLatestImage(out cameraImage);

        if (_cameraFeedBuffer == null || _cameraFeedBuffer.width != cameraImage.width || _cameraFeedBuffer.height != cameraImage.height)
        {
            _cameraFeedBuffer = new Texture2D(cameraImage.width, cameraImage.height, TextureFormat.RGBA32, false);
        }

        CameraImageTransformation imageTransformation = Input.deviceOrientation == DeviceOrientation.LandscapeRight ? CameraImageTransformation.MirrorY : CameraImageTransformation.MirrorX;
        XRCameraImageConversionParams conversionParams = new XRCameraImageConversionParams(cameraImage, TextureFormat.RGBA32, imageTransformation);

        NativeArray<byte> rawTextureData = _cameraFeedBuffer.GetRawTextureData<byte>();

        try
        {
            unsafe
            {
                cameraImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
        }
        finally
        {
            cameraImage.Dispose();
        }

        _cameraFeedBuffer.Apply();
        SetMaterialPropertyCF(_cameraFeedBuffer);
    }
    
    private void SetMaterialPropertyCF(Texture texture)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            _cameraFeed.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            _cameraFeed.SetFloat(PropertyID_UVFlip, 0);
            _cameraFeed.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            _cameraFeed.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            _cameraFeed.SetFloat(PropertyID_UVFlip, 1);
            _cameraFeed.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            _cameraFeed.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(texture));
            _cameraFeed.SetInt(PropertyID_OnWide, 0);
        }

        _cfRawImage.texture = texture;
    }
}
