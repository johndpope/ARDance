using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenMeshDrawRenderPassFeature : ScriptableRendererFeature
{
    class ScreenMeshDrawRenderPass : ScriptableRenderPass
    {
        static readonly Matrix4x4 _backgroundOrthoProjection = Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f);
        
        Mesh _backgroundMesh;
        RenderTargetIdentifier _colorTargetIdentifier;
        IScreenMeshDraw _screenMeshDraw;
        
        public void Setup(Mesh backgroundMesh, RenderTargetIdentifier colorTargetIdentifier)
        {
            _backgroundMesh = backgroundMesh;
            _colorTargetIdentifier = colorTargetIdentifier;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            //ConfigureTarget(_colorTargetIdentifier);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_screenMeshDraw != null)
            {
                if (_screenMeshDraw.ShouldDraw)
                {
                    var cmd = CommandBufferPool.Get(nameof(ScreenMeshDrawRenderPass));
                    cmd.SetViewProjectionMatrices(Matrix4x4.identity, _backgroundOrthoProjection);
                    cmd.DrawMesh(_backgroundMesh, Matrix4x4.identity, _screenMeshDraw.MeshMaterial);
                    cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix,
                        renderingData.cameraData.camera.projectionMatrix);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
            }
            else
            {
                _screenMeshDraw = renderingData.cameraData.camera.GetComponent<IScreenMeshDraw>();
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    ScreenMeshDrawRenderPass _scriptablePass;
    Mesh _backgroundMesh;

    public override void Create()
    {
        _scriptablePass = new ScreenMeshDrawRenderPass();
        _backgroundMesh = new Mesh();
        _backgroundMesh.vertices = new[]
        {
            new Vector3(0f, 0f, 0.1f),
            new Vector3(0f, 1f, 0.1f),
            new Vector3(1f, 1f, 0.1f),
            new Vector3(1f, 0f, 0.1f),
        };
        _backgroundMesh.uv = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
        };
        _backgroundMesh.triangles = new[] {0, 1, 2, 0, 2, 3};
        _scriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }    

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        Camera currentCamera = renderingData.cameraData.camera;
        if (currentCamera != null && currentCamera.cameraType == CameraType.Game)
        {
            _scriptablePass.Setup(_backgroundMesh, renderer.cameraColorTarget);
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}