Shader "Custom/DistantBlackHole"
{
    Properties
    {
        _DiskInner ("Disk Inner Color", Color) = (1, 0.95, 0.8, 1)
        _DiskMid ("Disk Mid Color", Color) = (1, 0.55, 0.15, 1)
        _DiskOuter ("Disk Outer Color", Color) = (0.7, 0.2, 0.05, 1)
        _Brightness ("Disk Brightness", Range(1, 8)) = 3.0
        _RotationSpeed ("Rotation Speed", Range(0, 3)) = 0.4
        _NoiseScale ("Noise Scale", Range(2, 20)) = 10.0
        _HoleRadius ("Hole Radius", Range(0.05, 0.5)) = 0.22
        _DiskWidth ("Disk Width", Range(0.1, 0.5)) = 0.3
        _PhotonRing ("Photon Ring Color", Color) = (1, 0.8, 0.5, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _DiskInner;
            fixed4 _DiskMid;
            fixed4 _DiskOuter;
            float _Brightness;
            float _RotationSpeed;
            float _NoiseScale;
            float _HoleRadius;
            float _DiskWidth;
            fixed4 _PhotonRing;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Centrar UV en (0,0)
                float2 uv = i.uv - 0.5;
                float dist = length(uv);
                float angle = atan2(uv.y, uv.x);

                fixed4 col = fixed4(0, 0, 0, 0);

                // === AGUJERO NEGRO (centro) ===
                if (dist < _HoleRadius)
                {
                    // Negro absoluto
                    return fixed4(0, 0, 0, 1);
                }

                // === PHOTON RING (anillo delgado brillante) ===
                float ringDist = abs(dist - _HoleRadius);
                float photonRing = smoothstep(0.04, 0.0, ringDist);
                col.rgb += _PhotonRing.rgb * photonRing * 2.0;
                col.a = max(col.a, photonRing);

                // === DISCO DE ACRECIÓN ===
                float diskOuter = _HoleRadius + _DiskWidth;
                if (dist >= _HoleRadius && dist < diskOuter)
                {
                    float diskT = (dist - _HoleRadius) / _DiskWidth;

                    // Patrón rotatorio
                    float2 noiseCoord = float2(angle / 6.28 * _NoiseScale + _Time.y * _RotationSpeed, diskT * _NoiseScale);
                    float n = fbm(noiseCoord);

                    // Color del disco según radio
                    fixed3 diskColor;
                    if (diskT < 0.5)
                        diskColor = lerp(_DiskInner.rgb, _DiskMid.rgb, diskT * 2.0);
                    else
                        diskColor = lerp(_DiskMid.rgb, _DiskOuter.rgb, (diskT - 0.5) * 2.0);

                    diskColor *= (0.6 + n * 0.8) * _Brightness;

                    // Fade en los bordes del disco
                    float diskAlpha = smoothstep(0.0, 0.15, diskT) * smoothstep(1.0, 0.7, diskT);
                    diskAlpha *= (0.7 + n * 0.3);

                    col.rgb += diskColor * diskAlpha;
                    col.a = max(col.a, diskAlpha);
                }

                return col;
            }
            ENDCG
        }
    }
}
