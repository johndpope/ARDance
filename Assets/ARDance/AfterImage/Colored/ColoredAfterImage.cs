using UnityEngine;

public class ColoredAfterImage
{
    private readonly Camera _camera;
    private readonly Material _material;
    private readonly Texture _texture;

    private readonly int PropertyID_UVMultiplierLandScape;
    private readonly int PropertyID_UVMultiplierPortrait;
    private readonly int PropertyID_UVFlip;
    private readonly int PropertyID_OnWide;
    private readonly int PropertyID_StencilTex;

    public ColoredAfterImage(Camera camera, Material material, Texture texture)
    {
        _camera = camera;
        _material = material;
        _texture = texture;

        PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
        PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
        PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
        PropertyID_OnWide = Shader.PropertyToID("_OnWide");
        PropertyID_StencilTex = Shader.PropertyToID("_StencilTex");
    }

    public void Draw()
    {
        _material.color = new Color(
            Random.Range(0f, 1f), 
            Random.Range(0f, 1f), 
            Random.Range(0f, 1f)
            );
        Graphics.DrawTexture(new Rect(0, 0, _camera.pixelWidth, _camera.pixelHeight), _texture, _material);
    }
    
    public void DrawCurrenetFrame(RenderTexture cameraFeed)
    {
        _material.mainTexture = cameraFeed;
        Graphics.DrawTexture(new Rect(0, 0, _camera.pixelWidth, _camera.pixelHeight), cameraFeed, _material);
    }

    public void SetMaterialProperty(Texture humanStencilTexture)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            _material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanStencilTexture));
            _material.SetFloat(PropertyID_UVFlip, 0);
            _material.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            _material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(humanStencilTexture));
            _material.SetFloat(PropertyID_UVFlip, 1);
            _material.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            _material.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(humanStencilTexture));
            _material.SetInt(PropertyID_OnWide, 0);
        }

        _material.SetTexture(PropertyID_StencilTex, humanStencilTexture);
    }

    private float CalculateUVMultiplierLandScape(Texture textureFromAROcclusionManager)
    {
        float screenAspect = (float) Screen.width / Screen.height;
        float cameraTextureAspect = (float) textureFromAROcclusionManager.width / textureFromAROcclusionManager.height;
        return screenAspect / cameraTextureAspect;
    }

    private float CalculateUVMultiplierPortrait(Texture textureFromAROcclusionManager)
    {
        float screenAspect = (float) Screen.height / Screen.width;
        float cameraTextureAspect = (float) textureFromAROcclusionManager.width / textureFromAROcclusionManager.height;
        return screenAspect / cameraTextureAspect;
    }
}
