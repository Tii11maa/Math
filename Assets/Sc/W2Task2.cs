using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class W2Task2 : MonoBehaviour
{
    public GameObject CubeEuler, CubeQuaternion;
    public Slider pitch;
    public Slider yaw;
    public Slider roll;
    public Button button;
    float pitchF,yawF,rollF;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonDown()
    {
        pitchF=pitch.value;
        yawF=yaw.value;
        rollF=roll.value;
        StartCoroutine(RollCube());
    }

    IEnumerator RollCube()
    {
        Quaternion toRotation = Quaternion.Euler(pitchF, yawF, rollF);
        float elapsedTime = 0;
        float duration = 1.0f; // adjust this value to control the speed of the interpolation

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            CubeEuler.transform.eulerAngles = Vector3.Lerp(CubeEuler.transform.eulerAngles, new Vector3(pitchF, yawF, rollF), t);
            CubeQuaternion.transform.rotation = Quaternion.Slerp(CubeQuaternion.transform.rotation, toRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is set
        CubeEuler.transform.eulerAngles = new Vector3(pitchF, yawF, rollF);
        CubeQuaternion.transform.rotation = toRotation;
    }
}
