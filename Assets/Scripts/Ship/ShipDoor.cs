using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Puerta deslizante de la nave. Se mueve suavemente entre una posicion cerrada
/// y una abierta a lo largo de un eje local. Puede abrirse por un boton VR,
/// por un trigger de proximidad, o desde codigo.
///
/// COLOCAR ESTE COMPONENTE EN: el GameObject de la hoja de puerta que se debe mover
/// (o en un contenedor que agrupe ambas hojas si es puerta doble; en ese caso
/// usa dos ShipDoor, uno por hoja, con direcciones opuestas).
/// </summary>
public class ShipDoor : MonoBehaviour
{
    public enum Eje { X, Y, Z }

    [Header("Movimiento")]
    [Tooltip("Eje LOCAL a lo largo del cual se desliza la puerta.")]
    public Eje ejeDeslizamiento = Eje.X;

    [Tooltip("Distancia que recorre la puerta al abrirse (en metros).")]
    public float distanciaApertura = 1.2f;

    [Tooltip("Velocidad de apertura/cierre (metros por segundo).")]
    public float velocidad = 2.0f;

    [Tooltip("Curva de suavizado del movimiento (0..1 en X e Y).")]
    public AnimationCurve suavizado = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Estado inicial")]
    [Tooltip("Si es true, la puerta arranca abierta.")]
    public bool empiezaAbierta = false;

    [Header("Cierre automatico")]
    [Tooltip("Si es mayor que 0, la puerta se cierra sola tras estos segundos de estar abierta.")]
    public float autoCerrarTras = 0f;

    [Header("Audio (opcional)")]
    public AudioSource audioSource;
    public AudioClip sonidoAbrir;
    public AudioClip sonidoCerrar;

    [Header("Eventos")]
    public UnityEvent onAbrir;
    public UnityEvent onCerrar;

    public bool EstaAbierta { get; private set; }

    private Vector3 posCerrada;
    private Vector3 posAbierta;
    private float t; // 0 = cerrada, 1 = abierta
    private float tiempoAbierta;

    private void Awake()
    {
        posCerrada = transform.localPosition;
        posAbierta = posCerrada + DireccionLocal() * distanciaApertura;

        EstaAbierta = empiezaAbierta;
        t = empiezaAbierta ? 1f : 0f;
        AplicarPosicion();
    }

    private Vector3 DireccionLocal()
    {
        switch (ejeDeslizamiento)
        {
            case Eje.X: return Vector3.right;
            case Eje.Y: return Vector3.up;
            default: return Vector3.forward;
        }
    }

    private void Update()
    {
        float objetivo = EstaAbierta ? 1f : 0f;

        if (!Mathf.Approximately(t, objetivo))
        {
            float paso = (velocidad / Mathf.Max(0.001f, distanciaApertura)) * Time.deltaTime;
            t = Mathf.MoveTowards(t, objetivo, paso);
            AplicarPosicion();
        }

        if (EstaAbierta && autoCerrarTras > 0f)
        {
            tiempoAbierta += Time.deltaTime;
            if (tiempoAbierta >= autoCerrarTras)
                Cerrar();
        }
    }

    private void AplicarPosicion()
    {
        float k = suavizado.Evaluate(t);
        transform.localPosition = Vector3.LerpUnclamped(posCerrada, posAbierta, k);
    }

    /// <summary>Alterna entre abrir y cerrar. Ideal para conectar a un boton.</summary>
    public void Alternar()
    {
        if (EstaAbierta) Cerrar();
        else Abrir();
    }

    public void Abrir()
    {
        if (EstaAbierta) return;
        EstaAbierta = true;
        tiempoAbierta = 0f;
        ReproducirSonido(sonidoAbrir);
        onAbrir?.Invoke();
    }

    public void Cerrar()
    {
        if (!EstaAbierta) return;
        EstaAbierta = false;
        ReproducirSonido(sonidoCerrar);
        onCerrar?.Invoke();
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // Dibuja en el editor la posicion abierta para facilitar el ajuste.
    private void OnDrawGizmosSelected()
    {
        Vector3 baseLocal = Application.isPlaying ? posCerrada : transform.localPosition;
        Vector3 dirWorld = transform.TransformDirection(DireccionLocal());
        Vector3 origenWorld = transform.parent != null
            ? transform.parent.TransformPoint(baseLocal)
            : baseLocal;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origenWorld, 0.05f);
        Gizmos.color = Color.cyan;
        Vector3 destino = origenWorld + dirWorld * distanciaApertura;
        Gizmos.DrawLine(origenWorld, destino);
        Gizmos.DrawWireSphere(destino, 0.05f);
    }
}
