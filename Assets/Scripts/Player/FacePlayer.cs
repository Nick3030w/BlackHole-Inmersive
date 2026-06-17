using UnityEngine;

/// <summary>
/// Hace que un objeto (como un panel UI) siempre mire hacia el jugador.
/// Útil para paneles informativos en World Space que necesitan ser legibles desde cualquier ángulo.
/// </summary>
public class FacePlayer : MonoBehaviour
{
    [Tooltip("Transform del jugador/cámara a mirar")]
    public Transform player;

    [Tooltip("Si es true, solo rota en el eje Y (mantiene vertical)")]
    public bool soloRotacionY = true;

    [Tooltip("Suavizado de la rotación (0 = instantáneo, valores altos = más suave)")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 lookDirection = player.position - transform.position;

        if (soloRotacionY)
            lookDirection.y = 0;

        if (lookDirection.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(-lookDirection);

        if (smoothSpeed > 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        else
            transform.rotation = targetRotation;
    }
}
