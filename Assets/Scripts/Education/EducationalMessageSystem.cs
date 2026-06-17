using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema de mensajes educativos que se activan basándose en la distancia al agujero negro.
/// El panel se posiciona como hijo de la cámara para GARANTIZAR que siempre esté frente al jugador.
/// </summary>
public class EducationalMessageSystem : MonoBehaviour
{
    [System.Serializable]
    public class EducationalMessage
    {
        [Header("Contenido")]
        public string titulo;
        [TextArea(3, 6)] public string contenido;
        public AudioClip audioNarración;

        [Header("Activación")]
        [Tooltip("Distancia al agujero negro a la cual se activa este mensaje")]
        public float distanciaActivacion = 40f;

        [Tooltip("Duración en segundos que el mensaje permanece visible")]
        public float duracion = 10f;

        [HideInInspector] public bool mostrado = false;
    }

    [Header("Mensajes Educativos")]
    public EducationalMessage[] mensajes = new EducationalMessage[]
    {
        new EducationalMessage
        {
            titulo = "La Velocidad de la Luz",
            contenido = "Estás viajando como un fotón: a 299,792 km/s.\nNada en el universo puede superar esta velocidad.\nPero incluso la luz es afectada por la gravedad...",
            distanciaActivacion = 45f,
            duracion = 12f
        },
        new EducationalMessage
        {
            titulo = "Curvatura del Espacio-Tiempo",
            contenido = "La masa del agujero negro curva el tejido del espacio-tiempo.\nTu trayectoria se desvía no porque una fuerza te jale,\nsino porque el espacio mismo está deformado.",
            distanciaActivacion = 35f,
            duracion = 12f
        },
        new EducationalMessage
        {
            titulo = "El Disco de Acreción",
            contenido = "La materia que orbita el agujero negro forma un disco brillante.\nSe calienta a millones de grados por la fricción\ny emite rayos X detectables desde la Tierra.",
            distanciaActivacion = 25f,
            duracion = 12f
        },
        new EducationalMessage
        {
            titulo = "Horizonte de Eventos",
            contenido = "Estás acercándote al punto de no retorno.\nUna vez cruzado el horizonte de eventos,\nni siquiera la luz puede escapar.\n¡Cuidado!",
            distanciaActivacion = 12f,
            duracion = 10f
        }
    };

    [Header("Referencias UI")]
    public Canvas mensajeCanvas;
    public CanvasGroup panelCanvasGroup;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoContenido;
    public Image panelBackground;

    [Header("Referencias de Sistema")]
    public BlackHoleGravity blackHole;
    public AudioSource audioSource;
    public Transform playerCamera;

    [Header("Posicionamiento del Panel")]
    [Tooltip("Distancia frente a la cámara donde aparece el panel")]
    public float distanciaFrontal = 2.5f;

    [Tooltip("Offset vertical (positivo = arriba, negativo = abajo)")]
    public float offsetVertical = -0.3f;

    [Header("Animación")]
    [Tooltip("Duración del fade in/out en segundos")]
    public float fadeSpeed = 1.2f;

    private bool mostrandoMensaje = false;
    private Coroutine mensajeActual;

    void Start()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.gameObject.SetActive(false);
        }

        // Anclar el canvas como hijo de la cámara para que siempre esté frente al jugador
        if (mensajeCanvas != null && playerCamera != null)
        {
            mensajeCanvas.transform.SetParent(playerCamera);
            mensajeCanvas.transform.localPosition = new Vector3(0f, offsetVertical, distanciaFrontal);
            mensajeCanvas.transform.localRotation = Quaternion.identity;
        }

        if (blackHole != null)
        {
            blackHole.OnDistanceChanged += EvaluarMensajes;
            blackHole.OnPlayerCaptured += MostrarMensajeFinal;
        }
    }

    void EvaluarMensajes(float distancia)
    {
        if (mostrandoMensaje) return;

        for (int i = 0; i < mensajes.Length; i++)
        {
            if (!mensajes[i].mostrado && distancia <= mensajes[i].distanciaActivacion)
            {
                mensajes[i].mostrado = true;
                mensajeActual = StartCoroutine(MostrarMensajeCoroutine(mensajes[i]));
                break;
            }
        }
    }

    IEnumerator MostrarMensajeCoroutine(EducationalMessage mensaje)
    {
        mostrandoMensaje = true;

        // Configurar contenido
        if (textoTitulo != null) textoTitulo.text = mensaje.titulo;
        if (textoContenido != null) textoContenido.text = mensaje.contenido;

        // Asegurar posición frente a la cámara
        if (mensajeCanvas != null)
        {
            mensajeCanvas.transform.localPosition = new Vector3(0f, offsetVertical, distanciaFrontal);
            mensajeCanvas.transform.localRotation = Quaternion.identity;
        }

        // Activar panel
        panelCanvasGroup.gameObject.SetActive(true);

        // Fade In
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeSpeed;
            panelCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }
        panelCanvasGroup.alpha = 1f;

        // Reproducir audio
        if (mensaje.audioNarración != null && audioSource != null)
        {
            audioSource.PlayOneShot(mensaje.audioNarración);
        }

        // Esperar duración
        yield return new WaitForSeconds(mensaje.duracion);

        // Fade Out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeSpeed;
            panelCanvasGroup.alpha = Mathf.SmoothStep(1f, 0f, t);
            yield return null;
        }
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.gameObject.SetActive(false);

        mostrandoMensaje = false;
    }

    void MostrarMensajeFinal()
    {
        EducationalMessage mensajeFinal = new EducationalMessage
        {
            titulo = "SINGULARIDAD",
            contenido = "Has cruzado el horizonte de eventos.\nAquí, el espacio y el tiempo intercambian roles.\nTodo movimiento te lleva inevitablemente\nhacia la singularidad central.",
            duracion = 15f
        };

        if (mensajeActual != null)
            StopCoroutine(mensajeActual);

        mostrandoMensaje = false;
        mensajeActual = StartCoroutine(MostrarMensajeCoroutine(mensajeFinal));
    }

    void OnDestroy()
    {
        if (blackHole != null)
        {
            blackHole.OnDistanceChanged -= EvaluarMensajes;
            blackHole.OnPlayerCaptured -= MostrarMensajeFinal;
        }
    }
}
