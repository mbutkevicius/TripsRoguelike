using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using NavMeshPlus.Extensions;
using Unity.VisualScripting;

public class RedGhost : MonoBehaviour
{
    [SerializeField] public Transform target;
    private NavMeshAgent agent;

    public float chaseSpeed = 5f;
    public float slipperyFactor = 0.5f; // A factor to make it feel slippery

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //agent.speed = chaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
        //agent.velocity *= slipperyFactor;
    }
}
