using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using NavMeshPlus.Extensions;
using Unity.VisualScripting;

public class RedGhost : MonoBehaviour
{
    public GameDataManager gameDataManager;

    [SerializeField] public Transform target;
    private NavMeshAgent agent;

    private bool facingRight = true;

    public float chaseSpeed = 5f;
    public float slipperyFactor = 0.5f; // A factor to make it feel slippery

    private float speedTimeMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //agent.speed = chaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        speedTimeMultiplier = gameDataManager.ghostTimeFraction;

        // flip ghost if facing the wrong direction
        if (agent.velocity.x < 0 && facingRight || agent.velocity.x > 0 && !facingRight){
            Flip();
        }

        // chase after player
        agent.SetDestination(target.position);

        agent.speed = chaseSpeed * speedTimeMultiplier;
        agent.acceleration = slipperyFactor * speedTimeMultiplier;
    }

    // animation to flip sprite
    public void Flip(){
        facingRight = !facingRight;
        // flip sprite along the y axis
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
}
