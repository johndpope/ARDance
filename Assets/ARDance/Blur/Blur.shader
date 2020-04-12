Shader "Unlit/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurColor ("Blur Color", Color) = (1, 0, 0, 1)
        _BlurColorPower ("Color Power", Range(0.0, 1.0)) = 0.1
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
            #include "Assets/ARDance/_Common/Util/UVAdjust.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OnReverseMul;
            float _OnReversePlu;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return BlurWithColor(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
