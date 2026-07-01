using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generador principal del entorno espacial para la escena Space_Travel.
/// Crea de forma procedural: campo de estrellas, planetas, estrellas lejanas,
/// asteroides, y el agujero negro distante e imponente.
/// 
/// INSTRUCCIONES:
/// 1. Crear un GameObject vacío "SpaceEnvironment"
/// 2. Agregar este script
/// 3. Asignar playerTransform (la cámara o XR Origin)
/// 4. Dale Play — todo se genera automáticamente
/// </summary>
public class SpaceEnvironmentGenerator : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform del jugador/cámara")]
    public Transform playerTransform;

    [Header("Agujero Negro Distante")]
    [Tooltip("Distancia del agujero negro (muy lejos para verse imponente)")]
    public float blackHoleDistance = 800f;

    [Tooltip("Tamaño del agujero negro distante")]
    public float blackHoleSize = 120f;

    [Header("Planetas")]
    [Tooltip("Número de planetas a generar a lo largo del recorrido")]
    public int planetCount = 6;

    [Tooltip("Rango de tamaño de los planetas")]
    public Vector2 planetSizeRange = new Vector2(8f, 25f);

    [Header("Estrellas (Soles)")]
    [Tooltip("Número de estrellas grandes/soles")]
    public int starCount = 3;

    [Tooltip("Rango de tamaño de las estrellas")]
    public Vector2 starSizeRange = new Vector2(30f, 60f);

    [Header("Asteroides")]
    [Tooltip("Número de asteroides")]
    public int asteroidCount = 40;

    [Tooltip("Rango de tamaño de asteroides")]
    public Vector2 asteroidSizeRange = new Vector2(0.5f, 3f);

    [Header("Distribución")]
    [Tooltip("Distancia máxima a la que se colocan los cuerpos desde el eje del recorrido")]
    public float spreadRadius = 150f;

    [Tooltip("Longitud del corredor de viaje")]
    public float corridorLength = 700f;

    [Tooltip("Semilla para generación reproducible (0 = aleatoria)")]
    public int seed = 12345;

    // Paletas de colores para planetas variados
    private readonly Color[][] planetPalettes = new Color[][]
    {
        // Planeta oceánico (azul)
        new Color[] { new Color(0.15f, 0.35f, 0.7f), new Color(0.8f, 0.9f, 1f), new Color(0.2f, 0.5f, 0.8f), new Color(0.4f, 0.7f, 1f) },
        // Planeta desértico (naranja/marrón)
        new Color[] { new Color(0.7f, 0.4f, 0.2f), new Color(0.9f, 0.8f, 0.6f), new Color(0.8f, 0.5f, 0.3f), new Color(1f, 0.7f, 0.4f) },
        // Planeta rocoso (gris)
        new Color[] { new Color(0.4f, 0.4f, 0.45f), new Color(0.7f, 0.7f, 0.75f), new Color(0.5f, 0.5f, 0.55f), new Color(0.6f, 0.65f, 0.8f) },
        // Gigante gaseoso (púrpura/violeta)
        new Color[] { new Color(0.4f, 0.25f, 0.6f), new Color(0.7f, 0.6f, 0.9f), new Color(0.5f, 0.35f, 0.7f), new Color(0.6f, 0.5f, 1f) },
        // Planeta helado (cyan claro)
        new Color[] { new Color(0.6f, 0.8f, 0.85f), new Color(0.9f, 0.95f, 1f), new Color(0.7f, 0.85f, 0.9f), new Color(0.5f, 0.8f, 1f) },
        // Planeta volcánico (rojo)
        new Color[] { new Color(0.5f, 0.15f, 0.1f), new Color(0.9f, 0.4f, 0.2f), new Color(0.6f, 0.2f, 0.15f), new Color(1f, 0.5f, 0.2f) }
    };

    // Colores de estrellas por temperatura
    private readonly Color[] starCoreColors = new Color[]
    {
        new Color(1f, 1f, 0.95f),     // Blanca-azul (caliente)
        new Color(1f, 0.95f, 0.8f),   // Amarilla (Sol)
        new Color(1f, 0.8f, 0.5f),    // Naranja
        new Color(1f, 0.6f, 0.4f)     // Roja (fría)
    };
    private readonly Color[] starEdgeColors = new Color[]
    {
        new Color(0.6f, 0.7f, 1f),
        new Color(1f, 0.6f, 0.2f),
        new Color(1f, 0.4f, 0.1f),
        new Color(0.9f, 0.2f, 0.1f)
    };

    private List<GameObject> generatedBodies = new List<GameObject>();
    private Transform blackHole;

    void Start()
    {
        if (seed != 0)
            Random.InitState(seed);

        SetupLighting();
        CreateStarfield();
        CreateDistantBlackHole();
        CreateStars();
        CreatePlanets();
        CreateAsteroids();

        Debug.Log("[SpaceEnvironment] Entorno generado: " + generatedBodies.Count + " cuerpos celestes.");
    }

    void SetupLighting()
    {
        // Configurar ambiente oscuro espacial
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.03f, 0.03f, 0.06f);
        RenderSettings.fog = false;
    }

    void CreateStarfield()
    {
        GameObject starfieldObj = new GameObject("Starfield");
        starfieldObj.transform.SetParent(transform);
        StarfieldGenerator sf = starfieldObj.AddComponent<StarfieldGenerator>();
        sf.followTarget = playerTransform;
        sf.starCount = 3500;
        sf.radius = 600f;
    }

    void CreateDistantBlackHole()
    {
        // El agujero negro se coloca MUY lejos, al final del corredor, en el eje Z
        GameObject bhObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bhObj.name = "DistantBlackHole";
        bhObj.transform.SetParent(transform);

        Vector3 startPos = playerTransform != null ? playerTransform.position : Vector3.zero;
        bhObj.transform.position = startPos + Vector3.forward * blackHoleDistance;
        bhObj.transform.localScale = Vector3.one * blackHoleSize;

        // Quitar collider del quad
        Destroy(bhObj.GetComponent<Collider>());

        // Aplicar shader del agujero negro distante
        Renderer rend = bhObj.GetComponent<Renderer>();
        Shader bhShader = Shader.Find("Custom/DistantBlackHole");
        if (bhShader != null)
        {
            Material bhMat = new Material(bhShader);
            rend.material = bhMat;
        }
        else
        {
            Debug.LogWarning("[SpaceEnvironment] Shader Custom/DistantBlackHole no encontrado");
        }

        // Hacer que siempre mire a la cámara (billboard)
        BillboardToCamera billboard = bhObj.AddComponent<BillboardToCamera>();
        billboard.target = playerTransform;

        blackHole = bhObj.transform;
        generatedBodies.Add(bhObj);
    }

    void CreateStars()
    {
        Shader starShader = Shader.Find("Custom/Star");

        for (int i = 0; i < starCount; i++)
        {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "Star_" + i;
            star.transform.SetParent(transform);

            // Posición dispersa a lo largo del corredor
            float z = Random.Range(corridorLength * 0.2f, corridorLength);
            float x = Random.Range(-spreadRadius, spreadRadius) * 1.5f;
            float y = Random.Range(-spreadRadius, spreadRadius);
            Vector3 basePos = playerTransform != null ? playerTransform.position : Vector3.zero;
            star.transform.position = basePos + new Vector3(x, y, z);

            float size = Random.Range(starSizeRange.x, starSizeRange.y);
            star.transform.localScale = Vector3.one * size;

            Destroy(star.GetComponent<Collider>());

            // Material de estrella
            int colorIdx = Random.Range(0, starCoreColors.Length);
            Renderer rend = star.GetComponent<Renderer>();
            if (starShader != null)
            {
                Material mat = new Material(starShader);
                mat.SetColor("_CoreColor", starCoreColors[colorIdx]);
                mat.SetColor("_EdgeColor", starEdgeColors[colorIdx]);
                mat.SetFloat("_Brightness", Random.Range(3f, 5f));
                mat.SetFloat("_PulseSpeed", Random.Range(0.3f, 1f));
                mat.SetFloat("_FlowSpeed", Random.Range(0.2f, 0.6f));
                rend.material = mat;
            }

            // Luz puntual para iluminar planetas cercanos
            Light starLight = star.AddComponent<Light>();
            starLight.type = LightType.Point;
            starLight.color = starCoreColors[colorIdx];
            starLight.range = size * 15f;
            starLight.intensity = 2f;

            // Rotación lenta
            CelestialBody cb = star.AddComponent<CelestialBody>();
            cb.bodyType = CelestialBody.BodyType.Star;
            cb.rotationSpeed = Random.Range(1f, 3f);
            cb.rotationAxis = Random.onUnitSphere;

            generatedBodies.Add(star);
        }
    }

    void CreatePlanets()
    {
        Shader planetShader = Shader.Find("Custom/Planet");

        for (int i = 0; i < planetCount; i++)
        {
            GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.name = "Planet_" + i;
            planet.transform.SetParent(transform);

            // Distribuir los planetas a lo largo del recorrido
            float z = (corridorLength / planetCount) * i + Random.Range(20f, 60f);
            float side = (i % 2 == 0) ? 1f : -1f;
            float x = side * Random.Range(spreadRadius * 0.3f, spreadRadius);
            float y = Random.Range(-spreadRadius * 0.5f, spreadRadius * 0.5f);
            Vector3 basePos = playerTransform != null ? playerTransform.position : Vector3.zero;
            planet.transform.position = basePos + new Vector3(x, y, z);

            float size = Random.Range(planetSizeRange.x, planetSizeRange.y);
            planet.transform.localScale = Vector3.one * size;

            Destroy(planet.GetComponent<Collider>());

            // Material del planeta
            Color[] palette = planetPalettes[i % planetPalettes.Length];
            Renderer rend = planet.GetComponent<Renderer>();
            if (planetShader != null)
            {
                Material mat = new Material(planetShader);
                mat.SetColor("_MainColor", palette[0]);
                mat.SetColor("_PoleColor", palette[1]);
                mat.SetColor("_EquatorColor", palette[2]);
                mat.SetColor("_AtmosphereColor", palette[3]);
                mat.SetFloat("_NoiseScale", Random.Range(5f, 12f));
                mat.SetFloat("_NoiseStrength", Random.Range(0.3f, 0.6f));
                mat.SetFloat("_AtmospherePower", Random.Range(2f, 4f));
                mat.SetFloat("_AtmosphereIntensity", Random.Range(0.8f, 1.5f));
                mat.SetFloat("_Roughness", Random.Range(0.6f, 0.9f));
                rend.material = mat;
            }

            // Rotación y órbita
            CelestialBody cb = planet.AddComponent<CelestialBody>();
            cb.bodyType = CelestialBody.BodyType.Planet;
            cb.rotationSpeed = Random.Range(2f, 8f);
            cb.rotationAxis = new Vector3(Random.Range(-0.3f, 0.3f), 1f, Random.Range(-0.3f, 0.3f)).normalized;

            // Algunos planetas tienen anillos
            if (Random.value > 0.6f)
            {
                CreatePlanetRing(planet.transform, size);
            }

            generatedBodies.Add(planet);
        }
    }

    void CreatePlanetRing(Transform planet, float planetSize)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "Ring";
        ring.transform.SetParent(planet);
        ring.transform.localPosition = Vector3.zero;
        // Aplanar el cilindro para hacer un anillo
        ring.transform.localScale = new Vector3(2f, 0.01f, 2f);
        ring.transform.localRotation = Quaternion.Euler(Random.Range(-25f, 25f), 0, Random.Range(-25f, 25f));

        Destroy(ring.GetComponent<Collider>());

        Renderer rend = ring.GetComponent<Renderer>();
        Material ringMat = new Material(Shader.Find("Standard"));
        // Modo transparente
        ringMat.SetFloat("_Mode", 3);
        ringMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        ringMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ringMat.SetInt("_ZWrite", 0);
        ringMat.EnableKeyword("_ALPHABLEND_ON");
        ringMat.renderQueue = 3000;
        ringMat.color = new Color(0.8f, 0.7f, 0.5f, 0.4f);
        rend.material = ringMat;
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    void CreateAsteroids()
    {
        // Crear grupos de asteroides (cinturones)
        for (int i = 0; i < asteroidCount; i++)
        {
            GameObject asteroid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            asteroid.name = "Asteroid_" + i;
            asteroid.transform.SetParent(transform);

            float z = Random.Range(0f, corridorLength);
            float x = Random.Range(-spreadRadius, spreadRadius);
            float y = Random.Range(-spreadRadius * 0.7f, spreadRadius * 0.7f);
            Vector3 basePos = playerTransform != null ? playerTransform.position : Vector3.zero;
            asteroid.transform.position = basePos + new Vector3(x, y, z);

            // Escala irregular para forma de roca
            float baseSize = Random.Range(asteroidSizeRange.x, asteroidSizeRange.y);
            asteroid.transform.localScale = new Vector3(
                baseSize * Random.Range(0.7f, 1.3f),
                baseSize * Random.Range(0.7f, 1.3f),
                baseSize * Random.Range(0.7f, 1.3f)
            );
            asteroid.transform.rotation = Random.rotation;

            Destroy(asteroid.GetComponent<Collider>());

            // Material rocoso gris oscuro
            Renderer rend = asteroid.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            float grayness = Random.Range(0.2f, 0.4f);
            mat.color = new Color(grayness, grayness * 0.95f, grayness * 0.9f);
            mat.SetFloat("_Glossiness", 0.1f);
            mat.SetFloat("_Metallic", 0.1f);
            rend.material = mat;

            // Rotación tumbling
            CelestialBody cb = asteroid.AddComponent<CelestialBody>();
            cb.bodyType = CelestialBody.BodyType.Asteroid;
            cb.rotationSpeed = Random.Range(10f, 40f);
            cb.rotationAxis = Random.onUnitSphere;

            generatedBodies.Add(asteroid);
        }
    }
}
