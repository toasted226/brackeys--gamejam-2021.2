using System.Collections;
using UnityEngine;
using Pathfinding;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LayerMask layer;
    public Transform enemyGFX;
    public Animator anim;
    public GameObject bullet;
    public Transform barrel;
    public GameObject whiteFlash;
    public GameObject fizzleFX;
    public GameObject deathFX;
    public CameraFollow cam;
    [Header("AI Movement")]
    public float maxDistance;
    public float avoidDistance;
    public float range;
    [Header("Attack")]
    public float shootAnimTime;
    public float attackDamage;
    public float fireRate;
    public int numOfBullets;
    public float spreadAngle;
    public float timeBetweenCircularBursts;
    public int numOfCircularBursts;
    [Tooltip("Leave at 0 to disable burst fire")]
    public int bulletsPerBurst;
    public float timeBetweenBursts;
    [Header("Life")]
    public float health;
    public float whiteFlashTime;
    public float dramaTime;

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
        m_Finder = GetComponent<AIDestinationSetter>();
        m_Ai = GetComponent<AIPath>();

        m_DefaultScale = enemyGFX.localScale.x;
        m_Target = new GameObject().transform;
        m_Finder.target = m_Target;
        m_MaxHealth = health;
        m_TimeBetweenShots = 1f / fireRate;

        StartCoroutine(Transform());
    }

    private void Update() 
    {
        //Check if still alive
        if(health > 0) 
        {
            #region AI Movement
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
            #endregion

            #region AI Shooting
            
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
            #endregion


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

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach(var bullet in bullets) 
        {
            Bullet b = bullet.GetComponent<Bullet>();
            b.StartCoroutine(b.Explode());
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<PolygonCollider2D>().enabled = false;

        whiteFlash.SetActive(true);

        cam.player = transform;
        CameraShake cs = Camera.main.GetComponent<CameraShake>();
        cs.StartCoroutine(cs.Shake(dramaTime + 0.5f, 0.35f));

        Vector3 rotation = new Vector3(90f, 0f, 0f);
        Quaternion q = Quaternion.Euler(rotation);
        Instantiate(fizzleFX, transform.position, q);
        yield return new WaitForSeconds(dramaTime);

        Instantiate(deathFX, transform.position, q);

        Destroy(m_Target.gameObject);
        enemyGFX.GetComponent<SpriteRenderer>().enabled = false;
        whiteFlash.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.5f);
        cam.player = player;

        Destroy(gameObject);
    }

    private IEnumerator Shoot() 
    {
        m_CanShoot = false;

        //Calculate angle of shot
        Vector2 direction = player.position - barrel.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;

        //Play animation and wait until it's finished to shoot
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(shootAnimTime);

        //Spawn a bullet
        GameObject b = Instantiate(bullet, barrel.position, Quaternion.identity);
        b.transform.localEulerAngles = new Vector3(0f, 0f, angle);
        b.GetComponent<Bullet>().damage = attackDamage;

        StartCoroutine(WaitForNextShot());
    }

    private IEnumerator WaitForNextShot() 
    {
        yield return new WaitForSeconds(m_TimeBetweenShots);
        m_CanShoot = true;
    }

    private IEnumerator WaitForNextBurst() 
    {
        for(int i = 0; i < numOfCircularBursts; i++) 
        {
            if(m_Alive) 
            {
                yield return new WaitForSeconds(timeBetweenCircularBursts);
                CircularBurst();
            }
        }

        yield return new WaitForSeconds(timeBetweenBursts);
        m_BulletsFired = 0;
        m_CanBurst = true;
    }

    private IEnumerator WhiteFlash() 
    {
        whiteFlash.SetActive(true);
        yield return new WaitForSeconds(whiteFlashTime);
        if(m_Alive) 
        {
            whiteFlash.SetActive(false);
        }
    }

    private IEnumerator Transform() 
    {
        anim.SetTrigger("transform");
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(WaitForNextShot());
        StartCoroutine(WaitForNextBurst());
    }

    private void CircularBurst()
    {
        anim.SetTrigger("attack");

        //Calculate angle of shot
        Vector2 direction = player.position - barrel.position;
        float dirAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        dirAngle -= 90f;

        for(int i = 1; i < numOfBullets + 1; i++) 
        {
            float angle;
            if(i % 2 == 0) 
                angle = spreadAngle * i;
            else 
                angle = -spreadAngle * i;

            GameObject b = Instantiate(bullet, barrel.position, Quaternion.identity);
            b.GetComponent<Bullet>().damage = attackDamage;
            b.transform.localEulerAngles = new Vector3(0f, 0f, dirAngle + angle);
        }
    }

    public void TakeDamage(float damage) 
    {
        if(m_Alive) 
        {
            health -= damage;
            StartCoroutine(WhiteFlash());
        }
    }
}
