using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveTarget : MonoBehaviour
{
    public Transform target, target2;
    bool move = false;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (move)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            // Move towards target
            transform.Translate(direction * speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                move = false;
            }
        }
        else
        {
            Vector3 direction = (target2.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target2.position) < 0.1f)
            {
                move = true;
            }
        }

     
    }
}
