Shader "Custom/Planet"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0.2, 0.4, 0.8, 1)
        _PoleColor ("Pole Color", Color) = (0.8, 0.85, 0.9, 1)
        _EquatorColor ("Equator Color", Color) = (0.3, 0.5, 0.7, 1)
        _NoiseScale ("Surface Detail Scale", Range(1, 30)) = 8.0
        _NoiseStrength ("Surface Detail Strength", Range(0, 1)) = 0.4
        _AtmosphereColor ("Atmosphere Color", Color) = (0.4, 0.6, 1.0, 1)
        _AtmospherePower ("Atmosphere Power", Range(0.5, 6)) = 2.5
        _AtmosphereIntensity ("Atmosphere Intensity", Range(0, 3)) = 1.0
        _Roughness ("Roughness", Range(0, 1)) = 0.8
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
            float3 worldPos;
            float3 localPos;
            float3 viewDir;
            float3 worldNormal;
        };

        fixed4 _MainColor;
        fixed4 _PoleColor;
        fixed4 _EquatorColor;
        float _NoiseScale;
        float _NoiseStrength;
        fixed4 _AtmosphereColor;
        float _AtmospherePower;
        float _AtmosphereIntensity;
        float _Roughness;

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

            // Bandas latitudinales (como un planeta gaseoso o continentes)
            float latitude = abs(dir.y);
            fixed3 baseColor = lerp(_EquatorColor.rgb, _PoleColor.rgb, smoothstep(0.5, 1.0, latitude));

            // Detalle de superficie con noise
            float surfaceNoise = fbm(dir * _NoiseScale);
            baseColor = lerp(baseColor, _MainColor.rgb, surfaceNoise * _NoiseStrength);

            // Variación adicional para continentes/nubes
            float detail = fbm(dir * _NoiseScale * 2.5 + 100.0);
            baseColor *= (0.7 + detail * 0.6);

            o.Albedo = baseColor;
            o.Metallic = 0;
            o.Smoothness = 1.0 - _Roughness;

            // Atmósfera (rim emisivo)
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
            float atmosphere = pow(rim, _AtmospherePower);
            o.Emission = _AtmosphereColor.rgb * atmosphere * _AtmosphereIntensity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
