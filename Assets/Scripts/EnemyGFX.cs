using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    public AIPath aiPath;

    private void Update() 
    {
        //Flip the gfx of the enemy if it's moving left or right
        if(aiPath.desiredVelocity.x >= 0.01f) 
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(aiPath.desiredVelocity.x <= -0.01f) 
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
