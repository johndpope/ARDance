﻿Shader "Unlit/ColoredAfterImage"
{
    Properties
    {
        _Color ("_Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1; 
                float2 uv2 : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };
            
            fixed4 _Color;
            sampler2D _StencilTex;
            float _UVMultiplierLandScape;
            float _UVMultiplierPortrait;
            float _UVFlip;
            int _OnWide;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // Adjust UV
                if(_OnWide == 1)
                {
                    o.uv1 = float2(v.uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (v.uv.y / _UVMultiplierLandScape));
                    o.uv2 = float2(lerp(1.0 - o.uv1.x, o.uv1.x, _UVFlip), lerp(o.uv1.y, 1.0 - o.uv1.y, _UVFlip));
                }
                else
                {
                    o.uv1 = float2(1.0 - v.uv.y, 1.0 - _UVMultiplierPortrait * 0.5f + v.uv.x / _UVMultiplierPortrait);
                    float2 forMask = float2((1.0 - (_UVMultiplierPortrait * 0.5f)) + (v.uv.x / _UVMultiplierPortrait), v.uv.y);
                    o.uv2 = float2(lerp(1.0 - forMask.y, forMask.y, 0), lerp(forMask.x, 1.0 - forMask.x, 1));
                }
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                float stencilCol = tex2D(_StencilTex, i.uv2);
                if (stencilCol < 1) {
                    discard;
                }
                return col;
            }
            ENDCG
        }
    }
}
