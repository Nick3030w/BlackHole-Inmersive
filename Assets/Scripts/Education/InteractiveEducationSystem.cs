using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

/// <summary>
/// Sistema de paneles educativos interactivos para la escena Space_Travel.
/// Los paneles aparecen en checkpoints del recorrido, pausan el viaje suavemente,
/// muestran información física real y esperan a que el usuario decida continuar.
/// 
/// Diseño visual espacial y profesional, armónico con el resto de la experiencia.
/// </summary>
public class InteractiveEducationSystem : MonoBehaviour
{
    [Header("Referencias de Sistema")]
    public Transform playerCamera;
    public SpaceTravelController travelController;
    public AudioSource audioSource;

    [Header("Checkpoints")]
    [Tooltip("Distancias de viaje a las que aparece cada panel educativo")]
    public float[] checkpointDistances = new float[] { 60f, 150f, 250f, 360f, 480f, 600f, 720f };

    [Header("Sonido (opcional)")]
    public AudioClip sonidoAparicion;
    public AudioClip sonidoContinuar;

    [Header("Configuración Visual")]
    public float distanciaPanel = 2.2f;
    public float offsetVertical = -0.15f;
    public float fadeSpeed = 1.5f;

    // Referencias UI generadas
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI tituloText;
    private TextMeshProUGUI subtituloText;
    private TextMeshProUGUI cuerpoText;
    private TextMeshProUGUI datoText;
    private TextMeshProUGUI contadorText;
    private Button continuarButton;
    private TextMeshProUGUI continuarButtonText;

    private SpaceEducationContent.EduPanel[] panels;
    private int currentPanelIndex = 0;
    private bool panelActivo = false;
    private bool[] checkpointTriggered;
    private PanelInteractionHandler interactionHandler;

    void Start()
    {
        panels = SpaceEducationContent.GetPanels();
        checkpointTriggered = new bool[checkpointDistances.Length];

        BuildUI();

        // Handler de interacción (teclado/gatillo como respaldo del botón)
        interactionHandler = gameObject.AddComponent<PanelInteractionHandler>();
        interactionHandler.onContinue = OnContinuarPressed;
        interactionHandler.isActive = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (travelController == null || panelActivo) return;

        float traveled = travelController.DistanceTraveled;

        // Verificar checkpoints
        for (int i = 0; i < checkpointDistances.Length; i++)
        {
            if (!checkpointTriggered[i] && traveled >= checkpointDistances[i])
            {
                checkpointTriggered[i] = true;
                if (currentPanelIndex < panels.Length)
                {
                    StartCoroutine(MostrarPanel(panels[currentPanelIndex]));
                    currentPanelIndex++;
                }
                break;
            }
        }
    }

    void LateUpdate()
    {
        // Mantener el panel frente a la cámara
        if (panelActivo && playerCamera != null && canvas != null)
        {
            Vector3 targetPos = playerCamera.position + playerCamera.forward * distanciaPanel;
            targetPos.y += offsetVertical;
            canvas.transform.position = targetPos;
            canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - playerCamera.position);
        }
    }

    IEnumerator MostrarPanel(SpaceEducationContent.EduPanel panel)
    {
        panelActivo = true;

        // Pausar el viaje suavemente
        if (travelController != null)
            travelController.StopTravel();

        // Llenar contenido
        tituloText.text = panel.titulo;
        subtituloText.text = panel.subtitulo;
        cuerpoText.text = panel.cuerpo;
        datoText.text = "<b>¿Sabías que...?</b>\n" + panel.dato;
        contadorText.text = currentPanelIndex + " / " + panels.Length;

        // Posicionar frente a la cámara
        if (playerCamera != null)
        {
            Vector3 pos = playerCamera.position + playerCamera.forward * distanciaPanel;
            pos.y += offsetVertical;
            canvas.transform.position = pos;
            canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - playerCamera.position);
        }

        canvasGroup.gameObject.SetActive(true);

        if (sonidoAparicion != null && audioSource != null)
            audioSource.PlayOneShot(sonidoAparicion);

        // Fade in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeSpeed;
            canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // El botón continuar controla el cierre — habilitar interacción
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Activar el handler de respaldo (teclado/gatillo)
        if (interactionHandler != null)
            interactionHandler.SetActive(true);
    }

    void OnContinuarPressed()
    {
        if (!panelActivo) return;

        // Desactivar handler para evitar doble input
        if (interactionHandler != null)
            interactionHandler.isActive = false;

        if (sonidoContinuar != null && audioSource != null)
            audioSource.PlayOneShot(sonidoContinuar);

        StartCoroutine(OcultarPanel());
    }

    IEnumerator OcultarPanel()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Fade out
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeSpeed;
            canvasGroup.alpha = Mathf.SmoothStep(1f, 0f, t);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);

        // Reanudar el viaje
        if (travelController != null)
            travelController.ResumeTravel();

        panelActivo = false;
    }

    // ================= CONSTRUCCIÓN DE UI =================

    void BuildUI()
    {
        // === CANVAS WORLD SPACE ===
        GameObject canvasObj = new GameObject("EducationCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1000, 640);
        canvasObj.transform.localScale = Vector3.one * 0.0016f; // Escala world space

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Soporte de interacción VR: agregar TrackedDeviceGraphicRaycaster si XR Toolkit está presente
        AddVRRaycaster(canvasObj);
        EnsureEventSystem();

        // === PANEL PRINCIPAL ===
        GameObject panel = CreateUIObject("Panel", canvasObj.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bgImage = panel.AddComponent<Image>();
        bgImage.color = new Color(0.03f, 0.04f, 0.09f, 0.94f);

        canvasGroup = panel.AddComponent<CanvasGroup>();

        // === BORDES LUMINOSOS ===
        CreateLine(panel.transform, new Vector2(0, 0.997f), new Vector2(1, 1), new Color(0.3f, 0.65f, 1f, 0.9f));       // Top
        CreateLine(panel.transform, new Vector2(0, 0), new Vector2(1, 0.003f), new Color(0.3f, 0.65f, 1f, 0.9f));        // Bottom
        CreateLine(panel.transform, new Vector2(0, 0), new Vector2(0.002f, 1), new Color(0.3f, 0.65f, 1f, 0.5f));        // Left
        CreateLine(panel.transform, new Vector2(0.998f, 0), new Vector2(1, 1), new Color(0.3f, 0.65f, 1f, 0.5f));        // Right

        // Esquinas HUD
        CreateCorner(panel.transform, new Vector2(0.0f, 0.94f), new Vector2(0.12f, 0.955f));
        CreateCorner(panel.transform, new Vector2(0.88f, 0.94f), new Vector2(1f, 0.955f));

        // === CONTADOR (esquina superior derecha) ===
        GameObject contadorObj = CreateUIObject("Contador", panel.transform);
        RectTransform contadorRect = contadorObj.GetComponent<RectTransform>();
        contadorRect.anchorMin = new Vector2(0.75f, 0.9f);
        contadorRect.anchorMax = new Vector2(0.96f, 0.98f);
        contadorRect.offsetMin = Vector2.zero;
        contadorRect.offsetMax = Vector2.zero;
        contadorText = contadorObj.AddComponent<TextMeshProUGUI>();
        contadorText.text = "1 / 7";
        contadorText.fontSize = 24;
        contadorText.alignment = TextAlignmentOptions.MidlineRight;
        contadorText.color = new Color(0.5f, 0.7f, 1f, 0.7f);

        // === TÍTULO ===
        GameObject tituloObj = CreateUIObject("Titulo", panel.transform);
        RectTransform tituloRect = tituloObj.GetComponent<RectTransform>();
        tituloRect.anchorMin = new Vector2(0.05f, 0.82f);
        tituloRect.anchorMax = new Vector2(0.95f, 0.94f);
        tituloRect.offsetMin = Vector2.zero;
        tituloRect.offsetMax = Vector2.zero;
        tituloText = tituloObj.AddComponent<TextMeshProUGUI>();
        tituloText.text = "TÍTULO";
        tituloText.fontSize = 46;
        tituloText.fontStyle = FontStyles.Bold;
        tituloText.alignment = TextAlignmentOptions.Center;
        tituloText.color = new Color(0.65f, 0.9f, 1f);
        tituloText.characterSpacing = 5f;

        // === SUBTÍTULO ===
        GameObject subObj = CreateUIObject("Subtitulo", panel.transform);
        RectTransform subRect = subObj.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.05f, 0.76f);
        subRect.anchorMax = new Vector2(0.95f, 0.82f);
        subRect.offsetMin = Vector2.zero;
        subRect.offsetMax = Vector2.zero;
        subtituloText = subObj.AddComponent<TextMeshProUGUI>();
        subtituloText.text = "Subtítulo";
        subtituloText.fontSize = 26;
        subtituloText.fontStyle = FontStyles.Italic;
        subtituloText.alignment = TextAlignmentOptions.Center;
        subtituloText.color = new Color(0.7f, 0.75f, 0.85f);

        // === SEPARADOR ===
        CreateLine(panel.transform, new Vector2(0.15f, 0.735f), new Vector2(0.85f, 0.74f), new Color(0.4f, 0.7f, 1f, 0.5f));

        // === CUERPO ===
        GameObject cuerpoObj = CreateUIObject("Cuerpo", panel.transform);
        RectTransform cuerpoRect = cuerpoObj.GetComponent<RectTransform>();
        cuerpoRect.anchorMin = new Vector2(0.08f, 0.36f);
        cuerpoRect.anchorMax = new Vector2(0.92f, 0.72f);
        cuerpoRect.offsetMin = Vector2.zero;
        cuerpoRect.offsetMax = Vector2.zero;
        cuerpoText = cuerpoObj.AddComponent<TextMeshProUGUI>();
        cuerpoText.text = "Contenido educativo...";
        cuerpoText.fontSize = 25;
        cuerpoText.alignment = TextAlignmentOptions.TopLeft;
        cuerpoText.color = new Color(0.88f, 0.92f, 0.97f);
        cuerpoText.lineSpacing = 12f;

        // === CAJA DE DATO CURIOSO ===
        GameObject datoBox = CreateUIObject("DatoBox", panel.transform);
        RectTransform datoBoxRect = datoBox.GetComponent<RectTransform>();
        datoBoxRect.anchorMin = new Vector2(0.08f, 0.16f);
        datoBoxRect.anchorMax = new Vector2(0.92f, 0.34f);
        datoBoxRect.offsetMin = Vector2.zero;
        datoBoxRect.offsetMax = Vector2.zero;
        Image datoBg = datoBox.AddComponent<Image>();
        datoBg.color = new Color(0.1f, 0.2f, 0.4f, 0.4f);

        // Barra lateral del dato
        CreateLine(datoBox.transform, new Vector2(0, 0), new Vector2(0.01f, 1), new Color(0.5f, 0.8f, 1f, 1f));

        GameObject datoObj = CreateUIObject("Dato", datoBox.transform);
        RectTransform datoRect = datoObj.GetComponent<RectTransform>();
        datoRect.anchorMin = new Vector2(0.03f, 0.1f);
        datoRect.anchorMax = new Vector2(0.97f, 0.9f);
        datoRect.offsetMin = Vector2.zero;
        datoRect.offsetMax = Vector2.zero;
        datoText = datoObj.AddComponent<TextMeshProUGUI>();
        datoText.text = "¿Sabías que...?";
        datoText.fontSize = 21;
        datoText.alignment = TextAlignmentOptions.Left;
        datoText.color = new Color(0.75f, 0.88f, 1f);
        datoText.fontStyle = FontStyles.Italic;

        // === BOTÓN CONTINUAR ===
        GameObject btnObj = CreateUIObject("BotonContinuar", panel.transform);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.35f, 0.04f);
        btnRect.anchorMax = new Vector2(0.65f, 0.13f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.15f, 0.35f, 0.7f, 0.8f);

        continuarButton = btnObj.AddComponent<Button>();
        continuarButton.targetGraphic = btnImg;

        // Colores del botón (hover, pressed)
        ColorBlock colors = continuarButton.colors;
        colors.normalColor = new Color(0.15f, 0.35f, 0.7f, 0.8f);
        colors.highlightedColor = new Color(0.25f, 0.5f, 0.9f, 0.95f);
        colors.pressedColor = new Color(0.1f, 0.25f, 0.55f, 1f);
        colors.selectedColor = new Color(0.2f, 0.45f, 0.85f, 0.9f);
        continuarButton.colors = colors;

        continuarButton.onClick.AddListener(OnContinuarPressed);

        // Borde del botón
        CreateLine(btnObj.transform, new Vector2(0, 0.94f), new Vector2(1, 1), new Color(0.5f, 0.8f, 1f, 0.9f));
        CreateLine(btnObj.transform, new Vector2(0, 0), new Vector2(1, 0.06f), new Color(0.5f, 0.8f, 1f, 0.9f));

        GameObject btnTextObj = CreateUIObject("Texto", btnObj.transform);
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;
        continuarButtonText = btnTextObj.AddComponent<TextMeshProUGUI>();
        continuarButtonText.text = "CONTINUAR  ▶";
        continuarButtonText.fontSize = 26;
        continuarButtonText.fontStyle = FontStyles.Bold;
        continuarButtonText.alignment = TextAlignmentOptions.Center;
        continuarButtonText.color = Color.white;
    }

    GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    void CreateLine(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject line = CreateUIObject("Line", parent);
        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image img = line.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
    }

    void CreateCorner(Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject corner = CreateUIObject("Corner", parent);
        RectTransform rect = corner.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image img = corner.AddComponent<Image>();
        img.color = new Color(0.5f, 0.85f, 1f, 0.8f);
        img.raycastTarget = false;
    }

    /// <summary>
    /// Agrega el TrackedDeviceGraphicRaycaster de XR Interaction Toolkit por reflexión.
    /// Esto permite que los rayos de los controladores VR interactúen con el botón.
    /// Si el paquete no está disponible, se omite sin romper la compilación.
    /// </summary>
    void AddVRRaycaster(GameObject canvasObj)
    {
        System.Type raycasterType = System.Type.GetType(
            "UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster, Unity.XR.Interaction.Toolkit");

        if (raycasterType != null)
        {
            canvasObj.AddComponent(raycasterType);
            Debug.Log("[Education] TrackedDeviceGraphicRaycaster agregado para interacción VR.");
        }
        else
        {
            Debug.Log("[Education] XR Toolkit UI raycaster no encontrado. Usando input de respaldo (gatillo/teclado).");
        }
    }

    /// <summary>
    /// Asegura que exista un EventSystem en la escena para procesar la UI.
    /// </summary>
    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject es = GameObject.Find("EventSystem");
        if (es == null)
        {
            es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();

            // Intentar agregar el XRUIInputModule; si no, usar el StandaloneInputModule
            System.Type xrModule = System.Type.GetType(
                "UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule, Unity.XR.Interaction.Toolkit");

            if (xrModule != null)
                es.AddComponent(xrModule);
            else
                es.AddComponent<StandaloneInputModule>();
        }
    }
}
