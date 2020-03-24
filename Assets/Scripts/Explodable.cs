using UnityEngine;

public class Explodable : MonoBehaviour
{

    [SerializeField] private GameObject explosion;
    [SerializeField] private int fuel;
    [SerializeField] private int point;

    public GameObject Explosion { get => explosion; set => explosion = value; }
    public int Fuel { get => fuel; set => fuel = value; }
    public int Point { get => point; set => point = value; }

    private void Explose()
    {
        GameObject explosionGo = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explosionGo, 2);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            Vector3 startPos = other.transform.position;

            Vector3 endPos = startPos + (Vector3)bullet.GetComponent<Rigidbody2D>().velocity.normalized;

            Player player = bullet.Origin.GetComponentInParent<Player>();

            if (player != null)
            {
                player.Score += this.Point;

                Explose();
                Destroy(gameObject);
            }
            //C_mainSlice sliceManager = FindObjectOfType<C_mainSlice>();

            //sliceManager.m_SYSLine(startPos, endPos, endPos.normalized);
        }
    }
}