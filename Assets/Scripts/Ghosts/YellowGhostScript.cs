using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class YellowGhostScript : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Transform trackingPoint;

    [Header("Script References")]
    public PlayerScript playerScript;
    public GameDataManager gameDataManager;
    public YellowGhostTrackingPointScript trackingPointScript;

    [Header("Movement Values")]
    [SerializeField] private float movementSpeed = 5f;
    [Tooltip("Controls how fast the inital dash boost is")]
    [SerializeField] private float boostMultiplier;
    private float originalBoostMultiplier;
    [Tooltip("Controls how fast the initial dash boost goes away. Higher value makes it disappear faster")]
    [SerializeField] private float boostDeceleration;
    [SerializeField] private float originalIdlingTime;

    private bool isChasingPlayer = false;

    private float speedTimeMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        // Find the player script
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        // Find the game data manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();

        // Get the Rigidbody2D and sprite
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // Set the tracking point transform
        trackingPoint = trackingPointScript.transform;

        // Begin the AI cycle
        StartCoroutine(State1A());

        // This is used to I can set the boost multiplier back to whatever value it was initally instead of manually entering it to reset it.
        originalBoostMultiplier = boostMultiplier;
    }

    IEnumerator State1A() // Idle state
    {
        yield return new WaitForSeconds(originalIdlingTime);
        StartCoroutine(State1B());
    }

    IEnumerator State1B() // State when Yellow Ghost is charging up the dash
    {
        yield return new WaitForSeconds(1);
        // Tells the tracking point to locate the player's current position
        trackingPointScript.TrackPlayer();

        // This transitions to State2, in the update function below
        isChasingPlayer = true;
    }

    void Update()
    {
        speedTimeMultiplier = gameDataManager.ghostTimeFraction;

        // This 'if' block contains State2 behavior. It's not super visually clean having this here but update function seemed the best for accomplishing this.
        if (isChasingPlayer == true)
        {
            // Used to determine how close the Ghost needs to be to the tracking point for the cycle to move on
            float distanceThreshold = 0.2f;

            if (Vector3.Distance(transform.position, trackingPoint.position) > distanceThreshold)
            {
                // Calculate direction to the tracking point
                Vector3 direction = (trackingPoint.position - transform.position).normalized;
                // Apply the velocity
                rb.velocity = direction * (movementSpeed * speedTimeMultiplier) * boostMultiplier;

                // This gives the Yellow Ghost a boost at the start of the dash, so it feels snappier. These values can be tweaked
                if (boostMultiplier > 1)
                {
                    boostMultiplier -= Time.deltaTime * boostDeceleration;
                }
                else
                {
                    boostMultiplier = 1;
                }
            }
            else // Happens when Yellow Ghost meets the distance threshold
            {
                isChasingPlayer = false; // Disables this whole 'if' block
                boostMultiplier = originalBoostMultiplier; // Reset the boost multiplier
                StartCoroutine(State1A()); // Reset the cycle
            }
        }

        // Get the velocity of the Rigidbody
        float velocityX = rb.velocity.x;

        // Flip the sprite based on the direction of movement
        Flip(velocityX);
    }

    // Flip the sprite based on movement direction
    private void Flip(float velocityX)
    {
        // If moving right, flip sprite to face right
        if (velocityX > 0)
        {
            sprite.flipX = false;
        }
        // If moving left, flip sprite to face left
        else if (velocityX < 0)
        {
            sprite.flipX = true;
        }
    }
}


