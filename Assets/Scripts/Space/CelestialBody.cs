using UnityEngine;

/// <summary>
/// Comportamiento de un cuerpo celeste individual (planeta, estrella, asteroide).
/// Maneja rotación propia y órbita opcional.
/// </summary>
public class CelestialBody : MonoBehaviour
{
    public enum BodyType { Planet, Star, Asteroid, GasGiant, Moon }

    [Header("Tipo")]
    public BodyType bodyType = BodyType.Planet;

    [Header("Rotación Propia")]
    [Tooltip("Velocidad de rotación sobre su propio eje (grados/segundo)")]
    public float rotationSpeed = 5f;

    [Tooltip("Eje de rotación (inclinación axial)")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Órbita (opcional)")]
    [Tooltip("Punto alrededor del cual orbita (null = no orbita)")]
    public Transform orbitCenter;

    [Tooltip("Velocidad orbital (grados/segundo)")]
    public float orbitSpeed = 0f;

    [Tooltip("Eje de la órbita")]
    public Vector3 orbitAxis = Vector3.up;

    void Start()
    {
        // Normalizar el eje de rotación con inclinación aleatoria leve
        if (rotationAxis == Vector3.zero)
            rotationAxis = Vector3.up;
        rotationAxis.Normalize();
    }

    void Update()
    {
        // Rotación sobre su propio eje
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);

        // Órbita alrededor de un centro
        if (orbitCenter != null && orbitSpeed != 0f)
        {
            transform.RotateAround(orbitCenter.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }
    }
}
