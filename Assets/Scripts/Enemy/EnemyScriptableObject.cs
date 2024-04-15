using UnityEngine;

public enum EnemyType
{
    CollisionEnemy,
    ShootingEnemy,
    SplittingEnemy,
    SummonerEnemy,
    ChargingEnemy,
    DashEnemy,
    ExplodingEnemy,
    BallEnemy
}

[CreateAssetMenu(fileName = "AllEnemyScriptableObject", menuName = "Enemy/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    // Define a property to hold the enemy type
    [SerializeField] internal EnemyType enemyType;
    public EnemyType EnemyType => enemyType; // Property to get the enemy type

    // Base stats for all types of enemies
    [SerializeField] 
    internal float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    [SerializeField]
    internal float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    [SerializeField] 
    internal float collisionDamage; // Collision damage
    public float CollisionDamage { get => collisionDamage; private set => collisionDamage = value; }

    [SerializeField] 
    internal float knockbackDuration = 0.2f; // Duration of knockback when hit
    public float KnockbackDuration => knockbackDuration;

    // Property for despawn distance (for all enemies)
    [SerializeField] private float despawnDistance;
    public float DespawnDistance => despawnDistance;

//ShootingEnemy
    [SerializeField] float shootingDamage;
    public float ShootingDamage => shootingDamage;

    [SerializeField] private float stoppingDistance;    // Property for stopping distance (for shooting enemies)
    public float StoppingDistance => stoppingDistance;

    [SerializeField] private float shootingDistance; // Property for shooting distance (for shooting enemies)
    public float ShootingDistance => shootingDistance;

    [SerializeField] internal GameObject projectilePrefab; // Prefab of the projectile the enemy shoots
    public GameObject ProjectilePrefab => projectilePrefab;

    [SerializeField] 
    internal float projectileSpeed = 10f; // Speed of the projectile
    internal  float ProjectileSpeed => projectileSpeed;

    [SerializeField] internal  float shootingCooldown = 20f; // Cooldown between shots
    public float ShootingCooldown => shootingCooldown;

//SliptEnemy
    public int numberOfSplits;// Number of prefabs to spawn on kill
    public GameObject enemySplitPrefab; // Reference to the prefab of the enemy to spawn

//SummonerEnemy
    public GameObject[] enemyPrefabsToSummon; // List of enemy prefabs to spawn

    [SerializeField]
    public float summonCooldown = 6f;
    public float SummonCooldown { get => summonCooldown; private set => summonCooldown = value; }

    [SerializeField]
    internal float summoningDistance = 6f;
    public float SummoningDistance { get => summoningDistance; private set => summoningDistance = value; }

    [SerializeField] internal  float summoningDuration = 3f;
    public float SummoningDuration => summoningDuration;

    [SerializeField] internal  float spawnOffsetDistance; // Property for spawn offset distance (for splitting enemies)
    public float SpawnOffsetDistance => spawnOffsetDistance;

    public int minSummonAmount = 1; // Minimum number of enemies to spawn
    public int maxSummonAmount = 3; // Maximum number of enemies to spawn

 //ChargingEnemy
    [SerializeField] internal float chargingDistance = 10f;
    public float ChargingDistance => chargingDistance;

    [SerializeField] internal float pauseDuration = 1f;
    public float PauseDuration => pauseDuration;

    [SerializeField] internal float increasedSpeed = 20f;
    public float IncreasedSpeed => increasedSpeed;

    [SerializeField] internal float chargeDuration = 2f;
    public float ChargeDuration => chargeDuration;

    [SerializeField] internal float chargeCooldown = 5f;
    public float ChargeCooldown => chargeCooldown;    

// DashEnemy properties
    [SerializeField] internal float dashDistance;
    public float DashDistance => dashDistance;

    [SerializeField] internal float dashSpeed;
    public float DashSpeed => dashSpeed;

    [SerializeField] internal float dashCooldown;
    public float DashCooldown => dashCooldown;

// ExplodingEnemy
    [SerializeField] internal float explosionRadius = 3f; // Radius of explosion
    public float ExplosionRadius => explosionRadius;

    [SerializeField] internal float explosionStartDistance = 6f; // Distance from player to start exploding
    public float ExplosionStartDistance => explosionStartDistance;

    [SerializeField] internal float explosionDamage = 15f; // Damage dealt by explosion
    public float ExplosionDamage => explosionDamage;

    [SerializeField] internal float movementSpeedIncrease = 3f; // Increase in movement speed during explosion
    public float MovementSpeedIncrease => movementSpeedIncrease;

    [SerializeField] internal Color flashColor = new Color(1, 0, 0, 1); // Color of flash during explosion
    public Color FlashColor => flashColor;

    [SerializeField] internal float flashDuration = 0.2f; // Duration of flash during explosion
    public float FlashDuration => flashDuration;

    [SerializeField] internal float explosionDuration = 4f; // Duration of explosion
    public float ExplosionDuration => explosionDuration;

    [SerializeField] internal float spiralRadiusIncreaseRate = 0.1f; // Rate of increase in spiral radius during explosion
    public float SpiralRadiusIncreaseRate => spiralRadiusIncreaseRate;
}
