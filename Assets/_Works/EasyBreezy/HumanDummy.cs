using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HumanDummy : MonoBehaviour, IScreenRTBlit
{

    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private UVAdjuster _uvAdjuster;

    private RenderTexture _cameraFeedBuffer;

    private void Awake()
    {
        _cameraFeedBuffer = new RenderTexture(Screen.width, Screen.height, 0); 
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public RenderTexture GetHumanDummy()
    {
        var rt = new RenderTexture(Screen.width, Screen.height, 0);
        Graphics.Blit(_cameraFeedBuffer, rt);
        return rt;
    }
}
