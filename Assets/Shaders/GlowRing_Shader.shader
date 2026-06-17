Shader "Custom/GlowRing"
{
    Properties
    {
        _Color ("Glow Color", Color) = (0.3, 0.5, 1.0, 1)
        _Intensity ("Intensity", Range(0.5, 10)) = 3.0
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.5
        _PulseMin ("Pulse Min", Range(0, 1)) = 0.5
        _FadeWidth ("Fade Width", Range(0.01, 1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        LOD 100
        Blend One One
        ZWrite Off
        Cull Off

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            fixed4 _Color;
            float _Intensity;
            float _PulseSpeed;
            float _PulseMin;
            float _FadeWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Rim-based glow
                float rim = 1.0 - saturate(dot(normalize(i.viewDir), normalize(i.worldNormal)));
                float glow = pow(rim, 2.0);

                // Pulse animation
                float pulse = lerp(_PulseMin, 1.0, (sin(_Time.y * _PulseSpeed) + 1.0) * 0.5);

                // Final color
                fixed4 col = _Color * glow * _Intensity * pulse;
                col.a = glow * pulse;

                return col;
            }
            ENDCG
        }
    }
}
