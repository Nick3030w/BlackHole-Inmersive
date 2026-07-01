using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Maneja la interacción con el botón "Continuar" de forma robusta,
/// soportando múltiples métodos de input:
/// - Clic con puntero VR (XR Ray Interactor sobre el botón)
/// - Tecla de teclado (Espacio / Enter) para pruebas en editor
/// - Gatillo del controlador VR
/// 
/// Esto asegura que el usuario siempre pueda avanzar sin importar la configuración.
/// </summary>
public class PanelInteractionHandler : MonoBehaviour
{
    [Tooltip("Callback que se ejecuta al confirmar 'continuar'")]
    public Action onContinue;

    [Tooltip("El panel debe estar activo para aceptar input")]
    public bool isActive = false;

    [Header("Teclas de confirmación (editor/desktop)")]
    public KeyCode[] confirmKeys = new KeyCode[] { KeyCode.Space, KeyCode.Return, KeyCode.KeypadEnter };

    private float cooldown = 0f;

    void Update()
    {
        if (!isActive) return;

        // Pequeño cooldown para evitar doble activación al aparecer
        if (cooldown > 0f)
        {
            cooldown -= Time.unscaledDeltaTime;
            return;
        }

        // Input por teclado (para pruebas en el editor / XR Device Simulator)
        foreach (KeyCode key in confirmKeys)
        {
            if (Input.GetKeyDown(key))
            {
                Confirm();
                return;
            }
        }

        // Input por gatillo de controlador VR (mapeo genérico)
        // Botón "Fire1" suele estar mapeado al gatillo o botón primario
        if (Input.GetButtonDown("Fire1"))
        {
            Confirm();
            return;
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (active)
            cooldown = 0.5f; // Evita activación accidental al aparecer
    }

    public void Confirm()
    {
        if (!isActive) return;
        isActive = false;
        onContinue?.Invoke();
    }
}
