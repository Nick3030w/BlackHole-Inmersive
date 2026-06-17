using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manager de narración basado en zonas de proximidad.
/// NOTA: Este script se mantiene por compatibilidad. Para la escena BlackHole_Core,
/// usa EducationalMessageSystem que integra directamente con BlackHoleGravity.
/// </summary>
public class NarrationManager : MonoBehaviour
{
    [System.Serializable]
    public class NarrationZone
    {
        public string titulo;
        [TextArea] public string subtitulo;
        public AudioClip audio;
        public float distanciaActivacion = 50f;
        [HideInInspector] public bool reproducido = false;
    }

    [Header("Zonas de narración")]
    public NarrationZone[] zonas = new NarrationZone[4];

    [Header("Referencias")]
    public GameObject panelSubtitulos;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoSubtitulo;
    public AudioSource audioSource;
    public Transform[] posicionesZona;
    public Transform jugador;

    [Header("Configuración")]
    public float tiempoMostrar = 12f;

    private float temporizador = 0f;
    private bool mostrando = false;

    void Start()
    {
        if (jugador == null)
            Debug.LogWarning("[NarrationManager] Jugador no asignado. Intentando buscar XR Origin...");

        if (panelSubtitulos != null)
            panelSubtitulos.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;

        for (int i = 0; i < zonas.Length; i++)
        {
            if (zonas[i].reproducido) continue;
            if (i >= posicionesZona.Length || posicionesZona[i] == null) continue;

            float distancia = Vector3.Distance(jugador.position, posicionesZona[i].position);

            if (distancia < zonas[i].distanciaActivacion)
            {
                ActivarZona(i);
                break;
            }
        }

        if (mostrando)
        {
            temporizador -= Time.deltaTime;
            if (temporizador <= 0f)
            {
                panelSubtitulos.SetActive(false);
                mostrando = false;
            }
        }
    }

    void ActivarZona(int indice)
    {
        NarrationZone zona = zonas[indice];
        zona.reproducido = true;

        if (textoTitulo != null)
            textoTitulo.text = zona.titulo;

        if (textoSubtitulo != null)
            textoSubtitulo.text = zona.subtitulo;

        if (panelSubtitulos != null)
        {
            panelSubtitulos.SetActive(true);
            mostrando = true;
            temporizador = tiempoMostrar;
        }

        Debug.Log("[NarrationManager] Zona activada: " + indice + " - " + zona.titulo);

        if (zona.audio != null && audioSource != null)
            audioSource.PlayOneShot(zona.audio);
    }
}
