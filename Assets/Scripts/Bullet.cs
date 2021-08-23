using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float travelSpeed;
    public float lifetime;
    public Animator bulletAnim;
    public float explodeAnimTime;
    [HideInInspector]public float damage;

    private bool m_IsActive = true;

    private void Start() 
    {
        StartCoroutine(Delete());
    }

    private void Update()
    {
        //Constantly move bullet if active
        if(m_IsActive) 
        {
            transform.Translate(Vector2.up * travelSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Delete() 
    {
        //Destroy the bullet after a certain amount of time
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        //TODO: check if is enemy or player, then call TakeDamage() for that object
        StartCoroutine(Explode());
    }

    private IEnumerator Explode() 
    {
        m_IsActive = false;
        bulletAnim.SetTrigger("explode");
        GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(explodeAnimTime);
        Destroy(gameObject);
    }
}
