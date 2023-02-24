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

    // Collision Events
    public bool sendHearts = false;
    public bool endFiring = false;
    public bool sendUp = false;
    public bool sendUpHalf = false;
    public bool liftLid = false;

    // End Game Firing
    [SerializeField] GameObject ders;
    [SerializeField] float fireFrequencyS;
    [SerializeField] float dersPercent;

    float lastFireS = 0;
    int nextHeartIndex = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Send_Hearts"))
        {
            sendHearts = true;
        }

        if (other.gameObject.CompareTag("Lift_Lid"))
        {
            liftLid = true;
        }

        if (other.gameObject.CompareTag("End_Battle"))
        {
            endFiring = true;
        }

        if (other.gameObject.CompareTag("Send_Up"))
        {
            sendUp = true;
        }

        if (other.gameObject.CompareTag("Send_Up_Half"))
        {
            sendUpHalf = true;
        }

        if (!other.gameObject.CompareTag("Heart"))
        {
            return;
        }

        currentHearts += 1;
        heartCounter.text = " " + currentHearts;
        
        other.enabled = false;
        hearts.Add(other.gameObject);

        return;
    }

    private void FixedUpdate()
    {
        if (sendHearts)
        {
            lastFireS += Time.deltaTime;

            if (lastFireS > fireFrequencyS)
            {
                fireHeart();
                lastFireS = 0;
            }

            for (int i = 0; i < nextHeartIndex; i++)
            {
                float distance = Vector3.Distance(ders.transform.position, hearts[i].transform.position);

                if (distance < 1)
                {
                    hearts[i].SetActive(false);
                    continue;
                }

                distance = distance * dersPercent;
                Vector3 difference = hearts[i].transform.position - ders.transform.position;
                difference = difference.normalized * 20;

                hearts[i].transform.position = hearts[i].transform.position - difference;
            }

            return;
        }

        for (int i = 0; i < hearts.Count; i ++)
        {
            if (!hearts[i].active)
            {
                continue;
            }

            float distance = Vector3.Distance(this.transform.position, hearts[i].transform.position);
            Vector3 toTarget = (this.transform.position - hearts[i].transform.position).normalized; // Remove if heart is behind player

            if (distance < 1 || Vector3.Dot(toTarget, transform.forward) > 0.8)
            {
                hearts[i].SetActive(false);
            }

            distance = distance * magnetPercent;
            Vector3 difference = hearts[i].transform.position - this.transform.position;
            difference = difference.normalized * distance;

            hearts[i].transform.position = this.transform.position + difference;
        }
    }

    private void fireHeart()
    {
        if (nextHeartIndex >= hearts.Count)
        {
            return;
        }

        hearts[nextHeartIndex].SetActive(true);
        hearts[nextHeartIndex].transform.position = this.transform.position;

        nextHeartIndex++;
        currentHearts--;
        heartCounter.text = " " + currentHearts;
    }
}
