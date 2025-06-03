Shader "Custom/WaveWaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveSpeed("Wave Speed", Float) = 2
        _WaveHeight("Wave Height", Float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _WaveSpeed;
            float _WaveHeight;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                // از _Time.y برای گذر زمان استفاده می‌کنیم
                float wave = sin(v.vertex.x * 10 + _Time.y * _WaveSpeed) * _WaveHeight;
                v.vertex.y += wave;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}

