Shader "ARF/CameraFeed"
{
    Properties
	{
		_Threshold ("Threshold", Range(0.0, 1.0)) = 1
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

            #include "UnityCG.cginc"
            #include "Assets/ARDance/Common/Util/CameraFeed.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; 
                float2 uvCameraFeed : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
            
			float _Threshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uvCameraFeed = UVCameraFeed(o.uv);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 camCol = CameraColor(i.uvCameraFeed);
                if (i.uv.y > _Threshold) {
                    discard;
                }
                return camCol;
            }
            ENDCG
        }
    }
}
