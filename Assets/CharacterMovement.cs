using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 2f;

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * movementSpeed * Time.deltaTime;

        transform.Translate(movement);
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
    }
}
