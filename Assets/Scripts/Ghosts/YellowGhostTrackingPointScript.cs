using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowGhostTrackingPointScript : MonoBehaviour
{
    public PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Find the player script
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrackPlayer()
    {
        transform.position = playerScript.transform.position;
    }
}
