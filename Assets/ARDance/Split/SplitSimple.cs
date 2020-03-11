using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SplitSimple : MonoBehaviour, IScreenRTBlit, IScreenMeshDrawMulti
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    public Material[] MeshMaterials => _materials;
    public bool ShouldDraw => _isShowing;
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private Text _pinButtonText;
    [SerializeField] private Text _startButtonText;
    [SerializeField] private Material[] _materials;
    
    private RenderTexture _cameraFeedBuffer;
    private BackgroundPinner _backgroundPinner;
    private Camera _camera;
    private bool _isShowing;
    private Tween _tween;
    private float _minThreshold { get; } = 0f;
    private float _maxThreshold { get; } = 0.2f;
    private float _seekDuration { get; } = 3.5f;
    
    private int PropertyID_UVMultiplierLandScape;
    private int PropertyID_UVMultiplierPortrait;
    private int PropertyID_UVFlip;
    private int PropertyID_OnWide;
    private int PropertyID_StencilTex;
    private int PropertyID_Threshold;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _backgroundPinner = GetComponent<BackgroundPinner>();
        
        _cameraFeedBuffer = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0);
        foreach (var mat in _materials)
        {
            mat.mainTexture = _cameraFeedBuffer;
        }

        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
        PropertyID_StencilTex = Shader.PropertyToID("_StencilTex");
        PropertyID_Threshold = Shader.PropertyToID("_Threshold");
    }
    
    private void Update()
    {
        if (_isShowing)
        {
            var humanStencil = _arOcclusionManager.humanStencilTexture;
            if (humanStencil)
            {
                foreach (var mat in _materials)
                {
                    SetMaterialProperty(mat, humanStencil);
                }
            }
        }
    }
    
    public void TakeBackground()
    {
        _backgroundPinner.TakeBackground();
    }

    public void HandleSeek()
    {
        if (_tween != null && _tween.IsPlaying())
        {
            _startButtonText.text = "Start";
            _tween.Kill();
        }
        else
        {
            _startButtonText.text = "Stop";
            SeekThreshold();
        }
        
    }
    public void HandlePin()
    {
        if (_isShowing)
        {
            _isShowing = false;
            _backgroundPinner.Unpin();
        }
        else
        {
            _isShowing = true;
            _backgroundPinner.Pin();
        }
        _pinButtonText.text = _isShowing ? "UnPin" : "Pin";
    }
    
    public void SeekThreshold(Slider slider)
    {
        foreach (var mat in _materials)
        {
            mat.SetFloat(PropertyID_Threshold, slider.value);
        }
    }
    
    private void SeekThreshold()
    {
        var value = _minThreshold;
        _tween = DOTween.To(
            () => value,
            num => value = num,
            _maxThreshold,
            _seekDuration
        ).OnUpdate(() =>
        {
            foreach (var material in _materials)
            {
                material.SetFloat(PropertyID_Threshold, value);
            }
        }).SetLoops(-1, LoopType.Yoyo);
    }
    
    private void SetMaterialProperty(Material mat, Texture humanStencilTexture)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            mat.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanStencilTexture));
            mat.SetFloat(PropertyID_UVFlip, 0);
            mat.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            mat.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanStencilTexture));
            mat.SetFloat(PropertyID_UVFlip, 1);
            mat.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            mat.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(humanStencilTexture));
            mat.SetInt(PropertyID_OnWide, 0);
        }

        mat.SetTexture(PropertyID_StencilTex, humanStencilTexture);
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
}
