Shader "Unlit/ColoredAfterImage"
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
            #include "Assets/ARDance/Common/Util/UVAdjust.cginc"
            #include "Assets/ARDance/Common/ImageEffect/CRT.cginc"

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
            fixed4 _Color;
            sampler2D _StencilTex;

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
                    discard;
                }
                fixed4 col = lerp(tex2D(_MainTex, i.uv), _Color, 0.3);
                col *= ScanLine(i.uv);
                return col;
            }
            ENDCG
        }
    }
}
