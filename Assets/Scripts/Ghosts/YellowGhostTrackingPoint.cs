using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowGhostTrackingPoint : MonoBehaviour
{
    [SerializeField] private Transform player;

    public void TrackPlayer()
    {
        transform.position = player.transform.position;
    }
}
