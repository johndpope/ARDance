Shader "Unlit/RotateEffect"
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
            uniform float4 _CenterRadius;
			uniform float4x4 _RotationMatrix;

            #define PI 3.141592

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = i.uv - _CenterRadius.xy;
				float2 distortedOffset = MultiplyUV (_RotationMatrix, offset.xy);
				float2 tmp = offset / _CenterRadius.zw;
				float t = min (1, length(tmp));
				
				offset = lerp (distortedOffset, offset, t);
				offset += _CenterRadius.xy;
                fixed4 col = tex2D(_MainTex, offset);
                return col;
            }
            ENDCG
        }
    }
}
