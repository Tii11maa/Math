using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W2Task1 : MonoBehaviour
{
    public GameObject monkey;
    public Matrix4x4 rotation;
    public Matrix4x4 scale;
    public Matrix4x4 position;
    Mesh mesh;
    Vector3[] vertices;
    Matrix4x4 combinedMatrix;
    // Start is called before the first frame update
    void Start()
    {
        mesh = monkey.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        rotation = Matrix4x4.Rotate(Quaternion.Euler(90, 90, 90));
        position = new Matrix4x4(
            new Vector4(1, 0, 0, 3),
            new Vector4(0, 1, 0, 3),
            new Vector4(0, 0, 1, 3),
            new Vector4(0, 0, 0, 1));
        scale = new Matrix4x4(
            new Vector4(3, 0, 0, 0),
            new Vector4(0, 3, 0, 0),
            new Vector4(0, 0, 3, 0),
            new Vector4(0, 0, 0, 1));
        combinedMatrix = position * rotation * scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Matrix()
    {

        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = combinedMatrix.MultiplyPoint3x4(vertices[i]);
        }
        mesh.SetVertices(vertices);
        print(vertices);
    }
}

