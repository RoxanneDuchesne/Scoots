using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] Rigidbody coots;
    [SerializeField] GameObject speedometer;

    [SerializeField] float maxVelocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       float velocityPercent = (-coots.velocity.x + coots.velocity.y) / maxVelocity;

        if (velocityPercent > 1)
        {
            velocityPercent = 1;
        }

        if (velocityPercent < 0)
        {
            velocityPercent = 0;
        }

        float rotationY = -175 * velocityPercent;
        speedometer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 145 + rotationY));
    }
}
