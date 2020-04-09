using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class Guardian : MonoBehaviour, IScreenRTBlit
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;

    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private Shader _shader;
    [SerializeField] private RawImage[] _rawImages;
    
    private RenderTexture _cameraFeedBuffer;
    private UVAdjuster _uvAdjuster;
    private Material _material0;
    private Material _material1;

    private void Awake()
    {
        _material0 = new Material(_shader);
        _material1 = new Material(_shader);
        //_material1.SetFloat("_Alpha", 0.5f);
        _cameraFeedBuffer = new RenderTexture(Screen.width, Screen.height, 0);
        _material0.mainTexture = _cameraFeedBuffer;
        foreach (var r in _rawImages)
        {
            r.texture = _cameraFeedBuffer;
            r.material = _material1;
        }
        _uvAdjuster = new UVAdjuster();
    }

    void Update()
    {
        var humanStencil = _arOcclusionManager.humanStencilTexture;
        if (humanStencil)
        {
            _uvAdjuster.SetMaterialProperty(_material0, humanStencil, UVAdjuster.TextureType.Stencil);
            _uvAdjuster.SetMaterialProperty(_material1, humanStencil, UVAdjuster.TextureType.Stencil);
        }
    }
    
    private void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _cameraFeedBuffer, _material0);
        }
    }
}
