Shader "Unlit/HumanTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EffectTex ("Effect Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1, 1, 1, 1)
        _Speed ("Speed", Range(0.0, 1.0)) = 0.015
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/ARDance/_Common/Util/UVAdjust.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _EffectTex;
            sampler2D _StencilTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = UV2(o.uv);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float stencilCol = tex2D(_StencilTex, i.uv2);
                if (stencilCol < 1) {
                    return tex2D(_MainTex, i.uv);
                } else {
                    i.uv2 += _Time.y * _Speed;
                    return tex2D(_EffectTex, i.uv2) * _Color;
                }
            }
            ENDCG
        }
    }
}
