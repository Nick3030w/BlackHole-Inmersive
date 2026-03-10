using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (player == null) return;

        // El panel siempre mira hacia el jugador
        transform.LookAt(player);
        transform.Rotate(0, 180, 0);
    }
}