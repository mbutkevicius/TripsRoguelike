using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GemSpawner;

public class WhiteGhostTrackingPoint : MonoBehaviour
{
    private System.Random random = new System.Random();

    [System.Serializable]
    public class targets
    {
        public bool isValidTarget;
        public Transform transform;
        public Vector2 direction;
        public BoxCollider2D collider;
        public void SetValid(bool isValid)
        {
            isValidTarget = isValid;
        }
    }

    public List<targets> targetList = new List<targets>();

    public targets selectedTarget;

    public void TrackPlayer(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }

    public void SelectRandomValidTarget()
    {
        List<targets> validTargets = targetList.FindAll(t => t.isValidTarget);

        if (validTargets.Count == 0)
        {
            Debug.LogWarning("No valid targets available!");
            return;
        }

        int index = random.Next(validTargets.Count);
        selectedTarget = validTargets[index];

        TrackPlayer(selectedTarget.transform);
    }

    public Vector3 GetSelectedTargetDirection()
    {
        if (selectedTarget != null)
        {
            return selectedTarget.direction;
        }
        return Vector2.zero;
    }
}
