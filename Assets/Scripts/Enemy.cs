using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public LayerMask layer;
    public Transform enemyGFX;

    public float maxDistance;
    public float avoidDistance;
    public float range;

    private Transform m_Target;
    private AIDestinationSetter m_Finder;
    private float m_DefaultScale;

    private void Start() 
    {
        //Initialisation
        m_Finder = GetComponent<AIDestinationSetter>();
        m_DefaultScale = enemyGFX.localScale.x;
        target = new GameObject().transform;
        m_Finder.target = target;
    }

    private void Update() 
    {
        float disToPlayer = Vector3.Distance(transform.position, player.position);
        Vector2 direction = player.position - transform.position;

        //Adjust sprite to look towards player
        if(direction.x > 0f) 
        {
            enemyGFX.localScale = new Vector3(-m_DefaultScale, m_DefaultScale, 1f);
        }
        else if(direction.x < 0f)
        {
            enemyGFX.localScale = new Vector3(m_DefaultScale, m_DefaultScale, 1f);
        }
        
        //Check if enemy is in range
        if(disToPlayer <= maxDistance) 
        {
            //Check if enemy is too close
            if(disToPlayer < avoidDistance) 
            {
                target.position = (-direction.normalized) * 5f; //Run away
            }
            else //Stand still if enemy can see player, else run towards them
            {
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, direction, range, layer);
                
                if(hit2D.collider.CompareTag("Player")) 
                {
                    target.position = transform.position;
                }
                else 
                {
                    target.position = player.position;
                }
            }
        }
        else //Run towards player if enemy is too far away
        {
            target.position = player.position;
        }
    }
}
