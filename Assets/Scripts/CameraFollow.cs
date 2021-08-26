using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothness;

    private void FixedUpdate() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 halfPos = new Vector3
            (
                (player.position.x + mousePos.x) / 2f, 
                (player.position.y + mousePos.y) / 2f,
                -10f
            );
        
        Vector3 followPos = new Vector3
            (
                (player.position.x + halfPos.x) / 2f,
                (player.position.y + halfPos.y) / 2f,
                -10f
            );

        Vector3 adjustedPos = Vector3.Lerp(transform.position, followPos, smoothness);
        transform.position = adjustedPos;
    }
}
