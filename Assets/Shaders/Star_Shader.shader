Shader "Custom/Star"
{
    Properties
    {
        _CoreColor ("Core Color", Color) = (1, 1, 0.9, 1)
        _EdgeColor ("Edge Color", Color) = (1, 0.5, 0.1, 1)
        _Brightness ("Brightness", Range(1, 10)) = 4.0
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.0
        _PulseAmount ("Pulse Amount", Range(0, 0.5)) = 0.1
        _NoiseScale ("Surface Noise Scale", Range(1, 20)) = 6.0
        _FlowSpeed ("Flow Speed", Range(0, 3)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        struct Input
        {
            float3 localPos;
            float3 viewDir;
            float3 worldNormal;
        };

        fixed4 _CoreColor;
        fixed4 _EdgeColor;
        float _Brightness;
        float _PulseSpeed;
        float _PulseAmount;
        float _NoiseScale;
        float _FlowSpeed;

        float hash(float3 p)
        {
            p = frac(p * 0.3183099 + 0.1);
            p *= 17.0;
            return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
        }

        float noise3D(float3 x)
        {
            float3 i = floor(x);
            float3 f = frac(x);
            f = f * f * (3.0 - 2.0 * f);
            return lerp(lerp(lerp(hash(i + float3(0,0,0)), hash(i + float3(1,0,0)), f.x),
                             lerp(hash(i + float3(0,1,0)), hash(i + float3(1,1,0)), f.x), f.y),
                        lerp(lerp(hash(i + float3(0,0,1)), hash(i + float3(1,0,1)), f.x),
                             lerp(hash(i + float3(0,1,1)), hash(i + float3(1,1,1)), f.x), f.y), f.z);
        }

        float fbm(float3 p)
        {
            float value = 0.0;
            float amplitude = 0.5;
            for (int i = 0; i < 4; i++)
            {
                value += amplitude * noise3D(p);
                p *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 dir = normalize(IN.localPos);

            // Flujo de plasma animado en la superficie
            float3 noiseCoord = dir * _NoiseScale + _Time.y * _FlowSpeed;
            float plasma = fbm(noiseCoord);

            // Granulación solar
            float granules = fbm(dir * _NoiseScale * 3.0 - _Time.y * _FlowSpeed * 0.5);

            // Mezcla de colores del núcleo al borde
            fixed3 color = lerp(_EdgeColor.rgb, _CoreColor.rgb, plasma);
            color *= (0.7 + granules * 0.6);

            // Pulsación
            float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;

            o.Albedo = fixed3(0, 0, 0);
            o.Emission = color * _Brightness * pulse;
            o.Metallic = 0;
            o.Smoothness = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
