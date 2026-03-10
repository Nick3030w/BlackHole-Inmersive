using UnityEngine;
using Unity.XR.CoreUtils;

public class BlackHoleGravity : MonoBehaviour
{
    [Header("Gravedad")]
    public float gravityStrength = 15f;
    public float eventHorizonRadius = 5f;
    public float gravityRadius = 30f;

    [Header("Referencias")]
    public Transform cameraOffset;
    public GameObject educationalPanel;

    private bool playerCaptured = false;

    void Update()
    {
        if (cameraOffset == null || playerCaptured) return;

        Vector3 playerPos = cameraOffset.position;
        float distance = Vector3.Distance(transform.position, playerPos);

        
        if (distance < gravityRadius)
        {
            Vector3 direction = (transform.position - playerPos).normalized;
            float force = gravityStrength * Time.deltaTime;
            Debug.Log("Distancia: " + distance + " Fuerza: " + force);
            cameraOffset.position += direction * force;
            
            
        }

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