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

    public void SpawnHearts() 
    {
        for(int i = 0; i < health; i++) 
        {
            GameObject h = Instantiate(heart, Vector3.zero, Quaternion.identity);
            h.transform.SetParent(transform);
            RectTransform pos = h.GetComponent<RectTransform>();
            pos.localPosition = new Vector3
                (
                    startingX + 250f + (i * offset),
                    height - 50f, 
                    0f
                );
            hearts.Add(h.GetComponent<Heart>());
        }
    }

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
