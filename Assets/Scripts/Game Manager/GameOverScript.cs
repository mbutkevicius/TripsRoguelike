using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using FullscreenEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;

public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI;
    public Animator transition;
    public GemSpawner gemSpawner;
    public PlayerScript playerScript;
    public YellowGhost yellowGhost;
    public PurpleGhost purpleGhost;
    public RedGhost redGhost;
    public GameDataManager gameDataManager;

    [HideInInspector] public bool isGameOver = false;
    [Tooltip("Dev tool: disables cursor during gameplay. I like this option on but can make it annoying during testing")]
    [SerializeField] private bool isCursorDisabled;
    [Tooltip("Determines the length of time required for gameOverUI to appear after death")]

    [Header("Delay Settings")]
    [SerializeField] private float deathDelay = 1.5f;
    [SerializeField] private float restartDelay = 1f;

    void Start(){
        // dev check to make it easier to work
        //disables the cursor during gameplay if true
        if (isCursorDisabled){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        Debug.Log(gameOverUI);

        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();
    }

    public void GameOver(){
        if (!isGameOver){
            gameDataManager.SetHighScore();
            transition.SetTrigger("Death");
            playerScript.KillPlayer();
            StopCoroutine(gemSpawner.SpawnGems());

            isGameOver = true;
            Debug.Log("GameOver");

            // disable ghost movement
            redGhost.DisableMovement();
            purpleGhost.DisableMovement();
            yellowGhost.DisableMovement();

            // disable player movement
            FindObjectOfType<UserInput>().OnDisable();
            FindObjectOfType<UserInput>().ClearInput();
            FindObjectOfType<PlayerScript>().DisableAnimation();

            // disable gem spawning
            gemSpawner.StopAllCoroutines();

            //disable timer
            FindObjectOfType<GameDataManager>().EndTimer();

            // invoke calls the function name and allows some sort of delay before calling it
            //Invoke(nameof(Restart), restartDelay);

            FindObjectOfType<AudioManager>().Play("PlayerDeath");
            FindObjectOfType<AudioManager>().Stop("Music");

            StartCoroutine(GameOverScreen());
        }
            
    }

    private IEnumerator GameOverScreen(){
        yield return new WaitForSeconds(deathDelay);
        gameOverUI.SetActive(true);
            // allow player to use the cursor to select from UI
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
    }

    public void Restart(){
        Debug.Log("Restart");
        StartCoroutine(Restarted());
    }

    IEnumerator Restarted()
    {
        transition.SetTrigger("Out");
        yield return new WaitForSeconds(deathDelay);

        isGameOver = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Debug.Log(gameOverUI);

        FindObjectOfType<UserInput>().OnEnable();
        FindObjectOfType<PlayerScript>().EnableAnimation();
    }

    public void MainMenu(){
        StartCoroutine(MainMenuTransition());
    }

    IEnumerator MainMenuTransition()
    {
        transition.SetTrigger("Out");
        yield return new WaitForSeconds(deathDelay);

        isGameOver = false;

        SceneManager.LoadScene(0);
    }

    public void Quit(){
        Application.Quit();
    }
}
