using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SplitManager : MonoBehaviour, IScreenMeshDrawMulti
{
    public Material[] MeshMaterials => _materials;
    public bool ShouldDraw => _isShowing;
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private Text _pinButtonText;
    [SerializeField] private Text _startButtonText;
    [SerializeField] private Material _materialLU;
    [SerializeField] private Material _materialLD;
    [SerializeField] private Material _materialRU;
    [SerializeField] private Material _materialRD;
    [SerializeField] private Material _materialCL;
    [SerializeField] private Material _materialCR;

    private enum SplitDirection
    {
        Left, Right, LeftUp, LeftDown, RightUp, RightDown
    }

    private const float GAP_WIDTH_CROSS = 0.04f;
    private const float GAP_WIDTH = 0.05f;

    private Material[] _materials;
    private CustomRenderManager _customRenderManager;
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
    private int PropertyID_ThresholdX;
    private int PropertyID_ThresholdY;
    private int PropertyID_OnX;
    private int PropertyID_OnY;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _backgroundPinner = GetComponent<BackgroundPinner>();
        _customRenderManager = GetComponent<CustomRenderManager>();

        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
        PropertyID_StencilTex = Shader.PropertyToID("_StencilTex");
        PropertyID_Threshold = Shader.PropertyToID("_Threshold");
        PropertyID_ThresholdX = Shader.PropertyToID("_ThresholdX");
        PropertyID_ThresholdY = Shader.PropertyToID("_ThresholdY");
        PropertyID_OnX = Shader.PropertyToID("_OnX");
        PropertyID_OnY = Shader.PropertyToID("_OnY");

        InitSplitDirection(_materialCL, SplitDirection.Left);
        InitSplitDirection(_materialCR, SplitDirection.Right);
        InitSplitDirection(_materialLU, SplitDirection.LeftUp);
        InitSplitDirection(_materialLD, SplitDirection.LeftDown);
        InitSplitDirection(_materialRU, SplitDirection.RightUp);
        InitSplitDirection(_materialRD, SplitDirection.RightDown);
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

    #region Public Method
    public void SetCross(float duration)
    {
        _materials = new[]
        {
            _materialCL, _materialCR 
        };
        _isShowing = true;
        _backgroundPinner.Pin();
        var value = 0f;
        DOTween.To(() => value, num => value = num, GAP_WIDTH_CROSS, duration).OnUpdate(() =>
        {
            _materialCL.SetFloat(PropertyID_Threshold, value);
            _materialCR.SetFloat(PropertyID_Threshold, value);
        });
    }
    
    public void BackCross(float duration)
    {
        var value = GAP_WIDTH_CROSS;
        DOTween.To(() => value, num => value = num, 0f, duration).OnUpdate(() =>
        {
            _materialCL.SetFloat(PropertyID_Threshold, value);
            _materialCR.SetFloat(PropertyID_Threshold, value);
        }).OnComplete(() =>
        {
            _isShowing = false;
            _backgroundPinner.Unpin();
        });
    }

    public void SetLeft(float duration)
    {
        var value = 0f;
        DOTween.To(() => value, num => value = num, GAP_WIDTH, duration).OnUpdate(() =>
        {
            _materialLU.SetFloat(PropertyID_ThresholdX, value);
        });
    }
    
    public void SetRight(float duration)
    {
        _materials = new[]
        {
            _materialLU, _materialRU
        };
        _materialLU.SetInt(PropertyID_OnX, 1);
        _materialRU.SetInt(PropertyID_OnX, 1);
        _isShowing = true;
        _backgroundPinner.Pin();
        var value = 0f;
        DOTween.To(() => value, num => value = num, GAP_WIDTH, duration).OnUpdate(() =>
        {
            _materialRU.SetFloat(PropertyID_ThresholdX, value);
        });
    }
    
    public void SetUp(float duration)
    {
        _materials = new[]
        {
            _materialLU, _materialRU, _materialLD, _materialRD
        };
        _materialLD.SetInt(PropertyID_OnX, 1);
        _materialRD.SetInt(PropertyID_OnX, 1);
        _materialLU.SetInt(PropertyID_OnY, 1);
        _materialRU.SetInt(PropertyID_OnY, 1);
        _materialLD.SetInt(PropertyID_OnY, 1);
        _materialRD.SetInt(PropertyID_OnY, 1);
        _materialLD.SetFloat(PropertyID_ThresholdX, GAP_WIDTH);
        _materialRD.SetFloat(PropertyID_ThresholdX, GAP_WIDTH);
        var value = 0f;
        DOTween.To(() => value, num => value = num, GAP_WIDTH, duration).OnUpdate(() =>
        {
            _materialLU.SetFloat(PropertyID_ThresholdY, value);
            _materialRU.SetFloat(PropertyID_ThresholdY, value);
        });
    }
    
    public void SetDown(float duration)
    {
        var value = 0f;
        DOTween.To(() => value, num => value = num, GAP_WIDTH, duration).OnUpdate(() =>
        {
            _materialLD.SetFloat(PropertyID_ThresholdY, value);
            _materialRD.SetFloat(PropertyID_ThresholdY, value);
        });
    }
    
    public void BackAll(float duration)
    {
        var value = GAP_WIDTH;
        DOTween.To(() => value, num => value = num, 0f, duration).OnUpdate(() =>
        {
            _materialLU.SetFloat(PropertyID_ThresholdX, value);
            _materialRU.SetFloat(PropertyID_ThresholdX, value);
            _materialLD.SetFloat(PropertyID_ThresholdX, value);
            _materialRD.SetFloat(PropertyID_ThresholdX, value);
            _materialLU.SetFloat(PropertyID_ThresholdY, value);
            _materialRU.SetFloat(PropertyID_ThresholdY, value);
            _materialLD.SetFloat(PropertyID_ThresholdY, value);
            _materialRD.SetFloat(PropertyID_ThresholdY, value);
        }).OnComplete(() =>
        {
            _isShowing = false;
            _backgroundPinner.Unpin();
            _materialLU.SetInt(PropertyID_OnY, 0);
            _materialRU.SetInt(PropertyID_OnY, 0);
        });
    }
    
    #endregion

    #region Debug
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
    
    public void SeekThresholdU(Slider slider)
    {
        _materialLU.SetInt(PropertyID_OnY, 1);
        _materialRU.SetInt(PropertyID_OnY, 1);
        _materialLU.SetFloat(PropertyID_ThresholdY, slider.value);
        _materialRU.SetFloat(PropertyID_ThresholdY, slider.value);
    }
    
    public void SeekThresholdR(Slider slider)
    {
        _materialRU.SetFloat(PropertyID_ThresholdX, slider.value);
        _materialRD.SetFloat(PropertyID_ThresholdX, slider.value);
    }
    
    public void SeekThresholdB(Slider slider)
    {
        _materialLD.SetInt(PropertyID_OnY, 1);
        _materialRD.SetInt(PropertyID_OnY, 1);
        _materialLD.SetFloat(PropertyID_ThresholdY, slider.value);
        _materialRD.SetFloat(PropertyID_ThresholdY, slider.value);
    }
    
    public void SeekThresholdL(Slider slider)
    {
//        _materialLU.SetFloat(PropertyID_ThresholdX, slider.value);
//        _materialLD.SetFloat(PropertyID_ThresholdX, slider.value);
        _materialCL.SetFloat(PropertyID_Threshold, slider.value);
        _materialCR.SetFloat(PropertyID_Threshold, slider.value);
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
    #endregion

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

    private void InitSplitDirection(Material mat, SplitDirection dir)
    {
        mat.DisableKeyword("_SplitX_Left");
        mat.DisableKeyword("_SplitX_Right");
        mat.DisableKeyword("_SplitY_Up");
        mat.DisableKeyword("_SplitY_Down");

        switch (dir)
        {
            case SplitDirection.Left:
                mat.EnableKeyword("_SplitX_Left");
                break;
            case SplitDirection.Right:
                mat.EnableKeyword("_SplitX_Right");
                break;
            case SplitDirection.LeftUp:
                mat.EnableKeyword("_SplitX_Left");
                mat.EnableKeyword("_SplitY_Up");
                break;
            case SplitDirection.LeftDown:
                mat.EnableKeyword("_SplitX_Left");
                mat.EnableKeyword("_SplitY_Down");
                break;
            case SplitDirection.RightUp:
                mat.EnableKeyword("_SplitX_Right");
                mat.EnableKeyword("_SplitY_Up");
                break;
            case SplitDirection.RightDown:
                mat.EnableKeyword("_SplitX_Right");
                mat.EnableKeyword("_SplitY_Down");
                break;
        }

        mat.mainTexture = _customRenderManager.GetCameraFeed();
    }
}
