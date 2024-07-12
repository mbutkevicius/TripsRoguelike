using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwisePostEvent : MonoBehaviour
{
    public AK.Wwise.Event OnStart;
    public AK.Wwise.Event playEvent;
    // Start is called before the first frame update
    void Start()
    {
        OnStart.Post(gameObject);
    }

    public void playSound()
    {
        playEvent.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
