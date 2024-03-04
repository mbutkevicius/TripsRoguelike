using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDespawnScript1 : MonoBehaviour
{
    public float effectLifespan;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyEffect());
    }

    IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(effectLifespan);
        Destroy(gameObject);
    }
}
