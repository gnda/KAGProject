using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private int size = 20;
    // Start is called before the first frame update
    void Start()
    {
        cam.orthographicSize = size;
    }

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize = size;    
    }
}
