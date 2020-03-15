using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 1.0f;
    
    public IEnumerator RotationCoroutine(Vector2 endPos)
    {
        endPos += (Vector2) transform.position;
        Vector2 dir = endPos - (Vector2) transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.identity;
        Quaternion qAngle = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion qEnd = Quaternion.Euler(qAngle.eulerAngles.x,
            qAngle.eulerAngles.y, qAngle.eulerAngles.z - 90);

        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            float elapsedTimePerc = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(
                transform.rotation, qEnd, elapsedTimePerc);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = qEnd;
    }
}
