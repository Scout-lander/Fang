using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyAnimator : MonoBehaviour
{
    Animator animator;
    ShootingEnemyMovement SenemyMovement;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        SenemyMovement = GetComponent<ShootingEnemyMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if the enemy is moving towards the player and flip sprite accordingly
        if (SenemyMovement.RetrievePlayerPosition() != transform.position)
        {
            FlipSpriteDirection();
        }
    }

    void FlipSpriteDirection()
    {
        if (SenemyMovement.RetrievePlayerPosition().x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Player is on the left side
        }
        else
        {
            spriteRenderer.flipX = false; // Player is on the right side
        }
    }
}
