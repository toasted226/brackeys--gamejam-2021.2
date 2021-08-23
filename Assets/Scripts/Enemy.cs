using System.Collections;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public LayerMask layer;
    public Transform enemyGFX;
    public Animator anim;
    public GameObject bullet;
    public Transform barrel;

    public float maxDistance;
    public float avoidDistance;
    public float range;

    public float shootAnimTime;
    public float attackDamage;
    public float fireRate;

    public float health;

    private float m_MaxHealth;
    private Transform m_Target;
    private AIDestinationSetter m_Finder;
    private float m_DefaultScale;
    private bool m_CanShoot = true;
    private float m_TimeBetweenShots;

    private void Start() 
    {
        //Initialisation
        m_Finder = GetComponent<AIDestinationSetter>();
        m_DefaultScale = enemyGFX.localScale.x;
        target = new GameObject().transform;
        m_Finder.target = target;
        m_MaxHealth = health;
        m_TimeBetweenShots = 1f / fireRate;
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

        //Try shoot the player
        Vector2 aimDir = player.position - barrel.position;
        RaycastHit2D checker = Physics2D.Raycast(barrel.position, aimDir, range, layer);
                
        if(checker.collider.CompareTag("Player")) 
        {
            if(m_CanShoot) 
            {
                StartCoroutine(Shoot());
            }
        }
    }

    private IEnumerator Shoot() 
    {
        m_CanShoot = false;

        //Calculate angle of shot
        Vector2 direction = player.position - barrel.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;

        //Play animation and wait until it's finished to shoot
        anim.SetTrigger("shoot");
        yield return new WaitForSeconds(shootAnimTime);

        //Spawn a bullet
        GameObject b = Instantiate(bullet, barrel.position, Quaternion.identity);
        b.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        b.GetComponent<Bullet>().damage = attackDamage;

        yield return new WaitForSeconds(m_TimeBetweenShots);
        m_CanShoot = true;
    }

    public void TakeDamage(float damage) 
    {
        anim.SetTrigger("takeDamage");
        health -= damage;
    }
}
