using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using DG.Tweening;

public class GridEffect : MonoBehaviour, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => true;
    
    [SerializeField] private Shader _shader;
    [SerializeField] private Material _material;
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    private UVAdjuster _uvAdjuster;
    private bool _isBackgroundEffect;

    private void Awake()
    {
        if (_shader)
        {
            _material = new Material(_shader);
        }
        _uvAdjuster = new UVAdjuster();
        _uvAdjuster.ReverseEffect(_material, _isBackgroundEffect);
    }

    private void Start()
    {
        ChangeThresholdAuto();
    }

    private void Update()
    {
        var humanStencil = _arOcclusionManager.humanStencilTexture;
        if (humanStencil)
        {
            _uvAdjuster.SetMaterialProperty(_material, humanStencil, UVAdjuster.TextureType.Stencil);
        }
    }

    public void ChangeThreshold(Slider slider)
    {
        _material.SetFloat("_Threshold", slider.value);
    }
    
    public void ReverseEffect()
    {
        _isBackgroundEffect = !_isBackgroundEffect;
        _uvAdjuster.ReverseEffect(_material, _isBackgroundEffect);
    }

    private void ChangeThresholdAuto()
    {
        var value = 0.05f;
        DOTween.To(() => value, num => value = num, 0.545f, 1.2f).OnUpdate(() =>
        {
            _material.SetFloat("_Threshold", value);
        }).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
