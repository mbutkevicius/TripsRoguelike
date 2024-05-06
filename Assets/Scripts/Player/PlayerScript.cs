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
using UnityEngine.XR;

public class PlayerScript : AnimatorManager
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
    [HideInInspector] public float xMoveInput;
    [HideInInspector] public float yMoveInput;

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
    public float coyoteTimeCounter;
    public bool bouncing = false;
    public float JumpBufferTime = 0.2f;
    private bool JumpBufferActive;
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
    [Tooltip("The lower the number, the longer it will take to trigger descending gravity")]
    [SerializeField] private float gameOverGravity = 60f;
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

    private Collider2D thinPlatformColl;

    [Header("Animation")]
    public Animator animator;
    // Important: this is used to determine the placement in the animator array in AnimatorManager.cs
    // if multiple parts were used, you would add each component in numeric order you want them to appear 
    // in the layers
    private const int SPRITE = 0;

    // Start is called before the first frame update
    void Start()
    {
        // initialize animation
        Initialize(GetComponent<Animator>().layerCount, Animations.IDLE, GetComponent<Animator>(), DefaultAnimation);

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        xMoveInput = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(xMoveInput);

        if (FindObjectOfType<GameOverScript>().isGameOver == false){
            // get the horizontal direction player is moving (right key=1 leftkey=-1 no key=0)
            GetXAxis();
            // get the vertical direction player is moving (right key=1 leftkey=-1 no key=0)
            GetYAxis();

            CheckMovementAnimation(SPRITE);

            // set animator movement values
            animator.SetFloat("horizontal", Mathf.Abs(Input.GetAxis("Horizontal")));
            animator.SetFloat("vertical", Mathf.Abs(yMoveInput));

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
                if (coyoteTimeCounter != maxCoyoteTime)
                {
                    coyoteTimeCounter = maxCoyoteTime;
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
                coyoteTimeCounter -= Time.deltaTime;
                // set airborn gravity
                Gravity();
            }
        }
        // if gameover is occuring, clear all player movement 
        else{
            // reset input
            FindObjectOfType<UserInput>().ClearInput();
            xMoveInput = 0;
            // stop movement
            rb.velocity = Vector2.zero;
            rb.drag = 0;
            // player would slowly fall through air without this value. I suspect something is happening with the drag 
            // did not test because this is a working solution. 
            rb.gravityScale = gameOverGravity;
        }
    }

    // physics updates contained here
    void FixedUpdate()
    {
        // check if game is still going on
        if (FindObjectOfType<GameOverScript>().isGameOver == false){
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
        // if gameover is occuring, clear all player movement 
        else{
            // reset input
            FindObjectOfType<UserInput>().ClearInput();
            xMoveInput = 0;
            // stop movement
            rb.velocity = Vector2.zero;
            rb.drag = 0;
            // player would slowly fall through air without this value. I suspect something is happening with the drag 
            // did not test because this is a working solution. 
            rb.gravityScale = gameOverGravity;
        }
    }

    // Method to modify linearDrag variables when player is moving
    public void ModifyPhysics(){
        bool changingDirection = false;

        // check if player player is changing directions
        if ((xMoveInput > 0 && rb.velocity.x < 0) || (xMoveInput < 0 && rb.velocity.x > 0)){
            changingDirection = true;
        }

        // check if player is grounded and not moving a directional input. Think of 0.4 as the deadzone to stop getting run input
        if(((Mathf.Abs(xMoveInput) < 0.4f) || changingDirection) && IsGrounded() && !bouncing){
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
        // note on syntax cause I forget sometimes (if facingRight 0 else 180)
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    #endregion

    #region Jump
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
            Debug.Log(bouncing);
            StartCoroutine(JumpBuffer());
            if (bouncing){
                StopCoroutine(JumpBuffer());
                JumpBufferActive = false;
                jumpTimeCounter = 0;
                isJumping = false;
                coyoteTimeCounter = 0;
            }

            // check if player can jump
            if (IsGrounded() || coyoteTimeCounter > 0f && isJumping == false) // added 'isJumping == false' here to ensure we can't jump while we're jumping 
            {
                isJumping = true;
                jumpTimeCounter = maxJumpTime;
                //jumpCount++;
                rb.velocity = new Vector2(rb.velocity.x, playerJumpForce);

                FindObjectOfType<AudioManager>().Play("PlayerJump");
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
            bouncing = false;
            coyoteTimeCounter = 0f;
        }

        // debugging tool you can see in gizmos
        DrawGroundCheck();
    }

    // determines player jump height when bouncing from jump pad
    public void BounceJump()
    {
        FindObjectOfType<AudioManager>().Play("MushroomBounce");

        // disable jump while using the bouncepad. 
        // NOTE: If multiple jumps allowed, will need to check if more jumping is allowed
        //isJumping = false;

        // don't let player bounce when jumping from below
        if (rb.velocity.y <= 0){
            bouncing = true;

            rb.gravityScale = ascGravity;
            rb.velocity = new Vector2(rb.velocity.x, playerJumpForce * bounceJump);
        }
        //bouncing = false;
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
    public bool IsGrounded()
    {
        // Note: This is probably what I am going to change to
        //groundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, whatIsGround);

        // only detects ground layer. Extra height gives player small wiggle room for touching the ground
        groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsGround);

        thinPlatformHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsThinPlatform);

        // if boxcast touches ground
        if (groundHit.collider != null)
        {
            return true; 
        }
        // check if player is on platform
        // to be especially careful, could add another && platform.bounds.max.y check to see if player is on top
        else if (thinPlatformHit.collider != null && rb.velocity.y == 0){
            Debug.Log("Platform");
            return true;
        }
        else
        {
            return false;
        }
    }

    // Boxcast to tell if player is touching ceiling
    private bool TouchingCeiling()
    {
        // Get the original size of the player's bounding box
        Vector2 originalSize = coll.bounds.size;

        // Reduce the size of the player's bounding box along the x-axis to 95% of the original size
        // this is to prevent bug where player would rarely register a ceiling hit when they touch wall
        float newSizeX = originalSize.x * 0.95f;

        // Create a new size vector with the adjusted x-component and the original y-component
        Vector2 newSize = new Vector2(newSizeX, originalSize.y);

        // Check for collision with the ceiling
        ceilingHit = Physics2D.BoxCast(coll.bounds.center, newSize, 0f, Vector2.up, ceilingDetection, whatIsGround);

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

    #region Animation

    // would use this if there were multiple layers to our character
    void DefaultAnimation(int layer){
        CheckMovementAnimation(SPRITE);

        /*
        if (layer == UPPERBODY){
            CheckTopAnimation();
        }
        else {
            CheckBottomAnimation();
        }
        */
    }

    // would use this if there were multiple layers to our character
    /*
    private void CheckTopAnimation(){
        CheckMovementAnimation(UPPERBODY)
    }

    private void CheckBottomAnimation(){
        CheckMovementAnimation(LOWERBODY)
    }
    */

    // checks which animation state to play
    private void CheckMovementAnimation(int layer){
        if (xMoveInput != 0 && IsGrounded()){
            Play(Animations.RUN, layer, false, false);
        }
        else if (rb.velocity.y > 0 && !IsGrounded()){
            Play(Animations.JUMP, layer, false, true);
        }
        else if (rb.velocity.y < 0 && !IsGrounded()){
            Play(Animations.FALLING, layer, false, true);
        }
        else {
            Play(Animations.IDLE, layer, false, false);
        }
    }

    // disable animations
    public void DisableAnimation(){
        if (animator != null){
            animator.enabled = false;
        }
    }

    // enable animations
    public void EnableAnimation(){
        if (animator != null){
            animator.enabled = true;
        }
    }

    #endregion
}