using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenMeshDrawMultiRenderPassFeature : ScriptableRendererFeature
{
    class ScreenMeshDrawMultiRenderPass : ScriptableRenderPass
    {
        static readonly Matrix4x4 _backgroundOrthoProjection = Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f);

        Mesh _backgroundMesh;
        IScreenMeshDrawMulti _screenMeshDraw;

        public void Setup(Mesh backgroundMesh)
        {
            _backgroundMesh = backgroundMesh;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_screenMeshDraw != null)
            {
                if (_screenMeshDraw.ShouldDraw)
                {
                    var cmd = CommandBufferPool.Get(nameof(ScreenMeshDrawMultiRenderPass));
                    cmd.SetViewProjectionMatrices(Matrix4x4.identity, _backgroundOrthoProjection);
                    foreach (var mat in _screenMeshDraw.MeshMaterials)
                    {
                        cmd.DrawMesh(_backgroundMesh, Matrix4x4.identity, mat);
                    }
                    cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix,
                        renderingData.cameraData.camera.projectionMatrix);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                }
            }
            else
            {
                _screenMeshDraw = renderingData.cameraData.camera.GetComponent<IScreenMeshDrawMulti>();
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    ScreenMeshDrawMultiRenderPass _scriptablePass;
    Mesh _backgroundMesh;

    public override void Create()
    {
        _scriptablePass = new ScreenMeshDrawMultiRenderPass();
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
            _scriptablePass.Setup(_backgroundMesh);
            renderer.EnqueuePass(_scriptablePass);
        }
    }
}