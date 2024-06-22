using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePadScript : MonoBehaviour
{
    public BoxCollider2D platformCollider;
    public Animator animator;
    public GameObject bounceEffect;

    void Start()
    {
        if (platformCollider == null)
        {
            Debug.LogError("Platform Collider is missing!");
        }
    }

    IEnumerator Delay(Collider2D other)
    {
        yield return new WaitForSeconds(0.01f);
        other.GetComponent<PlayerScript>().bouncing = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        // Disable collision with player when entering from below
        if (other.CompareTag("Player"))
        {
            animator.Play("BouncePad_Hit", -1, 0f);
            Instantiate(bounceEffect, new Vector3(transform.position.x, transform.position.y + 1), Quaternion.identity);
            StartCoroutine(Delay());

            Debug.Log("Touched sprite");

            // bounce player
            other.GetComponent<PlayerScript>().BounceJump();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Enable collision with player when leaving the platform
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(other, platformCollider, false);
            StartCoroutine(Delay(other));
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.33f);
        animator.Play("BouncePad_Idle", -1, 0f);
    }
}
