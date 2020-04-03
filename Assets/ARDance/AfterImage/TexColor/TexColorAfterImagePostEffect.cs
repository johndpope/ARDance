using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Camera))]
public class TexColorAfterImagePostEffect : MonoBehaviour, IScreenRTBlit
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffers.Last();
    
    [SerializeField] private AROcclusionManager _occlusionManager;
    [SerializeField] private Material[] _materials;

    private const int NUM_OF_IMAGES = 6;
    private const int FRAME_OF_INTERVAL = 5;

    private readonly (int, int)[] _humanStencilTextureResolution =
    {
        (256, 192), // Fastest
        (960, 720), // Medium
        (1920, 1440) // Best
    };

    private readonly List<TexColorAfterImage> _afterImages = new List<TexColorAfterImage>();
    private readonly List<RenderTexture> _stencilBuffers = new List<RenderTexture>();
    private readonly List<RenderTexture> _cameraFeedBuffers = new List<RenderTexture>();

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        for (int i = 0; i < NUM_OF_IMAGES; i++)
        {
            _afterImages.Add(new TexColorAfterImage(_camera, _materials[i]));
        }

        var resolution = (0, 0);
        switch (_occlusionManager.humanSegmentationStencilMode)
        {
            case SegmentationStencilMode.Fastest:
                resolution = _humanStencilTextureResolution[0];
                break;
            case SegmentationStencilMode.Medium:
                resolution = _humanStencilTextureResolution[1];
                break;
            case SegmentationStencilMode.Best:
                resolution = _humanStencilTextureResolution[2];
                break;
        }

        for (int i = 0; i < (NUM_OF_IMAGES - 1) * FRAME_OF_INTERVAL + 1; i++)
        {
            _cameraFeedBuffers.Add(new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0));
            _stencilBuffers.Add(new RenderTexture(resolution.Item1, resolution.Item2, 0));
        }
    }

    private void Update()
    {
        for (int i = 0; i < _cameraFeedBuffers.Count - 1; i++)
        {
            Graphics.Blit(_cameraFeedBuffers[i + 1], _cameraFeedBuffers[i]);
        }
        
        var humanStencil = _occlusionManager.humanStencilTexture;
        if (humanStencil)
        {
            if (_cameraFeedBuffers.Last().width != _camera.pixelWidth)
            {
                ReInitCameraFeedBuffers();
            }
            
            for (int i = 0; i < _stencilBuffers.Count - 1; i++)
            {
                Graphics.Blit(_stencilBuffers[i + 1], _stencilBuffers[i]);
            }

            Graphics.Blit(humanStencil, _stencilBuffers.Last());

            for (int i = 0; i < _afterImages.Count; i++)
            {
                _afterImages[i].SetMaterialProperty(_stencilBuffers[i * FRAME_OF_INTERVAL]);
            }
        }
    }

    private void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {
            for (int i = 0; i < _afterImages.Count; i++)
            {
                _afterImages[i].Draw(_cameraFeedBuffers[i * FRAME_OF_INTERVAL]);
            }
        }
    }
    
    private void ReInitCameraFeedBuffers()
    {
        var total = _cameraFeedBuffers.Count;
        foreach (var cameraFeed in _cameraFeedBuffers)
        {
            cameraFeed.Release();
        }
        _cameraFeedBuffers.Clear();
        for (int i = 0; i < total; i++)
        {
            _cameraFeedBuffers.Add(new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0));
        }
    }

    #region Debug

    [SerializeField] private Text debug;

    #endregion
}
