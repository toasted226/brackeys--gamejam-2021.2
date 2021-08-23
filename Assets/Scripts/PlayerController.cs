using System.Collections;
using UnityEngine;

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
    [Header("Attack")]
    public float moveSpeed;
    public float attackDamage;
    public float timeBeforeReload;
    public float fireRate;
    public int numOfBullets;
    public float spreadAngle;
    [Header("Health")]
    public float health;

    private Vector2 m_Movement;
    private Rigidbody2D m_Rigidbody;
    private float m_TimeBetweenShots;
    private float m_MaxHealth;
    private bool m_CanShoot = true;

    private void Start() 
    {
        //Initialisation
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_TimeBetweenShots = 1f / fireRate; //Calculate time between shots from fire rate
        m_MaxHealth = health;
    }

    private void Update() 
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

        //Disable shooting temporarily until canShoot, so a feeling of fire rate is achieved
        m_CanShoot = false;
        StartCoroutine(WaitUntilCanShoot());
    }

    private IEnumerator WaitUntilCanShoot() 
    {
        //Wait the time between shots then allow shooting again
        yield return new WaitForSeconds(timeBeforeReload);
        gunAnim.SetTrigger("reload");
        yield return new WaitForSeconds(m_TimeBetweenShots);
        m_CanShoot = true;
    }
}
