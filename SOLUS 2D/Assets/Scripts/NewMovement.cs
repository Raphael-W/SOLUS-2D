using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float rotationSpeed;
    private Quaternion targetRotation = Quaternion.identity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float rotationThisFrame = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        targetRotation *= Quaternion.Euler(0f, 0f, rotationThisFrame);
    }

    private void FixedUpdate()
    {
        Quaternion currentRotation = Quaternion.Euler(0f, 0f, rb.rotation);
        float interpolationFactor = rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(Quaternion.Lerp(currentRotation, targetRotation, interpolationFactor));
    }
}
