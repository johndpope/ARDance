using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Camera))]
public class BackgroundPinner : MonoBehaviour
{
    public Texture2D Background => _currentBackground;
    public bool ShouldPin => _isPinned;
    public Material BackgroundMaterial => _material;
    
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private ARCameraBackground _arCameraBackground;
    [SerializeField] private Shader _backgroundShader;
    
    private Texture2D _currentBackground;
    private bool _isPinned;
    private bool _isCopySucceeded;
    private Material _material;
    
    private int PropertyID_UVMultiplierLandScape;
    private int PropertyID_UVMultiplierPortrait;
    private int PropertyID_UVFlip;
    private int PropertyID_OnWide;

    private void Awake()
    {
        _material = new Material(_backgroundShader);
        
        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
    }

    private void Update()
    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);
//
//            if (touch.phase == TouchPhase.Began)
//            {
//                if (!_isCopySucceeded)
//                {
//                    TakeBackground();
//                    return;
//                }
//
//                if (_isPinned)
//                {
//                    Unpin();
//                }
//                else
//                {
//                    Pin();
//                }
//            }
//        }

        if (_isPinned && _isCopySucceeded)
        {
            SetMaterialProperty();
        }
    }
    
    public void Pin()
    {
        if (_currentBackground)
        {
            if (_isPinned || !_isCopySucceeded) return;
            //_arCameraBackground.enabled = false;
            _isPinned = true;
        }
    }
    
    public void Unpin()
    {
        if (!_isPinned) return;
        //_arCameraBackground.enabled = true;
        _isPinned = false;
    }

    public void TakeBackground()
    {
        _arCameraManager.frameReceived += OnCameraFrameReceived;
    }
    
    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        CopyCameraFeedTexture();
    }
    
    private void CopyCameraFeedTexture()
    {
        _isCopySucceeded = true;
        XRCameraImage cameraImage;
        _arCameraManager.TryGetLatestImage(out cameraImage);
        if (_currentBackground == null || _currentBackground.width != cameraImage.width || _currentBackground.height != cameraImage.height)
        {
            _currentBackground = new Texture2D(cameraImage.width, cameraImage.height, TextureFormat.RGBA32, false);
        }

        CameraImageTransformation imageTransformation = Input.deviceOrientation == DeviceOrientation.LandscapeRight ? CameraImageTransformation.MirrorY : CameraImageTransformation.MirrorX;
        XRCameraImageConversionParams conversionParams = new XRCameraImageConversionParams(cameraImage, TextureFormat.RGBA32, imageTransformation);

        NativeArray<byte> rawTextureData = _currentBackground.GetRawTextureData<byte>();

        try
        {
            unsafe
            {
                cameraImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
        }
        catch
        {
            _isCopySucceeded = false;
        }
        finally
        {
            cameraImage.Dispose();
            _arCameraManager.frameReceived -= OnCameraFrameReceived;
        }
        
        _currentBackground.Apply();
    }
    
    private void SetMaterialProperty()
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            _material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(_currentBackground));
            _material.SetFloat(PropertyID_UVFlip, 0);
            _material.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            _material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(_currentBackground));
            _material.SetFloat(PropertyID_UVFlip, 1);
            _material.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            _material.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(_currentBackground));
            _material.SetInt(PropertyID_OnWide, 0);
        }
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
    
    #region Debug

    [SerializeField] private Text debug;

    public void DebugLog(string log)
    {
        debug.text += log;
    }
    #endregion
}
