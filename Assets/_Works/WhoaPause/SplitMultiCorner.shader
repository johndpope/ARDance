Shader "Unlit/SplitMultiCorner"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _ThresholdX ("_ThresholdX", Range(0.0, 0.5)) = 0.0
        _ThresholdY ("_ThresholdY", Range(0.0, 0.5)) = 0.0
        [HDR] _EmissionColor ("_EmissionColor", Color) = (0, 0, 0, 0)
        _EmissionRange ("_EmissionRange", Range(0.0, 0.1)) = 0.00
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
            #pragma multi_compile _SplitX_Left _SplitX_Right
            #pragma multi_compile _SplitY_Up _SplitY_Down

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
            float _ThresholdX;
            float _ThresholdY;
            float _EmissionRange;
            float _UVMultiplierLandScape;
            float _UVMultiplierPortrait;
            float _UVFlip;
            int _OnWide;
            int _OnX;
            int _OnY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
#ifdef _SplitX_Left
                o.uv.x += _ThresholdX;
#elif _SplitX_Right
                o.uv.x -= _ThresholdX;
#endif
#ifdef _SplitY_Up
                o.uv.y -= _ThresholdY;
#elif _SplitY_Down
                o.uv.y += _ThresholdY;
#endif
                if(_OnWide == 1)
                {
                    o.uv1 = float2(o.uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (o.uv.y / _UVMultiplierLandScape));
                    o.uv2 = float2(lerp(1.0 - o.uv1.x, o.uv1.x, _UVFlip), lerp(o.uv1.y, 1.0 - o.uv1.y, _UVFlip));
                }
                else
                {
                    o.uv1 = float2(1.0 - o.uv.y, 1.0 - _UVMultiplierPortrait * 0.5f + o.uv.x / _UVMultiplierPortrait);
                    float2 forMask = float2((1.0 - (_UVMultiplierPortrait * 0.5f)) + (o.uv.x / _UVMultiplierPortrait), o.uv.y);
                    o.uv2 = float2(lerp(1.0 - forMask.y, forMask.y, 0), lerp(forMask.x, 1.0 - forMask.x, 1));
                }
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
//                float stencilCol = tex2D(_StencilTex, i.uv2);
//                if (stencilCol < 1) {
//                    discard;
//                }
                float texVal = 1;
                if (_OnX == 1) {
                    fixed rangeX = step(0.01, _ThresholdX) * _EmissionRange;
#ifdef _SplitX_Left
                    if (i.uv.x > 0.5 + rangeX) {
                        discard;
                    }
                    texVal *= step(i.uv.x, 0.5);
#elif _SplitX_Right
                    if (i.uv.x < 0.5 - rangeX) {
                        discard;
                    }
                    texVal *= step(0.5, i.uv.x);
#endif
                }
                if (_OnY == 1) {
                    fixed rangeY = step(0.01, _ThresholdY) * _EmissionRange;
#ifdef _SplitY_Up
                    if (i.uv.y < 0.5 - rangeY) {
                        discard;
                    }
                    texVal *= step(0.5, i.uv.y);
#elif _SplitY_Down
                    if (i.uv.y > 0.5 + rangeY) {
                        discard;
                    }
                    texVal *= step(i.uv.y, 0.5);
#endif
                }
                fixed4 col = tex2D(_MainTex, i.uv) * texVal + _EmissionColor * (1 - texVal);
                return col;
            }
            ENDCG
        }
    }
}
