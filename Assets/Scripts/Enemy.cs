using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float approachDistance;

    private Transform m_Target;
    private AIDestinationSetter m_Finder;

    private void Start() 
    {
        //Initialisation
        m_Finder = GetComponent<AIDestinationSetter>();
    }
}
