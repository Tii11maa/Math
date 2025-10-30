using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W2taskPH : MonoBehaviour
{
    public float ThrustForce;
    public float Torque;
    public float MaxVelocity;
    public float MaxAngularVelocity;
    public Rigidbody rb;
    // Start is called before the first frame update


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = MaxAngularVelocity;
        rb.maxLinearVelocity = MaxVelocity;
    }
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Monvemnet(true);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Monvemnet(false);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Rotate(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotate(false);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            Pitch(true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Pitch(false);
        }
    }

    public void Monvemnet(bool forward)
    {
        if (forward)
        {
            rb.AddForce(transform.forward * ThrustForce);
        }
        else
        {
            rb.AddForce(-transform.forward * ThrustForce);
        }
    }

    public void Rotate(bool left)
    {
        if (left)
        {
            rb.AddTorque(transform.forward * Torque);
        }
        else
        {
            rb.AddTorque(-transform.forward * Torque);
        }
    }

    public void Pitch(bool up)
    {
        if (up)
        {
            rb.AddTorque(transform.right * Torque);
        }
        else
        {
            rb.AddTorque(-transform.right * Torque);
        }
    }
}
