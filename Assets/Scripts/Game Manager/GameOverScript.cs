using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using FullscreenEditor;
using UnityEditor.EditorTools;

public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI;

    [HideInInspector] public bool isGameOver = false;
    [Tooltip("Dev tool: disables cursor during gameplay. I like this option on but can make it annoying during testing")]
    [SerializeField] private bool isCursorDisabled;
    [Tooltip("Determines the length of time required for gameOverUI to appear after death")]
    [SerializeField] private float restartDelay = 1.5f;

    void Start(){
        // dev check to make it easier to work
        //disables the cursor during gameplay if true
        if (isCursorDisabled){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        Debug.Log(gameOverUI);
    }

    public void GameOver(){
        if (!isGameOver){
            isGameOver = true;
            Debug.Log("GameOver");

            // disable player movement
            FindObjectOfType<UserInput>().OnDisable();
            FindObjectOfType<UserInput>().ClearInput();
            FindObjectOfType<PlayerScript>().DisableAnimation();

            //disable timer
            FindObjectOfType<GameDataManager>().EndTimer();

            // invoke calls the function name and allows some sort of delay before calling it
            //Invoke(nameof(Restart), restartDelay);

            StartCoroutine(GameOverScreen());
        }
            
    }

    private IEnumerator GameOverScreen(){
        yield return new WaitForSeconds(restartDelay);
        gameOverUI.SetActive(true);
            // allow player to use the cursor to select from UI
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
    }

    public void Restart(){
        Debug.Log("Restart");
        isGameOver = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Debug.Log(gameOverUI);

        FindObjectOfType<UserInput>().OnEnable();
        FindObjectOfType<PlayerScript>().EnableAnimation();
    }

    public void MainMenu(){
        //SceneManager.LoadScene("MainMenu");
    }

    public void Quit(){
        Application.Quit();
    }
}
