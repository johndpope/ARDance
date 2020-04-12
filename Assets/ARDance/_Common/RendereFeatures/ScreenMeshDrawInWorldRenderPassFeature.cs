using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenMeshDrawInWorldRenderPassFeature : ScriptableRendererFeature
{
    class ScreenMeshDrawInWorldRenderPass : ScriptableRenderPass
    {
        RenderTargetIdentifier _colorTargetIdentifier;
        RenderTargetIdentifier _depthTarget;
        IScreenMeshDrawInWorld _screenMeshDraw;
        
        public void Setup(RenderTargetIdentifier colorTargetIdentifier, RenderTargetIdentifier depth)
        {
            _colorTargetIdentifier = colorTargetIdentifier;
            _depthTarget = depth;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(_colorTargetIdentifier, _depthTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_screenMeshDraw != null)
            {
                if (_screenMeshDraw.ShouldDraw)
                {
                    var cmd = CommandBufferPool.Get(nameof(ScreenMeshDrawInWorldRenderPass));
                    cmd.DrawMesh(_screenMeshDraw.QuadMesh, _screenMeshDraw.Matrix, _screenMeshDraw.MeshMaterial);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
            }
            else
            {
                _screenMeshDraw = renderingData.cameraData.camera.GetComponent<IScreenMeshDrawInWorld>();
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    ScreenMeshDrawInWorldRenderPass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new ScreenMeshDrawInWorldRenderPass();
        _scriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        Camera currentCamera = renderingData.cameraData.camera;
        if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
        {
            _scriptablePass.Setup(renderer.cameraColorTarget, renderer.cameraDepth);
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}