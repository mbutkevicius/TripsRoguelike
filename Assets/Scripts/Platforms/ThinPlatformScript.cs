using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThinPlatformScript : MonoBehaviour
{
    private BoxCollider2D coll;
    private PlayerScript player;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();

        // need to get the player GameObject before you can assign the PlayerScript to it
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null){
            player = p.GetComponent<PlayerScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is touching platform and pressing down 
        if (player.IsGrounded() && player.yMoveInput < 0){
            coll.enabled = false;
            StartCoroutine(EnableColliderAfterDelay(0.5f));
        }
    }

    // create short delay to allow player to drop through platform
    private IEnumerator EnableColliderAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        coll.enabled = true;
    }
}
