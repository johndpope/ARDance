Shader "AV/AudioCenterObj"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Color ("Color", Color) = (1, 1, 1, 1)
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
            #include "Assets/ARDance/Common/Util/ShaderTools.cginc"

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
                float4 texCol = tex2D(_MainTex, i.uv);
				float dist = distance(i.uv, float2(0, 0));
				fixed4 hsl = RGBtoHSL(_Color);
				hsl.x += pow((dist), 0.2) * _HueShift;
                fixed4 col = texCol * HSLtoRGB(hsl);
                return col;
            }
            ENDCG
        }
    }
}
