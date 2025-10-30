using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // This is the object that the camera will follow
    public Vector3 offset; // This is the offset of the camera from the target

    // Start is called before the first frame update
    void Start()
    {
        // Make sure that the target is set
        if (target == null)
        {
            Debug.LogError("Target is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) // Only if the target is set
        {
            // Move the camera to the position of the target with offset
            transform.position = target.position + offset;

            // Rotate the camera to face the target
            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = rotation;
        }
    }
}