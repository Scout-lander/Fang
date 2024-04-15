using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupEnemyMovement : MonoBehaviour
{
    Vector3 targetPosition; // The target position for the enemies to move towards
    float distanceToRemove = 40f; // The distance threshold to remove the enemy
    List<Transform> enemies = new List<Transform>(); // List of enemies in the group

    void Start()
    {
        // Get the initial target position (player's position)
        targetPosition = FindObjectOfType<PlayerMovement>().transform.position;

        // Find all enemies in the group and add them to the list
        EnemyStats[] enemyStats = FindObjectsOfType<EnemyStats>();
        foreach (EnemyStats enemyStat in enemyStats)
        {
            enemies.Add(enemyStat.transform);
        }
    }

    void Update()
    {
        // Move enemies towards the target position
        foreach (Transform enemy in enemies)
        {
            if (enemy != null)
            {
                float moveSpeed = enemy.GetComponent<EnemyStats>().currentMoveSpeed;
                Vector3 direction = (targetPosition - enemy.position).normalized;
                enemy.position += direction * 5 * Time.deltaTime;
            }
        }

        // Check and remove enemies that are far from the target position
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] != null && Vector3.Distance(enemies[i].position, targetPosition) > distanceToRemove)
            {
                Destroy(enemies[i].gameObject);
                enemies.RemoveAt(i);
            }
        }
    }
}
