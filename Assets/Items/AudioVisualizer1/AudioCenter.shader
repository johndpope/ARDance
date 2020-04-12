Shader "AV/AudioCenter"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Range(0.0, 1.0)) = 0.5
        _Width ("Width", Range(0.0, 0.3)) = 0.05
        _HueShift ("HueShift", Range(0.0, 1.0)) = 0.002
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
            #include "Assets/ARDance/_Common/Util/ShaderTools.cginc"

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

            float4 _Color;
            float _Radius;
            float _Width;
            float _HueShift;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				float dist = distance(i.uv, float2(0.5, 0.5));
				float over = 1 - smoothstep(_Radius, _Radius + _Width, dist);
				float under = smoothstep(_Radius - _Width, _Radius, dist);
				fixed4 hsl = RGBtoHSL(_Color);
				hsl.x += pow((1 - i.uv.x), 0.2) * _HueShift;
                fixed4 col = HSLtoRGB(hsl) * over * under;
                return col;
            }
            ENDCG
        }
    }
}
