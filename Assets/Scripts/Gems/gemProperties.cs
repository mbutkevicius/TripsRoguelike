using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class gemProperties : MonoBehaviour
{
    public LogicManager LogicManager;

    public float GemLifespan;

    public int ScoreToAdd;

    // Start is called before the first frame update
    void Start()
    {
        LogicManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManager>();

        StartCoroutine(DespawnGem());
    }

    // Update is called once per frame
    void Update()
    {
    
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect if gem collides with player
        if (collision.gameObject.layer == 5)
        {
            LogicManager.Score += ScoreToAdd;
            Destroy(gameObject);
        }
    }



    IEnumerator DespawnGem()
    {
        yield return new WaitForSeconds(GemLifespan);
        Destroy (gameObject);
    }
}
