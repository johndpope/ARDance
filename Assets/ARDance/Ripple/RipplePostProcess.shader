Shader "Unlit/RipplePostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            sampler2D _RippleTex;
            sampler2D _StencilTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = UV2(v.uv);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float stencilCol = tex2D(_StencilTex, i.uv2);
                fixed r = tex2D(_RippleTex, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv) + fixed4(r, r, r, r) * (1 - stencilCol);
                return col;
            }
            ENDCG
        }
    }
}
