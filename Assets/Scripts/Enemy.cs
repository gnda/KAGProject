using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int point;


    public int Point { get => point; set => point = value; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();


        if (bullet != null)
        {
            Vector3 startPos = other.transform.position;
            Vector3 endPos = startPos + (Vector3)bullet.GetComponent<Rigidbody2D>().velocity.normalized;

            Player playerOfBullet = bullet.Origin.GetComponentInParent<Player>();
            
            Drone drone = bullet.Origin.GetComponentInParent<Drone>();
            if (playerOfBullet != null)
            {
                if (gameObject.GetComponent<Explodable>().CurrentLife == 0)
                {
                    playerOfBullet.Score += this.Point;
                    Destroy(gameObject);

                }
            }
        }
    }
    /**
    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            Vector3 startPos = other.transform.position;

            Vector3 endPos = startPos + (Vector3)bullet.GetComponent<Rigidbody2D>().velocity.normalized;

            Player player = bullet.Origin.GetComponentInParent<Player>();
            Drone drone = bullet.Origin.GetComponentInParent<Drone>();
            if (player != null)
            {
                player.Score += this.Point;
                //Explose();
                Destroy(gameObject);
            }
        }
    }**/
}