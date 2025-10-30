using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W1Task1PH : MonoBehaviour
{
  

    [Header("Movement Settings")]
    public float speed = 10f;        // Forward/backward movement speed
    public float pitchSpeed = 60f;   // Rotation around X-axis (W/S)
    public float rollSpeed = 90f;    // Rotation around Z-axis (A/D)

    void Update()
    {
        float deltaTime = Time.deltaTime;


        float pitchInput = 0f;
        float rollInput = 0f;

        if (Input.GetKey(KeyCode.W))
            pitchInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            pitchInput = -1f;

        if (Input.GetKey(KeyCode.A))
            rollInput = 1f;
        else if (Input.GetKey(KeyCode.D))
            rollInput = -1f;

        // Calculate pitch (around X) and roll (around Z)
        Quaternion pitchRotation = Quaternion.AngleAxis(pitchInput * pitchSpeed * deltaTime, transform.right);
        Quaternion rollRotation = Quaternion.AngleAxis(-rollInput * rollSpeed * deltaTime, transform.forward);

        // Apply rotations (combine both)
        transform.rotation = pitchRotation * rollRotation * transform.rotation;

       
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow))
            moveInput = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            moveInput = -1f;

        // Velocity in local forward direction
        Vector3 velocity = transform.forward * moveInput * speed;

        // Kinematic movement: position = old position + velocity * Δt
        transform.position += velocity * deltaTime;
    }
    }
