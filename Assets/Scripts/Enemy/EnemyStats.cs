using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyAnimator))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    //Current stats
    public float currentMoveSpeed;
    public float currentHealth;
    public float collisionDamage;
    public float despawnDistance = 20f;
    protected Transform player;
    public bool CollisionEnemy = false;

    // Shooting variables
    public bool ShootingEnemy = false;
    protected float shootingDamage;
    protected float shootingDistance;
    protected float projectileSpeed;
    protected float stoppingDistance;
    protected float shootingCooldown;
    protected float shootingDespawnDistance; // Rename this variable
    protected bool canShoot = false;
    protected Coroutine shootingCoroutine;

    // Summoning variables
    public bool SummonerEnemy = false;
    protected bool isSummoning = false;
    protected bool isOnCooldown = false;
    protected float summoningDuration = 3f;
    protected float summoningDistance;
    protected float summonCooldown;
    protected float cooldownTimer = 0f;
    protected Coroutine summonCoroutine;
    protected ParticleSystem summoningEffect;
    // Splitting variables
    public bool SplittingEnemy = false;
    protected float spawnOffsetDistance = .5f;
    protected int numberOfSplits; // Number of splits to spawn on kill
    protected bool canSplitOnDeath = false; // Boolean to enable splitting on death
    protected GameObject enemySplitPrefab;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1); // What the color of the damage flash should be.
    public float damageFlashDuration = 0.2f; // How long the flash should last.
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade.
    Color originalColor;
    SpriteRenderer sr;
    PlayerStats playerStats;
    protected  Animator animator;
    EnemyMovement movement;

    protected virtual void InitializeEnemy()
    {
            Debug.Log("Initializing enemy: " + enemyData.enemyType); // Add this line for debugging

        // Initialize enemy based on stats
        // You can add more initialization logic here
        switch (enemyData.enemyType)
        {
            case EnemyType.CollisionEnemy:
                // Initialize collision enemy
                CollisionEnemy = true;
                break;
            case EnemyType.ShootingEnemy:
                // Initialize shooting enemy
                ShootingEnemy = true;
                shootingDamage = enemyData.ShootingDamage;
                shootingDistance = enemyData.ShootingDistance;
                shootingCooldown = enemyData.ShootingCooldown;
                stoppingDistance = enemyData.StoppingDistance;

                canShoot = true;
                shootingCoroutine = StartCoroutine(ShootCoroutine());
                break;
            case EnemyType.SummonerEnemy:
                // Initialize summoning enemy
                SummonerEnemy = true;
                summoningDistance = enemyData.SummoningDistance;
                summonCooldown = enemyData.SummonCooldown;
                summonCoroutine = StartCoroutine(SummonEnemies());
                break;
            case EnemyType.SplittingEnemy:
                // Initialize splitting enemy
                SplittingEnemy = true;
                canSplitOnDeath = true;
                spawnOffsetDistance = enemyData.SpawnOffsetDistance;
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        //Assign the vaiables
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        collisionDamage = enemyData.CollisionDamage;
    }

    protected virtual void Start()
    {
        // Call InitializeEnemy() to set up enemy-specific properties
        InitializeEnemy();

        player = FindObjectOfType<PlayerStats>().transform;
        playerStats = FindObjectOfType<PlayerStats>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        animator = GetComponent<Animator>();

        movement = GetComponent<EnemyMovement>();
    
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    // This function always needs at least 2 values, the amount of damage dealt <dmg>, as well as where the damage is
    // coming from, which is passed as <sourcePosition>. The <sourcePosition> is necessary because it is used to calculate
    // the direction of the knockback.
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // Create the text popup when enemy takes damage.
        if (dmg > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

        // Update total damage done
        playerStats.IncrementTotalDamageDone(dmg);

        // Apply knockback if it is not zero.
        if(knockbackForce > 0)
        {
            // Gets the direction of knockback.
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        // Kills the enemy if the health drops below zero.
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    // This is a Coroutine function that makes the enemy flash when taking damage.
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        // Instantiate multiple enemies at random positions near the current position if splitting on death is enabled
        if (canSplitOnDeath)
        {
            Vector3 currentPosition = transform.position;
            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = Random.insideUnitCircle.normalized * spawnOffsetDistance;
                Instantiate(enemyData.enemySplitPrefab, currentPosition + offset, Quaternion.identity);
            }
        }

        StartCoroutine(KillFade());
        playerStats.IncrementKillCount(); // Increment kill count when enemy is killed.

    }

    // This is a Coroutine function that fades the enemy away slowly.
    IEnumerator KillFade()
    {
        // Waits for a single frame.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        // This is a loop that fires every frame.
        while(t < deathFadeTime) {
            yield return w;
            t += Time.deltaTime;

            // Set the colour for this frame.
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        //Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(collisionDamage); // Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
        }
    }

    private void OnDestroy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        if(es) es.OnEnemyKilled();
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }


///////////////////// ShootingEnemy ///////////////////////////
    protected IEnumerator ShootCoroutine()
    { Debug.Log("Starting Shooting"); // Add this line for debugging
        while (true)
        {
            yield return new WaitForSeconds(shootingCooldown);

            if (canShoot && Vector2.Distance(transform.position, player.position) <= shootingDistance)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (player == null)
            return;

        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(enemyData.ProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * enemyData.ProjectileSpeed;

        // Set damage of the projectile
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage((int)shootingDamage);
        }

        canShoot = false;
        StartCoroutine(ResetShoot());
    }

    private IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }

/////////////////////////SummonerEnmey////////////////////////

    public IEnumerator SummonEnemies()
    {
        // Check if player is null before accessing its position
        yield return new WaitUntil(() => player != null && Vector2.Distance(transform.position, player.position) <= summoningDistance);

        if (!isOnCooldown && !isSummoning)
        {
            isSummoning = true;
            isOnCooldown = true; // Start cooldown
            animator.SetBool("Summoning", true); // Set the "Summoning" bool to true
            PauseMovement(true); // Pause movement while summoning
            yield return new WaitForSeconds(summoningDuration);
            SpawnEnemies();
            animator.SetBool("Summoning", false); // Set the "Summoning" bool back to false
            PauseMovement(false); // Resume movement after summoning
            isSummoning = false;
            yield return new WaitForSeconds(summonCooldown);
            isOnCooldown = false; // Reset cooldown
            StartCoroutine(SummonEnemies());
        }
    }


    private void PauseMovement(bool pause)
    {
        if (pause)
        {
            // Set currentMoveSpeed to 0 to pause movement
            currentMoveSpeed = 0f;
        }
        else
        {
            // Restore currentMoveSpeed to its original value
            currentMoveSpeed = enemyData.MoveSpeed;
        }
    }

    private void SpawnEnemies()
    {
        // Calculate a random summon amount within the specified range
        int summonAmount = Random.Range(enemyData.minSummonAmount, enemyData.maxSummonAmount + 1);

        // Select a random enemy prefab from the list
        GameObject enemyPrefabToSummon = enemyData.enemyPrefabsToSummon[Random.Range(0, enemyData.enemyPrefabsToSummon.Length)];

        for (int i = 0; i < summonAmount; i++)
        {
            // Calculate spawn position relative to the current position of the enemy
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 2f; // Spawn within a small radius around the enemy

            // Instantiate the selected enemy prefab at the calculated spawn position
            Instantiate(enemyPrefabToSummon, spawnPosition, Quaternion.identity);
        }
    }

}
