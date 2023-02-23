using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinX : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rotation = this.transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(rotation + new Vector3(rotationSpeed, 0, 0));
    }
}
