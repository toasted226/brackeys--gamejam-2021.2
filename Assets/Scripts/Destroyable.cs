using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public int objectHealth;
    public GameObject explosionFX;

    private SpriteRenderer m_SpriteRenderer;
    private Material m_DefaultMat;
    private Material m_WhiteFlash;

    private void Start() 
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_DefaultMat = m_SpriteRenderer.material;
        m_WhiteFlash = Resources.Load("WhiteFlash") as Material;
    }

    public void TakeDamage() 
    {
        objectHealth--;
        m_SpriteRenderer.material = m_WhiteFlash;
        
        if(objectHealth <= 0) 
        {
            Instantiate(explosionFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        Invoke("ResetMat", 0.1f);
    }

    private void ResetMat() 
    {
        m_SpriteRenderer.material = m_DefaultMat;
    }
}
