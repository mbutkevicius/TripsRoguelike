using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private float splashScreenTimer;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SplashScreenTimer());
    }

    public IEnumerator SplashScreenTimer()
    {
        yield return new WaitForSeconds(splashScreenTimer);
        animator.SetTrigger("Out");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
