using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;

public class EdgeDetect : MonoBehaviour, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => _isActive;
    
    private bool _isActive;
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private Material _material;
    private bool _isBackgroundEdge;

    private int PropertyID_UVMultiplierLandScape;
    private int PropertyID_UVMultiplierPortrait;
    private int PropertyID_UVFlip;
    private int PropertyID_OnWide;
    private int PropertyID_StencilTex;
    private int PropertyID_OnReverseMul;
    private int PropertyID_OnReversePlu;
    private int PropertyID_EdgeColor;
    
    private void Awake()
    {
        _isActive = true;

        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
        PropertyID_StencilTex = Shader.PropertyToID("_StencilTex");
        PropertyID_OnReverseMul = Shader.PropertyToID("_OnReverseMul");
        PropertyID_OnReversePlu = Shader.PropertyToID("_OnReversePlu");
        PropertyID_EdgeColor = Shader.PropertyToID("_EdgeColor");

        SwitchBackgroundEdge();
    }

    private void Start()
    {
        StartCoroutine(ClearText());
        //ChangeColor();
    }

    private void Update()
    {
        var humanStencil = _arOcclusionManager.humanStencilTexture;
        if (humanStencil)
        {
            SetMaterialProperty(humanStencil);
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                SwitchBackgroundEdge();
            }
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

    private void SwitchBackgroundEdge()
    {
        _isBackgroundEdge = !_isBackgroundEdge;
        _material.SetFloat(PropertyID_OnReverseMul, _isBackgroundEdge ? 1 : -1);
        _material.SetFloat(PropertyID_OnReversePlu, _isBackgroundEdge ? 0 : -1);
    }
    
    #region Debug

    [SerializeField] private Text debug;

    public void DebugLog(string log)
    {
        debug.text += log;
    }

    private IEnumerator ClearText()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(5);
            debug.text = "";
        }
    }
    #endregion

    public void SeekSensitivity(Slider slider)
    {
        //_material.SetFloat("_Sensitivity", slider.value);
        s = slider.value;
    }

    private float h = 0f;
    private float s = 1f;
    private float v = 1f;
    
    private void ChangeColor()
    {
        DOTween.To(
            () => h,
            num => h = num,
            1f,
            5.0f
        ).OnUpdate(() =>
        {
            _material.SetColor(PropertyID_EdgeColor, Color.HSVToRGB(h, s, v));
        }).SetLoops(-1, LoopType.Yoyo);
    }

//    private void OnValidate()
//    {
//        _materialForPostProcess.SetFloat("_Sensitivity", _sensitivity);
//        _materialForPostProcess.SetFloat("_Threshold", _threshold);
//        _materialForPostProcess.SetColor("_EdgeColor", _edgeColor);
//
//        _materialForPostProcess.DisableKeyword("Sobel");
//        _materialForPostProcess.DisableKeyword("Prewitt");
//        _materialForPostProcess.DisableKeyword("RobertsCross");
//
//        switch(_altorithm)
//        {
//            case AlgorithmType.Sobel:
//                _materialForPostProcess.EnableKeyword("Sobel");
//                break;
//            case AlgorithmType.Prewitt:
//                _materialForPostProcess.EnableKeyword("Prewitt");
//                break;
//            case AlgorithmType.RobertsCross:
//                _materialForPostProcess.EnableKeyword("RobertsCross");
//                break;
//        }
//    }
}
