using UnityEngine;

public class ShootingEnemyMovement : MonoBehaviour
{
    private ShootingEnemyData enemy;
    private Transform player;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    void Start()
    {
        enemy = GetComponent<ShootingEnemyData>();
        player = FindObjectOfType<PlayerMovement>()?.transform;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // If the distance to the player is greater than the stopping distance, move towards the player
            if (distanceToPlayer > enemy.stoppingDistance && knockbackDuration <= 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, enemy.currentMoveSpeed * Time.deltaTime);
            }
        }

        if(knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
    }
    
    public Vector3 RetrievePlayerPosition()
    {
        if (player != null)
        {
            return player.position;
        }
        else
        {
            return Vector3.zero;
        }
    }
    
    public void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is greater than 0.
        if(knockbackDuration > 0) return;

        // Begins the knockback.
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}