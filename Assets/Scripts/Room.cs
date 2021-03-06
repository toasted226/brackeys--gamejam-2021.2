using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public float cameraSize = 5f;
    public Animator roomClearMessage;
    public TilemapCollider2D door;
    public List<GameObject> enemies;
    
    private List<GameObject> doors = new List<GameObject>();
    private bool m_PlayerInRoom;

    private void Start() 
    {        
        for(int i = 0; i < door.transform.childCount; i++) 
        {
            doors.Add(door.transform.GetChild(i).gameObject);
        }
    }

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
                    door.isTrigger = true;
                    m_PlayerInRoom = false;

                    roomClearMessage.SetTrigger("roomclear");

                    foreach(var doorObj in doors) 
                    {
                        doorObj.gameObject.SetActive(false);
                    }

                    enemies.Clear();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.CompareTag("Player")) 
        {
            Camera.main.orthographicSize = cameraSize;
        }

        if(other.transform.CompareTag("Player") && enemies.Count > 0) 
        {
            if(!m_PlayerInRoom) 
            {
                door.isTrigger = false;

                foreach(var doorObj in doors) 
                {
                    doorObj.gameObject.SetActive(true);
                }

                foreach(var enemy in enemies) 
                {
                    if(enemy.GetComponent<Enemy>() != null) 
                    {
                        enemy.GetComponent<Enemy>().enabled = true;
                    }
                    else 
                    {
                        enemy.GetComponent<HomingPushPin>().enabled = true;
                    }
                }
                m_PlayerInRoom = true;
            }
        }
    }
}
