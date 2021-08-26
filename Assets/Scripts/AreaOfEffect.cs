using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    public float targetSize;
    public float growthMultiplier;
    public float shrinkMultiplier;
    public float acceleration;
    public float boundary;

    private float m_StartingSize;
    private Animator m_Animator;
    private CircleCollider2D m_Collider;
    private bool m_IsExpanding = true;
    private bool m_IsShrinking;

    private void Start() 
    {
        m_StartingSize = transform.localScale.x;
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<CircleCollider2D>();
    }

    private void Update() 
    {
        if(m_IsExpanding) 
        {
            growthMultiplier += Time.deltaTime * acceleration;
            float change = transform.localScale.x + Time.deltaTime * growthMultiplier;
            transform.localScale = new Vector3(change, change, transform.localScale.z);

            if(targetSize - transform.localScale.x <= boundary) 
            {
                m_IsExpanding = false;
                m_Collider.enabled = true;
                m_Animator.SetTrigger("explode");
            }
        }

        if(m_IsShrinking) 
        {
            shrinkMultiplier += Time.deltaTime * acceleration;
            float change = transform.localScale.x - Time.deltaTime * shrinkMultiplier;
            transform.localScale = new Vector3(change, change, transform.localScale.z);

            if(transform.localScale.x <= boundary) 
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player")) 
        {
            other.GetComponent<PlayerController>().TakeDamage(1);
        }
    }

    public void StartShrink() 
    {
        m_IsShrinking = true;
    }
}
