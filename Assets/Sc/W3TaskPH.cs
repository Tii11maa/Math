using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W3TaskPH : MonoBehaviour
{
    public float mass;
    public float thrustPower;
    public float torquePower;
    public float linearDamping;
    public float angularDamping;
    public bool fixedb;
    Vector3 velocity;
    Vector3 angularVelocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(fixedb)
        {
            Vector3 thrustForce = CalculateThrustForce();
            Vector3 torque = CalculateTorque();

            // Apply physics integration manually
            DoPhysics(thrustForce, torque, Time.fixedDeltaTime);
        }

    }
    private void Update()
    {
        if(!fixedb)
        {
            Vector3 thrustForce = CalculateThrustForce();
            Vector3 torque = CalculateTorque();

            // Apply physics integration manually
            DoPhysics(thrustForce, torque, Time.fixedDeltaTime);
        }
    }

    Vector3 CalculateThrustForce()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            input = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            input = -1f;

        // Thrust direction is forward in local space
        Vector3 thrust = transform.forward * input * thrustPower;

        // Apply linear damping (air drag)
        Vector3 damping = -velocity * linearDamping;

        return thrust + damping;
    }
    Vector3 CalculateTorque()
    {
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

        // Pitch is rotation around the X axis, Roll around the Z axis
        Vector3 torque = new Vector3(pitchInput, 0f, -rollInput) * torquePower;

        // Apply angular damping
        Vector3 damping = -angularVelocity * angularDamping;

        return torque + damping;
    }
    void DoPhysics(Vector3 force, Vector3 torque, float deltaTime)
    {
        // ----- Linear motion -----
        Vector3 acceleration = force / mass;
        velocity += acceleration * deltaTime;
        transform.position += velocity * deltaTime;

        // ----- Angular motion -----
        Vector3 angularAcceleration = torque / mass;
        angularVelocity += angularAcceleration * deltaTime;

        // Convert angular velocity (radians per second) to Quaternion rotation
        float angle = angularVelocity.magnitude * Mathf.Rad2Deg * deltaTime; // convert to degrees
        if (angle != 0f)
        {
            Vector3 axis = angularVelocity.normalized;
            Quaternion rotationDelta = Quaternion.AngleAxis(angle, axis);
            transform.rotation = rotationDelta * transform.rotation;
        }
    }
}


