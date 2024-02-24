using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D playerRigidBody;

    public float playerSpeed = 7f;
    public float playerJump = 12.5f;
    public float baseGravity = 3;
    public float airGravity = 4;
    public float bounceJump = 2;
    // the higher the number, the faster the increased gravity will hit the player at the peak of the jump
    public float ApproachingPeakJump = 5;
    private float previousVelocityY;
    private bool isGrounded;
    private bool isTouchingWallRight;
    private bool isTouchingWallLeft;
    private bool isApproachingPeak = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // get the direction player is moving (right key=1 leftkey=-1 no key=0)
        float moveInput = Input.GetAxisRaw("Horizontal");
        // multiply direction by player speed and preserve vertical velocity
        playerRigidBody.velocity = new Vector2(moveInput * playerSpeed, playerRigidBody.velocity.y);

        // Jump if player presses spacebar
        Jump();
 
        // if player is grounded and velocity == 0, return player to regular gravity bounds
        if (playerRigidBody.velocity.y == 0 && isGrounded){
            playerRigidBody.gravityScale = baseGravity;
        }

        // checks if velocity is approaching increased gravity levels
        // TODO: gravity stays high if continuously bouncing on bounce pad. Need to check if player bounced to determine 
        // if gravity needs to go back to default
        if (previousVelocityY > ApproachingPeakJump && playerRigidBody.velocity.y <= ApproachingPeakJump)
        {
            isApproachingPeak = true;
        }
        // if velocity is approaching jump height peak, increase gravity 
        if  (isApproachingPeak){
            playerRigidBody.gravityScale = airGravity;
        }
        // continuously track previous vertical position
        previousVelocityY = playerRigidBody.velocity.y;
        isApproachingPeak = false;
    }

    void FixedUpdate()
    {

    }

    // TODO: work on making sure player doesn't stick to wall. This function is still buggy
    void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the player is grounded
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5)
            {
                isGrounded = true;
                return;
            }
            else if (contact.normal.x > 0.5)
            {
                isTouchingWallRight = true;
                Debug.Log(isTouchingWallRight);
            }
            else if (contact.normal.x < -0.5)
            {
                isTouchingWallLeft = true;
            }
        }
    }

    // same here, still buggy
    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    // allows the player to jump 
    public void Jump(){
        // get the player 
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerJump);  
    }

    // determines player jump height when bouncing from jump pad
    public void BounceJump(){
        // get the player horizontal direction
        float moveInput = Input.GetAxisRaw("Horizontal");
        // preserve horizontal velocity and uses bounceJump to make player go extra high
        playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerJump * bounceJump);
    }
}
