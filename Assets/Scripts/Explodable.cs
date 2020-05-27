using System;
using System.Collections;
using Drone;
using SDD.Events;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Explodable : MonoBehaviour, IScore
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private int life = 1;
    [SerializeField] private int points;
    [SerializeField] private MyAudioClip soundHurt;
    [SerializeField] private MyAudioClip soundDestroyed;

    public int Points
    {
        get => points;
        set => points = value;
    }

    public int Life { get => life;
        set
        {
            life = value;
            
            if (life == 0)
            {
                var player = GetComponentInParent<Player>();
                if (player != null)
                {
                    EventManager.Instance.Raise(new GameOverEvent());
                }
                if(soundDestroyed != null)
                    SfxManager.Instance.Play(soundDestroyed);
                Explode();
                Destroy(gameObject);
            }
        }
    }

    private void Explode()
    {
        if (GetComponent<Enemy>() != null)
        {
            SpawnBonus();
        }
        GameObject explosionGo = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(explosionGo, 1);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = GetComponentInParent<Player>();
        var bullet = other.transform.GetComponent<Bullet>();

        if (other.transform.CompareTag("Wall") && player != null)
        {
            Life--;
            if(soundHurt != null)
                SfxManager.Instance.Play(soundHurt);
        }
        else if (bullet != null)
        {
            var enemy = GetComponent<Enemy>();
            
            if (enemy != null && Life - 1 == 0)
            {
                player = bullet.Origin.GetComponent<Player>();

                if (player != null)
                {
                    player.Score += Points;
                    var scoreGo = Instantiate(HudManager.Instance.m_TxtAddPoints, 
                        transform.position, Quaternion.identity);
                    scoreGo.GetComponentInChildren<TextMesh>().text = "+" + Points;
                    Destroy(scoreGo, 0.55f);
                }

                var boss = GetComponent<Boss>();
                if (boss != null)
                {
                    EventManager.Instance.Raise(new RevealExitEvent());
                }
            }

            Life--;
            if(soundHurt != null)
                SfxManager.Instance.Play(soundHurt);
        }
        else if (other.transform.GetComponent<Enemy>() != null)
        {
            Life--;
            if(soundHurt != null)
                SfxManager.Instance.Play(soundHurt);
        }
    }

    private IEnumerator Blinking()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            var mb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mb);
            mb.SetVector("Blink", Vector3.right);
            r.SetPropertyBlock(mb);
            yield return new WaitForSeconds(20.0f);
            mb.SetVector("Blink", Vector4.zero);
        }
    }

    private void SpawnBonus()
    {
        var spawnBonusProba = Random.Range(0, 100);

        if (spawnBonusProba > 75)
        {
            var bonuses = GameManager.Instance.BonusPrefabs;
            Instantiate(bonuses[Random.Range(0, bonuses.Length)], 
                transform.position + new Vector3(0,0,0.8f), Quaternion.identity, 
                LevelsManager.Instance.CurrentLevel.transform);
        }
    }
}