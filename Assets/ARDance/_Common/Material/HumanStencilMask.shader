﻿Shader "ARF/HumanStencilMask"
{
    Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_StencilTex ("StencilTex", 2D) = "white" {}
		[Toggle(ADJUST_UV)] _ShouldAdjustUV("ShouldAdjustUV", Float) = 0
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature ADJUST_UV

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
            
            sampler2D _MainTex;
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
                if(_OnWide == 1)
                {
                    o.uv1 = float2(v.uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (v.uv.y / _UVMultiplierLandScape));
                    o.uv2 = float2(lerp(1.0 - o.uv1.x, o.uv1.x, _UVFlip), lerp(o.uv1.y, 1.0 - o.uv1.y, _UVFlip));
                }
                else
                {
                    float2 forMask = float2((1.0 - (_UVMultiplierPortrait * 0.5f)) + (v.uv.x / _UVMultiplierPortrait), v.uv.y);
                    o.uv2 = float2(lerp(1.0 - forMask.y, forMask.y, 0), lerp(forMask.x, 1.0 - forMask.x, 1));
                }
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
#ifdef ADJUST_UV
                float stencilCol = tex2D(_StencilTex, i.uv2);
#else
                float stencilCol = tex2D(_StencilTex, i.uv);
#endif
                
                if (stencilCol < 1) {
                    discard;
                }
                return col;
            }
            ENDCG
        }
    }
}
