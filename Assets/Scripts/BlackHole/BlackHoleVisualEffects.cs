using UnityEngine;

/// <summary>
/// Efectos visuales del agujero negro que responden a la cercanía del jugador.
/// Aumenta la rotación del disco de acreción y la intensidad del glow conforme el jugador se acerca.
/// </summary>
public class BlackHoleVisualEffects : MonoBehaviour
{
    [Header("Referencias")]
    public Transform accretionDisk;
    public Transform glowRing;
    public BlackHoleGravity blackHoleGravity;

    [Header("Rotación del Disco de Acreción")]
    [Tooltip("Velocidad base de rotación en grados/segundo")]
    public float baseRotationSpeed = 30f;

    [Tooltip("Multiplicador máximo de velocidad cuando el jugador está cerca")]
    public float maxSpeedMultiplier = 5f;

    [Header("Escala del Glow")]
    [Tooltip("Escala base del anillo de glow")]
    public float baseGlowScale = 1f;

    [Tooltip("Escala máxima cuando el jugador se acerca")]
    public float maxGlowScale = 1.5f;

    [Tooltip("Velocidad de pulsación del glow")]
    public float glowPulseSpeed = 2f;

    void Update()
    {
        if (blackHoleGravity == null) return;

        float proximity = 1f - blackHoleGravity.NormalizedDistance; // 0 = lejos, 1 = cerca

        // Rotar disco de acreción más rápido conforme el jugador se acerca
        if (accretionDisk != null)
        {
            float speed = baseRotationSpeed * Mathf.Lerp(1f, maxSpeedMultiplier, proximity);
            accretionDisk.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
        }

        // Pulsar el glow ring
        if (glowRing != null)
        {
            float pulse = Mathf.Sin(Time.time * glowPulseSpeed * (1f + proximity)) * 0.1f;
            float scale = Mathf.Lerp(baseGlowScale, maxGlowScale, proximity) + pulse;
            glowRing.localScale = Vector3.one * scale;
        }
    }
}
