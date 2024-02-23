using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gemProperties : MonoBehaviour
{

    public float gemLifespan = 3;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GemLifespan());
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    IEnumerator GemLifespan()
    {
        yield return new WaitForSeconds(gemLifespan);
        Destroy (gameObject);
    }
}
