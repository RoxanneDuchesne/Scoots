using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_Handler : MonoBehaviour
{
    [SerializeField] GameObject coots;
    [SerializeField] GameObject invisablePlane;

    [SerializeField] GameObject blaster1;
    [SerializeField] GameObject blaster2;

    [SerializeField] float fireFrequencyS;
    [SerializeField] float fireSpeedS;

    List<GameObject> blasts = new List<GameObject>();
    float lastFireS = 0;
    int nextOrbIndex = 0;

    bool startFiring = false;

    [SerializeField] AudioSource gameAudio;
    [SerializeField] AudioSource battleAudio;
    bool audioStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            blasts.Add(this.transform.GetChild(i).gameObject);
        }
        battleAudio.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (coots.GetComponent<Pickups>().endFiring)
        {
            return;
        }

        if (!startFiring)
        {
            RaycastHit groundHit; 
            Physics.Raycast(coots.transform.position, -transform.up, out groundHit, 100, 3, QueryTriggerInteraction.Ignore);

            if (groundHit.collider != null && groundHit.collider.gameObject.CompareTag("Start_Battle"))
            {
                startFiring = true;
            }

            return;
        }

        invisablePlane.SetActive(false);

        if (gameAudio.volume > 0 || battleAudio.volume < 0.1f)
        {
            gameAudio.volume -= 0.1f * Time.deltaTime;
            battleAudio.volume += 0.1f * Time.deltaTime;
        } if (battleAudio.volume > 0.1f)
        {
            battleAudio.volume = 0.1f;
        }

        lastFireS += Time.deltaTime;

        if (lastFireS > fireFrequencyS)
        {
            fireOrb();
            lastFireS = 0;
        }
    }

    private void fireOrb()
    {
        if (nextOrbIndex >= blasts.Count)
        {
            nextOrbIndex = 0;
        }

        blasts[nextOrbIndex].transform.position = nextOrbIndex % 2 == 0 ? blaster1.transform.position : blaster2.transform.position;

        Vector3 direction = Vector3.Normalize(coots.transform.position - blasts[nextOrbIndex].transform.position);
        blasts[nextOrbIndex].GetComponent<Rigidbody>().velocity = direction * fireSpeedS;

        nextOrbIndex++;
    }
}
