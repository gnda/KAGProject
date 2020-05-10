using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour{
    [SerializeField] private int fuel;
    [SerializeField] private int point;
    [SerializeField] private int m_lifes = 1;

    public int Fuel { get => fuel; set => fuel = value; }
    public int Point { get => point; set => point = value; }
    public int Lifes { get => m_lifes; set => m_lifes = value; }

    /*private void OnTriggerEnter2D(Collider2D other){
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        
        if (bullet != null){
            Vector3 startPos = other.transform.position;

            Vector3 endPos = startPos + (Vector3)bullet.GetComponent<Rigidbody2D>().velocity.normalized;

            Player player = bullet.Origin.GetComponentInParent<Player>();
            Drone drone = bullet.Origin.GetComponentInParent<Drone>();
            if (player != null){
                player.Score += this.Point;
                drone.Fuel += this.Fuel;
                Destroy(gameObject);
            }
            //C_mainSlice sliceManager = FindObjectOfType<C_mainSlice>();

            //sliceManager.m_SYSLine(startPos, endPos, endPos.normalized);
        }
        Player a_player = other.gameObject.GetComponentInParent<Player>();
        Drone a_drone = other.gameObject.GetComponentInParent<Drone>();
        if (a_player != null)
        {
            a_player.Score += this.Point;
            a_player.addLife(Lifes);
            a_drone.Fuel += this.Fuel;
            Destroy(gameObject);
        }
    }*/
}
