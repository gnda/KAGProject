using System;
using UnityEngine;

public class Zul : MonoBehaviour
{
    [SerializeField] Vector3[] teleportPoints;
    private Explodable ex;
    private int maxLife;
    private int currentPointIndex;
    private bool canTeleport = true;

    void Start()
    {
        ex = GetComponent<Explodable>();
        maxLife = ex.Life;
    }

    // Update is called once per frame
    void Update()
    {
        if (ex.Life < maxLife * 0.6f && canTeleport)
        {
            var messages = LevelsManager.Instance.CurrentLevel
                .GetComponentInChildren<Messages>();
            messages.transform.Find("NextStep").gameObject.SetActive(true);
            transform.position = teleportPoints[currentPointIndex];
            canTeleport = false;
        }
    }
}