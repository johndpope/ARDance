Shader "AV/AudioLightFront"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1, 1, 1, 1)
        _Length ("Length", Range(-1.0, 10.0)) = 1.0
        _RimPower ("RimPower", Range(0.01, 10.0)) = 3.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZWrite Off 
        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _RimPower;
            float _Length;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.y += _Length * step(0, v.vertex.y);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float rim = pow(abs(dot(worldViewDir, i.worldNormal)), _RimPower);
				
                fixed4 col = _Color * rim;
                return col;
            }
            ENDCG
        }
    }
}
