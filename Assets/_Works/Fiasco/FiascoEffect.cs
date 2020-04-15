using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiascoEffect : HumanSegmentationEffectBase, IScreenRTBlit
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    
    [SerializeField] private Material _material;
    [SerializeField] private RawImage[] _rawImages;
    
    private RenderTexture _cameraFeedBuffer;

    private void Start()
    {
        _cameraFeedBuffer = new RenderTexture(Screen.width, Screen.height, 0);
        _material.mainTexture = _cameraFeedBuffer;
        foreach (var r in _rawImages)
        {
            r.texture = _cameraFeedBuffer;
        }
        _humanSegmentMats = new List<Material> { _material };
    }
}
