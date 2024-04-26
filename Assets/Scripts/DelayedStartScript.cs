using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class DelayedStartScript : MonoBehaviour
{
    [Tooltip("Determines amount of delay before game will start")]
    [SerializeField] private float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGameAfterDelay(delayTime));
    }

    IEnumerator StartGameAfterDelay(float delayTime)
    {
        Time.timeScale = 0;
        float pauseTime = Time.realtimeSinceStartup + delayTime;
        while (Time.realtimeSinceStartup < pauseTime){
            yield return 0;
        }
        Time.timeScale = 1;
    }
}
