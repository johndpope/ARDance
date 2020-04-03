#ifndef UVAdjust_INCLUDED
#define UVAdjust_INCLUDED

float _UVMultiplierLandScape;
float _UVMultiplierPortrait;
float _UVFlip;
int _OnWide;

float2 UV1 (float2 uv)
{
    if(_OnWide == 1)
    {
        return float2(uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (uv.y / _UVMultiplierLandScape));
    }
    else
    {
        return float2(1.0 - uv.y, 1.0 - _UVMultiplierPortrait * 0.5f + uv.x / _UVMultiplierPortrait);
    }
}

float2 UV2(float2 uv)
{
    if(_OnWide == 1)
    {
        float2 temp = float2(uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (uv.y / _UVMultiplierLandScape));
        return float2(lerp(1.0 - temp.x, temp.x, _UVFlip), lerp(temp.y, 1.0 - temp.y, _UVFlip));
    }
    else
    {
        float2 temp = float2((1.0 - (_UVMultiplierPortrait * 0.5f)) + (uv.x / _UVMultiplierPortrait), uv.y);
        return float2(1.0 - temp.y, 1.0 - temp.x);
    }
}
#endif
