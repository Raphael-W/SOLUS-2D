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

    public float currentSpeed;
    public Vector3 rotatedDirection;

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
            rotatedDirection = rotation * direction;

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (currentSpeed < targetSpeed)
            {
                Debug.Log("Moving");
                currentSpeed += acceleration;
                Vector2 movementAmount = rotatedDirection * currentSpeed * Time.deltaTime;
                transform.Translate(-movementAmount);
            }
        }

        else
        {
            if (currentSpeed > 0f)
            {
                Vector2 movementAmount = rotatedDirection * currentSpeed * Time.deltaTime;
                transform.Translate(movementAmount);
                currentSpeed -= deceleration;
            }
        }


    }
}
