using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player;
    public float distanciaFrente = 2f;
    public float alturaOffset = 0f;

    void Update()
    {
        if (player == null) return;

        // Posicionar frente al jugador basado en la dirección que mira la cámara
        Vector3 posicion = player.position - player.forward * distanciaFrente;
        posicion.y += alturaOffset;
        transform.position = posicion;

        // Mirar al jugador
        transform.LookAt(player);
        transform.Rotate(0, 180, 0);
    }
}