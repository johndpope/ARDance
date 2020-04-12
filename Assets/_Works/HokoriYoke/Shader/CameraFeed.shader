Shader "ARF/CameraFeed"
{
    Properties
	{
	    [HDR] _Color ("Color", Color) = (1, 1, 1, 1)
		_Threshold ("Threshold", Range(-0.2, 1.2)) = 1
		_Width ("Width", Range(0.0, 0.3)) = 0.1
		_Power ("Power", Range(1.0, 6.0)) = 1.0
	}
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {   
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/ARDance/_Common/Util/CameraFeed.cginc"

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
            
            float4 _Color;
			float _Threshold;
			float _Width;
			float _Power;

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
                fixed4 camCol = CameraColor(i.uvCameraFeed);;
                float edgeSmooth = 1 - smoothstep(_Threshold - _Width, _Threshold, i.uv.y);
                float edgeUp = smoothstep(_Threshold - _Width, _Threshold + _Width, i.uv.y);
                float edgeDown = 1 - smoothstep(_Threshold - _Width, _Threshold + _Width, i.uv.y);
                return camCol * edgeSmooth + _Color * edgeUp * edgeDown * _Power;
            }
            ENDCG
        }
    }
}
