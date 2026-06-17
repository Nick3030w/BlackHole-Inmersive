using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Efecto visual de pulso luminoso para el borde del panel educativo.
/// Crea un efecto de "respiración" que hace el panel más atractivo visualmente.
/// </summary>
[RequireComponent(typeof(Image))]
public class PanelGlowEffect : MonoBehaviour
{
    [Header("Configuración del Pulso")]
    [Tooltip("Velocidad del pulso luminoso")]
    public float pulseSpeed = 1.5f;

    [Tooltip("Intensidad mínima del alpha")]
    public float minAlpha = 0.3f;

    [Tooltip("Intensidad máxima del alpha")]
    public float maxAlpha = 0.8f;

    [Tooltip("Color base del glow")]
    public Color glowColor = new Color(0.3f, 0.6f, 1f, 1f);

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (image == null) return;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        image.color = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
    }
}
