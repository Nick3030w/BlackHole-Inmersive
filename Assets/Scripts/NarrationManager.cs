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
        public bool reproducido = false;
    }

    [Header("Zonas de narración")]
    public NarrationZone[] zonas = new NarrationZone[4];

    [Header("Referencias")]
    public GameObject panelSubtitulos;
    public Text textoSubtitulo;
    public AudioSource audioSource;

    private float tiempoMostrar = 5f;
    private float temporizador = 0f;
    private bool mostrando = false;

    void Update()
    {
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

    public void ActivarZona(int indice)
    {
        if (indice < 0 || indice >= zonas.Length) return;
        NarrationZone zona = zonas[indice];
        if (zona.reproducido) return;

        zona.reproducido = true;
        textoSubtitulo.text = zona.subtitulo;
        panelSubtitulos.SetActive(true);
        mostrando = true;
        temporizador = tiempoMostrar;

        if (zona.audio != null && audioSource != null)
            audioSource.PlayOneShot(zona.audio);
    }
}