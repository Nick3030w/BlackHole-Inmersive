using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZona : MonoBehaviour
{
    public NarrationManager narrationManager;
    public int indiceZona;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            narrationManager.ActivarZona(indiceZona);
    }
}
