using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxMotion : MonoBehaviour
{

    public float parallaxMoveSpeed;
    public float parallaxDeadZone;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * parallaxMoveSpeed) * Time.deltaTime;

        if (transform.position.x <= parallaxDeadZone)
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }
}
