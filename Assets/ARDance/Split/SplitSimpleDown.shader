Shader "Unlit/SplitSimpleDown"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _Threshold ("_Threshold", Range(0.0, 0.5)) = 0.1
        [HDR] _EmissionColor ("_EmissionColor", Color) = (0, 0, 0, 0)
        _EmissionRange ("_EmissionRange", Range(0.0, 0.1)) = 0.02
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
            
            sampler2D _MainTex;
            sampler2D _StencilTex;
            float4 _EmissionColor;
            float _Threshold;
            float _EmissionRange;
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
                i.uv.y += _Threshold;
                i.uv2.x -= _Threshold;
                float stencilCol = tex2D(_StencilTex, i.uv2);
                if (stencilCol < 1) {
                    discard;
                }
                fixed range = step(0.01, _Threshold) * _EmissionRange;
                if (i.uv.y > 0.5 + range) {
                    discard;
                }
                float emissionVal = step(0.5, i.uv.y);
                fixed4 col = tex2D(_MainTex, i.uv) * (1 - emissionVal) + _EmissionColor * emissionVal;
                return col;
            }
            ENDCG
        }
    }
}
