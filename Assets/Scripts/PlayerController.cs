using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Transform hand;
    public Transform barrel;
    public GameObject bullet;
    public float moveSpeed;
    public float attackDamage;
    public float fireRate;
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

        //Allow the player to look around with the mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dirToMouse = mousePos - hand.position;
        float lookAngle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg;
        hand.localEulerAngles = new Vector3(0f, 0f, lookAngle - 90f);

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
        //Create instance of bullet and set its damage
        GameObject b = Instantiate(bullet, barrel.position, hand.transform.rotation);
        b.GetComponent<Bullet>().damage = attackDamage;

        //Disable shooting temporarily until canShoot, so a feeling of fire rate is achieved
        m_CanShoot = false;
        StartCoroutine(WaitUntilCanShoot());
    }

    private IEnumerator WaitUntilCanShoot() 
    {
        //Wait the time between shots then allow shooting again
        yield return new WaitForSeconds(m_TimeBetweenShots);
        m_CanShoot = true;
    }
}
