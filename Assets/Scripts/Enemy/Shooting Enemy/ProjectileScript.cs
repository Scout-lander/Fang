using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private int damage; // Damage of the projectile
    public float speed = 10f; // Speed of the projectile
    public float lifetime = 2f; // Lifetime of the projectile
    private Transform player; // Reference to the player's transform

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    void Start()
    {
        // Find the player's transform
        player = FindObjectOfType<PlayerStats>().transform;

        // Calculate direction to the player
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        // Apply velocity towards the player
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;

        // Destroy the projectile after a certain lifetime
        Destroy(gameObject, lifetime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the projectile collided with the player
        if (other.CompareTag("Player"))
        {
            // Retrieve the PlayerStats component from the collided object
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            // If the playerStats component exists, apply damage to the player
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            // Destroy the projectile upon collision with the player
            Destroy(gameObject);
        }
    }
}