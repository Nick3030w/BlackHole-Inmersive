using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema de mensajes educativos que se activan basándose en la distancia al agujero negro.
/// Los mensajes aparecen entre el jugador y el agujero negro (la dirección natural de mirada),
/// con animaciones de fade-in/out y diseño visual atractivo.
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
    [Tooltip("Canvas que contiene el panel de mensajes")]
    public Canvas mensajeCanvas;

    [Tooltip("Panel contenedor del mensaje (con CanvasGroup para fade)")]
    public CanvasGroup panelCanvasGroup;

    [Tooltip("Texto del título del mensaje")]
    public TextMeshProUGUI textoTitulo;

    [Tooltip("Texto del contenido del mensaje")]
    public TextMeshProUGUI textoContenido;

    [Tooltip("Imagen de fondo del panel (para glow effect)")]
    public Image panelBackground;

    [Header("Referencias de Sistema")]
    public BlackHoleGravity blackHole;
    public AudioSource audioSource;
    public Transform playerCamera;

    [Header("Configuración de Posicionamiento")]
    [Tooltip("Distancia del panel frente al jugador (en metros)")]
    public float distanciaPanel = 2.0f;

    [Tooltip("Altura offset del panel respecto a los ojos del jugador")]
    public float alturaPanel = 0f;

    [Tooltip("Velocidad con la que el panel sigue al jugador")]
    public float followSpeed = 10f;

    [Header("Configuración de Animación")]
    [Tooltip("Duración del fade in/out en segundos")]
    public float fadeSpeed = 1.5f;

    private bool mostrandoMensaje = false;
    private Coroutine mensajeActual;

    void Start()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.gameObject.SetActive(false);
        }

        if (blackHole != null)
        {
            blackHole.OnDistanceChanged += EvaluarMensajes;
            blackHole.OnPlayerCaptured += MostrarMensajeFinal;
        }
    }

    void LateUpdate()
    {
        if (!mostrandoMensaje || playerCamera == null || mensajeCanvas == null) return;

        // === POSICIONAMIENTO ===
        // Calcular la dirección hacia el agujero negro (donde el jugador naturalmente mira)
        Vector3 dirToBlackHole = Vector3.zero;

        if (blackHole != null)
        {
            dirToBlackHole = (blackHole.transform.position - playerCamera.position).normalized;
        }

        // Usar la dirección hacia el agujero negro como base,
        // pero proyectada horizontalmente para que no se vaya abajo
        Vector3 panelDirection;

        if (dirToBlackHole.sqrMagnitude > 0.01f)
        {
            panelDirection = dirToBlackHole;
            panelDirection.y = 0f; // Mantener horizontal
            panelDirection.Normalize();
        }
        else
        {
            // Fallback: usar forward de la cámara
            panelDirection = playerCamera.forward;
            panelDirection.y = 0f;
            panelDirection.Normalize();
        }

        // Posición objetivo: frente al jugador, en dirección al agujero negro
        Vector3 targetPos = playerCamera.position + panelDirection * distanciaPanel;
        targetPos.y = playerCamera.position.y + alturaPanel;

        // Mover suavemente hacia la posición objetivo
        mensajeCanvas.transform.position = Vector3.Lerp(
            mensajeCanvas.transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        // === ROTACIÓN: siempre mirar al jugador ===
        Vector3 lookDir = playerCamera.position - mensajeCanvas.transform.position;
        lookDir.y = 0f; // Mantener el panel vertical (no inclinado)

        if (lookDir.sqrMagnitude > 0.001f)
        {
            mensajeCanvas.transform.rotation = Quaternion.LookRotation(-lookDir, Vector3.up);
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

        // Posicionar instantáneamente frente al jugador antes de mostrar
        if (playerCamera != null && mensajeCanvas != null)
        {
            Vector3 dir = Vector3.forward;

            if (blackHole != null)
            {
                dir = (blackHole.transform.position - playerCamera.position).normalized;
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.01f) dir = playerCamera.forward;
                dir.Normalize();
            }

            Vector3 pos = playerCamera.position + dir * distanciaPanel;
            pos.y = playerCamera.position.y + alturaPanel;
            mensajeCanvas.transform.position = pos;

            // Rotación: mirar al jugador
            Vector3 lookDir = playerCamera.position - pos;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
                mensajeCanvas.transform.rotation = Quaternion.LookRotation(-lookDir, Vector3.up);
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

        // Reproducir audio si existe
        if (mensaje.audioNarración != null && audioSource != null)
        {
            audioSource.PlayOneShot(mensaje.audioNarración);
        }

        // Esperar duración del mensaje
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
