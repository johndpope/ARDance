Shader "Unlit/EdgeWithTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EffectTex ("Effect Texture", 2D) = "white" {}
        _Sensitivity ("Sensitivity", float) = 1.0
        _Threshold ("Threshold", float) = 0.0
        _Speed ("Speed", Range(0.0, 1.0)) = 0.015
        [HDR] _EdgeColor ("Edge Color", COLOR) = (1,1,1,1)
        _BaseColor ("Base Color", COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZTest Off ZWrite Off Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/ARDance/_Common/ImageEffect/Blur.cginc"
            #include "Assets/ARDance/_Common/ImageEffect/Edge.cginc"
            #include "Assets/ARDance/_Common/Util/UVAdjust.cginc"

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
                float2 uvEdge : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _StencilTex;
            sampler2D _EffectTex;
            float _OnReverseMul;
            float _OnReversePlu;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvEdge = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                o.uv2 = UV2(o.uv);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float stencilCol = tex2D(_StencilTex, i.uv2);
                float seg = step(1, stencilCol);
                seg = seg * (-1 * _OnReverseMul) + (1 + _OnReversePlu);
                if (seg < 1) {
                    i.uv2 += _Time.y * _Speed;
                    return EdgeWithColor(_MainTex, i.uvEdge, tex2D(_EffectTex, i.uv2));
                } else {
                    return tex2D(_MainTex, i.uv);
                }
            }
            ENDCG
        }
    }
}
