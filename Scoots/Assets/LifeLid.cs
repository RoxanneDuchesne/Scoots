using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLid : MonoBehaviour
{
    [SerializeField] GameObject coots;
    [SerializeField] GameObject lid;


    // Update is called once per frame
    void Update()
    {
        if (!coots.GetComponent<Pickups>().liftLid)
        {
            return;
        }

        if (lid.transform.position.y < 2000)
        {
            lid.transform.position = lid.transform.position + new Vector3(0, 200 * Time.deltaTime, 0);
        }

        if (lid.transform.position.y > 500 && lid.transform.position.z < 1800)
        {
            lid.transform.position = lid.transform.position + new Vector3(0, 0, 100 * Time.deltaTime);
        }

        Vector3 rotation = lid.transform.rotation.eulerAngles;
        if (rotation.x < 55)
        {
            lid.transform.rotation = Quaternion.Euler(rotation + new Vector3(5f * Time.deltaTime, 0, 0));
            rotation = lid.transform.rotation.eulerAngles;
        }
    }
}
