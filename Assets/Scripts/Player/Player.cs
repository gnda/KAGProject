using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int m_score = 0;

    public int Score
    {
        get => m_score;
        set => m_score = value;
    }
}