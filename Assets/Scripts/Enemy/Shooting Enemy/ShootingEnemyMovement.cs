using UnityEngine;

public class ShootingEnemyMovement : MonoBehaviour
{
    public ShootingEnemyScriptableObject enemyData;

    private Transform player; // Reference to the player's transform
    private bool canShoot = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
    }

    private void Update()
    {
        if (!canShoot)
        {
            MoveTowardsPlayer();
            CheckDistanceToPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null)
            return;

        transform.LookAt(player);
        transform.Translate(Vector3.forward * enemyData.moveSpeed * Time.deltaTime);
    }

    private void CheckDistanceToPlayer()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= enemyData.shootingDistance)
        {
            canShoot = true;
        }
    }
}
