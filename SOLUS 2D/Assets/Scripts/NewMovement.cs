using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    public float turnSpeed;
    private Vector3 direction;
    private Vector3 mousePosition;

    public float acceleration;
    public float deceleration;
    public float targetSpeed;

    private float currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            direction = (mousePosition - transform.position).normalized;
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            Vector3 rotatedDirection = rotation * direction;

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (currentSpeed < targetSpeed)
            {
                rb.velocity = transform.forward * currentSpeed;
                currentSpeed += acceleration;
            }
        }

        else
        {
            if (currentSpeed > 0f)
            {
                rb.velocity = transform.forward * currentSpeed;
                currentSpeed -= deceleration;
            }
        }


    }
}
