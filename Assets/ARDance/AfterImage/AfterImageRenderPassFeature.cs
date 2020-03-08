using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AfterImageRenderPassFeature : ScriptableRendererFeature
{
    class AfterImageRenderPass : ScriptableRenderPass
    {
        RenderTargetIdentifier _currentTarget;
        RenderTexture _screenBuffer;
        
        public void Setup(RenderTargetIdentifier target, RenderTexture buffer)
        {
            _currentTarget = target;
            _screenBuffer = buffer;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        { 
            var cmd = CommandBufferPool.Get(nameof(AfterImageRenderPass));
            cmd.Blit(_currentTarget, _screenBuffer);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    AfterImageRenderPass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new AfterImageRenderPass();

        _scriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var currentCamera = renderingData.cameraData.camera;
        if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
        {
            var postEffect = currentCamera.GetComponent<IAfterImageCommandBuffer>();
            if (postEffect == null) return;
            if (postEffect.LatestCameraFeedBuffer == null) return;
            _scriptablePass.Setup(renderer.cameraColorTarget, postEffect.LatestCameraFeedBuffer);
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}
