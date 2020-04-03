#ifndef Edge_INCLUDED
#define Edge_INCLUDED

float4 _MainTex_TexelSize;
float _Sensitivity;
float _Threshold;
half4 _EdgeColor;
half4 _BaseColor;

fixed4 EdgeDetect (sampler2D MainTex, float2 uv)
{
    float2 duv = _MainTex_TexelSize.xy;
    half2 uv0 = uv + half2(-duv.x, -duv.y);
    half2 uv1 = uv + half2(0, -duv.y);
    half2 uv2 = uv + half2(duv.x, -duv.y);
    half2 uv3 = uv + half2(-duv.x, 0);
    half2 uv4 = uv + half2(0, 0);
    half2 uv5 = uv + half2(duv.x, 0);
    half2 uv6 = uv + half2(-duv.x, duv.y);
    half2 uv7 = uv + half2(0, duv.y);
    half2 uv8 = uv + half2(duv.x, duv.y);
    half3 col0 = tex2D(MainTex, uv0);
    half3 col1 = tex2D(MainTex, uv1);
    half3 col2 = tex2D(MainTex, uv2);
    half3 col3 = tex2D(MainTex, uv3);
    half3 col4 = tex2D(MainTex, uv4);
    half3 col5 = tex2D(MainTex, uv5);
    half3 col6 = tex2D(MainTex, uv6);
    half3 col7 = tex2D(MainTex, uv7);
    half3 col8 = tex2D(MainTex, uv8);
    float3 cg1 = col8 - col0;
    float3 cg2 = col6 - col2;
    float cg = sqrt(dot(cg1, cg1) + dot(cg2, cg2));
    half4 edge = cg * _Sensitivity;
    fixed4 edgeCol = _EdgeColor * saturate(edge - _Threshold);
    return edgeCol;
}

fixed4 EdgeWithTex (sampler2D MainTex, float2 uv)
{
    float2 duv = _MainTex_TexelSize.xy;
    half2 uv0 = uv + half2(-duv.x, -duv.y);
    half2 uv1 = uv + half2(0, -duv.y);
    half2 uv2 = uv + half2(duv.x, -duv.y);
    half2 uv3 = uv + half2(-duv.x, 0);
    half2 uv4 = uv + half2(0, 0);
    half2 uv5 = uv + half2(duv.x, 0);
    half2 uv6 = uv + half2(-duv.x, duv.y);
    half2 uv7 = uv + half2(0, duv.y);
    half2 uv8 = uv + half2(duv.x, duv.y);
    half3 col0 = tex2D(MainTex, uv0);
    half3 col1 = tex2D(MainTex, uv1);
    half3 col2 = tex2D(MainTex, uv2);
    half3 col3 = tex2D(MainTex, uv3);
    half3 col4 = tex2D(MainTex, uv4);
    half3 col5 = tex2D(MainTex, uv5);
    half3 col6 = tex2D(MainTex, uv6);
    half3 col7 = tex2D(MainTex, uv7);
    half3 col8 = tex2D(MainTex, uv8);
    float3 cg1 = col8 - col0;
    float3 cg2 = col6 - col2;
    float cg = sqrt(dot(cg1, cg1) + dot(cg2, cg2));
    half4 edge = cg * _Sensitivity;
    float th = saturate(edge - _Threshold);
    fixed4 texCol = tex2D(MainTex, uv);
    fixed4 col = _EdgeColor * th + texCol * (1 - th);
    return col;
}

fixed4 EdgeGreyScale (sampler2D MainTex, float2 uv)
{
    _EdgeColor = float4(0, 0, 0, 1);
    float2 duv = _MainTex_TexelSize.xy;
    half2 uv0 = uv + half2(-duv.x, -duv.y);
    half2 uv1 = uv + half2(0, -duv.y);
    half2 uv2 = uv + half2(duv.x, -duv.y);
    half2 uv3 = uv + half2(-duv.x, 0);
    half2 uv4 = uv + half2(0, 0);
    half2 uv5 = uv + half2(duv.x, 0);
    half2 uv6 = uv + half2(-duv.x, duv.y);
    half2 uv7 = uv + half2(0, duv.y);
    half2 uv8 = uv + half2(duv.x, duv.y);
    half3 col0 = tex2D(MainTex, uv0);
    half3 col1 = tex2D(MainTex, uv1);
    half3 col2 = tex2D(MainTex, uv2);
    half3 col3 = tex2D(MainTex, uv3);
    half3 col4 = tex2D(MainTex, uv4);
    half3 col5 = tex2D(MainTex, uv5);
    half3 col6 = tex2D(MainTex, uv6);
    half3 col7 = tex2D(MainTex, uv7);
    half3 col8 = tex2D(MainTex, uv8);
    float3 cg1 = col8 - col0;
    float3 cg2 = col6 - col2;
    float cg = sqrt(dot(cg1, cg1) + dot(cg2, cg2));
    half4 edge = cg * _Sensitivity;
    float th = saturate(edge - _Threshold);
    fixed grey = dot(tex2D(MainTex, uv).rgb, fixed3(0.299, 0.587, 0.114));
    fixed4 col = _EdgeColor * th + fixed4(grey, grey, grey, 1) * (1 - th);
    return col;
}

fixed4 EdgeMonocro (sampler2D MainTex, float2 uv)
{
    _EdgeColor = float4(0, 0, 0, 1);
    float2 duv = _MainTex_TexelSize.xy;
    half2 uv0 = uv + half2(-duv.x, -duv.y);
    half2 uv1 = uv + half2(0, -duv.y);
    half2 uv2 = uv + half2(duv.x, -duv.y);
    half2 uv3 = uv + half2(-duv.x, 0);
    half2 uv4 = uv + half2(0, 0);
    half2 uv5 = uv + half2(duv.x, 0);
    half2 uv6 = uv + half2(-duv.x, duv.y);
    half2 uv7 = uv + half2(0, duv.y);
    half2 uv8 = uv + half2(duv.x, duv.y);
    half3 col0 = tex2D(MainTex, uv0);
    half3 col1 = tex2D(MainTex, uv1);
    half3 col2 = tex2D(MainTex, uv2);
    half3 col3 = tex2D(MainTex, uv3);
    half3 col4 = tex2D(MainTex, uv4);
    half3 col5 = tex2D(MainTex, uv5);
    half3 col6 = tex2D(MainTex, uv6);
    half3 col7 = tex2D(MainTex, uv7);
    half3 col8 = tex2D(MainTex, uv8);
    float3 cg1 = col8 - col0;
    float3 cg2 = col6 - col2;
    float cg = sqrt(dot(cg1, cg1) + dot(cg2, cg2));
    half4 edge = cg * _Sensitivity;
    float th = saturate(edge - _Threshold);
    fixed4 col = _EdgeColor * th + _BaseColor * (1 - th);
    return col;
}
#endif
