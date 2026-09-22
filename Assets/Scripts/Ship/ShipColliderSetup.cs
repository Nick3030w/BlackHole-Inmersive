using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recorre toda la geometria hija de la nave y garantiza que cada mesh visible
/// tenga un collider, para que el jugador (CharacterController) no atraviese
/// paredes, pisos ni props.
///
/// COLOCAR ESTE COMPONENTE EN: un GameObject raiz que contenga la nave
/// (por ejemplo "_Level", "_Props", o un padre comun de la estructura de la nave).
/// Tambien se puede poner en varios grupos distintos.
///
/// USO:
///  - Marca "runOnStart" para que se ejecute automaticamente al entrar en Play.
///  - O usa el menu contextual (clic derecho en el componente > "Generar Colliders Ahora")
///    en el editor para dejarlos serializados en la escena (recomendado para rendimiento).
/// </summary>
public class ShipColliderSetup : MonoBehaviour
{
    public enum ColliderKind
    {
        /// <summary>MeshCollider convexo=false: preciso para geometria estatica (paredes, pisos).</summary>
        MeshColliderPreciso,
        /// <summary>BoxCollider ajustado a los bounds del mesh: barato, ideal para props simples.</summary>
        BoxColliderAproximado
    }

    [Header("Que tipo de collider generar")]
    [Tooltip("MeshCollider = preciso pero mas costoso. Box = barato y suficiente para props.")]
    public ColliderKind tipoCollider = ColliderKind.MeshColliderPreciso;

    [Header("Cuando ejecutar")]
    [Tooltip("Si es true, genera los colliders automaticamente al iniciar la escena.")]
    public bool runOnStart = true;

    [Header("Filtros")]
    [Tooltip("No tocar objetos que ya tengan algun Collider.")]
    public bool saltarSiYaTieneCollider = true;

    [Tooltip("Ignorar meshes cuyo nombre contenga alguno de estos textos (ej. 'Glass', 'Decal').")]
    public string[] ignorarPorNombre = new string[] { "Decal", "FX", "Glass_NoCollide" };

    [Tooltip("Ignorar meshes muy pequenos (bounds menores a este tamano en metros).")]
    public float ignorarSiMenorQue = 0.02f;

    private void Start()
    {
        if (runOnStart)
            GenerarColliders();
    }

    [ContextMenu("Generar Colliders Ahora")]
    public void GenerarColliders()
    {
        MeshFilter[] filtros = GetComponentsInChildren<MeshFilter>(true);
        int agregados = 0;

        foreach (MeshFilter mf in filtros)
        {
            if (mf == null || mf.sharedMesh == null)
                continue;

            GameObject go = mf.gameObject;

            if (saltarSiYaTieneCollider && go.GetComponent<Collider>() != null)
                continue;

            if (DebeIgnorarse(go.name))
                continue;

            Bounds b = mf.sharedMesh.bounds;
            float maxDim = Mathf.Max(b.size.x, b.size.y, b.size.z);
            if (maxDim < ignorarSiMenorQue)
                continue;

            if (tipoCollider == ColliderKind.MeshColliderPreciso)
            {
                MeshCollider mc = go.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                mc.convex = false; // estatico: no convexo permite geometria arbitraria
            }
            else
            {
                BoxCollider bc = go.AddComponent<BoxCollider>();
                bc.center = b.center;
                bc.size = b.size;
            }

            agregados++;
        }

        Debug.Log($"[ShipColliderSetup] Colliders generados en '{name}': {agregados} (revisados {filtros.Length} meshes).");
    }

    private bool DebeIgnorarse(string nombre)
    {
        if (ignorarPorNombre == null) return false;
        foreach (string clave in ignorarPorNombre)
        {
            if (!string.IsNullOrEmpty(clave) && nombre.Contains(clave))
                return true;
        }
        return false;
    }
}
