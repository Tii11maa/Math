using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W2Task3 : MonoBehaviour
{
    public Transform CannonBase;
    public Transform CannonBarrel;
    public Transform Target;
    public GameObject BulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RotateCanon());
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void rotate()
    {
        StartCoroutine(RotateCanon());
    }

    IEnumerator RotateCanon()
    {
        print("start");
        
        float elapsedTime = 0;
        float duration = 1.0f; 
        Vector3 vector=(Target.transform.position-CannonBase.transform.position).normalized;
        Vector3 forwardDirection = CannonBarrel.transform.forward;
       float cost = Vector3.Dot(forwardDirection, vector);


        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Quaternion baseRotation = Quaternion.LookRotation(new Vector3(cost, 0, cost), Vector3.up);
            CannonBase.transform.rotation = Quaternion.Slerp(CannonBase.transform.rotation, baseRotation, t);
            Quaternion barrelRotation = Quaternion.LookRotation(new Vector3(vector.x,0,0), Vector3.up);
            CannonBarrel.transform.rotation = Quaternion.Slerp(CannonBarrel.transform.rotation, barrelRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


    }
   
    }
    
