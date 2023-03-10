using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalCutscene : MonoBehaviour
{
    [SerializeField] GameObject coots;
    [SerializeField] GameObject cootsPosition;

    [SerializeField] GameObject rightArm;
    [SerializeField] GameObject leftArm;
    [SerializeField] GameObject body;
    [SerializeField] GameObject ders;
    [SerializeField] GameObject dersPosition;

    [SerializeField] GameObject heart;

    float offsetY = 0;
    bool isPulseIncreasing = false;
    Quaternion startPosRight;
    Quaternion startPosLeft;
    Vector3 startPositionBody;
    Vector3 startPositionDers;

    float timer = 0;
    float destructionDelay = 3;
    float armsFallDelay = 4.5f;
    float dersDelay = 8;
    float creditsDelay = 8;

    bool firstDers = true;

    // Start is called before the first frame update
    void Start()
    {
        startPosRight = rightArm.transform.rotation;
        startPosLeft = leftArm.transform.rotation;
        startPositionBody = body.transform.position;
        startPositionDers = ders.transform.position;
    }

    void move()
    {
        if (offsetY >= 20)
        {
            isPulseIncreasing = false;
        }
        else if (offsetY <= -20)
        {
            isPulseIncreasing = true;
        }

        offsetY += isPulseIncreasing ? Time.deltaTime * 50 : -Time.deltaTime * 50;

        rightArm.transform.rotation = Quaternion.Euler(startPosRight.eulerAngles + new Vector3(0, offsetY, 0));
        leftArm.transform.rotation = Quaternion.Euler(startPosLeft.eulerAngles + new Vector3(0, -offsetY, 0));

        body.transform.position = startPositionBody + new Vector3(0, offsetY * 3, 0);
        ders.transform.position = startPositionDers + new Vector3(0, offsetY * 3, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (coots.GetComponent<Pickups>().sendHearts && !coots.GetComponent<Pickups>().endFiring)
        {
            move();
        }

        if (!coots.GetComponent<Pickups>().endFiring)
        {
            return;
        }

        if (creditsDelay < timer)
        {
            SceneManager.LoadScene("Credits");
            return;
        }

        timer += Time.deltaTime;

        if (destructionDelay > timer)
        {
            rightArm.transform.rotation = Quaternion.Euler(rightArm.transform.rotation.eulerAngles + new Vector3(0, 500, 0) * Time.deltaTime);
            leftArm.transform.rotation = Quaternion.Euler(leftArm.transform.rotation.eulerAngles + new Vector3(0, -500, 0) * Time.deltaTime);
        }

        if (destructionDelay < timer && armsFallDelay > timer)
        {
            rightArm.transform.rotation = Quaternion.Euler(rightArm.transform.rotation.eulerAngles + new Vector3(360, 0, 0) * Time.deltaTime);
            leftArm.transform.rotation = Quaternion.Euler(rightArm.transform.rotation.eulerAngles + new Vector3(360, 0, 0) * Time.deltaTime);
            rightArm.transform.position += new Vector3(0, -10, 0);
            leftArm.transform.position += new Vector3(0, -10, 0);

            body.transform.rotation = Quaternion.Euler(rightArm.transform.rotation.eulerAngles + new Vector3(360, 0, 0) * Time.deltaTime);
            ders.SetActive(false);
        }

        if (armsFallDelay < timer && dersDelay > timer)
        {
            if (firstDers)
            {
                ders.SetActive(true);
                ders.transform.localScale = new Vector3(100, 100, 100);
                ders.transform.position = dersPosition.transform.position;
                ders.transform.rotation = Quaternion.Euler(new Vector3(-90, -10, 90));
                heart.transform.position = dersPosition.transform.position;
                heart.transform.localScale = new Vector3(10, 10, 10);
                firstDers = false;
            }

            heart.transform.position += new Vector3(0, 5, 0) * Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        if (!coots.GetComponent<Pickups>().endFiring)
        {
            return;
        }

        coots.transform.position = cootsPosition.transform.position;
    }
}
