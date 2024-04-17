using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI;

    public bool gameIsOver = false;

    public float restartDelay = 1.5f;

    void Start(){
        //gameOverUI = GameObject.Find("ScreenCanvas/GameOverScreen");
        //gameOverUI.SetActive(false);

        // uncomment this for final release!!!
        /*
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */

        Debug.Log(gameOverUI);
    }

    void OnLevelWasLoaded(){
        //gameOverUI = GameObject.Find("ScreenCanvas/GameOverScreen");
        Debug.Log(gameOverUI);
        //gameOverUI.SetActive(false);

        // uncomment this for final release!!!
        /*
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        */

        Debug.Log(gameOverUI);
    }

    public void GameOver(){
        if (!gameIsOver){
            gameIsOver = true;
            Debug.Log("GameOver");

            // disable player movement
            FindObjectOfType<UserInput>().OnDisable();
            FindObjectOfType<PlayerScript>().DisableAnimation();

            // invoke calls the function name and allows some sort of delay before calling it
            //Invoke(nameof(Restart), restartDelay);

            StartCoroutine(GameOverScreen());
        }
            
    }

    private IEnumerator GameOverScreen(){
        yield return new WaitForSeconds(restartDelay);
        gameOverUI.SetActive(true);

        // uncomment this for final release!!!
        /*
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */
    }

    public void Restart(){
        Debug.Log("Restart");
        gameIsOver = false;

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
