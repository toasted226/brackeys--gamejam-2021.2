using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingPushPin : MonoBehaviour
{
    public Transform target;
    public GameObject FX;
    public float damage;
    public float speed;
    public float rotateSpeed;

    private Rigidbody2D m_Rigidbody;

    private void Start() 
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() 
    {
        Vector2 direction = (Vector2)target.position - m_Rigidbody.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        m_Rigidbody.angularVelocity = -rotateAmount * rotateSpeed;

        m_Rigidbody.velocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.collider.CompareTag("Player")) 
        {
            other.transform.GetComponent<PlayerController>().TakeDamage(Mathf.RoundToInt(damage));
        }
        else if(other.collider.CompareTag("Enemy")) 
        {
            other.transform.GetComponent<Enemy>().TakeDamage(damage);
        }

        Instantiate(FX, other.contacts[0].point, Quaternion.identity);
        CameraShake cs = Camera.main.GetComponent<CameraShake>();
        cs.StartCoroutine(cs.Shake(0.25f, 0.3f));
        Destroy(gameObject);
    }
}
