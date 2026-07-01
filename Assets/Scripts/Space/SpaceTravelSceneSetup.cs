using UnityEngine;

/// <summary>
/// Configuración maestra de la escena Space_Travel.
/// Un solo componente que arma toda la experiencia de viaje espacial:
/// - Entorno (estrellas, planetas, agujero negro distante, asteroides)
/// - Movimiento del jugador hacia el agujero negro
/// - Iluminación espacial
/// 
/// INSTRUCCIONES:
/// 1. Crear un GameObject vacío llamado "SceneSetup" en la escena Space_Travel
/// 2. Agregar este script
/// 3. Asignar la Main Camera (o el XR Origin si usas VR)
/// 4. Dale Play — toda la escena se genera y el viaje comienza
/// </summary>
public class SpaceTravelSceneSetup : MonoBehaviour
{
    [Header("Referencia del Jugador")]
    [Tooltip("La cámara principal o el XR Origin que se moverá por el espacio")]
    public Transform playerTransform;

    [Header("Configuración del Viaje")]
    [Tooltip("Velocidad de crucero del viaje espacial")]
    public float cruiseSpeed = 20f;

    [Tooltip("Distancia del agujero negro (mayor = se ve más lejano)")]
    public float blackHoleDistance = 800f;

    [Tooltip("Tamaño del agujero negro distante")]
    public float blackHoleSize = 130f;

    [Header("Densidad del Entorno")]
    public int planetCount = 6;
    public int starCount = 3;
    public int asteroidCount = 50;

    [Header("Movimiento del Jugador")]
    [Tooltip("Si es true, el jugador se mueve automáticamente hacia el agujero negro")]
    public bool autoTravel = true;

    [Header("Sistema Educativo")]
    [Tooltip("Activar los paneles educativos interactivos durante el viaje")]
    public bool enableEducation = true;

    private SpaceEnvironmentGenerator environment;
    private SpaceTravelController travelController;
    private InteractiveEducationSystem educationSystem;

    void Start()
    {
        // Auto-detectar cámara si no se asignó
        if (playerTransform == null)
        {
            if (Camera.main != null)
            {
                playerTransform = Camera.main.transform;
                Debug.Log("[SpaceTravelSetup] Usando Camera.main como jugador: " + playerTransform.name);
            }
            else
            {
                Debug.LogError("[SpaceTravelSetup] No hay playerTransform asignado ni Camera.main disponible!");
                return;
            }
        }

        SetupCamera();
        SetupEnvironment();
        SetupTravel();
        SetupEducation();

        Debug.Log("[SpaceTravelSetup] Escena de viaje espacial lista.");
    }

    void SetupCamera()
    {
        // Configurar la cámara para el espacio: fondo negro, far clip amplio
        Camera cam = playerTransform.GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.farClipPlane = 2000f; // Ver el agujero negro distante
            cam.nearClipPlane = 0.1f;
        }
    }

    void SetupEnvironment()
    {
        GameObject envObj = new GameObject("SpaceEnvironment");
        environment = envObj.AddComponent<SpaceEnvironmentGenerator>();
        environment.playerTransform = playerTransform;
        environment.blackHoleDistance = blackHoleDistance;
        environment.blackHoleSize = blackHoleSize;
        environment.planetCount = planetCount;
        environment.starCount = starCount;
        environment.asteroidCount = asteroidCount;
        environment.corridorLength = blackHoleDistance * 0.85f;
        environment.spreadRadius = 150f;
    }

    void SetupTravel()
    {
        if (!autoTravel) return;

        // Determinar qué transform mover: el XR Origin (padre) si existe, sino la cámara
        Transform moveTarget = playerTransform;

        // Si la cámara está dentro de un rig, mover el rig raíz
        Transform root = playerTransform;
        while (root.parent != null)
        {
            root = root.parent;
        }
        if (root != playerTransform)
        {
            moveTarget = root;
            Debug.Log("[SpaceTravelSetup] Moviendo el rig raíz: " + moveTarget.name);
        }

        travelController = moveTarget.gameObject.AddComponent<SpaceTravelController>();
        travelController.cruiseSpeed = cruiseSpeed;
        travelController.travelDirection = Vector3.forward;
        travelController.accelerationTime = 4f;

        // Conectar con el agujero negro para el boost gravitacional
        if (environment != null)
        {
            // El agujero negro se crea en SpaceEnvironmentGenerator; lo buscamos tras un frame
            Invoke(nameof(LinkBlackHoleToTravel), 0.1f);
        }
    }

    void LinkBlackHoleToTravel()
    {
        GameObject bh = GameObject.Find("DistantBlackHole");
        if (bh != null && travelController != null)
        {
            travelController.blackHole = bh.transform;
            travelController.gravitationalRange = blackHoleDistance * 0.4f;
        }
    }

    void SetupEducation()
    {
        if (!enableEducation) return;

        GameObject eduObj = new GameObject("EducationSystem");
        educationSystem = eduObj.AddComponent<InteractiveEducationSystem>();
        educationSystem.playerCamera = playerTransform;
        educationSystem.travelController = travelController;

        // AudioSource para sonidos de UI
        AudioSource audio = eduObj.AddComponent<AudioSource>();
        audio.spatialBlend = 0f;
        audio.volume = 0.6f;
        educationSystem.audioSource = audio;

        // Escalar los checkpoints según la distancia del corredor
        float corridor = blackHoleDistance * 0.85f;
        int numPanels = 7;
        float[] checkpoints = new float[numPanels];
        for (int i = 0; i < numPanels; i++)
        {
            // Distribuir los checkpoints uniformemente en el 90% del corredor
            checkpoints[i] = (corridor * 0.9f / numPanels) * (i + 1);
        }
        educationSystem.checkpointDistances = checkpoints;

        Debug.Log("[SpaceTravelSetup] Sistema educativo configurado con " + numPanels + " paneles.");
    }
}
