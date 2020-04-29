#ifndef ShaderTools_INCLUDED
#define ShaderTools_INCLUDED

// Hash function from H. Schechter & R. Bridson, goo.gl/RXiKaH
uint Hash(uint s)
{
	s ^= 2747636419u;
	s *= 2654435769u;
	s ^= s >> 16;
	s *= 2654435769u;
	s ^= s >> 16;
	s *= 2654435769u;
	return s;
}

float Random(uint seed)
{
	return float(Hash(seed)) / 4294967295.0; // 2^32-1
}

float Random2(float2 uv)
{
	return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453123);
}

inline fixed4 RGBtoHSL(fixed4 rgb) {
	fixed4 hsl = fixed4(0.0, 0.0, 0.0, rgb.w);
	
	fixed vMin = min(min(rgb.x, rgb.y), rgb.z);
	fixed vMax = max(max(rgb.x, rgb.y), rgb.z);
	fixed vDelta = vMax - vMin;
	
	hsl.z = (vMax + vMin) / 2.0;
	
	if (vDelta == 0.0) {
		hsl.x = hsl.y = 0.0;
	}
	else {
		if (hsl.z < 0.5) hsl.y = vDelta / (vMax + vMin);
		else hsl.y = vDelta / (2.0 - vMax - vMin);
		
		float rDelta = (((vMax - rgb.x) / 6.0) + (vDelta / 2.0)) / vDelta;
		float gDelta = (((vMax - rgb.y) / 6.0) + (vDelta / 2.0)) / vDelta;
		float bDelta = (((vMax - rgb.z) / 6.0) + (vDelta / 2.0)) / vDelta;
		
		if (rgb.x == vMax) hsl.x = bDelta - gDelta;
		else if (rgb.y == vMax) hsl.x = (1.0 / 3.0) + rDelta - bDelta;
		else if (rgb.z == vMax) hsl.x = (2.0 / 3.0) + gDelta - rDelta;
		
		if (hsl.x < 0.0) hsl.x += 1.0;
		if (hsl.x > 1.0) hsl.x -= 1.0;
	}
	
	return hsl;
}

inline fixed hueToRGB(float v1, float v2, float vH) {
	if (vH < 0.0) vH+= 1.0;
	if (vH > 1.0) vH -= 1.0;
	if ((6.0 * vH) < 1.0) return (v1 + (v2 - v1) * 6.0 * vH);
	if ((2.0 * vH) < 1.0) return (v2);
	if ((3.0 * vH) < 2.0) return (v1 + (v2 - v1) * ((2.0 / 3.0) - vH) * 6.0);
	return v1;
}

inline fixed4 HSLtoRGB(fixed4 hsl) {
	fixed4 rgb = fixed4(0.0, 0.0, 0.0, hsl.w);
	
	if (hsl.y == 0) {
		rgb.xyz = hsl.zzz;
	}
	else {
		float v1;
		float v2;
		
		if (hsl.z < 0.5) v2 = hsl.z * (1 + hsl.y);
		else v2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);
		
		v1 = 2.0 * hsl.z - v2;
		
		rgb.x = hueToRGB(v1, v2, hsl.x + (1.0 / 3.0));
		rgb.y = hueToRGB(v1, v2, hsl.x);
		rgb.z = hueToRGB(v1, v2, hsl.x - (1.0 / 3.0));
	}
	
	return rgb;
}

inline float4 rgb2hsv(float4 rgb)
{
    float4 hsv = float4(0, 0, 0, rgb.w);

    // RGBの三つの値で最大のもの
    float maxValue = max(rgb.r, max(rgb.g, rgb.b));
    // RGBの三つの値で最小のもの
    float minValue = min(rgb.r, min(rgb.g, rgb.b));
    // 最大値と最小値の差
    float delta = maxValue - minValue;
    
    // V（明度）
    // 一番強い色をV値にする
    hsv.z = maxValue;
    
    // S（彩度）
    // 最大値と最小値の差を正規化して求める
    if (maxValue != 0.0){
        hsv.y = delta / maxValue;
    } else {
        hsv.y = 0.0;
    }
    
    // H（色相）
    // RGBのうち最大値と最小値の差から求める
    if (hsv.y > 0.0){
        if (rgb.r == maxValue) {
            hsv.x = (rgb.g - rgb.b) / delta;
        } else if (rgb.g == maxValue) {
            hsv.x = 2 + (rgb.b - rgb.r) / delta;
        } else {
            hsv.x = 4 + (rgb.r - rgb.g) / delta;
        }
        hsv.x /= 6.0;
        if (hsv.x < 0)
        {
            hsv.x += 1.0;
        }
    }
    
    return hsv;
}
        
// HSV->RGB変換
inline float4 hsv2rgb(float4 hsv)
{
    float4 rgb = float4(0, 0, 0, hsv.w);

    if (hsv.y == 0){
        // S（彩度）が0と等しいならば無色もしくは灰色
        rgb.r = rgb.g = rgb.b = hsv.z;
    } else {
        // 色環のH（色相）の位置とS（彩度）、V（明度）からRGB値を算出する
        hsv.x *= 6.0;
        float i = floor (hsv.x);
        float f = hsv.x - i;
        float aa = hsv.z * (1 - hsv.y);
        float bb = hsv.z * (1 - (hsv.y * f));
        float cc = hsv.z * (1 - (hsv.y * (1 - f)));
        if( i < 1 ) {
            rgb.r = hsv.z;
            rgb.g = cc;
            rgb.b = aa;
        } else if( i < 2 ) {
            rgb.r = bb;
            rgb.g = hsv.z;
            rgb.b = aa;
        } else if( i < 3 ) {
            rgb.r = aa;
            rgb.g = hsv.z;
            rgb.b = cc;
        } else if( i < 4 ) {
            rgb.r = aa;
            rgb.g = bb;
            rgb.b = hsv.z;
        } else if( i < 5 ) {
            rgb.r = cc;
            rgb.g = aa;
            rgb.b = hsv.z;
        } else {
            rgb.r = hsv.z;
            rgb.g = aa;
            rgb.b = bb;
        }
    }
    return rgb;
}

inline float3 hsv2rgbShort(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

inline float3 rgb2hsvShort(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
 
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
#endif
