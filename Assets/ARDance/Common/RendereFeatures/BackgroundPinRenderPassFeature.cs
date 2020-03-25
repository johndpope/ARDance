using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BackgroundPinRenderPassFeature : ScriptableRendererFeature
{
    class BackgroundPinRenderPass : ScriptableRenderPass
    {
        private RenderTargetIdentifier _currentTarget;
        private BackgroundPinner _backgroundPinner;

        public void Setup(RenderTargetIdentifier target)
        {
            _currentTarget = target;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_backgroundPinner)
            {
                if (_backgroundPinner.ShouldPin && _backgroundPinner.Background)
                {
                    var cmd = CommandBufferPool.Get(nameof(BackgroundPinRenderPassFeature));
                    cmd.Blit(_backgroundPinner.Background, _currentTarget, _backgroundPinner.BackgroundMaterial);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
            }
            else
            {
                _backgroundPinner = renderingData.cameraData.camera.GetComponent<BackgroundPinner>();
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }
    
    private BackgroundPinRenderPass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new BackgroundPinRenderPass();
        _scriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var currentCamera = renderingData.cameraData.camera;
        if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
        {
            _scriptablePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}
