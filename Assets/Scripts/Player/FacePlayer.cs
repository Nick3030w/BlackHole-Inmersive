using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player;
    public float distanciaFrente = 3f;
    public float alturaOffset = -0.5f;

    void Update()
    {
        if (player == null) return;

        // Posicionar frente al jugador
        Vector3 direccion = player.forward;
        direccion.y = 0;
        transform.position = player.position + direccion * distanciaFrente + Vector3.up * alturaOffset;

        // Mirar hacia el jugador
        transform.LookAt(player);
        transform.Rotate(0, 180, 0);
    }
}