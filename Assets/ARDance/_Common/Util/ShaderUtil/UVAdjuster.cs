using UnityEngine;

public class UVAdjuster
{
    public enum TextureType
    {
        Stencil, Depth, CameraFeed, Main, None
    }
    
    private readonly int PropertyID_UVMultiplierLandScape = Shader.PropertyToID("_UVMultiplierLandScape");
    private readonly int PropertyID_UVMultiplierPortrait = Shader.PropertyToID("_UVMultiplierPortrait");
    private readonly int PropertyID_UVFlip = Shader.PropertyToID("_UVFlip");
    private readonly int PropertyID_OnWide = Shader.PropertyToID("_OnWide");
    private readonly int PropertyID_StencilTex = Shader.PropertyToID("_StencilTex");
    private readonly int PropertyID_DepthTex = Shader.PropertyToID("_DepthTex");
    private readonly int PropertyID_CameraFeed = Shader.PropertyToID("_CameraFeed");
    private readonly int PropertyID_OnReverseMul = Shader.PropertyToID("_OnReverseMul");
    private readonly int PropertyID_OnReversePlu = Shader.PropertyToID("_OnReversePlu");

    public void ReverseEffect(Material material, bool onBackground)
    {
        material.SetFloat(PropertyID_OnReverseMul, onBackground ? 1 : -1);
        material.SetFloat(PropertyID_OnReversePlu, onBackground ? 0 : -1);
    }

    public void SetMaterialProperty(Material material, Texture texture, TextureType type)
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            material.SetFloat(PropertyID_UVFlip, 0);
            material.SetInt(PropertyID_OnWide, 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            material.SetFloat(PropertyID_UVMultiplierLandScape, CalculateUVMultiplierLandScape(texture));
            material.SetFloat(PropertyID_UVFlip, 1);
            material.SetInt(PropertyID_OnWide, 1);
        }
        else
        {
            material.SetFloat(PropertyID_UVMultiplierPortrait, CalculateUVMultiplierPortrait(texture));
            material.SetInt(PropertyID_OnWide, 0);
        }

        SetTexture(material, texture, type);
    }

    public void SetTexture(Material material, Texture texture, TextureType type)
    {
        switch (type)
        {
            case TextureType.Stencil:
                material.SetTexture(PropertyID_StencilTex, texture);
                break;
            case TextureType.Depth:
                material.SetTexture(PropertyID_DepthTex, texture);
                break;
            case TextureType.CameraFeed:
                material.SetTexture(PropertyID_CameraFeed, texture);
                break;
            case TextureType.Main:
                material.mainTexture = texture;
                break;
            case TextureType.None:
                break;
        }
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
