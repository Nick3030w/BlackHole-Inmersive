using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Boton VR que abre/cierra una o varias puertas de la nave al ser activado.
/// Funciona con cualquier interaccion del XR Interaction Toolkit que dispare
/// "Select" sobre un XRSimpleInteractable: apuntar y gatillo (Ray Interactor),
/// tocar con la mano (Direct Interactor) o empujar (Poke Interactor).
///
/// COLOCAR ESTE COMPONENTE EN: el GameObject del boton fisico junto a la puerta.
/// REQUISITOS en ese mismo GameObject:
///   - Un Collider (BoxCollider suele bastar) para poder ser seleccionado/tocado.
///   - Un XRSimpleInteractable (se agrega automaticamente si falta).
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class ShipDoorButton : MonoBehaviour
{
    public enum Accion { Alternar, SoloAbrir, SoloCerrar }

    [Header("Puertas controladas")]
    [Tooltip("Puertas que este boton controla. Puede ser una o varias.")]
    public ShipDoor[] puertas;

    [Header("Comportamiento")]
    [Tooltip("Que hace el boton al activarse.")]
    public Accion accion = Accion.Alternar;

    [Tooltip("Tiempo minimo entre activaciones para evitar dobles disparos (segundos).")]
    public float cooldown = 0.4f;

    [Header("Feedback visual (opcional)")]
    [Tooltip("Parte del boton que se hunde al presionarse.")]
    public Transform tapaBoton;
    [Tooltip("Cuanto se hunde la tapa al presionar (metros, en su eje local Y).")]
    public float hundimiento = 0.01f;
    [Tooltip("Velocidad de retorno de la tapa a su posicion original.")]
    public float velocidadRetorno = 8f;

    [Header("Feedback de luz (opcional)")]
    [Tooltip("Renderer del boton para cambiar su color emisivo segun estado.")]
    public Renderer indicadorRenderer;
    public Color colorInactivo = new Color(0.6f, 0.1f, 0.1f);
    public Color colorActivo = new Color(0.1f, 0.8f, 0.2f);

    [Header("Audio (opcional)")]
    public AudioSource audioSource;
    public AudioClip sonidoClick;

    private XRSimpleInteractable interactable;
    private float ultimoUso = -999f;
    private Vector3 tapaPosOriginal;
    private bool tieneTapa;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (tapaBoton != null)
        {
            tapaPosOriginal = tapaBoton.localPosition;
            tieneTapa = true;
        }

        ActualizarIndicador();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Presionar();
    }

    /// <summary>Activa el boton. Publico para poder llamarlo tambien desde otros eventos.</summary>
    public void Presionar()
    {
        if (Time.time - ultimoUso < cooldown)
            return;
        ultimoUso = Time.time;

        if (puertas != null)
        {
            foreach (ShipDoor puerta in puertas)
            {
                if (puerta == null) continue;
                switch (accion)
                {
                    case Accion.Alternar: puerta.Alternar(); break;
                    case Accion.SoloAbrir: puerta.Abrir(); break;
                    case Accion.SoloCerrar: puerta.Cerrar(); break;
                }
            }
        }

        if (audioSource != null && sonidoClick != null)
            audioSource.PlayOneShot(sonidoClick);

        if (tieneTapa)
            tapaBoton.localPosition = tapaPosOriginal - Vector3.up * hundimiento;

        ActualizarIndicador();
    }

    private void Update()
    {
        // Devolver la tapa a su posicion original de forma suave.
        if (tieneTapa)
        {
            tapaBoton.localPosition = Vector3.Lerp(
                tapaBoton.localPosition,
                tapaPosOriginal,
                Time.deltaTime * velocidadRetorno);
        }
    }

    private void ActualizarIndicador()
    {
        if (indicadorRenderer == null) return;

        bool algunaAbierta = false;
        if (puertas != null)
        {
            foreach (ShipDoor p in puertas)
            {
                if (p != null && p.EstaAbierta) { algunaAbierta = true; break; }
            }
        }

        Color c = algunaAbierta ? colorActivo : colorInactivo;
        Material mat = indicadorRenderer.material; // instancia para no tocar el shared material
        mat.EnableKeyword("_EMISSION");
        mat.SetColor(EmissionColorId, c);
    }
}
