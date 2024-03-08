using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class YellowGhostScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public YellowGhostTrackingPointScript trackingPointScript;

    public Transform trackingPoint;

    public float movementSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        trackingPoint = trackingPointScript.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (trackingPoint != null)
        {
            // Calculate direction towards the player
            Vector3 direction = (trackingPoint.position - transform.position).normalized;

            // Move towards the player at a constant speed
            transform.position += direction * movementSpeed * Time.deltaTime;
        }
        else
        {
            Debug.LogError("Player transform is not assigned!");
        }
    }
}
