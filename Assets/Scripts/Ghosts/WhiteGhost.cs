using System.Collections;
using UnityEngine;


public class WhiteGhost : MonoBehaviour
{
    public DelayedStartScript countdownTimer;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Transform trackingPoint;

    public CameraShakeEffect cameraShakeEffect;

    [Header("Script References")]
    public PlayerScript playerScript;
    public GameDataManager gameDataManager;
    public WhiteGhostTrackingPoint trackingPointScript;
    public GemSpawner gemSpawner;

    [Header("Movement Values")]
    [SerializeField] private float movementSpeed = 5f;
    [Tooltip("Determines the minimum possible invisible idling time during the first phase")]
    [Range(0, 5)]
    [SerializeField] private int phase1IdleLowValue;
    [Tooltip("Determines the maximum possible invisible idling time during the first phase")]
    [Range(0, 5)]
    [SerializeField] private int phase1IdleHighValue;
    [Tooltip("Determines the minimum possible invisible idling time during the second phase")]
    [Range(0, 5)]
    [SerializeField] private int phase2IdleLowValue;
    [Tooltip("Determines the maximum possible invisible idling time during the second phase")]
    [Range(0, 5)]
    [SerializeField] private int phase2IdleHighValue;
    [SerializeField] private float fadeInTime;
    [SerializeField] private int appearancesBeforeBreak;
    [SerializeField] private int breakTime;

    [Header("Animation")]
    public Animator animator;

    // Start is called before the first frame update
    private AudioManager AudioManager;
    void Start()
    {
        //Get Audio Manager
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
        // Find the player script
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        // Find the game data manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();
        // Find the game over script
        countdownTimer = GameObject.FindGameObjectWithTag("Logic").GetComponent<DelayedStartScript>();

        // Get the Rigidbody2D and sprite
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // Set the tracking point transform
        trackingPoint = trackingPointScript.transform;
    }

    int i = 0;
    public IEnumerator State1A() // Idle state when white ghost is invisible
    {
        int randomValue;

        // Phase 1 idling time range
        if (gameDataManager.ghostTimeFraction < 1.5)
        {
            randomValue = UnityEngine.Random.Range(phase1IdleLowValue, phase1IdleHighValue);
        }
        // Phase 2 idling time range
        else
        {
            randomValue = UnityEngine.Random.Range(phase2IdleLowValue, phase2IdleHighValue);
        }

        rb.velocity = Vector3.zero;
        sprite.enabled = false;
        yield return new WaitForSeconds(randomValue);

        i++;

        if (i < appearancesBeforeBreak)
        {
            StartCoroutine(State1B());
        }
        else
        {
            StartCoroutine(Wait());
            i = 0;
        }
    }

    public IEnumerator State1B() // Dash state
    {
        // Select a valid target and go to that position
        trackingPointScript.SelectRandomValidTarget();
        transform.position = trackingPoint.position;

        // turn on sprite
        sprite.enabled = true;

        // play animation and sounds
        animator.Play("WhiteGhostFadeInAnimation", -1, 0f);
        //FindObjectOfType<AudioManager>().Play("WhiteGhostAppear");
        AudioManager.playSoundName("ghost_appear", gameObject);
        // let the ghost fade in
        yield return new WaitForSeconds(fadeInTime);

        // play animation and sounds
        animator.Play("WhiteGhostFadeOutAnimation", -1, 0f);
        //FindObjectOfType<AudioManager>().Play("WhiteGhostAggro");
        AudioManager.playSoundName("ghost_chase", gameObject);

        // get the direction from the selected target
        Vector3 direction = trackingPointScript.GetSelectedTargetDirection();
        // set the rigidbody velocity
        rb.velocity = direction * movementSpeed * gameDataManager.ghostTimeFraction;

        // shake camera
        StartCoroutine(cameraShakeEffect.CustomCameraShake(0.1f, 0.08f));

        // let animation finish and restart cycle
        yield return new WaitForSeconds(1);

        if (canChasePlayer)
        {
            StartCoroutine(State1A());
        }
    }

    bool b = false;
    public bool waiting = false;
    public IEnumerator Wait()
    {
        if (b)
        {
            gemSpawner.DecreaseGemWeight();
        }
        b = true;

        waiting = true;
        yield return new WaitForSeconds(breakTime);
        waiting = false;
        gemSpawner.IncreaseGemWeight();
        StartCoroutine(State1A());
    }

    void Update()
    {
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
            transform.rotation = Quaternion.AngleAxis(0f, Vector3.up);
            // sprite.flipX = false;
        }
        // If moving left, flip sprite to face left
        else if (velocityX < 0)
        {
            transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
            // sprite.flipX = true;
        }
    }

    private bool canChasePlayer = true;
    public void DisableMovement()
    {
        canChasePlayer = false;
    }
}


