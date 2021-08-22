using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Transform hand;

    private Vector2 m_Movement;
    private Rigidbody2D m_Rigidbody;

    private void Start() 
    {
        //Initialisation
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() 
    {
        //Get input as movement vector
        m_Movement = new Vector2(
            Input.GetAxis("Horizontal"), 
            Input.GetAxis("Vertical")
            );

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dirToMouse = mousePos - hand.position;
        float lookAngle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg;
        hand.localEulerAngles = new Vector3(0f, 0f, lookAngle - 90f);
    }

    private void FixedUpdate() 
    {
        //Apply movement
        Vector2 temp = m_Movement * moveSpeed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + temp);
    }
}
