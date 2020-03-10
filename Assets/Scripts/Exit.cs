using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class Exit : MonoBehaviour
{
    // When collision between these object and Player
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.gameObject.GetComponentInParent<Player>() != null){
            EventManager.Instance.Raise(new AskToGoToNextLevelEvent());
        }
    }
}
