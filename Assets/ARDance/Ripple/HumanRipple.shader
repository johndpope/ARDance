Shader "Unlit/HumanRipple"
{
    Properties
    {
        _DeltaUV("Delta UV", Float) = 3
        _Speed("Speed", Range(0.0, 0.5)) = 0.5
        _Attenuation("Attenuation", Range(0.8, 1.0)) = 0.999
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
            #include "Assets/ARDance/_Common/Util/UVAdjust.cginc"

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

            sampler2D _Prev_1;
            sampler2D _Prev_2;
            sampler2D _StencilTex;
            
            float _TextureWidth;
            float _TextureHeight;
            float _DeltaUV;
            float _Speed;
            float _Attenuation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = UV2(v.uv);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float du = 1.0 / _TextureWidth;
                float dv = 1.0 / _TextureHeight;
                float3 duv = float3(du, dv, 0) * _DeltaUV;
                
                // Get current acceleration
                fixed curAccel = tex2D(_Prev_1, i.uv) - tex2D(_Prev_2, i.uv);
                // Apply Laplacian filter to update acceleration
                fixed accel = tex2D(_Prev_1, i.uv + duv.xz)
                             + tex2D(_Prev_1, i.uv - duv.xz)
                             + tex2D(_Prev_1, i.uv + duv.zy)
                             + tex2D(_Prev_1, i.uv - duv.zy)
                             - tex2D(_Prev_1, i.uv) * 4.0;
                // Multiply propagation speed
                accel *= _Speed;
                fixed h = (tex2D(_Prev_1, i.uv) + curAccel + accel) * _Attenuation;
                h += tex2D(_StencilTex, i.uv2);
                fixed4 col = fixed4(h, 0, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
