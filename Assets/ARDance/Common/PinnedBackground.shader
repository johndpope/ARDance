Shader "Unlit/PinnedBackground"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off ZWrite Off ZTest Always

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
                float2 uv1 : TEXCOORD1; 
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _UVMultiplierLandScape;
            float _UVMultiplierPortrait;
            float _UVFlip;
            int _OnWide;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                if(_OnWide == 1)
                {
                    o.uv1 = float2(v.uv.x, (1.0 - (_UVMultiplierLandScape * 0.5f)) + (v.uv.y / _UVMultiplierLandScape));
                }
                else
                {
                    o.uv1 = float2(1.0 - v.uv.y, 1.0 - _UVMultiplierPortrait * 0.5f + v.uv.x / _UVMultiplierPortrait);
                }
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv1);         
                return col;
            }
            ENDCG
        }
    }
}
