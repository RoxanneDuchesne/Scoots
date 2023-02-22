using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] GameObject coots;
    [SerializeField] GameObject wasdTutorial;
    [SerializeField] GameObject spaceTutorial;

    [SerializeField] float startDelayS;
    [SerializeField] float wasdDelayS;
    [SerializeField] float spaceDurationS;

    [SerializeField] float spaceDisplayZ1;
    [SerializeField] float spaceDisplayZ2;

    Vector3 startPosition;
    float delayTimeS = 0;

    float spaceDurS = 0;
    bool spaceDisplayHit1 = false;
    bool spaceDisplayHit2 = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = coots.transform.position;
        wasdTutorial.SetActive(false);
        spaceTutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        delayTimeS += Time.deltaTime;

        if (delayTimeS < startDelayS)
        {
            coots.transform.position = startPosition;
        } 
        else if (delayTimeS < wasdDelayS)
        {
            wasdTutorial.SetActive(true);
        }
        else
        {
            wasdTutorial.SetActive(false);
        }

        if (coots.transform.position.z < spaceDisplayZ1 && !spaceDisplayHit1)
        {
            spaceDurS = spaceDurationS;
            spaceDisplayHit1 = true;
        } 
        else if (coots.transform.position.z < spaceDisplayZ2 && !spaceDisplayHit2)
        {
            spaceDurS = spaceDurationS;
            spaceDisplayHit2 = true;
        }

        if (spaceDurS > 0)
        {
            spaceDurS -= Time.deltaTime;
            spaceTutorial.SetActive(true);
        } 
        else
        {
            spaceTutorial.SetActive(false);
        }
    }
}
