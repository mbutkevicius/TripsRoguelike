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
    [Tooltip("Max speed player can accelerate to")]
    [SerializeField] private float maxRunningSpeed = 7;
    [Tooltip("Minimum speed player can move. Player will start at this speed when running")]
    [SerializeField] private float minRunningSpeed = 2;
    [Tooltip("Default starting speed. Tbh probably don't need this as it should be the same as minRunningSpeed. For now keep them the same value")]
    [SerializeField] private float playerSpeed = 7;
    [Tooltip("This value determines the speed that a player will accelerate to max speed. The higher the value the faster they reach max speed")]
    [SerializeField] private float groundAcceleration = 0.05f;
    [Tooltip("This value determines the speed that a player will decelerate to max speed. The higher the value the faster they reach minimum speed")]
    [SerializeField] private float groundDeceleration = 0.05f;
    [Tooltip("Not implemented. Not sure we will need these values because I don't see a way you can exceed maxRunningSpeed")]
    [SerializeField] private float maxPosXAxisSpeed;
    [Tooltip("Not implemented. Not sure we will need these values because I don't see a way you can exceed maxRunningSpeed")]
    [SerializeField] private float maxNegXAxisSpeed;
    [Tooltip("Not implemented. Not sure we will need this value")]
    [SerializeField] private float maxPosYAxisSpeed;
    [Tooltip("Not implemented. Not sure we will need this value because I don't think you can go past maxGravity")]
    [SerializeField] private float maxNegYAxisSpeed;
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
    [Tooltip("Allows player to reach peak jump gravity change. This value is a little strange. I don't quite understand why it reacts the way it does. I would keep it between 3-7")]
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
        if (IsGrounded())
        {
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
        // move across the x axis
        HorizontalMovement(rb.velocity.y);
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

    // Not sure I need to even do this. Will test more on it later
    void CapMaxHorizontalMovement()
    {
        if (rb.velocity.x > maxNegXAxisSpeed)
        {
            playerSpeed = maxRunningSpeed;
        }
    }

    // Not sure I need to even do this. Will test more on it later
    void CapMaxVerticalMovement()
    {
        if (rb.velocity.y > maxNegYAxisSpeed)
        {
            playerSpeed = maxJumpForce;
        }
    }

    // allows the player to move across the xaxis
    public void HorizontalMovement(float velocityY)
    {
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

    #endregion

    #region Jump

    public float JumpBufferTime = 0.2f;
    public bool JumpBufferActive;
    // allows the player to jump
    private void Jump()
    {
        // this detects if we have pressed the jump input (doesn't count holding presses) and will activate a coroutine that makes a short jump buffer window

        // coroutine that activates the jump buffer for a period we define
        IEnumerator JumpBuffer()
        {
            JumpBufferActive = true;
            yield return new WaitForSeconds(JumpBufferTime);
            JumpBufferActive = false;
        }

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

    // determines player jump height when bouncing from jump pad
    public void BounceJump()
    {
        // disable jump while using the bouncepad. 
        // NOTE: If multiple jumps allowed, will need to check if more jumping is allowed
        if (isJumping)
        {
            isJumping = false;
        }

        // resets gravity when player touches bouncepad
        rb.gravityScale = ascGravity;

        // preserve horizontal velocity and uses bounceJump to make player go extra high
        rb.velocity = new Vector2(rb.velocity.x, playerJumpForce * bounceJump);
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