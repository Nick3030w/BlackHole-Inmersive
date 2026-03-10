using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleGravity : MonoBehaviour
{
    [Header("Gravedad")]
    public float gravityStrength = 10f;
    public float eventHorizonRadius = 5f;
    public float gravityRadius = 30f;

    [Header("Referencias")]
    public Transform player;
    public GameObject educationalPanel;

    private bool playerCaptured = false;

    void Update()
    {
        if (player == null || playerCaptured) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Aplicar gravedad si el jugador está dentro del radio
        if (distance < gravityRadius)
        {
            Vector3 direction = (transform.position - player.position).normalized;
            float force = gravityStrength / (distance * distance);
            player.position += direction * force * Time.deltaTime;
        }

        // Horizonte de sucesos
        if (distance < eventHorizonRadius && !playerCaptured)
        {
            playerCaptured = true;
            TriggerEventHorizon();
        }
    }

    void TriggerEventHorizon()
    {
        if (educationalPanel != null)
            educationalPanel.SetActive(true);
    }
}