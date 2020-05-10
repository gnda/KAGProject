using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public Transform targetTransform;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetTransform != null)
            transform.rotation = targetTransform.rotation;
    }
}