using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Asegura que el XR Origin (XR Rig) tenga la fisica necesaria para caminar dentro de la nave:
/// un CharacterController que sigue la cabeza del jugador y aplica gravedad y colisiones.
///
/// El DynamicMoveProvider de los Starter Assets ya aplica gravedad (m_UseGravity),
/// pero SOLO funciona si el XR Origin tiene un CharacterController. Este script
/// crea/configura ese CharacterController automaticamente y lo mantiene alineado con la cabeza.
///
/// COLOCAR ESTE COMPONENTE EN: el GameObject "XR Origin (XR Rig)".
/// </summary>
[DefaultExecutionOrder(1)] // corre despues del XR Origin
[RequireComponent(typeof(CharacterController))]
public class PlayerGravitySetup : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("XR Origin. Si se deja vacio, se busca en este GameObject o en los padres.")]
    public XROrigin xrOrigin;

    [Tooltip("Transform de la camara/cabeza. Si se deja vacio, se toma de XR Origin.")]
    public Transform cameraTransform;

    [Header("Dimensiones del cuerpo del jugador")]
    [Tooltip("Radio del cuerpo del jugador en metros (colision con paredes).")]
    public float bodyRadius = 0.25f;

    [Tooltip("Altura minima del CharacterController (evita valores negativos si el jugador se agacha).")]
    public float minHeight = 0.9f;

    [Tooltip("Margen sobre la cabeza para calcular la altura total.")]
    public float headHeightPadding = 0.1f;

    [Tooltip("Distancia que el CharacterController puede subir (escalones/umbral de puertas).")]
    public float stepOffset = 0.3f;

    private CharacterController characterController;

    private void Awake()
    {
        if (xrOrigin == null)
            xrOrigin = GetComponentInParent<XROrigin>();

        if (cameraTransform == null && xrOrigin != null && xrOrigin.Camera != null)
            cameraTransform = xrOrigin.Camera.transform;

        characterController = GetComponent<CharacterController>();
        ConfigureController();
    }

    private void ConfigureController()
    {
        // Valores base razonables para VR de pie.
        characterController.radius = bodyRadius;
        characterController.height = minHeight;
        characterController.stepOffset = stepOffset;
        characterController.center = new Vector3(0f, minHeight * 0.5f, 0f);
    }

    private void Update()
    {
        // Mantener el CharacterController alineado con la posicion real de la cabeza,
        // para que la colision y la gravedad correspondan a donde esta parado el jugador.
        if (xrOrigin == null || cameraTransform == null || characterController == null)
            return;

        // Altura del jugador = altura de la camara respecto al piso del rig.
        float headHeight = Mathf.Clamp(
            xrOrigin.CameraInOriginSpaceHeight,
            minHeight,
            3f);

        float targetHeight = headHeight + headHeightPadding;
        characterController.height = targetHeight;

        // Centrar el capsule bajo la cabeza (en espacio local del rig).
        Vector3 centerInRig = xrOrigin.CameraInOriginSpacePos;
        characterController.center = new Vector3(centerInRig.x, targetHeight * 0.5f, centerInRig.z);
    }
}
