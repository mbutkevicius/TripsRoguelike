using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripDeathAnimation : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] private float deathBounceStrength;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = Vector2.up * deathBounceStrength;
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
