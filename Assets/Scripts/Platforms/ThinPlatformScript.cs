using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThinPlatformScript : MonoBehaviour
{
    private BoxCollider2D coll;
    private PlayerScript player;
    [Tooltip("controls the distance analog stick has to be pressed down before falling through")]
    [SerializeField] private float deadzone = -0.4f;
    private bool delayActive = false;

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
        // check if player is pressing down 
        if (player.yMoveInput < deadzone && player.IsGrounded())
        {
            player.coyoteTimeCounter = 0f;
            coll.enabled = false;

            if (delayActive == false)
            {
                StartCoroutine(EnableColliderAfterDelay(0.15f));
            }
        }
        else if (delayActive == false)
        {
            coll.enabled = true;
            StopCoroutine(EnableColliderAfterDelay(0));
        }

        if (player.yMoveInput >= deadzone && player.IsGrounded()! && delayActive == false)
        {
            coll.enabled = true;
            StopCoroutine(EnableColliderAfterDelay(0));
        }
    }

    // create short delay to allow player to drop through platform
    private IEnumerator EnableColliderAfterDelay(float delay){
        delayActive = true;
        yield return new WaitForSeconds(delay);
        delayActive = false;
    }
}
