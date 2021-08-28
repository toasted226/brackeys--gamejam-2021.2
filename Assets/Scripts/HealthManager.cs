using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heart;

    public float height;
    public float startingX;
    public float offset;

    public List<Heart> hearts = new List<Heart>();

    [HideInInspector]public int health;

    public void UpdateHealth() 
    {
        if(health >= 0) 
        {
            for(int i = 0; i < hearts.Count; i++) 
            {
                hearts[i].SetEmpty();
            }

            for(int i = 0; i < hearts.Count; i++) 
            {
                if(i + 1 <= health) 
                {
                    hearts[i].SetFull();
                }
            }
        }
    }
}
