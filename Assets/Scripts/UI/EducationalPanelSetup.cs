using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script de utilidad que crea programáticamente el panel UI educativo con estilo visual espacial.
/// Agregar a un GameObject vacío en la escena y ejecutar desde el menú contextual del Inspector.
/// 
/// INSTRUCCIONES DE USO EN UNITY:
/// 1. Crear un GameObject vacío llamado "EducationalUI"
/// 2. Agregar este script
/// 3. Click derecho en el componente → "Setup Educational Panel"
///    O simplemente entrar a Play mode — se configura automáticamente en Awake si no detecta el canvas.
/// </summary>
public class EducationalPanelSetup : MonoBehaviour
{
    [Header("Generado automáticamente")]
    public Canvas canvas;
    public CanvasGroup panelCanvasGroup;
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI contenidoText;
    public Image backgroundImage;
    public Image glowBorder;

    [ContextMenu("Setup Educational Panel")]
    public void SetupPanel()
    {
        // === WORLD SPACE CANVAS ===
        GameObject canvasObj = new GameObject("EducationalCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1.6f, 0.9f);
        canvasRect.localScale = Vector3.one;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // === PANEL PRINCIPAL ===
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        backgroundImage = panel.AddComponent<Image>();
        backgroundImage.color = new Color(0.02f, 0.02f, 0.08f, 0.85f); // Azul espacial oscuro semi-transparente

        panelCanvasGroup = panel.AddComponent<CanvasGroup>();
        panelCanvasGroup.alpha = 0f;

        // === BORDE GLOW ===
        GameObject border = new GameObject("GlowBorder");
        border.transform.SetParent(panel.transform, false);

        RectTransform borderRect = border.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-4, -4);
        borderRect.offsetMax = new Vector2(4, 4);

        glowBorder = border.AddComponent<Image>();
        glowBorder.color = new Color(0.3f, 0.6f, 1f, 0.6f); // Azul brillante
        glowBorder.raycastTarget = false;

        // Mover el borde detrás del fondo
        border.transform.SetAsFirstSibling();

        // === TÍTULO ===
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.05f, 0.7f);
        titleRect.anchorMax = new Vector2(0.95f, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        tituloText = titleObj.AddComponent<TextMeshProUGUI>();
        tituloText.text = "TÍTULO";
        tituloText.fontSize = 0.08f;
        tituloText.fontStyle = FontStyles.Bold;
        tituloText.alignment = TextAlignmentOptions.Center;
        tituloText.color = new Color(0.4f, 0.8f, 1f); // Cyan brillante

        // === CONTENIDO ===
        GameObject contentObj = new GameObject("ContentText");
        contentObj.transform.SetParent(panel.transform, false);

        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.08f, 0.05f);
        contentRect.anchorMax = new Vector2(0.92f, 0.68f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        contenidoText = contentObj.AddComponent<TextMeshProUGUI>();
        contenidoText.text = "Contenido educativo aquí...";
        contenidoText.fontSize = 0.05f;
        contenidoText.alignment = TextAlignmentOptions.Center;
        contenidoText.color = Color.white;

        // === LÍNEA DECORATIVA ===
        GameObject lineObj = new GameObject("DividerLine");
        lineObj.transform.SetParent(panel.transform, false);

        RectTransform lineRect = lineObj.AddComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.1f, 0.68f);
        lineRect.anchorMax = new Vector2(0.9f, 0.69f);
        lineRect.offsetMin = Vector2.zero;
        lineRect.offsetMax = Vector2.zero;

        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = new Color(0.3f, 0.6f, 1f, 0.8f);
        lineImage.raycastTarget = false;

        Debug.Log("[EducationalPanelSetup] Panel creado exitosamente. Asigna las referencias en EducationalMessageSystem.");
    }

    void Awake()
    {
        // Auto-setup si no tiene canvas hijo
        if (canvas == null && GetComponentInChildren<Canvas>() == null)
        {
            SetupPanel();
        }
    }
}
