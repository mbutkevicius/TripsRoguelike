using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GemProperties : MonoBehaviour
{
    public GameDataManager gameDataManager;

    public float GemLifespan;
    public int ScoreToAdd;
    public GameObject GemSparkle;
    public SpriteRenderer gemSprite;
    private float gemFlickerLengthSlow = 0.125f;
    private float gemFlickerLengthFast = 0.05f;

    public GameObject hundredScoreEffect;
    public GameObject threeHundredScoreEffect;
    public GameObject thousandScoreEffect;
    private float scoreEffectHeightOffset = 1.5f;

    public GameObject shatterEffect;
    public GameObject spawnEffect;

    // Start is called before the first frame update
    void Start()
    {
        // Find the game data manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();

        Instantiate(spawnEffect, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

        StartCoroutine(DespawnGem());
    }

    // Update is called once per frame
    void Update()
    {
    
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect if gem collides with player
        if (collision.gameObject.layer == 8)
        {
            if (ScoreToAdd == 100)
            {
                Instantiate(hundredScoreEffect, new Vector3(transform.position.x, transform.position.y + scoreEffectHeightOffset), Quaternion.identity);
            }
            if (ScoreToAdd == 300)
            {
                Instantiate(threeHundredScoreEffect, new Vector3(transform.position.x, transform.position.y + scoreEffectHeightOffset), Quaternion.identity);
            }
            if (ScoreToAdd == 1000)
            {
                Instantiate(thousandScoreEffect, new Vector3(transform.position.x, transform.position.y + scoreEffectHeightOffset), Quaternion.identity);
            }
            gameDataManager.score += ScoreToAdd;
            gameDataManager.scoreText.text = gameDataManager.score.ToString();
            Instantiate(GemSparkle, new Vector3 (transform.position.x, transform.position.y), Quaternion.identity);
            Destroy(gameObject);
        }
    }



    IEnumerator DespawnGem()
    {
        StartCoroutine(SpriteFlickerSlow());
        StartCoroutine(SpriteFlickerFast());
        yield return new WaitForSeconds(GemLifespan);
        Instantiate(shatterEffect, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
        Destroy (gameObject);
    }


    private bool fastFlicker = false;
    IEnumerator SpriteFlickerSlow()
    {
        yield return new WaitForSeconds(GemLifespan - 1.5f);
        while (fastFlicker == false)
        {
            gemSprite.enabled = false;
            yield return new WaitForSeconds(gemFlickerLengthSlow);
            gemSprite.enabled = true;
            yield return new WaitForSeconds(gemFlickerLengthSlow);
        }
        while (fastFlicker == true)
        {
            gemSprite.enabled = false;
            yield return new WaitForSeconds(gemFlickerLengthFast);
            gemSprite.enabled = true;
            yield return new WaitForSeconds(gemFlickerLengthFast);
        }
    }

    IEnumerator SpriteFlickerFast()
    {
        yield return new WaitForSeconds(GemLifespan - 0.5f);
        fastFlicker = true;

    }
}
