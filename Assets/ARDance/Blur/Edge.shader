﻿Shader "Unlit/Edge"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Sensitivity ("Sensitivity", float) = 1.0
        _Threshold ("Threshold", float) = 0.0
        _EdgeColor ("Edge Color", COLOR) = (1,1,1,1)
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
            #include "Assets/ARDance/Common/ImageEffect/Edge.cginc"
            #include "Assets/ARDance/Common/Util/UVAdjust.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return EdgeWithTex(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}