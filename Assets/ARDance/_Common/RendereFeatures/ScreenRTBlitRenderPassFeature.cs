using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenRTBlitRenderPassFeature : ScriptableRendererFeature
{
    class ScreenRTBlitRenderPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier _currentTarget;
        private RenderTargetIdentifier _depthTarget;
        private RenderTexture _screenBuffer;

        public void Setup(RenderTargetIdentifier target, RenderTargetIdentifier depth, RenderTexture buffer)
        {
            _currentTarget = target;
            _depthTarget = depth;
            _screenBuffer = buffer;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(_currentTarget, _depthTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        { 
            var cmd = CommandBufferPool.Get(nameof(ScreenRTBlitRenderPass));
            cmd.Blit(_currentTarget, _screenBuffer);
            cmd.SetRenderTarget(_currentTarget, _depthTarget);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    ScreenRTBlitRenderPass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new ScreenRTBlitRenderPass();

        _scriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var currentCamera = renderingData.cameraData.camera;
        if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
        {
            var postEffect = currentCamera.GetComponent<IScreenRTBlit>();
            if (postEffect == null) return;
            if (postEffect.LatestCameraFeedBuffer == null) return;
            _scriptablePass.Setup(renderer.cameraColorTarget, renderer.cameraDepth, postEffect.LatestCameraFeedBuffer);
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}
