using UnityEngine;

public class Menu : MonoBehaviour
{
    private PlayerController m_Player;

    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_Player.enabled = false;
    }

    public void Play() 
    {
        m_Player.enabled = true;
        gameObject.SetActive(false);
    }
}
