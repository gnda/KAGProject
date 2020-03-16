using UnityEngine;

public class Sliceable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            Vector3 startPos = other.transform.position;
            
            Vector3 endPos = startPos + (Vector3) bullet.GetComponent<Rigidbody2D>().velocity.normalized;

            C_mainSlice sliceManager = FindObjectOfType<C_mainSlice>();
            
            sliceManager.m_SYSLine(startPos, endPos, endPos.normalized);
        }
    }
}