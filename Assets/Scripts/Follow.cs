using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform targetTransform;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetTransform != null)
            transform.position = targetTransform.position;
    }
}
