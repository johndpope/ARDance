using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class HumanOnVirtualBG : HumanSegmentationEffectBase, IScreenMeshDrawMulti, IAddtionalPostProcess
{
    public enum EffectType
    {
        CameraFeed,
        Galaxy,
        Grid,
        CRT
    }

    public Material[] MeshMaterials => _matsToDraw;
    public bool ShouldDraw => true;
    
    public Material MaterialForPostProcess => _crtMat;
    public bool IsEnable => _isCRTOn;

    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private Material _gridMat;
    [SerializeField] private Material _galaxyMat;
    [SerializeField] private Material _crtMat;
    [SerializeField] private Material _cameraFeedMat;
    [SerializeField] private Material _stencilMaskMat;

    private EffectType _currentEffect = EffectType.CameraFeed;
    private Material[] _matsToDraw = new Material[0];
    private bool _isCRTOn;
    private CRTRandomizer _crtRandomizer;
    private Tween _gridEffectTween;
    private Coroutine _cRTEffectCoroutine;

    private readonly int PropertyID_Threshold = Shader.PropertyToID("_Threshold");
    private readonly int PropertyID_TransitionThreshold = Shader.PropertyToID("_TransitionThreshold");

    private void Start()
    {
        _crtRandomizer = new CRTRandomizer();
        SetEffect(_currentEffect);
    }

    protected override void Update()
    {
        base.Update();
        _crtRandomizer.ApplyPrameter(_crtMat);
    }

    private void OnEnable()
    {
        _arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        _arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    public void ChangeThreshold(Slider slider)
    {
        //_cameraFeedMat.SetFloat("_Threshold", slider.value);
        //_crtMat.SetFloat(PropertyID_TransitionThreshold, slider.value);
        _gridMat.SetFloat("_Width", slider.value);
    }

    private void ChangeThresholdAuto()
    {
        var value = 0.05f;
        _gridEffectTween = DOTween.To(() => value, num => value = num, 0.545f, 1.2f)
            .OnUpdate(() => { _gridMat.SetFloat(PropertyID_Threshold, value); })
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public void SetEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.CameraFeed:
                _isCRTOn = false;
                if (_cRTEffectCoroutine != null) StopCoroutine(_cRTEffectCoroutine);
                _galaxyMat.SetFloat(PropertyID_TransitionThreshold, 0);
                _matsToDraw = new[] {_galaxyMat, _cameraFeedMat};
                _humanSegmentMats = new List<Material> { _galaxyMat };
                _cameraFeedMats = new List<Material> { _galaxyMat, _cameraFeedMat };
                _cameraFeedMat.SetFloat(PropertyID_Threshold, 1.2f);
                break;
            case EffectType.Galaxy:
                var value = 1f;
                DOTween.To(() => value, num => value = num, 0f, 2f)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => { _cameraFeedMat.SetFloat(PropertyID_Threshold, value); })
                    .OnComplete(() =>
                    {
                        _matsToDraw = new[] { _galaxyMat };
                        _cameraFeedMats.Remove(_cameraFeedMat);
                    });
                break;
            case EffectType.Grid:
                Transit(true, _galaxyMat, 0.4f)
                    .OnComplete(() =>
                    {
                        _matsToDraw = new[] { _gridMat };
                        _humanSegmentMats = new List<Material> { _gridMat };
                        _cameraFeedMats = new List<Material> { _gridMat };
                        ChangeThresholdAuto();
                        Transit(false, _gridMat, 0.4f);
                    });
                break;
            case EffectType.CRT:
                Transit(true, _gridMat, 0.4f)
                    .OnComplete(() =>
                    {
                        if (_gridEffectTween != null) _gridEffectTween.Kill();
                        _matsToDraw = new[] { _stencilMaskMat };
                        _humanSegmentMats = new List<Material> { _stencilMaskMat, _crtMat };
                        _isCRTOn = true;
                        _cRTEffectCoroutine = StartCoroutine(RandomInterval());
                        Transit(false, _crtMat, 0.4f);
                    });
                break;
        }
        _currentEffect = type;
    }

    private Tween Transit(bool isOut, Material mat, float duration)
    {
        var value = isOut ? 0f : 1f;
        _galaxyMat.SetFloat(PropertyID_TransitionThreshold, value);
        return DOTween.To(() => value, num => value = num, isOut ? 1f : 0f, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => { mat.SetFloat(PropertyID_TransitionThreshold, value); });
    }
    
    private IEnumerator RandomInterval()
    {
        while (_isCRTOn)
        {
            float wait = Random.Range(0.2f, 0.8f);
            _crtRandomizer.SetParameter(_crtMat, wait);
            yield return new WaitForSeconds(wait);
        }
    }
}