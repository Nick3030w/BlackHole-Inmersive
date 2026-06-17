using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Configuración automática de la escena BlackHole_Core.
/// Crea el sistema educativo con panel anclado a la cámara del jugador.
/// 
/// INSTRUCCIONES:
/// 1. Agregar a un GameObject vacío "GameManager"
/// 2. Asignar: blackHoleTransform, playerRig, playerCamera
/// 3. IMPORTANTE: En el BlackHole existente, DESACTIVAR el componente BlackHoleGravity viejo
///    (el que tiene "cameraOffset" asignado) para evitar conflictos.
/// </summary>
public class BlackHoleSceneSetup : MonoBehaviour
{
    [Header("Referencias Obligatorias")]
    [Tooltip("Transform del objeto BlackHole en la escena")]
    public Transform blackHoleTransform;

    [Tooltip("Transform del Camera Offset (hijo del XR Origin, lo que se mueve)")]
    public Transform playerRig;

    [Tooltip("Cámara principal del jugador (Main Camera dentro de Camera Offset)")]
    public Transform playerCamera;

    [Header("Configuración Opcional")]
    public AudioSource audioSource;

    private BlackHoleGravity gravitySystem;
    private EducationalMessageSystem messageSystem;

    void Awake()
    {
        SetupSystems();
    }

    void SetupSystems()
    {
        if (blackHoleTransform == null || playerRig == null || playerCamera == null)
        {
            Debug.LogError("[BlackHoleSceneSetup] Referencias no asignadas! Asigna blackHoleTransform, playerRig y playerCamera.");
            return;
        }

        // === 1. Configurar gravedad ===
        // Buscar si ya existe un BlackHoleGravity con el campo nuevo "playerRig"
        gravitySystem = blackHoleTransform.GetComponent<BlackHoleGravity>();
        if (gravitySystem == null)
            gravitySystem = blackHoleTransform.gameObject.AddComponent<BlackHoleGravity>();

        gravitySystem.playerRig = playerRig;

        // === 2. AudioSource ===
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
            audioSource.volume = 0.8f;
        }

        // === 3. Crear UI Educativa anclada a la cámara ===
        CreateEducationalUI();

        Debug.Log("[BlackHoleSceneSetup] Sistemas configurados. BlackHole en " + blackHoleTransform.position + ", PlayerRig en " + playerRig.position);
    }

    void CreateEducationalUI()
    {
        // === CANVAS WORLD SPACE ===
        GameObject canvasObj = new GameObject("EducationalCanvas");
        // NO lo anclamos aquí — el EducationalMessageSystem lo hará a la cámara en Start()

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1.4f, 0.7f);
        canvasRect.localScale = Vector3.one;

        canvasObj.AddComponent<CanvasScaler>();

        // === PANEL PRINCIPAL ===
        GameObject panel = new GameObject("MessagePanel");
        panel.transform.SetParent(canvasObj.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Fondo: gradiente oscuro espacial con transparencia
        Image bgImage = panel.AddComponent<Image>();
        bgImage.color = new Color(0.02f, 0.03f, 0.08f, 0.92f);

        CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // === BORDE EXTERIOR — línea fina luminosa ===
        CreateBorderLine(panel.transform, new Vector2(0, 0), new Vector2(1, 0.003f), new Color(0.3f, 0.7f, 1f, 0.8f)); // Bottom
        CreateBorderLine(panel.transform, new Vector2(0, 0.997f), new Vector2(1, 1), new Color(0.3f, 0.7f, 1f, 0.8f)); // Top
        CreateBorderLine(panel.transform, new Vector2(0, 0), new Vector2(0.003f, 1), new Color(0.3f, 0.7f, 1f, 0.6f)); // Left
        CreateBorderLine(panel.transform, new Vector2(0.997f, 0), new Vector2(1, 1), new Color(0.3f, 0.7f, 1f, 0.6f)); // Right

        // === ESQUINAS DECORATIVAS (estilo HUD sci-fi) ===
        CreateCornerAccent(panel.transform, new Vector2(0, 0.92f), new Vector2(0.15f, 0.925f)); // Top-Left
        CreateCornerAccent(panel.transform, new Vector2(0.85f, 0.92f), new Vector2(1f, 0.925f)); // Top-Right
        CreateCornerAccent(panel.transform, new Vector2(0, 0.075f), new Vector2(0.15f, 0.08f)); // Bottom-Left
        CreateCornerAccent(panel.transform, new Vector2(0.85f, 0.075f), new Vector2(1f, 0.08f)); // Bottom-Right

        // === TÍTULO ===
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.05f, 0.72f);
        titleRect.anchorMax = new Vector2(0.95f, 0.92f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "";
        titleTMP.fontSize = 0.07f;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = new Color(0.6f, 0.9f, 1f); // Cyan claro
        titleTMP.enableWordWrapping = true;

        // === SEPARADOR HORIZONTAL ===
        GameObject separator = new GameObject("Separator");
        separator.transform.SetParent(panel.transform, false);
        RectTransform sepRect = separator.AddComponent<RectTransform>();
        sepRect.anchorMin = new Vector2(0.1f, 0.70f);
        sepRect.anchorMax = new Vector2(0.9f, 0.705f);
        sepRect.offsetMin = Vector2.zero;
        sepRect.offsetMax = Vector2.zero;
        Image sepImg = separator.AddComponent<Image>();
        sepImg.color = new Color(0.4f, 0.7f, 1f, 0.5f);
        sepImg.raycastTarget = false;

        // === CONTENIDO ===
        GameObject contentObj = new GameObject("ContentText");
        contentObj.transform.SetParent(panel.transform, false);

        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.08f, 0.1f);
        contentRect.anchorMax = new Vector2(0.92f, 0.68f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        TextMeshProUGUI contentTMP = contentObj.AddComponent<TextMeshProUGUI>();
        contentTMP.text = "";
        contentTMP.fontSize = 0.045f;
        contentTMP.alignment = TextAlignmentOptions.Center;
        contentTMP.color = new Color(0.85f, 0.9f, 0.95f); // Blanco ligeramente azulado
        contentTMP.enableWordWrapping = true;
        contentTMP.lineSpacing = 10f;

        // === INDICADOR INFERIOR (pequeña barra animada) ===
        GameObject indicator = new GameObject("BottomIndicator");
        indicator.transform.SetParent(panel.transform, false);
        RectTransform indRect = indicator.AddComponent<RectTransform>();
        indRect.anchorMin = new Vector2(0.35f, 0.03f);
        indRect.anchorMax = new Vector2(0.65f, 0.04f);
        indRect.offsetMin = Vector2.zero;
        indRect.offsetMax = Vector2.zero;
        Image indImg = indicator.AddComponent<Image>();
        indImg.color = new Color(0.3f, 0.6f, 1f, 0.4f);
        indImg.raycastTarget = false;

        // === CONFIGURAR EducationalMessageSystem ===
        messageSystem = gameObject.AddComponent<EducationalMessageSystem>();
        messageSystem.mensajeCanvas = canvas;
        messageSystem.panelCanvasGroup = canvasGroup;
        messageSystem.textoTitulo = titleTMP;
        messageSystem.textoContenido = contentTMP;
        messageSystem.panelBackground = bgImage;
        messageSystem.blackHole = gravitySystem;
        messageSystem.audioSource = audioSource;
        messageSystem.playerCamera = playerCamera;
    }

    void CreateBorderLine(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject line = new GameObject("Border");
        line.transform.SetParent(parent, false);
        RectTransform rect = line.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image img = line.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
    }

    void CreateCornerAccent(Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject corner = new GameObject("CornerAccent");
        corner.transform.SetParent(parent, false);
        RectTransform rect = corner.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image img = corner.AddComponent<Image>();
        img.color = new Color(0.5f, 0.85f, 1f, 0.7f);
        img.raycastTarget = false;
    }
}
