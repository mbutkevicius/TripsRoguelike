using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class DelayedStartScript : MonoBehaviour
{
    [Header("Ghosts")]
    public YellowGhost yellowGhost;
    public PurpleGhost purpleGhost;
    public RedGhost redGhost;

    [Header("Game Info References")]
    public GameDataManager gameDataManager;
    public GameObject HudContainer;

    [Header("Countdown Timer Settings")]
    [Tooltip("Determines amount of delay before game will start")]
    [SerializeField] private float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        // disable player movement
        FindObjectOfType<UserInput>().OnDisable();

        // get the game manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();

        // start timer
        StartCoroutine(CountdownTimer());

        // hide the HUD
        HudContainer.SetActive(false);
    }

    public IEnumerator CountdownTimer()
    {
        yield return new WaitForSeconds(delayTime);

        // activate ghost movement
        purpleGhost.EnableMovement();
        redGhost.EnableMovement();
        StartCoroutine(yellowGhost.State1A());

        // activate player
        FindObjectOfType<UserInput>().OnEnable();

        // activate timer
        gameDataManager.BeginTimer();

        // show HUD
        HudContainer.SetActive(true);
    }
}
