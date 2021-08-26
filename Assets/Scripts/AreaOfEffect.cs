using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    public float targetSize;
    public float growthMultiplier;
    public float acceleration;
    public float growthBoundary;

    private float m_StartingSize;
    private Animator m_Animator;
    private CircleCollider2D m_Collider;
    private bool m_IsExpanding = true;

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

            if(targetSize - transform.localScale.x <= growthBoundary) 
            {
                m_IsExpanding = false;
                m_Animator.SetTrigger("explode");
            }
        }
    }
}
