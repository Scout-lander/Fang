using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;
    Vector2 movementDirection; // Store the movement direction for BallEnemy

    Vector2 knockbackVelocity;
    float knockbackDuration;

    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;

        if (enemy.BallEnemy)
        {
            // Example: Set movementDirection to move towards player initially
            movementDirection = (player.position - transform.position).normalized;
        }
    }

    void Update()
    {
        // If we are currently being knocked back, then process the knockback.
        if (knockbackDuration > 0 && !enemy.BallEnemy)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            // Otherwise, constantly move the enemy towards the player
            MoveTowardsPlayer();
        }

        if (enemy.BallEnemy) // Only for BallEnemy
        {
            // Move the BallEnemy in the initial movement direction if it hasn't moved yet
            if (enemy.BallEnemy)
            {
                MoveInDirection();
            }
        }
    }

    // Method to move the enemy towards the player
    void MoveTowardsPlayer()
    {
        if (player != null && !enemy.BallEnemy)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemy.currentMoveSpeed * Time.deltaTime);
        }
    }

    // Method to move the BallEnemy in the initial direction
    void MoveInDirection()
    {
        // Move the enemy in the initial movement direction
        transform.position += (Vector3)(movementDirection * enemy.currentMoveSpeed * Time.deltaTime);
    }

    // Method to retrieve player position
    public Vector3 RetrievePlayerPosition()
    {
        if (player != null)
        {
            return player.position;
        }
        else
        {
            // If player is not found, return Vector3.zero
            return Vector3.zero;
        }
    }

    // This is meant to be called from other scripts to create knockback.
    public void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is greater than 0.
        if(knockbackDuration > 0) return;

        // Begins the knockback.
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
