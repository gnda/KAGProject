using UnityEngine;

public class ReducePlayer : MonoBehaviour
{
    [SerializeField] private int m_reduceLifePlayer = 3;
    public int ReduceLifePlayer { get => m_reduceLifePlayer; set => m_reduceLifePlayer = value; }
    
    //private void OnCollisionEnter(Collision2D other)
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            player.reduceLife(ReduceLifePlayer);
        }

    }
}
