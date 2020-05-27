using System;
using SDD.Events;
using UnityEngine;

public class Sauerkraut : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            transform.parent.Find("Sauerkraut-1").gameObject.SetActive(false);
            transform.parent.Find("Sauerkraut-2").gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            transform.parent.Find("Sauerkraut-2").gameObject.SetActive(false);
            transform.parent.Find("Sauerkraut-1").gameObject.SetActive(true);
        }
    }
}
