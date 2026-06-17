Shader "Custom/BlackHole"
{
    Properties
    {
        _MainColor ("Core Color", Color) = (0, 0, 0, 1)
        _RimColor ("Rim Color", Color) = (0.1, 0.2, 0.8, 1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
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
        float _DistortionStrength;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Centro completamente negro
            o.Albedo = _MainColor.rgb;
            o.Metallic = 0;
            o.Smoothness = 0;

            // Rim lighting para simular el borde del horizonte de eventos
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
            float rimIntensity = pow(rim, _RimPower);

            o.Emission = _RimColor.rgb * rimIntensity * 2.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
