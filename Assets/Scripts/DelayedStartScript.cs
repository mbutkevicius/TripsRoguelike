using System.Collections;

using UnityEngine;

public class DelayedStartScript : MonoBehaviour
{
    [Header("Ghosts")]
    public YellowGhost yellowGhost;
    public PurpleGhost purpleGhost;
    public RedGhost redGhost;
    public WhiteGhost whiteGhost;
    public AK.Wwise.Event sceneMusic;

    [Header("Game Info References")]
    public bool isCountdown = true;
    public GameDataManager gameDataManager;
    public GameObject HudContainer;

    [Header("Countdown Timer Settings")]
    [Tooltip("Determines amount of delay before game will start")]
    [SerializeField] private float delayTime;

    private AudioManager AudioManager;
    // Start is called before the first frame update
    void Start()
    {
        // FindObjectOfType<AudioManager>().Play("Music");
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
        AudioManager.toggleMusic();
        //FindObjectOfType<AudioManager>().toggleMusic();

        // disable player movement
        //FindObjectOfType<UserInput>().OnDisable();

        // get the game manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();

        // start timer
        StartCoroutine(CountdownTimer());
        // start timer for sounds/effects (tracks each tick)
        StartCoroutine(TimerTicks());

        // hide the HUD
        HudContainer.SetActive(false);
    }

    public IEnumerator CountdownTimer()
    {
        yield return new WaitForSeconds(delayTime);

        isCountdown = false;

        // activate ghost movement
        purpleGhost.EnableMovement();
        redGhost.EnableMovement();
        StartCoroutine(yellowGhost.State1A());
        StartCoroutine(whiteGhost.State1A());

        // activate player
        //FindObjectOfType<UserInput>().OnEnable();

        // activate timer
        gameDataManager.BeginTimer();

        // show HUD
        HudContainer.SetActive(true);
    }

    public IEnumerator TimerTicks()
    {
        yield return new WaitForSeconds(1);
        // "3"
        //FindObjectOfType<AudioManager>().Play("TimerTick");
        AudioManager.playSoundName("timer_tick", gameObject);
        yield return new WaitForSeconds(0.666f);
        // "2"
        //FindObjectOfType<AudioManager>().Play("TimerTick");
        AudioManager.playSoundName("timer_tick", gameObject);
        yield return new WaitForSeconds(0.666f);
        // "1"
        //FindObjectOfType<AudioManager>().Play("TimerTick");
        AudioManager.playSoundName("timer_tick", gameObject);
        yield return new WaitForSeconds(0.666f);
        // "GO!"
        //FindObjectOfType<AudioManager>().Play("TimerFinish");
        AudioManager.playSoundName("timer_finish", gameObject);
    }
}
