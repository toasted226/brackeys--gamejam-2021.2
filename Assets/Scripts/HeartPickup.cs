using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public float amplitude;
    public float speed;

    private void Update() 
    {
        float y = amplitude * Mathf.Sin(speed * Time.timeSinceLevelLoad) * Time.deltaTime;
        transform.position = new Vector3
        (
            transform.position.x, 
            transform.position.y + y,
            transform.position.z
        );
    }
}
