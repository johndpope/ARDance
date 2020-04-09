Shader "AV/AudioLightBack"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1, 1, 1, 1)
        _Anchor ("Anchor", Range(0.0, 1.0)) = 0.1
        _Attenuation ("Attenuation", Range(0.0, 10.0)) = 1.0
        _Strength ("Strength", Range(0.0, 10.0)) = 1.0
        _RimPower ("RimPower", Range(0.0, 10.0)) = 3.0
        _Length ("Length", Range(-1.0, 20.0)) = 1.0
        _Width ("Width", Range(0, 2.0)) = 1.0
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
            float _Anchor;
            float _Attenuation;
            float _Strength;
            float _RimPower;
            float _Length;
            float _Width;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.y += _Length * step(0, v.vertex.y);
                float vertexHeight = v.uv.y * _Width * smoothstep(0.6, 1, v.uv.y); 
				v.vertex.xz = v.vertex.xz + v.normal.xz * vertexHeight;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				float dist = max(0.02, distance(i.uv.y, _Anchor));
				float grad = 0.1 / sqrt(dist * _Attenuation);
				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float rim = pow(abs(dot(worldViewDir, i.worldNormal)), _RimPower);
				float mask = smoothstep(0.1, 1-_Anchor, distance(i.uv.y, 1.0));
                fixed4 col = _Color * grad * rim * mask;
                return col;
            }
            ENDCG
        }
    }
}
