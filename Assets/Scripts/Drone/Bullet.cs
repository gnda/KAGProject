using UnityEngine;

namespace Drone
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float duration = 2f;
        private Transform _origin;

        public Transform Origin
        {
            get => _origin;
            set
            {
                _origin = value;
            
                foreach (var col in _origin.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(col, GetComponent<Collider>());
                }
                
                foreach (var col in GameManager.Instance._3DCollisions)
                {
                    Physics.IgnoreCollision(col.GetComponent<Collider>(), GetComponent<Collider>());
                }
            }
        }

        private void Start()
        {
            Destroy(gameObject, duration);
        }

        private void OnCollisionEnter(Collision other)
        {
            var explodable = other.transform.GetComponentInParent<Explodable>();
            if (explodable != null)
            {
                var player = _origin.GetComponent<Player>();
                if (player != null && explodable.Life - 1 == 0)
                {
                    player.Score++;
                }
                
                explodable.Life--;
            }
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            GameObject explosionGo = Instantiate(explosionPrefab, 
                transform.position,  Quaternion.identity);
        
            var main = explosionGo.GetComponent<ParticleSystem>().main;
            main.startColor = GetComponent<SpriteRenderer>().color;
        
            Destroy(explosionGo, 0.5f);
        }
    }
}