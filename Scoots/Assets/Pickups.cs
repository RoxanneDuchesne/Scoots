using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pickups : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI heartCounter;
    [SerializeField] float magnetPercent;

    List<GameObject> hearts = new List<GameObject>();
    int currentHearts = 0;

    void OnTriggerEnter(Collider other)
    {
        currentHearts += 1;
        heartCounter.text = " " + currentHearts;
        
        other.enabled = false;
        hearts.Add(other.gameObject);

        return;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < hearts.Count; i ++)
        {
            float distance = Vector3.Distance(this.transform.position, hearts[i].transform.position);
            Vector3 toTarget = (this.transform.position - hearts[i].transform.position).normalized; // Remove if heart is behind player

            if (distance < 1 || Vector3.Dot(toTarget, transform.forward) > 0.8)
            {
                hearts[i].SetActive(false);
                hearts.RemoveAt(i);
                i--;
                continue;
            }

            distance = distance * magnetPercent;
            Vector3 difference = hearts[i].transform.position - this.transform.position;
            difference = difference.normalized * distance;

            hearts[i].transform.position = this.transform.position + difference;
        }
    }
}
