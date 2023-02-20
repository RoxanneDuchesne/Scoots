using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pickups : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI heartCounter;

    int currentHearts = 0;

    void OnTriggerEnter(Collider other)
    {
        currentHearts += 1;
        heartCounter.text = " " + currentHearts;

        other.gameObject.SetActive(false);

        return;
    }
}
