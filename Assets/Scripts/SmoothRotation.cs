using System.Collections;
using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 1.0f;
    public bool IsRotating { get; set; }
    
    public IEnumerator RotationCoroutine(Vector2 endPos)
    {
        Vector2 dir = endPos - (Vector2) transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        Quaternion qAngle = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion qEnd = Quaternion.Euler(qAngle.eulerAngles.x,
            qAngle.eulerAngles.y, qAngle.eulerAngles.z - 90);

        float elapsedTime = 0;

        IsRotating = true;

        while (elapsedTime < rotationDuration)
        {
            float elapsedTimePerc = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                qEnd, elapsedTimePerc);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        IsRotating = false;

        transform.rotation = qEnd;
    }
}
