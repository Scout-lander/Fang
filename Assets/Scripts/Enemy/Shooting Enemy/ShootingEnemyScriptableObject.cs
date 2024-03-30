using UnityEngine;

[CreateAssetMenu(fileName = "New Shooting Enemy Data", menuName = "Enemy/Shooting Enemy Data")]
public class ShootingEnemyScriptableObject : ScriptableObject
{
    public float moveSpeed = 5f; // Speed of enemy movement towards player
    public float maxHealth = 100f; // Maximum health of the enemy
    public float damage = 10f; // Damage dealt by the enemy on collision with player
    public float shootingCooldown = 2f; // Cooldown between shots
    public float shootingDistance = 10f; // Distance at which the enemy can shoot
    public GameObject projectilePrefab; // Prefab of the projectile the enemy shoots
    public float projectileSpeed = 10f; // Speed of the projectile
    public float shootingDamage = 20f; // Damage dealt by the enemy's projectile
}
