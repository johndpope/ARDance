﻿Shader "PostEffects/Grid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TileTex ("Tile Texture", 2D) = "white" {}
		_GradMap ("Grad Map", 2D) = "white" {}
		[HDR] _GridColor ("Grid Color", Color) = (0, 0, 0, 1)
		_Threshold ("Threshold", Range(0.0, 1.2)) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			#include "Assets/ARDance/Common/Util/UVAdjust.cginc"
 
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
 
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv2 = UV2(o.uv);
				return o;
			}
 
			sampler2D _MainTex;
			sampler2D _TileTex;
			sampler2D _GradMap;
			sampler2D _StencilTex;
			float _OnReverseMul;
            float _OnReversePlu;
			float4 _TileTex_ST;
			float4 _GridColor;
			float _Threshold;
 
			fixed4 frag (v2f i) : SV_Target
			{	
			    float4 texCol = tex2D(_MainTex, i.uv);	
			    float stencilCol = tex2D(_StencilTex, i.uv2);
			    float seg = step(1, stencilCol);
                seg = seg * (-1 * _OnReverseMul) + (1 + _OnReversePlu);
			    if (seg < 1) {
			        return texCol;
			    }
			    float onGrid = 1 - tex2D(_TileTex, i.uv * _TileTex_ST.xy);
			    fixed4 m = tex2D(_GradMap, i.uv);
                half g = m.r * 0.2 + m.g * 0.7 + m.b * 0.1;
			    float trigger = step(_Threshold - 0.2, g) * step(g, _Threshold);
			    fixed4 col = lerp(texCol, _GridColor, onGrid * trigger);
				return col;
			}
			ENDCG
		}
	}
}
