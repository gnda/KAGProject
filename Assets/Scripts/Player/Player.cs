using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int m_score = 0;
    [SerializeField] private int m_life = 4; // current life
    [SerializeField] private static int m_lifeFull = 6; // max life
    [SerializeField] private int m_lifePoint = 5; // current life point
    [SerializeField] private static int m_lifePointFull = 5; // max life point 

    public int Score
    {
        get => m_score;
        set => m_score = value;
    }

    public int Life
    {
        get => m_life;
        set => m_life = value;
    }

    public int LifeFull
    {
        get => m_lifeFull;
        set => m_lifeFull = value;
    }

    public int LifePoint
    {
        get => m_lifePoint;
        set => m_lifePoint = value;
    }

    public static int LifePointFull
    {
        get => m_lifePointFull;
        set => m_lifePointFull = value;
    }

    public void reduceLife(int p_lifePoint)
    {
        int soustraction = m_lifePoint - p_lifePoint;

        if (soustraction > 0)
        {
            LifePoint = LifePoint - p_lifePoint;
        }
        else
        {
            if (Life > 0)
            {
                Life--;
                LifePoint = LifePointFull;
            }
            else
            {
                Debug.Log("game over");
                //gameover
            }
        }
    }

    public void addLife(int nb_lifes)
    {
        if ((Life + nb_lifes) < LifeFull)
        {
            Life = Life + 2;
            //LifePoint = LifePointFull;
        }
        else if (Life == LifeFull)
        {
            Life = LifeFull;
            LifePoint = LifePointFull;
        }
    }
}