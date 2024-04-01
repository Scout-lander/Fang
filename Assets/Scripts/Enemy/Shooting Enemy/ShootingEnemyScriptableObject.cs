using UnityEngine;

[CreateAssetMenu(fileName = "New Shooting Enemy Data", menuName = "Enemy/Shooting Enemy Data")]
public class ShootingEnemyScriptableObject : ScriptableObject
{
    // Base stats for the enemy
    [SerializeField] 
    float moveSpeed;
    public float MoveSpeed => moveSpeed;

     [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    [SerializeField] 
    float collisionDamage; // Collision damage
    public float CollisionDamage => collisionDamage;

    [SerializeField] 
    float shootingDamage;
    public float ShootingDamage => shootingDamage;

    [SerializeField] 
    float shootingDistance = 10f; // Distance at which the enemy can shoot
    public float ShootingDistance => shootingDistance;

    [SerializeField]
     GameObject projectilePrefab; // Prefab of the projectile the enemy shoots
    public GameObject ProjectilePrefab => projectilePrefab;

    [SerializeField]
     float projectileSpeed = 10f; // Speed of the projectile
    public float ProjectileSpeed => projectileSpeed;

    [SerializeField] 
     float shootingCooldown = 20f; // Cooldown between shots - changed to public
    public float ShootingCooldown => shootingCooldown;

    [SerializeField]
     float knockbackDuration = 0.2f; // Duration of knockback when hit - changed to public
    public float KnockbackDuration => knockbackDuration;

    // Additional properties can be added as needed
}