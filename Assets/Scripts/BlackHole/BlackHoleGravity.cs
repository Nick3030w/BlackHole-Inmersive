using UnityEngine;

/// <summary>
/// Simula la atracción gravitacional de un agujero negro sobre el jugador (XR Rig).
/// La fuerza aumenta con la cercanía usando una ley de inversa del cuadrado.
/// </summary>
public class BlackHoleGravity : MonoBehaviour
{
    [Header("Configuración de Gravedad")]
    public float gravityStrength = 10f;
    public float eventHorizonRadius = 3f;
    public float gravityRadius = 50f;
    public float maxForce = 20f;

    [Header("Efecto de Dilatación Temporal")]
    public float timeDistortionIntensity = 0.3f;

    [Header("Referencias")]
    [Tooltip("Transform que se mueve (Camera Offset o XR Origin)")]
    public Transform playerRig;

    // Campo legacy para compatibilidad con la escena existente
    [HideInInspector] public Transform cameraOffset;
    [HideInInspector] public GameObject educationalPanel;

    [Header("Debug")]
    [SerializeField] private float currentDistance;
    [SerializeField] private float currentForce;

    private bool playerCaptured = false;

    public System.Action OnPlayerCaptured;
    public System.Action<float> OnDistanceChanged;

    public float NormalizedDistance
    {
        get
        {
            Transform target = GetTarget();
            if (target == null) return 1f;
            float dist = Vector3.Distance(transform.position, target.position);
            return Mathf.InverseLerp(eventHorizonRadius, gravityRadius, dist);
        }
    }

    public bool IsPlayerCaptured => playerCaptured;

    /// <summary>
    /// Obtiene el transform objetivo — usa playerRig si está asignado, sino cameraOffset (legacy)
    /// </summary>
    Transform GetTarget()
    {
        if (playerRig != null) return playerRig;
        if (cameraOffset != null) return cameraOffset;
        return null;
    }

    void Update()
    {
        Transform target = GetTarget();
        if (target == null || playerCaptured) return;

        Vector3 toBlackHole = transform.position - target.position;
        currentDistance = toBlackHole.magnitude;

        OnDistanceChanged?.Invoke(currentDistance);

        if (currentDistance < gravityRadius)
        {
            // Ley de inversa del cuadrado
            float normalizedDist = currentDistance / gravityRadius;
            float forceMagnitude = gravityStrength / (normalizedDist * normalizedDist + 0.1f);
            forceMagnitude = Mathf.Min(forceMagnitude, maxForce);
            currentForce = forceMagnitude;

            Vector3 direction = toBlackHole.normalized;
            target.position += direction * forceMagnitude * Time.deltaTime;

            // Dilatación temporal sutil
            float timeScale = Mathf.Lerp(1f - timeDistortionIntensity, 1f, NormalizedDistance);
            Time.timeScale = Mathf.Max(0.1f, timeScale);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        if (currentDistance < eventHorizonRadius && !playerCaptured)
        {
            playerCaptured = true;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            OnPlayerCaptured?.Invoke();

            // Legacy: activar panel educativo si está asignado directamente
            if (educationalPanel != null)
                educationalPanel.SetActive(true);

            Debug.Log("[BlackHole] Jugador capturado por el horizonte de eventos");
        }
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eventHorizonRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }
}
