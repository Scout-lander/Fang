using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator animator;
    EnemyMovement enemyMovement;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if the enemy is moving towards the player and flip sprite accordingly
        if (enemyMovement.RetrievePlayerPosition() != transform.position)
        {
            FlipSpriteDirection();
        }
    }

    void FlipSpriteDirection()
    {
        if (enemyMovement.RetrievePlayerPosition().x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Player is on the left side
        }
        else
        {
            spriteRenderer.flipX = false; // Player is on the right side
        }
    }
}
