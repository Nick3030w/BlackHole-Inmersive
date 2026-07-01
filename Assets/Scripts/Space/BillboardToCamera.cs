using UnityEngine;

/// <summary>
/// Hace que un objeto (quad plano) siempre mire hacia la cámara.
/// Usado para el agujero negro distante renderizado como billboard.
/// </summary>
public class BillboardToCamera : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Orientar el quad para que su cara mire a la cámara
        Vector3 dir = transform.position - target.position;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
