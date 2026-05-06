using UnityEngine;
using UnityEngine.UI;

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
    public Text textoSubtitulo;
    public AudioSource audioSource;
    public Transform[] posicionesZona;
    public Transform jugador;

    private float tiempoMostrar = 6f;
    private float temporizador = 0f;
    private bool mostrando = false;

    void Start()
    {
        Debug.Log("NarrationManager iniciado");
        if (jugador == null) Debug.LogError("JUGADOR NO ASIGNADO");
        if (panelSubtitulos == null) Debug.LogError("PANEL NO ASIGNADO");
        if (textoSubtitulo == null) Debug.LogError("TEXTO NO ASIGNADO");
    }

    void Update()
    {
        if (jugador == null) return;

        for (int i = 0; i < zonas.Length; i++)
        {
            if (zonas[i].reproducido) continue;
            if (i >= posicionesZona.Length) continue;

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
        textoSubtitulo.text = zona.subtitulo;
        panelSubtitulos.SetActive(true);
        mostrando = true;
        temporizador = tiempoMostrar;
        Debug.Log("ZONA ACTIVADA: " + indice + " texto: " + zona.subtitulo);

        if (zona.audio != null && audioSource != null)
            audioSource.PlayOneShot(zona.audio);
    }
}