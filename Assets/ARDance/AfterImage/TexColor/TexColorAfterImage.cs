using UnityEngine;

public class TexColorAfterImage
{
    private readonly Camera _camera;
    private readonly Material _material;
    private readonly UVAdjuster _uvAdjuster;

    public TexColorAfterImage(Camera camera, Material material)
    {
        _camera = camera;
        _material = material;
        _uvAdjuster = new UVAdjuster();
    }

    public void Draw(RenderTexture cameraFeed)
    {
        _material.mainTexture = cameraFeed;
        Graphics.DrawTexture(new Rect(0, 0, _camera.pixelWidth, _camera.pixelHeight), cameraFeed, _material);
    }

    public void SetMaterialProperty(Texture humanStencilTexture)
    {
        _uvAdjuster.SetMaterialProperty(_material, humanStencilTexture, UVAdjuster.TextureType.Stencil);
    }
}
