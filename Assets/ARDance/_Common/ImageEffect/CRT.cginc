#ifndef CRT_INCLUDED
#define CRT_INCLUDED

float ScanLineH (float2 uv)
{
    float scanLineColor = sin(_Time.y * 10 + uv.y * 500) / 2 + 0.5;
	return 0.5 + clamp(scanLineColor + 0.5, 0, 1) * 0.5;
}

float ScanLineV (float2 uv)
{
    float scanLineColor = sin(_Time.y * 10 + uv.x * 500) / 2 + 0.5;
	return 0.5 + clamp(scanLineColor + 0.5, 0, 1) * 0.5;
}
#endif
