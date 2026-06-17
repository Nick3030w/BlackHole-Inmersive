using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script de configuración automática para la escena BlackHole_Core.
/// Conecta todos los sistemas (gravedad, mensajes educativos, UI) al entrar en Play mode.
/// 
/// INSTRUCCIONES:
/// 1. Agregar este script a un GameObject vacío llamado "GameManager" en la escena BlackHole_Core
/// 2. Asignar en el Inspector:
///    - blackHoleTransform: el objeto del agujero negro
///    - playerRig: el XR Origin o Camera Offset
///    - playerCamera: la cámara principal del jugador (Main Camera dentro del XR Rig)
/// 3. El script crea automáticamente la UI educativa si no existe
/// </summary>
public class BlackHoleSceneSetup : MonoBehaviour
{
    [Header("Referencias Obligatorias")]
    [Tooltip("Transform del objeto BlackHole en la escena")]
    public Transform blackHoleTransform;

    [Tooltip("Transform del XR Origin / Camera Offset (lo que se mueve)")]
    public Transform playerRig;

    [Tooltip("Cámara principal del jugador (Main Camera)")]
    public Transform playerCamera;

    [Header("Configuración Opcional")]
    [Tooltip("AudioSource para narración (se crea automáticamente si no se asigna)")]
    public AudioSource audioSource;

    private BlackHoleGravity gravitySystem;
    private EducationalMessageSystem messageSystem;

    void Awake()
    {
        SetupSystems();
    }

    void SetupSystems()
    {
        // === 1. Configurar BlackHoleGravity ===
        if (blackHoleTransform != null)
        {
            gravitySystem = blackHoleTransform.GetComponent<BlackHoleGravity>();
            if (gravitySystem == null)
                gravitySystem = blackHoleTransform.gameObject.AddComponent<BlackHoleGravity>();

            gravitySystem.playerRig = playerRig;
        }
        else
        {
            Debug.LogError("[BlackHoleSceneSetup] blackHoleTransform no asignado!");
            return;
        }

        // === 2. Crear AudioSource si no existe ===
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D para narración
            audioSource.volume = 0.8f;
        }

        // === 3. Crear UI Educativa ===
        SetupEducationalUI();

        Debug.Log("[BlackHoleSceneSetup] Todos los sistemas configurados correctamente.");
    }

    void SetupEducationalUI()
    {
        // Crear el Canvas en World Space
        GameObject canvasObj = new GameObject("EducationalCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1.8f, 1f);
        canvasRect.localScale = Vector3.one;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Panel con CanvasGroup para animaciones de fade
        GameObject panel = new GameObject("MessagePanel");
        panel.transform.SetParent(canvasObj.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Fondo semi-transparente azul oscuro espacial
        Image bgImage = panel.AddComponent<Image>();
        bgImage.color = new Color(0.01f, 0.02f, 0.06f, 0.88f);

        CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // Borde luminoso (glow)
        GameObject borderGlow = new GameObject("BorderGlow");
        borderGlow.transform.SetParent(panel.transform, false);
        RectTransform borderRect = borderGlow.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-3, -3);
        borderRect.offsetMax = new Vector2(3, 3);
        Image borderImage = borderGlow.AddComponent<Image>();
        borderImage.color = new Color(0.2f, 0.5f, 1f, 0.5f);
        borderImage.raycastTarget = false;
        borderGlow.transform.SetAsFirstSibling();

        // Línea decorativa superior
        GameObject topLine = new GameObject("TopAccent");
        topLine.transform.SetParent(panel.transform, false);
        RectTransform topLineRect = topLine.AddComponent<RectTransform>();
        topLineRect.anchorMin = new Vector2(0.15f, 0.88f);
        topLineRect.anchorMax = new Vector2(0.85f, 0.89f);
        topLineRect.offsetMin = Vector2.zero;
        topLineRect.offsetMax = Vector2.zero;
        Image topLineImg = topLine.AddComponent<Image>();
        topLineImg.color = new Color(0.4f, 0.8f, 1f, 0.9f);
        topLineImg.raycastTarget = false;

        // Texto del título
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.05f, 0.7f);
        titleRect.anchorMax = new Vector2(0.95f, 0.87f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "";
        titleTMP.fontSize = 0.09f;
        titleTMP.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = new Color(0.4f, 0.85f, 1f); // Cyan

        // Línea separadora
        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(panel.transform, false);
        RectTransform divRect = divider.AddComponent<RectTransform>();
        divRect.anchorMin = new Vector2(0.2f, 0.68f);
        divRect.anchorMax = new Vector2(0.8f, 0.685f);
        divRect.offsetMin = Vector2.zero;
        divRect.offsetMax = Vector2.zero;
        Image divImg = divider.AddComponent<Image>();
        divImg.color = new Color(0.3f, 0.6f, 1f, 0.6f);
        divImg.raycastTarget = false;

        // Texto del contenido
        GameObject contentObj = new GameObject("ContentText");
        contentObj.transform.SetParent(panel.transform, false);
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.08f, 0.08f);
        contentRect.anchorMax = new Vector2(0.92f, 0.65f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        TextMeshProUGUI contentTMP = contentObj.AddComponent<TextMeshProUGUI>();
        contentTMP.text = "";
        contentTMP.fontSize = 0.055f;
        contentTMP.alignment = TextAlignmentOptions.Center;
        contentTMP.color = new Color(0.9f, 0.95f, 1f); // Blanco azulado

        // Línea decorativa inferior
        GameObject bottomLine = new GameObject("BottomAccent");
        bottomLine.transform.SetParent(panel.transform, false);
        RectTransform bottomLineRect = bottomLine.AddComponent<RectTransform>();
        bottomLineRect.anchorMin = new Vector2(0.15f, 0.05f);
        bottomLineRect.anchorMax = new Vector2(0.85f, 0.06f);
        bottomLineRect.offsetMin = Vector2.zero;
        bottomLineRect.offsetMax = Vector2.zero;
        Image bottomLineImg = bottomLine.AddComponent<Image>();
        bottomLineImg.color = new Color(0.4f, 0.8f, 1f, 0.5f);
        bottomLineImg.raycastTarget = false;

        // === 4. Configurar EducationalMessageSystem ===
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
}
