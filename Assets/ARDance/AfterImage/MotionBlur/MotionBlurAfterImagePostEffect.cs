using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MotionBlurAfterImagePostEffect : HumanSegmentationEffectBase, IScreenRTBlit, IScreenMeshDrawMulti
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffers.Last();

    public Material[] MeshMaterials => _materials.ToArray();
    public bool ShouldDraw => true;
    
    [SerializeField] private Shader _shader;

    private const int NUM_OF_IMAGES = 4;
    private const int FRAME_OF_INTERVAL = 1;

    private readonly (int, int)[] _humanStencilTextureResolution =
    {
        (256, 192), // Fastest
        (960, 720), // Medium
        (1920, 1440) // Best
    };

    private readonly List<Material> _materials = new List<Material>();
    private readonly List<RenderTexture> _stencilBuffers = new List<RenderTexture>();
    private readonly List<RenderTexture> _cameraFeedBuffers = new List<RenderTexture>();

    private void Start()
    {
        for (int i = 0; i < NUM_OF_IMAGES; i++)
        {
            var mat = new Material(_shader);
            if (i == NUM_OF_IMAGES - 1)
            {
                mat.SetFloat("_Alpha", 1f);
            }
            else
            {
                mat.SetFloat("_Alpha", 1f / NUM_OF_IMAGES);
            }
            _materials.Add(mat);
        }

        var resolution = (0, 0);
        switch (_arOcclusionManager.humanSegmentationStencilMode)
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

        var camera = GetComponent<Camera>();
        for (int i = 0; i < (NUM_OF_IMAGES - 1) * FRAME_OF_INTERVAL + 1; i++)
        {
            _cameraFeedBuffers.Add(new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0));
            _stencilBuffers.Add(new RenderTexture(resolution.Item1, resolution.Item2, 0));
        }
    }

    protected override void Update()
    {
        for (int i = 0; i < _cameraFeedBuffers.Count - 1; i++)
        {
            Graphics.Blit(_cameraFeedBuffers[i + 1], _cameraFeedBuffers[i]);
        }
        
        var humanStencil = _arOcclusionManager.humanStencilTexture;
        if (humanStencil)
        {
            for (int i = 0; i < _stencilBuffers.Count - 1; i++)
            {
                Graphics.Blit(_stencilBuffers[i + 1], _stencilBuffers[i]);
            }

            Graphics.Blit(humanStencil, _stencilBuffers.Last());

            for (int i = 0; i < _materials.Count; i++)
            {
                _uvAdjuster.SetMaterialProperty(_materials[i], _stencilBuffers[i * FRAME_OF_INTERVAL], UVAdjuster.TextureType.Stencil);
                _materials[i].mainTexture = _cameraFeedBuffers[i * FRAME_OF_INTERVAL];
            }
        }
    }
}
