using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody2D playerRigidBody;
    private float playerSpeed = 8f;
    private float playerJump = 8f;
    private bool isGrounded;
    private bool isTouchingWallRight;
    private bool isTouchingWallLeft;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {

        float moveInput = Input.GetAxisRaw("Horizontal");
        playerRigidBody.velocity = new Vector2(moveInput * playerSpeed, playerRigidBody.velocity.y);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerJump);
        }        
    }

        void FixedUpdate()
    {
        // Horizontal movement only when grounded
        if (isGrounded)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            playerRigidBody.velocity = new Vector2(moveInput * playerSpeed, playerRigidBody.velocity.y);
        }
    }

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

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
