Shader "ARF/HumanCRT"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseX("NoiseX", Range(0, 1)) = 0
		_Offset("Offset", Vector) = (0, 0, 0, 0)
		_RGBNoise("RGBNoise", Range(0, 1)) = 0
		_SinNoiseWidth("SineNoiseWidth", Float) = 1
		_SinNoiseScale("SinNoiseScale", Float) = 1
		_SinNoiseOffset("SinNoiseOffset", Float) = 1
		_ScanLineTail("Tail", Float) = 0.5
		_ScanLineSpeed("TailSpeed", Float) = 100
		_TransitionThreshold("TransitionThreshold", Range(0.0, 1.0)) = 0.0
		[HDR] _TransitionColor("TransitionColor", Color) = (1, 1, 1, 1)
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
			#include "Assets/ARDance/_Common/Util/UVAdjust.cginc"
			#include "Assets/ARDance/_Common/Util/Transition.cginc"
 
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
				o.uv2 = UV2(v.uv);
				return o;
			}
 
			float rand(float2 co) {
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}
 
			float2 mod(float2 a, float2 b)
			{
				return a - floor(a / b) * b;
			}
			sampler2D _MainTex;
			sampler2D _StencilTex;
			float _NoiseX;
			float2 _Offset;
			float _RGBNoise;
			float _SinNoiseWidth;
			float _SinNoiseScale;
			float _SinNoiseOffset;
			float _ScanLineTail;
			float _ScanLineSpeed;
 
			fixed4 frag (v2f i) : SV_Target
			{
			    float stencilCol = tex2D(_StencilTex, i.uv2);
                if (stencilCol < 1) {
                    discard;
                }
				
				float2 inUV = i.uv;
				float2 uv = i.uv - 0.5;	
 
				// 色を計算
				float3 col;
 
				// ノイズ、オフセットを適用
				//inUV.x += sin(inUV.y * _SinNoiseWidth + _SinNoiseOffset) * _SinNoiseScale;
				//inUV += _Offset;
				inUV.x += (rand(floor(inUV.y * 500) + _Time.y) - 0.5) * _NoiseX;
				inUV = mod(inUV, 1);
 
				// 色を取得、RGBを少しずつずらす
				col.r = tex2D(_MainTex, inUV).r;
				col.g = tex2D(_MainTex, inUV - float2(0.002, 0)).g;
				col.b = tex2D(_MainTex, inUV - float2(0.004, 0)).b;
 
				// RGBノイズ
				if (rand((rand(floor(inUV.y * 500) + _Time.y) - 0.5) + _Time.y) < _RGBNoise)
				{
					col.r = rand(uv + float2(123 + _Time.y, 0));
					col.g = rand(uv + float2(123 + _Time.y, 1));
					col.b = rand(uv + float2(123 + _Time.y, 2));
				}
 
				// ピクセルごとに描画するRGBを決める
				float floorX = fmod(inUV.x * _ScreenParams.x / 3, 1);
				col.r *= floorX > 0.3333;
				col.g *= floorX < 0.3333 || floorX > 0.6666;
				col.b *= floorX < 0.6666;
 
				// スキャンラインを描画
				float scanLineColor = sin(_Time.y * 10 + uv.y * 500) / 2 + 0.5;
				col *= 0.5 + clamp(scanLineColor + 0.5, 0, 1) * 0.5;
				
				return TransitWithColor(float4(col, 1));
			}
			ENDCG
		}
	}
}
