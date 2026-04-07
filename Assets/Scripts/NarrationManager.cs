using System.Collections;
using System.Collections.Generic;
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
        public float distanciaActivacion = 10f;
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

        if (zona.audio != null && audioSource != null)
            audioSource.PlayOneShot(zona.audio);

        Debug.Log("Zona activada: " + indice);
    }
}