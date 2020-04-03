using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRenderManager : MonoBehaviour, IScreenRTBlit
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeedBuffer;
    [SerializeField] private RenderTexture _cameraFeedBuffer;

    public RenderTexture GetCameraFeed()
    {
        return _cameraFeedBuffer;
    }
}
