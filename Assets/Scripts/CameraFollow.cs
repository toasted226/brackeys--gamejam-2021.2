using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothness;

    private void FixedUpdate() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 followPos = new Vector3
            (
                (player.position.x + mousePos.x) / 2, 
                (player.position.y + mousePos.y) / 2,
                -10f
            ) + offset;

        Vector3 adjustedPos = Vector3.Lerp(transform.position, followPos, smoothness);
        transform.position = adjustedPos;
    }
}
