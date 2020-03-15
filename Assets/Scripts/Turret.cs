using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject m_ammunitionPrefab;
    [SerializeField] private float m_shootDuration = 1f;
    
    private bool m_isShooting = false;

    public bool IsShooting => m_isShooting;
    
    public IEnumerator ShootCoroutine(Vector2 targetPos)
    {
        float timePerc = 0f;
        Vector2 startPos = transform.position;
        GameObject ammunitionGO = Instantiate(m_ammunitionPrefab);

        m_isShooting = true;
        
        while (timePerc < m_shootDuration)
        {
            timePerc += 1f;
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, timePerc / m_shootDuration);
            ammunitionGO.transform.position = currentPos;

            yield return null;
        }

        m_isShooting = false;

        ammunitionGO.transform.position = targetPos;
        
        Destroy(ammunitionGO);
    }
}
