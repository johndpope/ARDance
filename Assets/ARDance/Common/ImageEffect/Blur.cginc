#ifndef Blur_INCLUDED
#define Blur_INCLUDED

half4 _Offsets;
static const int samplingCount = 10;
half _Weights[samplingCount];
sampler2D _StencilTex;
float4 _BlurColor;
float _BlurColorPower;

fixed4 SimpleBlur (sampler2D MainTex, float2 uv)
{
    half4 col = 0;    
    [unroll]
    for (int j = samplingCount - 1; j > 0; j--)
    {
        col += tex2D(MainTex, uv - (_Offsets.xy * j)) * _Weights[j];
    }
    [unroll]
    for (int j = 0; j < samplingCount; j++)
    {
        col += tex2D(MainTex, uv + (_Offsets.xy * j)) * _Weights[j];
    }
    return col;
}

fixed4 BlurWithColor (sampler2D MainTex, float2 uv)
{
    half4 col = 0; 
    [unroll]
    for (int j = samplingCount - 1; j > 0; j--)
    {
        col += lerp(tex2D(MainTex, uv - (_Offsets.xy * j)), _BlurColor, _BlurColorPower) * _Weights[j];
    }
    [unroll]
    for (int j = 0; j < samplingCount; j++)
    {
        col += lerp(tex2D(MainTex, uv - (_Offsets.xy * j)), _BlurColor, _BlurColorPower) * _Weights[j];
    }
    return col;
}
#endif
