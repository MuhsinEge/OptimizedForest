using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 2f;
    public LayerMask ground;
    Terrain terrain;
    void Update()
    {
        terrain = Terrain.activeTerrain;
        HandleMovement();
        SurfaceAllignment();
    }
    private void LateUpdate()
    {
        HandleRotation();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);
        var terrainPosition = terrain.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, terrainPosition + 1, transform.position.z);
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
    }

    void SurfaceAllignment()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit info = new RaycastHit();
        Quaternion RotationRef = Quaternion.Euler(0f, 0f, 0f);

        if (Physics.Raycast(ray, out info, ground))
        {
            Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, info.normal);
            Quaternion yRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            // Combine the ground normal rotation with the character's y-axis rotation
            Quaternion finalRotation = groundRotation * yRotation;

            // Smoothly interpolate the rotation
            RotationRef = Quaternion.Lerp(transform.rotation, finalRotation, Time.deltaTime);
            transform.rotation = RotationRef;
        }
    }
}
