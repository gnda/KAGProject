using UnityEngine;

public class Collectible : MonoBehaviour, IScore
{
    [SerializeField] public GameObject effect;
    [SerializeField] private int points;
    [SerializeField] private MyAudioClip sound;

    public int Points
    {
        get => points;
        set => points = value;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            if(sound != null)
                SfxManager.Instance.Play(sound);
            player.Score += Points;
            var scoreGo = Instantiate(HudManager.Instance.m_TxtAddPoints, 
                transform.position, Quaternion.identity);
            scoreGo.GetComponentInChildren<TextMesh>().text = "+" + Points;
            Destroy(scoreGo, 0.55f);
            Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}