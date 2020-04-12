Shader "ARF/GridCameraFeed"
{
    Properties
	{
		_TileTex ("Tile Texture", 2D) = "white" {}
		_GradMap ("Grad Map", 2D) = "white" {}
		[HDR] _GridColor ("Grid Color", Color) = (0, 0, 0, 1)
		_Threshold ("Threshold", Range(0.0, 1.2)) = 0
		_Width ("Width", Range(0.0, 1.0)) = 0.2
		[HDR] _TransitionColor("TransitionColor", Color) = (1, 1, 1, 1)
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
            #include "Assets/ARDance/_Common/Util/UVAdjust.cginc"
            #include "Assets/ARDance/_Common/Util/CameraFeed.cginc"
            #include "Assets/ARDance/_Common/Util/Transition.cginc"
            #include "Assets/ARDance/_Common/Util/ShaderTools.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; 
                float2 uv2 : TEXCOORD1;
                float2 uvCameraFeed : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _TileTex;
			sampler2D _GradMap;
            sampler2D _StencilTex;
            float4 _TileTex_ST;
			float4 _GridColor;
			float _Threshold;
			float _Width;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = UV2(o.uv);
                o.uvCameraFeed = UVCameraFeed(o.uv);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 camCol = CameraColor(i.uvCameraFeed);
                float stencilCol = tex2D(_StencilTex, i.uv2);
                if (stencilCol < 1) {
                    discard;
                }
                float onGrid = 1 - tex2D(_TileTex, i.uv * _TileTex_ST.xy);
			    float g = tex2D(_GradMap, i.uv2);
			    float trigger = step(_Threshold - _Width, g) * step(g, _Threshold);
			    float h = sin(i.uv.y + _Time.y * 5) / 2 + 0.5;
			    float4 hsl = float4(h, 0.5, 0.5, 1);
			    fixed4 baseCol = lerp(camCol, HSLtoRGB(hsl), 0.5);
			    float4 col = lerp(baseCol, _GridColor, onGrid * trigger);
                return TransitWithColor(col);
            }
            ENDCG
        }
    }
}
