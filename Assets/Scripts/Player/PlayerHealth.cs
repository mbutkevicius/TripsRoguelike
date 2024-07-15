using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // update hearts 
    public GameObject heartPrefab;
    public Transform heartsParent;
    public CameraShakeEffect cameraShakeEffect;
    
    // player sprite for blink
    private SpriteRenderer playerSprite;
    
    public int health;
    public int maxHealth = 3;
    public bool isInvincible = false;

    [Header("i-frames")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.075f;

    [Header("Effects")]
    public GameObject damagedEffect;

    public class Heart : MonoBehaviour
    {
        public bool isEmptied = false;
    }

    // Start is called before the first frame update
    private AudioManager AudioManager;
    void Start()
    {
        //Get Audio Manager
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
        // set sprite renderer and health
        playerSprite = GetComponent<SpriteRenderer>();
        health = maxHealth;

        // display health
        UpdateHearts();
    }

    // Update is called once per frame
    void Update()
    {
        if (health == 0)
        {
            playerSprite.enabled = false;
        }
    }

    // method for when player is hurt
    public void TakeDamage(int amount){
        // remove health from player
        if (health > 0)
        {
            health -= amount;

            // shake camera
            StartCoroutine(cameraShakeEffect.CameraShake());

            Instantiate(damagedEffect, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

            //FindObjectOfType<AudioManager>().Play("PlayerHurt");
            AudioManager.playSoundName("trip_hurt", gameObject);

            // indicate player invulnerability time
            StartCoroutine(InvincibilityFrames());
        }

        // check if GameOver condition is met
        if (health <= 0){
            FindObjectOfType<GameOverScript>().GameOver();
        }

        // update hp
        UpdateHearts();
    }

    // Gives player invincibility time
    private IEnumerator InvincibilityFrames(){
        isInvincible = true;
        // flash player sprite to indicate i frames
        StartCoroutine(Blink());

        // wait for specified time
        yield return new WaitForSeconds(invincibilityDuration);

        // disable Blink
        StopCoroutine(Blink());
        isInvincible = false;
    }

    // allows player sprite to quickly appear and disappear
    private IEnumerator Blink(){
        while (isInvincible && health > 0){
            // alternate displaying sprite 
            playerSprite.enabled = !playerSprite.enabled;
            // wait for duration before alternating sprite
            yield return new WaitForSeconds(blinkInterval);
        }
        

        // Enable sprite if left off
        playerSprite.enabled = true;
    }

    // update hearts on UI for player to see
    private void UpdateHearts()
{
    int currentHearts = heartsParent.childCount;
    int heartsToEmpty = maxHealth - health;

    for (int i = currentHearts - 1; i >= 0; i--)
    {
        Heart heart = heartsParent.GetChild(i).GetComponent<Heart>() ?? heartsParent.GetChild(i).gameObject.AddComponent<Heart>();
        Animator heartAnimator = heart.GetComponent<Animator>();

        if (heartAnimator == null) continue;

        bool shouldEmpty = i >= currentHearts - heartsToEmpty;
        if (shouldEmpty && !heart.isEmptied)
        {
            heartAnimator.Play("Heart_Loss", -1, 0f);
            heart.isEmptied = true;
        }
        else if (!shouldEmpty && heart.isEmptied)
        {
            heart.isEmptied = false;
            heartAnimator.Play("Heart_Fill", -1, 0f);
        }
    }

    // Add new hearts if current health is greater than the number of existing hearts
    for (int i = currentHearts; i < health; i++)
    {
        GameObject newHeart = Instantiate(heartPrefab, heartsParent);
        newHeart.transform.localPosition = new Vector2(i * 20, 0);
    }
}
}
