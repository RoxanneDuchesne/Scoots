using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] float startDelay;
    float timeElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        float time = timeElapsed - startDelay;

        if (time < 0)
        {
            time = 0;
        }

        string minutes = "" + (int) time / 60;
        string seconds = "" + (int) time % 60;

        if (minutes.Length == 1)
        {
            minutes = "0" + minutes;   
        }

        if (seconds.Length == 1)
        {
            seconds = "0" + seconds;
        }

        timer.text = minutes + ":" + seconds;
    }
}
