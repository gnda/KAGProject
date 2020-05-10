using UnityEngine;

public class Rotatable : MonoBehaviour
{
    public void Rotate(Vector2 endPos)
    {
        SmoothRotation smr = GetComponent<SmoothRotation>();
        if (!smr.IsRotating)
        {
            StartCoroutine(smr.RotationCoroutine(endPos));
        }
    }
}