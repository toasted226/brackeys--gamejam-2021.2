using System.Collections;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LayerMask layer;
    public Transform enemyGFX;
    public Animator anim;
    public GameObject bullet;
    public Transform barrel;
    public GameObject healthPickup;
    public float healthSpawnChance;
    [Header("AI Movement")]
    public float maxDistance;
    public float avoidDistance;
    public float range;
    [Header("Attack")]
    public float shootAnimTime;
    public float attackDamage;
    public float fireRate;
    [Tooltip("Leave at 0 to disable burst fire")]
    public int bulletsPerBurst;
    public float timeBetweenBursts;
    [Header("Life")]
    public float health;
    public float deathAnimTime;

    private float m_MaxHealth;
    private Transform m_Target;
    private AIPath m_Ai;
    private AIDestinationSetter m_Finder;
    private float m_DefaultScale;
    private bool m_CanShoot;
    private bool m_Alive = true;
    private float m_TimeBetweenShots;
    private int m_BulletsFired;
    private bool m_CanBurst;

    private void Start() 
    {
        //Initialisation
        player = GameObject.FindGameObjectWithTag("Player").transform;
        m_Finder = GetComponent<AIDestinationSetter>();
        m_Ai = GetComponent<AIPath>();
        m_DefaultScale = enemyGFX.localScale.x;
        m_Target = new GameObject().transform;
        m_Finder.target = m_Target;
        m_MaxHealth = health;
        m_TimeBetweenShots = 1f / fireRate;

        StartCoroutine(WaitForNextShot());
        StartCoroutine(WaitForNextBurst());
    }

    private void Update() 
    {
        //Check if still alive
        if(health > 0) 
        {
            //Check if enemy is walking
            if(m_Ai.desiredVelocity.magnitude > 0.01f) 
            {
                anim.SetBool("isWalking", true);
            }
            else 
            {
                anim.SetBool("isWalking", false);
            }

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
                    m_Target.position = (-direction.normalized) * 5f; //Run away
                }
                else //Stand still if enemy can see player, else run towards them
                {
                    RaycastHit2D hit2D = Physics2D.Raycast(transform.position, direction, range, layer);
                    
                    if(hit2D) 
                    {
                        if(hit2D.collider.CompareTag("Player")) 
                        {
                            m_Target.position = transform.position;
                        }
                        else 
                        {
                            m_Target.position = player.position;
                        }
                    }
                }
            }
            else //Run towards player if enemy is too far away
            {
                m_Target.position = player.position;
            }

            //Try shoot the player
            Vector2 aimDir = player.position - barrel.position;
            RaycastHit2D checker = Physics2D.Raycast(barrel.position, aimDir, range, layer);

            if(checker) 
            {     
                if(checker.collider.CompareTag("Player")) 
                {
                    if(bulletsPerBurst == 0) 
                    {
                        if(m_CanShoot) 
                        {
                            StartCoroutine(Shoot());
                        }
                    }
                    else 
                    {
                        if(m_CanBurst && m_CanShoot) 
                        {
                            m_BulletsFired++;
                            if(m_BulletsFired <= bulletsPerBurst) 
                            {
                                StartCoroutine(Shoot());
                            }
                            else 
                            {
                                m_CanBurst = false;
                                StartCoroutine(WaitForNextBurst());
                            }
                        }
                    }
                }
            }
        }
        else 
        {
            //Die if not already dead
            if(m_Alive) 
            {
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die()
    {
        m_Alive = false;
        anim.SetTrigger("die");
        //Wait until death animation has played
        yield return new WaitForSeconds(deathAnimTime);

        float c = Random.Range(0f, 100f);
        if(c < healthSpawnChance) 
        {
            GameObject.Instantiate(healthPickup, transform.position, Quaternion.identity);
        }

        //TODO: destroy enemy in some fancy way
        Destroy(m_Target.gameObject);
        Destroy(gameObject);
    }

    private IEnumerator Shoot() 
    {
        if(m_Alive) 
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

            StartCoroutine(WaitForNextShot());
        }
    }

    private IEnumerator WaitForNextShot() 
    {
        yield return new WaitForSeconds(m_TimeBetweenShots);
        m_CanShoot = true;
    }

    private IEnumerator WaitForNextBurst() 
    {
        yield return new WaitForSeconds(timeBetweenBursts);
        m_BulletsFired = 0;
        m_CanBurst = true;
    }

    public void TakeDamage(float damage) 
    {
        if(m_Alive) 
        {
            anim.SetTrigger("takeDamage");
            health -= damage;
        }
    }
}
