#ifndef CameraFeed_INCLUDED
#define CameraFeed_INCLUDED

CBUFFER_START(UnityPerFrame)
// Device display transform is provided by the AR Foundation camera background renderer.
float4x4 _UnityDisplayTransform;
CBUFFER_END

float2 UVCameraFeed (float2 uv)
{
    // Remap the texture coordinates based on the device rotation.
    float2 texcoord = mul(float3(uv, 1.0f), _UnityDisplayTransform).xy;
    return texcoord;
}

CBUFFER_START(ARKitColorTransformations)
static const float4x4 s_YCbCrToSRGB = float4x4(
    float4(1.0h,  0.0000h,  1.4020h, -0.7010h),
    float4(1.0h, -0.3441h, -0.7141h,  0.5291h),
    float4(1.0h,  1.7720h,  0.0000h, -0.8860h),
    float4(0.0h,  0.0000h,  0.0000h,  1.0000h)
);
CBUFFER_END

Texture2D _textureY;
SamplerState sampler_textureY;
Texture2D _textureCbCr;
SamplerState sampler_textureCbCr;

float4 CameraColor (float2 uv)
{
    // Sample the video textures (in YCbCr).
    float4 ycbcr = float4(_textureY.Sample(sampler_textureY, uv).r,
                        _textureCbCr.Sample(sampler_textureCbCr, uv).rg,
                        1.0h);

    // Convert from YCbCr to sRGB.
    float4 videoColor = mul(s_YCbCrToSRGB, ycbcr);

#if !UNITY_COLORSPACE_GAMMA
    // If rendering in linear color space, convert from sRGB to RGB.
    videoColor.xyz = GammaToLinearSpace(videoColor.xyz);
#endif // !UNITY_COLORSPACE_GAMMA

    return videoColor;
}

#endif
