using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody2D playerRigidBody;
    private float playerSpeed = 8f;
    private float playerJump = 8f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float moveInput = Input.GetAxisRaw("Horizontal");
        playerRigidBody.velocity = new Vector2(moveInput * playerSpeed, playerRigidBody.velocity.y);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerJump);
        }        
    }
}
