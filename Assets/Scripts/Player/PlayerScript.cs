using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float maxRunningSpeed = 7;
    [SerializeField] private  float minRunningSpeed = 2;
    [SerializeField] private  float playerSpeed = 7;
    [SerializeField] private  float groundAcceleration = 0.05f;
    [SerializeField] private  float groundDeceleration = 0.05f;
    [SerializeField] private  float maxPosXAxisSpeed;
    [SerializeField] private  float maxNegXAxisSpeed;
    [SerializeField] private  float maxPosYAxisSpeed;
    [SerializeField] private  float maxNegYAxisSpeed;
    [HideInInspector] private float xMoveInput;
    [HideInInspector] private float yMoveInput;

    [Header("Jump")]
    [SerializeField] private float playerJumpForce = 12.5f;
    [SerializeField] private float maxJumpForce = 15f;
    [SerializeField] private float peakJumpGravity = 4;
    [SerializeField] private float bounceJump = 2;
    // the higher the number, the faster the increased gravity will hit the player at the peak of the jump
    [SerializeField] private float ApproachingPeakJump = 5;
    private bool isJumping = false;
    private float jumpTimeCounter;
    [SerializeField] private float maxJumpTime = 3;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField] private float maxCoyoteTime = 0.2f;
    private float CoyoteTimeCounter;
    private float peakJumpCounter;
    private float maxPeakJumpTime = 0.2f;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 3;
    private float previousVelocityY;
    private bool isFalling = false;
    private bool isApproachingPeak = false;

    [Header("Ground Check")]
    private Collider2D coll;
    private RaycastHit2D groundHit;
    // allows the player to jump slightly before touching the ground
    [SerializeField] private float extraHeight = 0.45f;
    [SerializeField] private LayerMask whatIsGround; 

    // Start is called before the first frame update
    void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // get the horizontal direction player is moving (right key=1 leftkey=-1 no key=0)
        GetXAxis();
        // get the vertical direction player is moving (right key=1 leftkey=-1 no key=0)
        GetYAxis();

        Jump();

        if (IsGrounded()){
            CoyoteTimeCounter = maxCoyoteTime;
            rb.gravityScale = baseGravity;
        }
        else {
            CoyoteTimeCounter -= Time.deltaTime;
        }

        // checks if velocity is approaching increased gravity levels
        // TODO: gravity stays high if continuously bouncing on bounce pad. Need to check if player bounced to determine 
        // if gravity needs to go back to default
        if (previousVelocityY > ApproachingPeakJump && rb.velocity.y <= ApproachingPeakJump)
        {
            isApproachingPeak = true;
            peakJumpCounter = maxPeakJumpTime;
        }
        // if velocity is approaching jump height peak, increase gravity 
        if  (isApproachingPeak){
            rb.gravityScale = peakJumpGravity;  
            //peakJumpCounter -= Time.deltaTime;
        }

        /*
        if (peakJumpCounter < 0){
            isFalling = true;
            isApproachingPeak = false;
            rb.gravityScale = 10f;
        }
        */

        // continuously track previous vertical position
        previousVelocityY = rb.velocity.y;
        isApproachingPeak = false;
    }

    void FixedUpdate()
    {
        // move across the x axis
        HorizontalMovement(rb.velocity.y);
    }

    // get the horizontal direction player is moving (right key=1 left key=-1 no key=0)
    public void GetXAxis(){
        xMoveInput = UserInput.instance.moveInput.x;
    }

    // get the vertical direction player is moving (up key=1 down key=-1 no key=0)
    public void GetYAxis(){
        yMoveInput = UserInput.instance.moveInput.y;
    }

    // allows the player to move across the xaxis
    public void HorizontalMovement(float velocityY){
        // get input
        GetXAxis();

        // if player is moving
        if (xMoveInput != 0)
        {
            // accelerate towards max running speed
            playerSpeed = Mathf.MoveTowards(playerSpeed, maxRunningSpeed, groundAcceleration);
        }
        // no input (player isn't moving)
        else
        {
            // Decelerate towards minRunningSpeed
            playerSpeed = Mathf.MoveTowards(playerSpeed, minRunningSpeed, groundDeceleration);
        }

        // // multiply direction by player speed and preserve vertical velocity
        rb.velocity = new Vector2(xMoveInput * playerSpeed, velocityY);
    }

    private void Jump(){
        // button was just pushed
        if (UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame()){

            // check if player can jump
            if (IsGrounded() || CoyoteTimeCounter > 0f){
                isJumping = true;
                jumpTimeCounter = maxJumpTime;
                //jumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, playerJumpForce);
            }
        }

        // button is being held down
        if (UserInput.instance.controls.Jumping.Jump.IsPressed()){
            if (jumpTimeCounter > 0 && isJumping){
                rb.velocity = new Vector2(rb.velocity.x, playerJumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else {
                isJumping = false;
            }
        }

        // jump button was released this frame
        if (UserInput.instance.controls.Jumping.Jump.WasReleasedThisFrame()){
            isJumping = false;
            CoyoteTimeCounter = 0f;
        }

        DrawGroundCheck();
    }

    // contains gravity functions
    private void Gravity(){

    }
    // lowers gravity at the peak of the jump
    private void PeakJumpGravity(){

    }

    // determines player jump height when bouncing from jump pad
    public void BounceJump(){
        // disable jump while using the bouncepad. 
        // NOTE: If multiple jumps allowed, will need to check if more jumping is allowed
        if (isJumping){
            isJumping = false;
        }

        // resets gravity when player touches bouncepad
        rb.gravityScale = baseGravity;
        // preserve horizontal velocity and uses bounceJump to make player go extra high
        rb.velocity = new Vector2(rb.velocity.x, playerJumpForce * bounceJump);
    }
    
    // Not sure I need to even do this. Will test more on it later
    void CapMaxHorizontalMovement(){
        if (rb.velocity.x > maxNegXAxisSpeed) {
            playerSpeed = maxRunningSpeed;
        }
    }

    // Not sure I need to even do this. Will test more on it later
    void CapMaxVerticalMovement(){
        if (rb.velocity.y > maxNegYAxisSpeed) {
            playerSpeed = maxJumpForce;
        }
    }

    private bool IsGrounded(){
        // only detects ground layer. Extra height gives player small wiggle room for touching the ground
        groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsGround);

        // if boxcast touches ground
        if (groundHit.collider != null){
            return true;
        }
        else {
            return false;
        }
    }

    private void DrawGroundCheck(){
        Color rayColor;
        if (IsGrounded()){
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }

        Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + extraHeight), Vector2.right * (coll.bounds.extents.y * 2), rayColor);
    }
}
