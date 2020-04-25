using UnityEngine;

public class Explodable : MonoBehaviour
{

    [SerializeField] private GameObject explosion;
    [SerializeField] private int life = 1;
    [SerializeField] private int currentLife = 1;

    public GameObject Explosion { get => explosion; set => explosion = value; }
    public int Life { get => life; set => life = value; }
    public int CurrentLife { get => currentLife; set => currentLife = value; }

    private void Explose()
    {
        GameObject explosionGo = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explosionGo, 2);
    }

    private void OnTriggerEnter2D(Collider2D other){
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null){
            Vector3 startPos = other.transform.position;
            Vector3 endPos = startPos + (Vector3)bullet.GetComponent<Rigidbody2D>().velocity.normalized;
            Player player = bullet.Origin.GetComponentInParent<Player>();
            if (player != null){
                if (CurrentLife > 0){
                    CurrentLife = CurrentLife - 1;
                    if (CurrentLife == 0){
                        Explose();
                        Debug.Log("explosion");
                    }
                }
            }
        }
    }
}