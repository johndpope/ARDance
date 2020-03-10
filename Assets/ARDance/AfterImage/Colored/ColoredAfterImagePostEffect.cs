using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(Camera))]
public class ColoredAfterImagePostEffect : MonoBehaviour, IScreenRTBlit
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    
    [SerializeField] private AROcclusionManager _occlusionManager;
    [SerializeField] private Shader _shader;
    [SerializeField] private Shader _shaderForCurrentFrame;
    [SerializeField] private Texture _texture;

    private const int NUM_OF_IMAGES = 4;
    private const int FRAME_OF_INTERVAL = 8;

    private readonly (int, int)[] _humanStencilTextureResolution =
    {
        (256, 192), // Fastest
        (960, 720), // Medium
        (1920, 1440) // Best
    };

    private readonly List<ColoredAfterImage> _afterImages = new List<ColoredAfterImage>();
    private readonly List<RenderTexture> _stencilBuffers = new List<RenderTexture>();
    private RenderTexture _cameraFeedBuffer;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        for (int i = 0; i < NUM_OF_IMAGES; i++)
        {
            if (i == NUM_OF_IMAGES - 1)
            {
                _afterImages.Add(new ColoredAfterImage(_camera, new Material(_shaderForCurrentFrame), null));
            }
            else
            {
                _afterImages.Add(new ColoredAfterImage(_camera, new Material(_shader), _texture));
            }
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
            _stencilBuffers.Add(new RenderTexture(resolution.Item1, resolution.Item2, 0));
        }

        _cameraFeedBuffer = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0);
    }

    private void Update()
    {
        var humanStencil = _occlusionManager.humanStencilTexture;
        if (humanStencil)
        {
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
                if (i == _afterImages.Count - 1)
                {
                    _afterImages[i].DrawCurrenetFrame(_cameraFeedBuffer);
                }
                else
                {
                    _afterImages[i].Draw();
                }
            }
        }
    }

    #region Debug

    [SerializeField] private Text debug;

    #endregion
}
