using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlateform : MonoBehaviour
{
    [SerializeField] float moveZ;
    [SerializeField] float zPerSecond;

    bool isPulseIncreasing = false;
    Vector3 startPosition;
    float offsetX = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    void move()
    {
        if (offsetX >= moveZ)
        {
            isPulseIncreasing = false;
        }
        else if (offsetX <= -moveZ)
        {
            isPulseIncreasing = true;
        }

        offsetX += isPulseIncreasing ? Time.deltaTime * zPerSecond : -Time.deltaTime * zPerSecond;

        this.transform.position = startPosition + new Vector3(0, 0, offsetX);
    }
}
