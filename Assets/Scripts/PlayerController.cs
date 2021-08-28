using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform hand;
    public Transform barrel;
    public Transform gun;
    public Transform playerGFX;
    public GameObject bullet;
    public Animator playerAnim;
    public Animator gunAnim;
    public HealthManager healthManager;
    public Animator deathPanel;
    public AudioClip shotgunShoot;
    public AudioClip shotgunReload;
    [Header("Attack")]
    public float moveSpeed;
    public float attackDamage;
    public float timeBeforeReload;
    public float fireRate;
    public int numOfBullets;
    public float spreadAngle;
    [Header("Health")]
    public int health;
    public float invulnerableTime;

    [HideInInspector]public Vector2 m_Movement;
    private Rigidbody2D m_Rigidbody;
    private AudioSource m_AudioSource;
    private CameraShake m_CameraShake;
    private float m_TimeBetweenShots;
    private int m_MaxHealth;
    private bool m_CanShoot = true;
    private bool m_CanTakeDamage = true;
    private bool m_IsAlive = true;

    private void Start() 
    {
        //Initialisation
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_AudioSource = GetComponent<AudioSource>();
        m_CameraShake = Camera.main.GetComponent<CameraShake>();
        m_TimeBetweenShots = 1f / fireRate; //Calculate time between shots from fire rate
        m_MaxHealth = health;
        healthManager.health = m_MaxHealth;
        healthManager.SpawnHearts();
        healthManager.UpdateHealth();
    }

    private void Update() 
    {
        if(health > 0) 
        {
            //Get input as movement vector
            m_Movement = new Vector2(
                Input.GetAxis("Horizontal"), 
                Input.GetAxis("Vertical")
                );
            
            //Check if the player is moving
            if(Mathf.Abs(m_Movement.magnitude) > 0) 
            {
                playerAnim.SetBool("isWalking", true);
            } 
            else 
            {
                playerAnim.SetBool("isWalking", false);
            }

            //Allow the player to look around with the mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dirToMouse = mousePos - hand.position;
            float lookAngle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg;
            hand.localEulerAngles = new Vector3(0f, 0f, lookAngle - 90f);

            //Check which direction the player is facing
            if(dirToMouse.x > 0) 
            {
                //Facing right
                playerGFX.localScale = new Vector3(1f, 1f, 1f);
                gun.localEulerAngles = new Vector3(0f, 0f, gun.localEulerAngles.z);
            } 
            else if(dirToMouse.x < 0) 
            {
                //Facing left
                playerGFX.localScale = new Vector3(-1f, 1f, 1f);
                gun.localEulerAngles = new Vector3(0f, 180f, gun.localEulerAngles.z);
            }

            //Shoot when player clicks mouse
            if(Input.GetButton("Fire1")) 
            {
                if(m_CanShoot) 
                {
                    Shoot();
                }
            }
        }
        else 
        {
            if(m_IsAlive) 
            {
                m_IsAlive = false;
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die() 
    {
        hand.transform.position = new Vector3
        (
            hand.transform.position.x,
            hand.transform.position.y - 0.25f,
            hand.transform.position.z
        );

        Time.timeScale = 0.4f;
        Camera.main.orthographicSize = 4;
        playerAnim.SetTrigger("die");
        m_Rigidbody.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(1f);
        deathPanel.SetTrigger("fade");
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate() 
    {
        //Apply movement
        Vector2 temp = m_Movement * moveSpeed * Time.fixedDeltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + temp);
    }

    private void Shoot() 
    {
        //Create instance of bullet and set its angle and damage
        for(int i = 1; i < numOfBullets + 1; i++) 
        {
            float angle;
            if(i % 2 == 0) 
                angle = spreadAngle * i;
            else 
                angle = -spreadAngle * i;

            Vector3 rotation = new Vector3
            (
                hand.transform.localEulerAngles.x,
                hand.transform.localEulerAngles.y,
                hand.transform.localEulerAngles.z + angle
            );

            Quaternion q = Quaternion.Euler(rotation);

            GameObject b = Instantiate(bullet, barrel.position, q);
            b.GetComponent<Bullet>().damage = attackDamage;
        }

        //Play sound effect
        m_AudioSource.clip = shotgunShoot;
        m_CameraShake.StartCoroutine(m_CameraShake.Shake(0.2f, 0.1f));
        m_AudioSource.Play();

        //Disable shooting temporarily until canShoot, so a feeling of fire rate is achieved
        m_CanShoot = false;
        StartCoroutine(WaitUntilCanShoot());
    }

    private IEnumerator WaitUntilCanShoot() 
    {
        //Wait the time between shots then allow shooting again
        yield return new WaitForSeconds(timeBeforeReload);
        m_AudioSource.clip = shotgunReload;
        m_AudioSource.Play();
        gunAnim.SetTrigger("reload");
        yield return new WaitForSeconds(m_TimeBetweenShots);
        m_CanShoot = true;
    }

    public void TakeDamage(int damage) 
    {
        if(m_IsAlive) 
        {
            if(m_CanTakeDamage) 
            {
                health -= damage;
                healthManager.health = health;
                healthManager.UpdateHealth();
                playerAnim.SetTrigger("takeDamage");

                m_CanTakeDamage = false;
                Invoke("AllowDamage", invulnerableTime);
            }
        }
    }

    private void AllowDamage() 
    {
        m_CanTakeDamage = true;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Pickup") && health < m_MaxHealth) 
        {
            health += 1;
            healthManager.health = health;
            healthManager.UpdateHealth();
            Destroy(other.gameObject);
        }
    }
}
