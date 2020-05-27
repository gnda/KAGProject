using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    [SerializeField] private float duration = 1;
    [SerializeField] private MyAudioClip sound;
    // Start is called before the first frame update
    void Start()
    {
        if(sound != null)
            SfxManager.Instance.Play(sound);
        Destroy(gameObject, duration);
    }
}
