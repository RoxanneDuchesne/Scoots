using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollY : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI credits;
    [SerializeField] float duration;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer > duration)
        {
            SceneManager.LoadScene("Title");
        }

        credits.transform.position += new Vector3(0, 50 * Time.deltaTime, 0);
    }
}
