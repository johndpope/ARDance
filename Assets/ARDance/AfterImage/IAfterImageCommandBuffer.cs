using UnityEngine;

public interface IAfterImageCommandBuffer
{
    RenderTexture LatestCameraFeedBuffer { get; }
}
