using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public SpriteRenderer playerSpriteRenderer;

    public int damage = 1;

    public bool canDoDamage = true;


    private  void OnTriggerStay2D(Collider2D collision){
        // check if player collides with enemy
        if (collision.gameObject.tag == "Player" && canDoDamage){
            // get PlayerHealth script
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // if player health exists and player isn't invincible 
            if (playerHealth && !playerHealth.isInvincible){
                // call player health script to damage player
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
