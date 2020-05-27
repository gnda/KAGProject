using UnityEngine;

public class BonusVitesseTir : MonoBehaviour
{
    [SerializeField] private float m_vitesse = 0.05f;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.transform.GetComponentInParent<Player>();
        if (player != null)
        {
            player.GetComponentInChildren<Shooter>().shootDelay = m_vitesse;
        }
    }
}
