using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public TilemapCollider2D door;
    public List<GameObject> enemies;

    private bool m_PlayerInRoom;

    private void Update() 
    {
        if(m_PlayerInRoom && enemies.Count > 0) 
        {
            for(int i = 0; i < enemies.Count; i++) 
            {
                if(enemies[i] != null) 
                {
                    break;
                }
                
                if(i == enemies.Count - 1) 
                {
                    //TODO: Display message that room is clear
                    door.isTrigger = true;
                    m_PlayerInRoom = false;
                    enemies.Clear();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) 
        {
            if(!m_PlayerInRoom) 
            {
                door.isTrigger = false;
                foreach(var enemy in enemies) 
                {
                    enemy.GetComponent<Enemy>().enabled = true;
                }
                m_PlayerInRoom = true;
            }
        }
    }
}
