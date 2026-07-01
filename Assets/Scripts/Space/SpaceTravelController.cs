using UnityEngine;

/// <summary>
/// Controla el viaje del jugador a través del espacio hacia el agujero negro.
/// Movimiento suave a velocidad constante para evitar cinetosis (motion sickness) en VR.
/// 
/// El jugador avanza automáticamente por el corredor espacial. La velocidad puede
/// acelerar sutilmente conforme se acerca al agujero negro (atracción gravitacional).
/// 
/// INSTRUCCIONES:
/// 1. Agregar al XR Origin / Camera Offset o a la Main Camera
/// 2. Configurar la velocidad y dirección
/// </summary>
public class SpaceTravelController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad de crucero constante (unidades/segundo)")]
    public float cruiseSpeed = 15f;

    [Tooltip("Dirección del viaje (normalmente hacia adelante en Z)")]
    public Vector3 travelDirection = Vector3.forward;

    [Tooltip("Aceleración suave al inicio (segundos hasta alcanzar velocidad de crucero)")]
    public float accelerationTime = 3f;

    [Header("Atracción Gravitacional (opcional)")]
    [Tooltip("Referencia al agujero negro para acelerar al acercarse")]
    public Transform blackHole;

    [Tooltip("Distancia a la cual empieza la aceleración gravitacional")]
    public float gravitationalRange = 200f;

    [Tooltip("Multiplicador máximo de velocidad por gravedad")]
    public float maxGravityBoost = 2.5f;

    [Header("Balanceo Sutil (inmersión)")]
    [Tooltip("Activar un leve balanceo para dar sensación de vuelo")]
    public bool enableSway = true;

    [Tooltip("Amplitud del balanceo")]
    public float swayAmount = 0.3f;

    [Tooltip("Velocidad del balanceo")]
    public float swaySpeed = 0.5f;

    [Header("Estado")]
    public bool isTraveling = true;

    private float currentSpeed = 0f;
    private float travelTime = 0f;
    private Vector3 basePosition;
    private Vector3 startPosition;

    void Start()
    {
        travelDirection.Normalize();
        startPosition = transform.position;
        basePosition = transform.position;
    }

    void Update()
    {
        if (!isTraveling) return;

        travelTime += Time.deltaTime;

        // Aceleración suave inicial (evita mareo por arranque brusco)
        float targetSpeed = cruiseSpeed;

        // Boost gravitacional al acercarse al agujero negro
        if (blackHole != null)
        {
            float distToBH = Vector3.Distance(transform.position, blackHole.position);
            if (distToBH < gravitationalRange)
            {
                float proximity = 1f - (distToBH / gravitationalRange);
                float boost = Mathf.Lerp(1f, maxGravityBoost, proximity * proximity);
                targetSpeed *= boost;
            }
        }

        // Interpolar la velocidad actual hacia la objetivo
        float accelRate = cruiseSpeed / Mathf.Max(0.1f, accelerationTime);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.deltaTime);

        // Avanzar la posición base
        basePosition += travelDirection * currentSpeed * Time.deltaTime;

        // Aplicar balanceo sutil para inmersión
        Vector3 finalPosition = basePosition;
        if (enableSway)
        {
            float swayX = Mathf.Sin(travelTime * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(travelTime * swaySpeed * 0.7f) * swayAmount * 0.6f;
            finalPosition += new Vector3(swayX, swayY, 0f);
        }

        transform.position = finalPosition;
    }

    /// <summary>
    /// Distancia total recorrida desde el inicio
    /// </summary>
    public float DistanceTraveled => Vector3.Distance(startPosition, basePosition);

    /// <summary>
    /// Detener el viaje (para transiciones o llegada)
    /// </summary>
    public void StopTravel()
    {
        isTraveling = false;
    }

    /// <summary>
    /// Reanudar el viaje
    /// </summary>
    public void ResumeTravel()
    {
        isTraveling = true;
    }
}
