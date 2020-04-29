Shader "Unlit/HumanOcclusion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
 
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
            float4 _MainTex_ST;
            sampler2D_float _DepthTex;
            sampler2D _CameraFeed;
            sampler2D_float _CameraDepthTexture;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = UV2(v.uv);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 cameraFeedCol = tex2D(_CameraFeed, i.uv);
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
                float occlusionDepth = tex2D(_DepthTex, i.uv2) * 0.625;
                float humanSegment = step(0.01, occlusionDepth);
                float showOccluder = step(occlusionDepth, sceneDepth) * humanSegment;
                return lerp(col, cameraFeedCol, showOccluder);
            }
            ENDCG
        }
    }
}
