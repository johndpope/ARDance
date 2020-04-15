Shader "nkdtr/RippleEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _width ("width", Float) = 1024
        _height ("height", Float) = 1024
        _speed ("speed", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Always Cull Off ZWrite Off

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
            sampler2D _prev_1;
            sampler2D _prev_2;
            sampler2D _draw;
            fixed _width;
            fixed _height;
            fixed _speed;

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
                fixed strideX = _speed/_width;
                fixed strideY = _speed/_height;

                fixed4 retval = tex2D(_prev_1, i.uv) * 2 - tex2D(_prev_2, i.uv)
                    + (
                      tex2D(_prev_1, half2(i.uv.x+strideX, i.uv.y))
                      + tex2D(_prev_1, half2(i.uv.x-strideX, i.uv.y))
                      + tex2D(_prev_1, half2(i.uv.x, i.uv.y+strideY))
                      + tex2D(_prev_1, half2(i.uv.x, i.uv.y-strideY))
                      - tex2D(_prev_1, i.uv) * 4
                    ) * 0.5;
            
                fixed4 halfvec = fixed4(0.5,0.5,0.5,0);
                retval = halfvec + (retval - halfvec)*0.985;
                retval += tex2D(_draw, i.uv);
            
                fixed4 col = retval;
                return col;
            }
            ENDCG
        }
    }
}
