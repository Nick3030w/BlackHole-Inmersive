using UnityEngine;

/// <summary>
/// Configura la apariencia visual del agujero negro con materiales procedurales.
/// Aplica shaders personalizados al core, disco de acreción y glow ring.
/// 
/// INSTRUCCIONES:
/// 1. Agregar este script al GameObject "BlackHole"
/// 2. Asignar las referencias de AccretionDisk y GlowRing
/// 3. Los materiales se crean automáticamente al entrar en Play
/// 
/// Si prefieres configurar manualmente:
/// - Crea materiales con los shaders Custom/BlackHole, Custom/AccretionDisk, Custom/GlowRing
/// - Asígnalos a los renderers correspondientes
/// </summary>
public class BlackHoleAppearance : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform del disco de acreción (debe tener MeshRenderer)")]
    public Transform accretionDisk;

    [Tooltip("Transform del anillo de glow (debe tener MeshRenderer/ParticleSystemRenderer)")]
    public Transform glowRing;

    [Header("Configuración del Core")]
    [Tooltip("Color del borde del horizonte de eventos")]
    public Color rimColor = new Color(1f, 0.4f, 0.05f, 1f);

    [Tooltip("Intensidad del rim")]
    public float rimPower = 3.5f;

    [Header("Configuración del Disco de Acreción")]
    public Color innerColor = new Color(1f, 0.95f, 0.7f, 1f);
    public Color outerColor = new Color(1f, 0.5f, 0.1f, 1f);
    public Color edgeColor = new Color(0.8f, 0.2f, 0.05f, 0.5f);
    public float diskBrightness = 2.0f;
    public float scrollSpeed = 0.6f;

    [Header("Configuración del Glow")]
    public Color glowColor = new Color(1f, 0.6f, 0.1f, 1f);
    public float glowIntensity = 3f;

    void Start()
    {
        ApplyBlackHoleMaterial();
        ApplyAccretionDiskMaterial();
        ApplyGlowRingMaterial();
    }

    void ApplyBlackHoleMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }

        if (renderer == null)
        {
            Debug.LogWarning("[BlackHoleAppearance] No se encontró Renderer en el BlackHole core");
            return;
        }

        Shader shader = Shader.Find("Custom/BlackHole");
        if (shader == null)
        {
            // Fallback: usar shader estándar con configuración oscura
            Debug.LogWarning("[BlackHoleAppearance] Shader Custom/BlackHole no encontrado, usando fallback");
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.black;
            mat.SetFloat("_Metallic", 0f);
            mat.SetFloat("_Glossiness", 0f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", rimColor * 0.5f);
            renderer.material = mat;
            return;
        }

        Material blackHoleMat = new Material(shader);
        blackHoleMat.SetColor("_MainColor", Color.black);
        blackHoleMat.SetColor("_RimColor", rimColor);
        blackHoleMat.SetFloat("_RimPower", rimPower);
        blackHoleMat.SetFloat("_RimIntensity", 1.5f);
        renderer.material = blackHoleMat;
    }

    void ApplyAccretionDiskMaterial()
    {
        if (accretionDisk == null) return;

        Renderer renderer = accretionDisk.GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = accretionDisk.GetComponentInChildren<Renderer>();
        }

        if (renderer == null)
        {
            Debug.LogWarning("[BlackHoleAppearance] No se encontró Renderer en AccretionDisk");
            return;
        }

        Shader shader = Shader.Find("Custom/AccretionDisk");
        if (shader == null)
        {
            // Fallback: material emisivo naranja
            Debug.LogWarning("[BlackHoleAppearance] Shader Custom/AccretionDisk no encontrado, usando fallback");
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetFloat("_Mode", 3); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.color = new Color(1f, 0.5f, 0.1f, 0.7f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", outerColor * diskBrightness);
            renderer.material = mat;
            return;
        }

        Material diskMat = new Material(shader);
        diskMat.SetColor("_Color1", innerColor);
        diskMat.SetColor("_Color2", outerColor);
        diskMat.SetColor("_Color3", edgeColor);
        diskMat.SetFloat("_ScrollSpeed", scrollSpeed);
        diskMat.SetFloat("_Brightness", diskBrightness);
        diskMat.SetFloat("_NoiseScale", 8f);
        diskMat.SetFloat("_InnerFade", 0.15f);
        diskMat.SetFloat("_OuterFade", 0.1f);
        diskMat.SetFloat("_PulseSpeed", 0.5f);
        diskMat.SetFloat("_PulseIntensity", 0.2f);
        renderer.material = diskMat;
    }

    void ApplyGlowRingMaterial()
    {
        if (glowRing == null) return;

        // Intentar con ParticleSystemRenderer primero
        ParticleSystemRenderer psRenderer = glowRing.GetComponent<ParticleSystemRenderer>();
        Renderer renderer = psRenderer != null ? psRenderer : glowRing.GetComponent<Renderer>();

        if (renderer == null)
        {
            Debug.LogWarning("[BlackHoleAppearance] No se encontró Renderer en GlowRing");
            return;
        }

        Shader shader = Shader.Find("Custom/GlowRing");
        if (shader == null)
        {
            // Fallback: material additive
            Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
            mat.SetColor("_Color", glowColor * glowIntensity);
            renderer.material = mat;
            return;
        }

        Material glowMat = new Material(shader);
        glowMat.SetColor("_Color", glowColor);
        glowMat.SetFloat("_Intensity", glowIntensity);
        glowMat.SetFloat("_PulseSpeed", 1.5f);
        glowMat.SetFloat("_PulseMin", 0.5f);
        renderer.material = glowMat;
    }
}
