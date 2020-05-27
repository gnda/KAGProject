using UnityEngine;
using VolumetricLines;

namespace Drone
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float duration = 2f;
        [SerializeField] private MyAudioClip sound;
        private Transform _origin;

        public Transform Origin
        {
            get => _origin;
            set
            {
                _origin = value;
            
                foreach (var col in _origin.GetComponents<Collider2D>())
                {
                    Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
                }
                
                foreach (var col in _origin.GetComponentsInChildren<Collider2D>())
                {
                    Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
                }

                if (_origin.GetComponent<Enemy>() != null)
                {
                    foreach (var e in GameManager.Instance.Enemies)
                    {
                        foreach (var col in e.GetComponents<Collider2D>())
                        {
                            Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
                        }
            
                        foreach (var col in e.GetComponentsInChildren<Collider2D>())
                        {
                            Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
                        }
                    }
                    
                    Destroy(GetComponent<CanScratch>());
                }
            }
        }

        private void Start()
        {
            if(sound != null)
                SfxManager.Instance.Play(sound);
            Destroy(gameObject, duration);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Explode();
        }
        
        private void OnCollisionStay2D(Collision2D other)
        {
            Explode();
        }

        public void Explode()
        {
            GameObject explosionGo = Instantiate(explosionPrefab, 
                transform.position,  Quaternion.identity, 
                    LevelsManager.Instance.CurrentLevel.transform);
        
            var main = explosionGo.GetComponent<ParticleSystem>().main;
            main.startColor = GetComponent<VolumetricLineStripBehavior>().LineColor;
        
            Destroy(explosionGo, 0.5f);
            Destroy(gameObject);
        }
    }
}