Shader "Custom/AccretionDisk"
{
    Properties
    {
        _Color1 ("Inner Color", Color) = (1, 0.8, 0.2, 1)
        _Color2 ("Outer Color", Color) = (1, 0.3, 0.05, 1)
        _Color3 ("Edge Color", Color) = (0.5, 0.1, 0.8, 0.5)
        _ScrollSpeed ("Scroll Speed", Range(0.1, 5.0)) = 1.0
        _NoiseScale ("Noise Scale", Range(1, 20)) = 8.0
        _Brightness ("Brightness", Range(0.5, 5.0)) = 2.0
        _InnerFade ("Inner Fade", Range(0.01, 0.5)) = 0.15
        _OuterFade ("Outer Fade", Range(0.01, 0.5)) = 0.1
        _PulseSpeed ("Pulse Speed", Range(0, 3)) = 0.5
        _PulseIntensity ("Pulse Intensity", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
            float3 viewDir;
            float2 uv_MainTex;
        };

        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        float _ScrollSpeed;
        float _NoiseScale;
        float _Brightness;
        float _InnerFade;
        float _OuterFade;
        float _PulseSpeed;
        float _PulseIntensity;

        // Simplex-like noise function
        float hash(float2 p)
        {
            return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            f = f * f * (3.0 - 2.0 * f);

            float a = hash(i);
            float b = hash(i + float2(1.0, 0.0));
            float c = hash(i + float2(0.0, 1.0));
            float d = hash(i + float2(1.0, 1.0));

            return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
        }

        float fbm(float2 p)
        {
            float value = 0.0;
            float amplitude = 0.5;
            for (int i = 0; i < 4; i++)
            {
                value += amplitude * noise(p);
                p *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Calcular coordenadas polares desde el centro del objeto
            float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
            float dist = length(localPos.xz);
            float angle = atan2(localPos.z, localPos.x);

            // Normalizar distancia (0 = centro, 1 = borde)
            float normalizedDist = saturate(dist / 5.0); // Ajustar según escala del disco

            // Generar patrón de flujo espiral
            float2 noiseCoord = float2(angle * _NoiseScale / 6.28, normalizedDist * _NoiseScale);
            noiseCoord.x += _Time.y * _ScrollSpeed; // Rotación
            noiseCoord.y += _Time.y * _ScrollSpeed * 0.3; // Flujo radial sutil

            float noiseValue = fbm(noiseCoord);

            // Mezclar colores basado en distancia al centro
            fixed4 color;
            if (normalizedDist < 0.4)
                color = lerp(_Color1, _Color2, normalizedDist / 0.4);
            else
                color = lerp(_Color2, _Color3, (normalizedDist - 0.4) / 0.6);

            // Aplicar variación de noise para crear filamentos
            color.rgb *= (0.6 + noiseValue * 0.8);

            // Pulsación sutil
            float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseIntensity;
            color.rgb *= pulse;

            // Brillo
            color.rgb *= _Brightness;

            // Fade en bordes interno y externo
            float alphaInner = smoothstep(0.0, _InnerFade, normalizedDist);
            float alphaOuter = smoothstep(1.0, 1.0 - _OuterFade, normalizedDist);
            float alpha = alphaInner * alphaOuter;

            // Agregar variación al alpha con noise
            alpha *= (0.7 + noiseValue * 0.3);

            o.Albedo = fixed3(0, 0, 0);
            o.Emission = color.rgb;
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = alpha;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
