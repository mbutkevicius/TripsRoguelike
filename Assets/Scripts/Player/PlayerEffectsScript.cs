using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerEffectsScript : MonoBehaviour
{
    public GameObject HeadBumpEffect;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHeadBumpEffect()
    {
        Instantiate(HeadBumpEffect, new Vector3(transform.position.x, transform.position.y + 1.5f), Quaternion.identity);
    }
}
