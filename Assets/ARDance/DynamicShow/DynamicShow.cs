using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(BackgroundPinner))]
public class DynamicShow : MonoBehaviour, IScreenRTBlit, IScreenMeshDraw
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    public Material MeshMaterial => _material;
    public bool ShouldDraw => _isShowing;

    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private Material _material;
    [SerializeField] private Text _pinButtonText;
    [SerializeField] private Text _startButtonText;
    [SerializeField] private GameObject _canvas;

    private RenderTexture _cameraFeedBuffer;
    private BackgroundPinner _backgroundPinner;
    private Camera _camera;
    private bool _isShowing;
    private Tween _tween;

    protected virtual float _minThreshold { get; } = 0f;
    protected virtual float _maxThreshold { get; } = 1f;
    protected virtual float _seekDuration { get; } = 3f;
    
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
        _material.mainTexture = _cameraFeedBuffer;

        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
        PropertyID_StencilTex = Shader.PropertyToID("_StencilTex");
        PropertyID_Threshold = Shader.PropertyToID("_Threshold");
    }

    private void Start()
    {
        StartCoroutine(ClearText());
    }

    private void Update()
    {
        if (_isShowing)
        {
            var humanStencil = _arOcclusionManager.humanStencilTexture;
            if (humanStencil)
            {
                SetMaterialProperty(humanStencil);
            }
        }
    }

    public void TakeBackground()
    {
        _backgroundPinner.TakeBackground();
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

    public void HandleCanvasVisibility()
    {
        _canvas.SetActive(!_canvas.activeSelf);
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
    
    private void SetMaterialProperty(Texture humanStencilTexture)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            _material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanStencilTexture));
            _material.SetFloat(PropertyID_UVFlip, 0);
            _material.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            _material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanStencilTexture));
            _material.SetFloat(PropertyID_UVFlip, 1);
            _material.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            _material.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(humanStencilTexture));
            _material.SetInt(PropertyID_OnWide, 0);
        }

        _material.SetTexture(PropertyID_StencilTex, humanStencilTexture);
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

    private void SeekThreshold()
    {
        var value = _minThreshold;
        _tween = DOTween.To(
            () => value,
            num => value = num,
            _maxThreshold,
            3.0f
        ).OnUpdate(() =>
        {
            _material.SetFloat(PropertyID_Threshold, value);
        }).SetLoops(-1, LoopType.Yoyo);
    }
    
    public void SeekThreshold(Slider slider)
    {
        _material.SetFloat(PropertyID_Threshold, slider.value);
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
    
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();  // リストを使いまわす
    private bool IsPointerOverUIObject(Vector2 screenPosition) {
        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        EventSystem.current.RaycastAll(eventDataCurrentPosition, _raycastResults);
        var over = _raycastResults.Count > 0;
        _raycastResults.Clear();
        return over;
    }
    #endregion
}
