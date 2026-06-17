using UnityEngine;

/// <summary>
/// Simula la atracción gravitacional de un agujero negro sobre el jugador (XR Rig).
/// La fuerza aumenta con la cercanía usando una ley de inversa del cuadrado (similar a la gravedad real).
/// Integra con EducationalMessageSystem para desplegar información progresiva.
/// </summary>
public class BlackHoleGravity : MonoBehaviour
{
    [Header("Configuración de Gravedad")]
    [Tooltip("Fuerza base de atracción gravitacional")]
    public float gravityStrength = 10f;

    [Tooltip("Radio del horizonte de eventos — game over si el jugador entra aquí")]
    public float eventHorizonRadius = 3f;

    [Tooltip("Radio máximo donde la gravedad empieza a actuar")]
    public float gravityRadius = 50f;

    [Tooltip("Fuerza máxima permitida para evitar que el jugador se dispare")]
    public float maxForce = 20f;

    [Header("Efecto Visual de Cercanía")]
    [Tooltip("Intensidad del efecto de distorsión temporal conforme se acerca")]
    public float timeDistortionIntensity = 0.3f;

    [Header("Referencias")]
    [Tooltip("Transform del XR Origin / Camera Offset que se va a mover")]
    public Transform playerRig;

    [Header("Estado")]
    [SerializeField] private float currentDistance;
    [SerializeField] private float currentForce;
    private bool playerCaptured = false;

    // Evento que otros scripts pueden escuchar
    public System.Action OnPlayerCaptured;
    public System.Action<float> OnDistanceChanged;

    /// <summary>
    /// Distancia normalizada del jugador al agujero negro (1 = borde de gravedad, 0 = horizonte de eventos)
    /// </summary>
    public float NormalizedDistance
    {
        get
        {
            if (playerRig == null) return 1f;
            float dist = Vector3.Distance(transform.position, playerRig.position);
            return Mathf.InverseLerp(eventHorizonRadius, gravityRadius, dist);
        }
    }

    public bool IsPlayerCaptured => playerCaptured;

    void Update()
    {
        if (playerRig == null || playerCaptured) return;

        Vector3 toBlackHole = transform.position - playerRig.position;
        currentDistance = toBlackHole.magnitude;

        // Notificar cambio de distancia
        OnDistanceChanged?.Invoke(currentDistance);

        if (currentDistance < gravityRadius)
        {
            // Ley de inversa del cuadrado para gravedad más realista
            float normalizedDist = currentDistance / gravityRadius;
            float forceMagnitude = gravityStrength / (normalizedDist * normalizedDist + 0.1f);
            forceMagnitude = Mathf.Min(forceMagnitude, maxForce);

            currentForce = forceMagnitude;

            Vector3 direction = toBlackHole.normalized;
            playerRig.position += direction * forceMagnitude * Time.deltaTime;

            // Efecto de dilatación temporal (slow motion sutil al acercarse)
            float timeScale = Mathf.Lerp(1f - timeDistortionIntensity, 1f, NormalizedDistance);
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * timeScale;
        }

        // Horizonte de eventos alcanzado
        if (currentDistance < eventHorizonRadius && !playerCaptured)
        {
            playerCaptured = true;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            OnPlayerCaptured?.Invoke();
            Debug.Log("[BlackHole] Jugador capturado por el horizonte de eventos");
        }
    }

    void OnDisable()
    {
        // Restaurar time scale al desactivar
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar radios en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eventHorizonRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }
}
