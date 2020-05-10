using UnityEngine;

public class Explodable : MonoBehaviour
{

    [SerializeField] private GameObject explosion;
    [SerializeField] private int life = 1;

    public int Life { get => life;
        set
        {
            life = value;
            
            if (life == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        GameObject explosionGo = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explosionGo, 1);
    }
}