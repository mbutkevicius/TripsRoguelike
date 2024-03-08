using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowGhostTrackingPointScript : MonoBehaviour
{

    public PlayerScript playerScript;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TrackPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TrackPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            transform.position = playerScript.transform.position;
        }
    }
}
