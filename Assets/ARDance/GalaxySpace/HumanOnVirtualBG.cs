using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class HumanOnVirtualBG : HumanSegmentationEffectBase, IScreenMeshDrawMulti
{
    private enum EffectType
    {
        CameraFeed = 0, Galaxy
    }
    
    public Material[] MeshMaterials => _matsToDraw;
    public bool ShouldDraw => true;
    
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private Material _gridMat;
    [SerializeField] private Material _galaxyMat;
    [SerializeField] private Material _cameraFeedMat;

    private EffectType _currentEffect = EffectType.CameraFeed;
    private Material[] _matsToDraw = new Material[0];
    
    private void Start()
    {
        SetEffect(_currentEffect);
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
        _cameraFeedMat.SetFloat("_Threshold", slider.value);
    } 
    
    private void ChangeThresholdAuto()
    {
        var value = 0f;
        DOTween.To(() => value, num => value = num, 1.2f, 2f).OnUpdate(() =>
        {
            _gridMat.SetFloat("_Threshold", value);
        }).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void SetEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.CameraFeed:
                _matsToDraw = new [] { _galaxyMat, _cameraFeedMat };
                _humanSegmentMats.Add(_galaxyMat);
                _cameraFeedMats.Add(_cameraFeedMat);
                break;
            case EffectType.Galaxy:
                break;
        }
    }
}
