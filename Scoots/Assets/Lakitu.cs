using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lakitu : MonoBehaviour
{
    [SerializeField]  GameObject coots;
    [SerializeField]  float minY;

    List<GameObject> respawnPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
            respawnPoints.Add(this.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit groundHit;
        Physics.Raycast(coots.transform.position, -coots.transform.up, out groundHit, Mathf.Infinity, 3, QueryTriggerInteraction.Ignore);

        float closestDistance = Vector3.Distance(coots.transform.position, respawnPoints[0].transform.position);
        Vector3 closestPoint = respawnPoints[0].transform.position;

        if ((groundHit.collider != null && groundHit.collider.CompareTag("Lakitu") && groundHit.distance < 1) || coots.transform.position.y < minY)
        {
            for (int i = 1; i < respawnPoints.Count; i++)
            {
                Vector3 point = respawnPoints[i].transform.position;
                float distance = Vector3.Distance(coots.transform.position, point);

                if (distance < closestDistance && coots.transform.position.x < point.x)
                {
                    closestDistance = distance;
                    closestPoint = point;
                }
            }
            coots.transform.position = closestPoint;
            coots.transform.rotation = Quaternion.identity;
        }
    }
}
