using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
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

    // Start is called before the first frame update
    void Start()
    {
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

            FindObjectOfType<AudioManager>().Play("PlayerHurt");

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
    private void UpdateHearts(){
        // Clear existing hearts
        foreach (Transform child in heartsParent){
            Destroy(child.gameObject);
        }

        // Instantiate hearts based on current health
        for (int i = 0; i < health; i++){
            GameObject newHeart = Instantiate(heartPrefab, heartsParent);
            // Position each heart horizontally next to the previous one
            newHeart.transform.localPosition = new Vector2(i * 20, 0);     // Adjust the x value to change spacing
        }
    } 
}
