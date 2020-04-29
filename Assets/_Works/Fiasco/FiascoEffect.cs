using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FiascoEffect : HumanSegmentationEffectBase, IScreenRTBlit, IAddtionalPostProcess
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    public Material MaterialForPostProcess => _humanOcclusionMaterial;
    public bool IsEnable => true;
    
    [SerializeField] private Material _humanOcclusionMaterial;
    [SerializeField] private Material _depthMaskMaterial;
    [SerializeField] private Rotater[] _rotaters;
    [SerializeField] private Transform _baseShriken;
    [SerializeField] private AudioSource _audioSource;
    private RenderTexture _cameraFeedBuffer;
    private AudioSpectrum _audioSpectrum;
    private List<int> _indexBox;
    
    private void Start()
    {
        _cameraFeedBuffer = new RenderTexture(Screen.width, Screen.height, 0);
        _humanSegmentMats = new List<Material> { _humanOcclusionMaterial, _depthMaskMaterial };
        _uvAdjuster.SetTexture(_humanOcclusionMaterial, _cameraFeedBuffer, UVAdjuster.TextureType.CameraFeed);
        _uvAdjuster.SetTexture(_depthMaskMaterial, _cameraFeedBuffer, UVAdjuster.TextureType.Main);
        StartCoroutine(OnStartEffect());
        _baseShriken.DOLocalRotate(new Vector3(0, 360, 0), 3f)
            .SetRelative()
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        
        _audioSpectrum = new AudioSpectrum(_audioSource, 90);
    }

    protected override void Update()
    {
        base.Update();
        _audioSpectrum.GetSpectrum(); 
        var value = _audioSpectrum.Max * 5f;
        if (value > 1f)
        {
            _indexBox = new List<int> { 0, 1, 2, 3, 4, 5 };
            foreach (var rotater in _rotaters)
            {
                var r = Random.Range(0, _indexBox.Count);
                rotater.RandomSetPosition(_indexBox[r]);
                _indexBox.RemoveAt(r);
            }
        }
    }

    private IEnumerator OnStartEffect()
    {
        while (gameObject.activeSelf)
        {
            foreach (var rotater in _rotaters)
            {
                rotater.StartRotate();
            }
            yield return new WaitForSeconds(22);
        }
    }

    public void ChangeZPos(Slider slider)
    {
        _baseShriken.position = new Vector3(_baseShriken.position.x, _baseShriken.position.y, slider.value);
    }
    
    public void ChangeYPos(Slider slider)
    {
        _baseShriken.position = new Vector3(_baseShriken.position.x, slider.value, _baseShriken.position.z);
    }
    
    public void ChangeXRot(Slider slider)
    {
        _baseShriken.eulerAngles = new Vector3(slider.value, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
