using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSparkleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySparkle());
    }

    IEnumerator DestroySparkle()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
