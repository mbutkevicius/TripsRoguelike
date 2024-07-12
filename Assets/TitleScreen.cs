using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public float startDelay;
    public float fadeDelay;

    public Text scoreText;

    [HideInInspector]
    private bool isInputLocked = true;

    public Animator transition;
    public Animator keyPrompt;


    // Start is called before the first frame update
    private AudioManager AudioManager;
    void Start()
    {
        //Get Audio Manager
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !isInputLocked)
        {
            StartCoroutine(PlayGame());
        }
    }

    IEnumerator PlayGame()
    {
        isInputLocked = true;

        transition.SetTrigger("Death");

        //FindObjectOfType<AudioManager>().Stop("TitleScreenMusic");
        AudioManager.toggleMusic();
        //FindObjectOfType<AudioManager>().Play("TitleScreenKeyPress");
        AudioManager.playSoundName("title_keypress", gameObject);

        keyPrompt.SetTrigger("PressedKey");

        yield return new WaitForSeconds(fadeDelay);

        transition.SetTrigger("Out");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1);
    }

    IEnumerator StartDelay()
    {
        //FindObjectOfType<AudioManager>().Play("TitleScreenMusic");
        AudioManager.toggleMusic();

        yield return new WaitForSeconds(startDelay);

        isInputLocked = false;
    }
}
