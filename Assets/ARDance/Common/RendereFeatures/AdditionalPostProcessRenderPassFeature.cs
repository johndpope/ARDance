using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AdditionalPostProcessRenderPassFeature : ScriptableRendererFeature
{
    class AdditionalPostProcessRenderPass : ScriptableRenderPass
    {
        private IAddtionalPostProcess _postProcess;
        private RenderTargetIdentifier _currentTarget;

        public void Setup(RenderTargetIdentifier target)
        {
            _currentTarget = target;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_postProcess != null)
            {
                if (_postProcess.IsEnable && _postProcess.MaterialForPostProcess)
                {
                    var cmd = CommandBufferPool.Get(nameof(AdditionalPostProcessRenderPass));
                    var cameraData = renderingData.cameraData;
                    var w = cameraData.camera.scaledPixelWidth;
                    var h = cameraData.camera.scaledPixelHeight;
                    var renderTextureId = Shader.PropertyToID("_Sample");
                    cmd.GetTemporaryRT(renderTextureId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
                    cmd.Blit(_currentTarget, renderTextureId);
                    cmd.Blit(renderTextureId, _currentTarget, _postProcess.MaterialForPostProcess);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
            }
            else
            {
                _postProcess = renderingData.cameraData.camera.GetComponent<IAddtionalPostProcess>();
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    AdditionalPostProcessRenderPass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new AdditionalPostProcessRenderPass();

        _scriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
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
