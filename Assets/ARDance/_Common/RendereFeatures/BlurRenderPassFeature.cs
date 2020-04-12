using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderPassFeature : ScriptableRendererFeature
{
    class BlurRenderPass : ScriptableRenderPass
    {
        private IBlurPostProcess _blurPostProcess;
        private RenderTargetIdentifier _colorTarget;
        
        public void Setup(RenderTargetIdentifier colorTarget)
        {
            _colorTarget = colorTarget;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(_colorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_blurPostProcess != null)
            {
                var cmd = CommandBufferPool.Get(nameof(BlurRenderPass));
                var cameraData = renderingData.cameraData;
                var w = cameraData.camera.scaledPixelWidth / 2;
                var h = cameraData.camera.scaledPixelHeight / 2;
                var rt1 = Shader.PropertyToID("_temp1");
                var rt2 = Shader.PropertyToID("_temp2");
                cmd.GetTemporaryRT(rt1, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
                cmd.GetTemporaryRT(rt2, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
                float x = _blurPostProcess.Offset / w;
                float y = _blurPostProcess.Offset / h;
                cmd.Blit(_colorTarget, rt1);
                cmd.SetGlobalVector("_Offsets", new Vector4(x, 0, 0, 0));
                //_blurPostProcess.BlurMaterial.SetVector("_Offsets", new Vector4(x, 0, 0, 0));
                cmd.Blit(rt1, rt2, _blurPostProcess.BlurMaterial);
                cmd.SetGlobalVector("_Offsets", new Vector4(0, y, 0, 0));
                //_blurPostProcess.BlurMaterial.SetVector("_Offsets", new Vector4(0, y, 0, 0));
                cmd.Blit(rt2, _colorTarget, _blurPostProcess.BlurMaterial);
                cmd.ReleaseTemporaryRT(rt1);
                cmd.ReleaseTemporaryRT(rt2);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            else
            {
                _blurPostProcess = renderingData.cameraData.camera.GetComponent<IBlurPostProcess>();
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    BlurRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new BlurRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var currentCamera = renderingData.cameraData.camera;
        if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
        {
            m_ScriptablePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}
