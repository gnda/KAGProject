using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // When collision between these object and Player
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.GetComponent<Player>() != null){
            EventManager.Instance.Raise(new AskToGoToNextLevelEvent());
        }
    }
}
