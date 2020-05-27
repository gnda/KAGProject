using UnityEngine;

public class ZulTriggers : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            transform.parent.Find("Zul-1").gameObject.SetActive(false);
            transform.parent.Find("Zul-2").gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null)
        {
            transform.parent.Find("Zul-2").gameObject.SetActive(false);
            transform.parent.Find("Zul-1").gameObject.SetActive(true);
        }
    }
}
