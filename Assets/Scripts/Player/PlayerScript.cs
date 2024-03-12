using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [Tooltip("Max speed player can accelerate to")]
    [SerializeField] private float maxRunningSpeed = 8f;
    [Tooltip("Value for how quickly player moves into a run. It should be higher than your maxRunningSpeed")]
    [SerializeField] private float playerAcceleration = 7f;
    [Tooltip("This value determines how quickly the player is able to turn around on the ground. The higher the value the faster they can turn")]
    [SerializeField] private float groundedLinearDrag = 5.35f;
    [Tooltip("This value determines how quickly the player is able to turn around in the air. This value does affect how gravity will feel. Will need to tweak gravity settings as this value is tweaked. I recommend keeping it relatively close to 0")]
    [SerializeField] private float aerialLinearDrag = 2f;
    private Vector2 direction;
    private bool facingRight = true;
    [HideInInspector] private float xMoveInput;
    [HideInInspector] private float yMoveInput;

    [Header("Jump")]
    [Tooltip("Determines the jumping power a player has. The higher the number, the higher the player jumps")]
    [SerializeField] private float playerJumpForce = 12.5f;
    [Tooltip("Not implemented. Don't think we will need this")]
    [SerializeField] private float maxJumpForce = 15f;
    [Tooltip("Determines the bounce force factor on a bouncepad. bounceJump is multiplied by playerJumpForce")]
    [SerializeField] private float bounceJump = 2;
    public bool isJumping = false;
    private float jumpTimeCounter;
    [Tooltip("Determines how long player can hold jump button before jump ends")]
    [SerializeField] private float maxJumpTime = 3;
    //private int jumpCount = 0;
    [Tooltip("Not implemented. I can code this in if we decide we ever want to have multiple jumps")]
    [SerializeField] private int maxJumpCount = 1;
    [Tooltip("The amount of leeway you have to jump after leaving a ledge. The lower the number the less grace period you have")]
    [SerializeField] private float maxCoyoteTime = 0.2f;
    [Tooltip("The force that pushes you down when you hit a ceiling (make a positive number")]
    [SerializeField] private float playerCeilingBumpForce = 12.5f;
    private float CoyoteTimeCounter;
    public bool bouncing = false;
    //private float peakJumpCounter;
    //private float maxPeakJumpTime = 0.2f;

    [Header("Gravity")]
    [Tooltip("The strength of gravity on the ascent of your jump")]
    [SerializeField] private float ascGravity = 3;
    [Tooltip("The strength of gravity on the descent of your jump")]
    [SerializeField] private float descGravity = 5;
    [Tooltip("The strength of gravity at the peak of your jump")]
    [SerializeField] private float peakJumpGravity = 2;
    [Tooltip("The max gravity player can reach. Gravity will continue to get stronger the longer you fall until you reach the max value")]
    [SerializeField] private float maxGravity = 8;
    [Tooltip("How quickly you will reach max gravity")]
    [SerializeField] private float descGravityAceleration = 0.3f;
    [Tooltip("Allows player to reach peak jump gravity change.")]
    [SerializeField] private float ApproachingPeakJump = 5;
    //[SerializeField] private float ascPeakJump = 0.1f;
    [Tooltip("The lower the number, the longer it will take to trigger descending gravity")]
    [SerializeField] private float descPeakJump = 0.1f;
    private float previousVelocityY;
    //private bool isFalling = false;
    private bool isApproachingPeak = false;

    [Header("Ground Check")]
    // allows the player to jump slightly before touching the ground
    [Tooltip("Gives the player a buffer jump if they press the jump button without touching the ground. The higher the number, the more leeway the player has. Turn on gizmos to see a visual representation")]
    [SerializeField] private float extraHeight = 0.45f;
    [SerializeField] public float ceilingDetection = 0.45f;
    [Tooltip("Need to create a ground layer and set all ground objects to ground layer. This layer determines whether player is able to jump")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsThinPlatform;
    private Collider2D coll;
    private RaycastHit2D groundHit;
    private RaycastHit2D thinPlatformHit;
    private RaycastHit2D ceilingHit;

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

        // check if player presses jump
        Jump();

        // track coyote time and reset gravity is player isgrounded
        // NOTE: I'm not sure it's efficient to check grounded status every frame. However, I don't know a better way right now
        // also don't know if this should be in FixedUpdate
        if (IsGrounded())
        {
            // reset descGravity to default
            descGravity = 6f;

            // track coyoteTimer
            if (CoyoteTimeCounter != maxCoyoteTime)
            {
                CoyoteTimeCounter = maxCoyoteTime;
            }
            // reset gravity
            if (rb.gravityScale != ascGravity)
            {
                rb.gravityScale = ascGravity;
            }
        }
        // if airborn
        else
        {
            // reduce coyote time
            CoyoteTimeCounter -= Time.deltaTime;
            // set airborn gravity
            Gravity();
        }
    }

    // physics updates containedhere
    void FixedUpdate()
    {
        GroundedHorizontalMovement(rb.velocity.y);
        if (IsGrounded()){
            //groundedLinearDrag = 5.35f;
            // move across the x axis
        }
        else {
            //groundedLinearDrag = 2;
        }


        ModifyPhysics();
    }

    // Method to modify linearDrag variables when player is moving
    public void ModifyPhysics(){
        bool changingDirection = false;

        // check if player player is changing directions
        if ((xMoveInput > 0 && rb.velocity.x < 0) || (xMoveInput < 0 && rb.velocity.x > 0)){
            changingDirection = true;
        }

        // check if player is grounded and not moving a directional input. Think of 0.4 as the deadzone to stop getting run input
        if(((Mathf.Abs(xMoveInput) < 0.4f) || changingDirection) && IsGrounded()){
            rb.drag = groundedLinearDrag;
        }
        // check if player is in the air and not moving a directional input. Think of 0.4 as the deadzone to stop getting run input
        else if(((Mathf.Abs(xMoveInput) < 0.4f) || changingDirection) && !IsGrounded()){
            rb.drag = aerialLinearDrag;
            /*
            if (xMoveInput == 0){
                playerAcceleration = Mathf.MoveTowards(playerAcceleration, 0, 0.2f);
                rb.velocity = new Vector2(playerAcceleration * xMoveInput, rb.velocity.y);
            }
            else{
                playerAcceleration = 14f;
            }
            */
        }
        // rest drag to 0 is grounded
        else if (IsGrounded()){
            rb.drag = 0;
        }
        // must be in the air in this situation. This case is here for the chance that I make it so that the linearDrag can change values in the air like the ground
        else {
            //aerialLinearDrag = 0;
            rb.drag = aerialLinearDrag;
        }
    }

    #region Movement

    // get the horizontal direction player is moving (right key=1 left key=-1 no key=0)
    public void GetXAxis()
    {
        xMoveInput = UserInput.instance.moveInput.x;
    }

    // get the vertical direction player is moving (up key=1 down key=-1 no key=0)
    public void GetYAxis()
    {
        yMoveInput = UserInput.instance.moveInput.y;
    }

    public void GroundedHorizontalMovement(float velocityY){
        GetXAxis();

        // apply force to move vector. Note: this is AddForce not creating a new vector. This allows the slide to happen during turns and stops
        rb.AddForce(Vector2.right * xMoveInput * playerAcceleration);

        // check if player is changing direction
        if ((xMoveInput > 0 && !facingRight) || (xMoveInput < 0 && facingRight)){
            Flip();
        }
        // cap players velocity if they are moving past max speed
        if (Mathf.Abs(rb.velocity.x) > maxRunningSpeed){
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxRunningSpeed, rb.velocity.y);
        }

    }

    // Method to flip the player sprite
    public void Flip(){
        facingRight = !facingRight;
        // flip sprite along the y axis
        // note on syntax cause I forget sometimes (if facingRight 0 else 100)
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    #endregion

    #region Jump

    public float JumpBufferTime = 0.2f;
    private bool JumpBufferActive;

    // this detects if we have pressed the jump input (doesn't count holding presses) and will activate a coroutine that makes a short jump buffer window

    // coroutine that activates the jump buffer for a period we define
    IEnumerator JumpBuffer()
    {
        JumpBufferActive = true;
        yield return new WaitForSeconds(JumpBufferTime);
        JumpBufferActive = false;
    }

    // allows the player to jump
    private void Jump()
    {
        // check if button was just pushed
        if (UserInput.instance.controls.Jumping.Jump.WasPressedThisFrame() || JumpBufferActive == true) // now jump will take 'JumpBufferActive' as a valid alternative
        {
            StartCoroutine(JumpBuffer());
            // check if player can jump
            if (IsGrounded() || CoyoteTimeCounter > 0f && isJumping == false) // added 'isJumping == false' here to ensure we can't jump while we're jumping 
            {
                isJumping = true;
                jumpTimeCounter = maxJumpTime;
                //jumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, playerJumpForce);
            }
        }

        // cancel jump if player touches the ceiling
        if (TouchingCeiling() == true)
        {
            isJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, playerCeilingBumpForce * -1); // add a bit of a downward push to simulate a sudden 'bump'
            JumpBufferActive = false; // in case we hit our head in a tiny narrow space, the jump buffer won't immediately make us jump again
            StopCoroutine(JumpBuffer());
        }

        // button is being held down
        if (UserInput.instance.controls.Jumping.Jump.IsPressed())
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, playerJumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        else
        {
            isJumping = false;
        }

        // jump button was released this frame
        if (UserInput.instance.controls.Jumping.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            CoyoteTimeCounter = 0f;
        }

        // debugging tool you can see in gizmos
        DrawGroundCheck();
    }

    IEnumerator BounceBuffer()
    {
        JumpBufferActive = true;
        yield return new WaitForSeconds(JumpBufferTime);
        JumpBufferActive = false;
    }

    // determines player jump height when bouncing from jump pad
    public void BounceJump()
    {
        bouncing = true;

        // resets gravity when player touches bouncepad
        rb.gravityScale = ascGravity;

        // disable jump while using the bouncepad. 
        // NOTE: If multiple jumps allowed, will need to check if more jumping is allowed
        if (isJumping)
        {
            isJumping = false;
        }

        // preserve horizontal velocity and uses bounceJump to make player go extra high
        rb.velocity = new Vector2(rb.velocity.x, playerJumpForce * bounceJump);
        bouncing = false;
    }

    #endregion

    #region Gravity

    // contains gravity changes for when player is airborn
    private void Gravity()
    {
        // This part is a little iffy. I was just playing around with stuff until it worked LOL 
        if (previousVelocityY > ApproachingPeakJump && rb.velocity.y <= ApproachingPeakJump)
        {
            // entering peak of jump
            isApproachingPeak = true;

            // if velocity is approaching jump height peak, increase gravity 
            if (isApproachingPeak)
            {
                rb.gravityScale = peakJumpGravity;
            }
        }
        // when players velocity goes below descending value, change gravity again
        else if (rb.velocity.y < descPeakJump)
        {
            // exit peak jump
            isApproachingPeak = false;
            // increase gravity the longer you fall until it reaches max
            descGravity = Mathf.MoveTowards(rb.gravityScale, maxGravity, descGravityAceleration);
            // apply gravity change
            rb.gravityScale = descGravity;
        }

        // continuously track previous vertical position
        previousVelocityY = rb.velocity.y;
        isApproachingPeak = false;
    }

    #endregion

    #region Ground/Ceiling Check
    // renamed to ground/ceiling check
    private bool IsGrounded()
    {
        // Note: This is probably what I am going to change to
        //groundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, whatIsGround);

        // only detects ground layer. Extra height gives player small wiggle room for touching the ground
        groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsGround);

        thinPlatformHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsThinPlatform);

        // if boxcast touches ground
        if (groundHit.collider != null || thinPlatformHit.collider != null)
        {
            return true; 
        }
        else
        {
            return false;
        }
    }
    private bool TouchingCeiling()
    {
        // Check for collision with the ceiling
        ceilingHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, ceilingDetection, whatIsGround);

        // If boxcast touches ceiling
        if (ceilingHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // debug tool to see when player touches the ground. Can activate in gizmos
    private void DrawGroundCheck()
    {
        Color rayColor;
        if (IsGrounded())
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + extraHeight), Vector2.right * (coll.bounds.extents.y * 2), rayColor);
    }

    #endregion
}