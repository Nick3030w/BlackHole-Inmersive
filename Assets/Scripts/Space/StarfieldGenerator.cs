using UnityEngine;

/// <summary>
/// Genera un campo de estrellas de fondo usando un sistema de partículas o mesh de puntos.
/// Crea una esfera de estrellas alrededor del jugador para simular el espacio profundo.
/// </summary>
public class StarfieldGenerator : MonoBehaviour
{
    [Header("Configuración del Campo de Estrellas")]
    [Tooltip("Número de estrellas de fondo")]
    public int starCount = 3000;

    [Tooltip("Radio de la esfera de estrellas")]
    public float radius = 500f;

    [Tooltip("Tamaño mínimo de las estrellas")]
    public float minSize = 0.5f;

    [Tooltip("Tamaño máximo de las estrellas")]
    public float maxSize = 2.5f;

    [Header("Colores")]
    [Tooltip("Las estrellas varían entre estos colores (temperatura estelar)")]
    public Color[] starColors = new Color[]
    {
        new Color(0.6f, 0.7f, 1f),    // Azul (calientes)
        new Color(0.8f, 0.85f, 1f),   // Blanco-azul
        Color.white,                   // Blanca
        new Color(1f, 0.95f, 0.8f),   // Amarilla (como el Sol)
        new Color(1f, 0.8f, 0.6f),    // Naranja
        new Color(1f, 0.7f, 0.5f)     // Roja (frías)
    };

    [Tooltip("Seguir a este transform (normalmente la cámara/jugador)")]
    public Transform followTarget;

    private ParticleSystem starParticles;

    void Start()
    {
        GenerateStarfield();
    }

    void LateUpdate()
    {
        // Mantener el campo de estrellas centrado en el jugador (parallax infinito)
        if (followTarget != null)
        {
            transform.position = followTarget.position;
        }
    }

    void GenerateStarfield()
    {
        // Crear ParticleSystem
        starParticles = gameObject.GetComponent<ParticleSystem>();
        if (starParticles == null)
            starParticles = gameObject.AddComponent<ParticleSystem>();

        var main = starParticles.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = starCount;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startSize = 1f;

        // Desactivar emisión continua
        var emission = starParticles.emission;
        emission.enabled = false;

        // Configurar renderer con material aditivo
        var renderer = GetComponent<ParticleSystemRenderer>();
        Shader shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Legacy Shaders/Particles/Additive");

        Material starMat = new Material(shader);
        renderer.material = starMat;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Generar las partículas manualmente
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[starCount];

        for (int i = 0; i < starCount; i++)
        {
            // Posición aleatoria en la superficie de una esfera
            Vector3 pos = Random.onUnitSphere * radius;

            particles[i].position = pos;
            particles[i].startSize = Random.Range(minSize, maxSize);
            particles[i].startColor = starColors[Random.Range(0, starColors.Length)];
            particles[i].startLifetime = Mathf.Infinity;
            particles[i].remainingLifetime = Mathf.Infinity;
            particles[i].velocity = Vector3.zero;
        }

        starParticles.SetParticles(particles, starCount);
    }
}
