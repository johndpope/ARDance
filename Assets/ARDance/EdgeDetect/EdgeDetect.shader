Shader "Custom/EdgeDetect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Sensitivity ("Sensitivity", float) = 1.0
        _Threshold ("Threshold", float) = 0.0
        _EdgeColor ("Edge Color", COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #pragma multi_compile _ Sobel
           #pragma multi_compile _ Prewitt
           #pragma multi_compile _ RobertsCross
           #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uvEdge : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float2 uv1 : TEXCOORD2; 
                float2 uv2 : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _Sensitivity;
            float _Threshold;
            half4 _EdgeColor;
            sampler2D _StencilTex;
            float _UVMultiplierLandScape;
            float _UVMultiplierPortrait;
            float _UVFlip;
            int _OnWide;
            float _OnReverseMul;
            float _OnReversePlu;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvEdge = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                if(_OnWide == 1)
                {
                    o.uv1 = float2(v.uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (v.uv.y / _UVMultiplierLandScape));
                    o.uv2 = float2(lerp(1.0 - o.uv1.x, o.uv1.x, _UVFlip), lerp(o.uv1.y, 1.0 - o.uv1.y, _UVFlip));
                }
                else
                {
                    o.uv1 = float2(1.0 - v.uv.y, 1.0 - _UVMultiplierPortrait * 0.5f + v.uv.x / _UVMultiplierPortrait);
                    float2 forMask = float2((1.0 - (_UVMultiplierPortrait * 0.5f)) + (v.uv.x / _UVMultiplierPortrait), v.uv.y);
                    o.uv2 = float2(lerp(1.0 - forMask.y, forMask.y, 0), lerp(forMask.x, 1.0 - forMask.x, 1));
                }
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float stencilCol = tex2D(_StencilTex, i.uv2);
                float seg = step(1, stencilCol);
                seg = seg * (-1 * _OnReverseMul) + (1 + _OnReversePlu);
                
                float2 duv = _MainTex_TexelSize.xy;
                half2 uv0 = i.uvEdge + half2(-duv.x, -duv.y);
                half2 uv1 = i.uvEdge + half2(0, -duv.y);
                half2 uv2 = i.uvEdge + half2(duv.x, -duv.y);
                half2 uv3 = i.uvEdge + half2(-duv.x, 0);
                half2 uv4 = i.uvEdge + half2(0, 0);
                half2 uv5 = i.uvEdge + half2(duv.x, 0);
                half2 uv6 = i.uvEdge + half2(-duv.x, duv.y);
                half2 uv7 = i.uvEdge + half2(0, duv.y);
                half2 uv8 = i.uvEdge + half2(duv.x, duv.y);
                half3 col0 = tex2D(_MainTex, uv0);
                half3 col1 = tex2D(_MainTex, uv1);
                half3 col2 = tex2D(_MainTex, uv2);
                half3 col3 = tex2D(_MainTex, uv3);
                half3 col4 = tex2D(_MainTex, uv4);
                half3 col5 = tex2D(_MainTex, uv5);
                half3 col6 = tex2D(_MainTex, uv6);
                half3 col7 = tex2D(_MainTex, uv7);
                half3 col8 = tex2D(_MainTex, uv8);
                float3 cg1 = col8 - col0;
                float3 cg2 = col6 - col2;
                float cg = sqrt(dot(cg1, cg1) + dot(cg2, cg2));
                half4 edge = cg * _Sensitivity;
                fixed4 edgeCol = _EdgeColor * saturate(edge - _Threshold);
            
                fixed4 texCol = tex2D(_MainTex, i.uv);
                
                return texCol * (1-seg) + edgeCol * seg;
                
            }
            ENDCG
        }
    }
}
