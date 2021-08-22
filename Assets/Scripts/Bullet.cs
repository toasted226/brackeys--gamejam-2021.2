using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float travelSpeed;
    public float lifetime;
    [HideInInspector]public float damage;

    private void Start() 
    {
        StartCoroutine(Delete());
    }

    private void Update()
    {
        //Constantly move bullet
        transform.Translate(Vector2.up * travelSpeed * Time.deltaTime);
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
        Destroy(gameObject);
    }
}
