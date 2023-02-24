using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingY: MonoBehaviour
{
    [SerializeField] float moveY;
    [SerializeField] float yPerSecond;

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
        if (offsetX >= moveY)
        {
            isPulseIncreasing = false;
        }
        else if (offsetX <= -moveY)
        {
            isPulseIncreasing = true;
        }

        offsetX += isPulseIncreasing ? Time.deltaTime * yPerSecond : -Time.deltaTime * yPerSecond;

        this.transform.position = startPosition + new Vector3(0, offsetX, 0);
    }
}