using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossRoom : MonoBehaviour
{
    public TilemapCollider2D doors;
    public Transform boss;
    public Transform player;
    public GameObject healthbar;

    private CameraFollow m_CameraFollow;
    private AudioSource m_AudioSource;
    private bool m_PlayedCinematic;

    private void Start() 
    {
        m_CameraFollow = Camera.main.transform.GetComponentInParent<CameraFollow>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player")) 
        {
            //Why do I hear boss music?
            m_AudioSource.Play();
            StartCoroutine(EntranceCinematic());
        }
    }

    private IEnumerator EntranceCinematic() 
    {
        if(!m_PlayedCinematic) 
        {
            doors.isTrigger = false;
            m_CameraFollow.player = boss;
            Camera.main.orthographicSize = 4;

            yield return new WaitForSeconds(3f);

            boss.GetComponent<Boss>().enabled = true;

            yield return new WaitForSeconds(2f);

            Camera.main.orthographicSize = 6;
            m_CameraFollow.player = player;

            healthbar.SetActive(true);

            m_PlayedCinematic = true;
        }
    }
}
