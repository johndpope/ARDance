using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;

public class BlurPostProcess : MonoBehaviour, IBlurPostProcess, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => true;
    public Material BlurMaterial => _material;
    public float Offset => _offset;

    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private Shader _shader;
    [SerializeField] private Material _material;
    [SerializeField, Range(1f, 10f)] private float _offset;
    [SerializeField, Range(10f, 1000f)] private float _blur;
    
    private float[] _weights = new float[10];
    private bool _isInitialized;
    private UVAdjuster _uvAdjuster;
    private bool _isBackgroundEffect;
    
    private readonly int PropertyID_Weights = Shader.PropertyToID("_Weights");
    private readonly int PropertyID_BlurColorPower = Shader.PropertyToID("_BlurColorPower");
    private readonly int PropertyID_Sensitivity = Shader.PropertyToID("_Sensitivity");
    private readonly int PropertyID_BlurColor = Shader.PropertyToID("_BlurColor");
    private readonly int PropertyID_EdgeColor = Shader.PropertyToID("_EdgeColor");
    
    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        ChangeColor();
    }

    private void OnValidate() 
    { 
        if (!Application.isPlaying) return; 
        if (!_isInitialized) 
        { 
            Initialize(); 
        }
        else
        { 
            UpdateWeights(); 
        }
    }

    private void Update()
    {
        var humanStencil = _arOcclusionManager.humanStencilTexture;
        if (humanStencil)
        {
            _uvAdjuster.SetMaterialProperty(_material, humanStencil, UVAdjuster.TextureType.Stencil);
        }
    }

    public void ReverseEffect()
    {
        _isBackgroundEffect = !_isBackgroundEffect;
        _uvAdjuster.ReverseEffect(_material, _isBackgroundEffect);
    }

    public void ChangeBlur(Slider slider)
    {
        _blur = slider.value;
        UpdateWeights();
    }
    
    public void ChangeOffset(Slider slider)
    {
        _offset = slider.value;
    }
    
    public void ChangeBlurColorPower(Slider slider)
    {
        _material.SetFloat(PropertyID_BlurColorPower, slider.value);
    }
    
    public void ChangeSensitivity(Slider slider)
    {
        _material.SetFloat(PropertyID_Sensitivity, slider.value);
    }

    private void ChangeColor()
    {
        var hue = 0f;
        DOTween.To(() => hue, num => hue = num, 1f, 5f).OnUpdate(() =>
        {
            _material.SetColor(PropertyID_BlurColor, Color.HSVToRGB(hue, 1, 1));
            _material.SetColor(PropertyID_EdgeColor, Color.HSVToRGB(1-hue, 1, 1));
        }).SetLoops(-1, LoopType.Restart);
    }
    
    private void Initialize()
    {
        if (_isInitialized) return;
        _uvAdjuster = new UVAdjuster();
        if (_shader)
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.HideAndDontSave;
        }
        _uvAdjuster.ReverseEffect(_material, _isBackgroundEffect);
        UpdateWeights();
        _isInitialized = true;
    }

    private void UpdateWeights()
    {
        float total = 0;
        float d = _blur * _blur * 0.001f;

        for (int i = 0; i < _weights.Length; i++)
        {
            // Offset position per x.
            float x = i * 2f;
            float w = Mathf.Exp(-0.5f * (x * x) / d);
            _weights[i] = w;

            if (i > 0)
            {
                w *= 2.0f;
            }

            total += w;
        }

        for (int i = 0; i < _weights.Length; i++)
        {
            _weights[i] /= total;
        }
        
        _material.SetFloatArray(PropertyID_Weights, _weights);
    }
}
