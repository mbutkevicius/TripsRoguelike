using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WhiteGhostTrackingPoint;

public class TriggerZoneManager : MonoBehaviour
{
    public WhiteGhostTrackingPoint trackingPoint;

    private void Start()
    {
        transform.position = Vector3.zero;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        targets target = GetTargetFromCollider(other);
        if (target != null)
        {
            target.SetValid(true);
            Debug.Log($"{target.transform.name} entered the trigger zone and is now valid.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        targets target = GetTargetFromCollider(other);
        if (target != null)
        {
            target.SetValid(false);
            Debug.Log($"{target.transform.name} exited the trigger zone and is now invalid.");
        }
    }

    targets GetTargetFromCollider(Collider2D collider)
    {
        foreach (var target in trackingPoint.targetList)
        {
            if (target.collider == collider)
            {
                return target;
            }
        }
        return null;
    }
}
