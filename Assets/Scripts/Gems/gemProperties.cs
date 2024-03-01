using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class gemProperties : MonoBehaviour
{
    public LogicManager LogicManager;
    public float GemLifespan;
    public int ScoreToAdd;
    public GameObject GemSparkle;
    public SpriteRenderer gemSprite;
    public float gemFlickerLength = 0.1f;

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
            Instantiate(GemSparkle, new Vector3 (transform.position.x, transform.position.y), Quaternion.identity);
            Destroy(gameObject);
        }
    }



    IEnumerator DespawnGem()
    {
        StartCoroutine(SpriteFlicker());
        yield return new WaitForSeconds(GemLifespan);
        Destroy (gameObject);
    }

    IEnumerator SpriteFlicker()
    {
        yield return new WaitForSeconds(GemLifespan - 2);
        while (true)
        {
            gemSprite.enabled = false;
            yield return new WaitForSeconds(gemFlickerLength);
            gemSprite.enabled = true;
            yield return new WaitForSeconds(gemFlickerLength);
        }
    }
}
