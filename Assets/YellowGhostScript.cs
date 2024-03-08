using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class YellowGhostScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public YellowGhostTrackingPointScript trackingPointScript;

    public Transform trackingPoint;

    public float movementSpeed = 5f;

    public SpriteRenderer sprite;

    public bool isChasingPlayer = false;

    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        trackingPoint = trackingPointScript.transform;

        StartCoroutine(State1A());

        originalBoostMultiplier = boostMultiplier;
    }

    public float boostMultiplier;
    private float originalBoostMultiplier;

    public float boostDeceleration;

    // Update is called once per frame
    void Update()
    {
        if (isChasingPlayer == true)
        {
            float distanceThreshold = 0.2f;
            if (Vector3.Distance(transform.position, trackingPoint.position) > distanceThreshold)
            {
                // Calculate direction towards the player
                // Vector3 direction = (trackingPoint.position - transform.position).normalized;

                // Move towards the player at a constant speed
                //transform.position += direction * movementSpeed * Time.deltaTime;

                SpeedBurst();

                // Calculate direction to the target
                Vector3 direction = (trackingPoint.position - transform.position).normalized;

                // Apply velocity based on direction and moveSpeed
                rb.velocity = direction * movementSpeed * boostMultiplier;

                if (boostMultiplier > 1)
                {
                    boostMultiplier -= Time.deltaTime * boostDeceleration;
                }
                else
                {
                    boostMultiplier = 1;
                }
            }
            else
            {
                isChasingPlayer = false; // Stop chasing the player
                boostMultiplier = originalBoostMultiplier;
                StartCoroutine(State1A());
            }
        }

        // Get the velocity of the Rigidbody
        float velocityX = rb.velocity.x;

        // Flip the sprite based on the direction of movement
        Flip(velocityX);
    }

    void SpeedBurst()
    {
        // Calculate direction to the target
        Vector3 direction = (trackingPoint.position - transform.position).normalized;

        // Apply velocity based on direction and moveSpeed
        rb.velocity = direction * (movementSpeed * 2);
    }

    IEnumerator State1A()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(State1B());
    }

    IEnumerator State1B()
    {
        yield return new WaitForSeconds(1);
        trackingPointScript.TrackPlayer();
        isChasingPlayer = true;
    }

    // Method to flip the sprite based on movement direction
    private void Flip(float velocityX)
    {
        // If moving right (positive velocity), flip sprite to face right
        if (velocityX > 0)
        {
            sprite.flipX = false; // Not flipped
        }
        // If moving left (negative velocity), flip sprite to face left
        else if (velocityX < 0)
        {
            sprite.flipX = true; // Flipped
        }
    }
}


