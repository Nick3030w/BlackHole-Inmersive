Shader "Custom/BlackHole"
{
    Properties
    {
        _MainColor ("Core Color", Color) = (0, 0, 0, 1)
        _RimColor ("Rim Color", Color) = (1, 0.4, 0.05, 1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 4.0
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1.5
        _DistortionStrength ("Distortion Strength", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 viewDir;
            float3 worldNormal;
        };

        fixed4 _MainColor;
        fixed4 _RimColor;
        float _RimPower;
        float _RimIntensity;
        float _DistortionStrength;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Centro absolutamente negro — absorbe toda la luz
            o.Albedo = fixed3(0, 0, 0);
            o.Metallic = 0;
            o.Smoothness = 0;

            // Rim muy delgado en el borde — simula el photon ring
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
            float rimIntensity = pow(rim, _RimPower);

            // Solo brilla en un anillo muy delgado en el borde extremo
            o.Emission = _RimColor.rgb * rimIntensity * _RimIntensity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
