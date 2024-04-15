using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    //Current stats
    public float currentMoveSpeed;
    public float currentHealth;
    public float collisionDamage;
    public float despawnDistance = 20f;
    
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
    public ParticleSystem summoningEffect;

    // Splitting variables
    public bool SplittingEnemy = false;
    protected float spawnOffsetDistance = .8f;
    protected int numberOfSplits; // Number of splits to spawn on kill
    protected bool canSplitOnDeath = false; // Boolean to enable splitting on death
    protected GameObject enemySplitPrefab;
    public GameObject splittingEffect;

    // Dash variables
    public bool DashEnemy = false;
    protected float dashDistance;
    protected float dashSpeed;
    protected float dashCooldown;
    protected bool canDash = true;
    protected bool isCharging = false;
    protected Coroutine dashCoroutine;

    // Exploding variables
    public bool ExplodingEnemy = false;
    protected float explosionRadius = 3f;
    protected float explosionStartDistance = 6f;
    protected float explosionDamage = 15f;
    protected float movementSpeedIncrease = 3f;
    protected Color flashColor = new Color(1,0,0,1);
    protected float flashDuration = 0.2f;
    protected float explosionDuration = 4f;
    protected float spiralRadiusIncreaseRate = 0.1f;
    protected bool isExploding = false;
    public ParticleSystem explodingEffect;

    // Ball Enemy
    public  bool BallEnemy = false;
    protected Vector2 initialMovementDirection; // Store the initial movement direction for BallEnemy

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1); // What the color of the damage flash should be.
    public float damageFlashDuration = 0.2f; // How long the flash should last.
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade.

    ////////////////////
    Color originalColor;
    SpriteRenderer sr;
    PlayerStats playerStats;
    protected  Animator animator;
    EnemyMovement movement;
    PlayerMovement playerMovement;
    protected Transform player;

    protected virtual void InitializeEnemy()
    {
            Debug.Log("Initializing enemy: " + enemyData.enemyType); // Line for debugging

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
                numberOfSplits = enemyData.numberOfSplits;
                break;

            case EnemyType.DashEnemy:
                // Initialize dash enemy
                DashEnemy = true;
                dashDistance = enemyData.DashDistance;
                dashSpeed = enemyData.DashSpeed;
                dashCooldown = enemyData.DashCooldown;
                break;

             case EnemyType.ExplodingEnemy:
                // Initialize exploding enemy
                ExplodingEnemy = true;
                summonCoroutine = StartCoroutine(ExplodeCoroutine());
                break;
            
            case EnemyType.BallEnemy:
                // Initialize BallEnemy
                BallEnemy = true;
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
        player = FindObjectOfType<PlayerStats>()?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found.");
            return;
        }

        player = FindObjectOfType<PlayerStats>().transform;
        playerStats = FindObjectOfType<PlayerStats>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        animator = GetComponent<Animator>();

        movement = GetComponent<EnemyMovement>();
        playerMovement = GetComponent<PlayerMovement>();
    
        if (DashEnemy)
        {
            // Start checking for dashing conditions
            StartCoroutine(CheckDashConditions());
        }
    }

    void Update()
    {
        if (player != null && !BallEnemy && Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }

        // Only perform despawn check for BallEnemy
        if (BallEnemy && player != null && Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            Destroy(gameObject); // Destroy the BallEnemy when it reaches the despawn distance
        }

        if (BallEnemy && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            //Debug.Log("Distance to player: " + distanceToPlayer);
            if (distanceToPlayer >= despawnDistance)
            {
                Debug.Log("Despawning BallEnemy");
                Destroy(gameObject);
            }
        }
    }

    public Vector2 GetInitialMovementDirection()
    {
        return initialMovementDirection;
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
            if (splittingEffect != null)
            {
                Vector3 Position = transform.position; // Store the position where the enemy will split
                Instantiate(splittingEffect, Position, Quaternion.identity);
            } 
            StartCoroutine(SpawnEnemiesWithDelay());
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
        if (col.gameObject.CompareTag("Player") && ExplodingEnemy && isExploding)
        {
            Explode();
        }
        //Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(collisionDamage); // Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
        }

        //Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player") && DashEnemy && isCharging)
        {
            // Get the PlayerMovement component from the collided player object
            PlayerMovement playerMovement = col.gameObject.GetComponent<PlayerMovement>();

            // Apply stun and knockback to the player
            Vector2 sourcePosition = transform.position; // The position of the enemy causing the stun
            float knockbackForce = 35f; // Adjust as needed
            float knockbackDuration = 0.3f; // Adjust as needed
            StartCoroutine(playerMovement.ApplyStun(sourcePosition, knockbackForce, knockbackDuration));
        }
    }

    public void HandleBallEnemyDespawn()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer >= despawnDistance)
            {
                Destroy(gameObject); // Destroy the BallEnemy when it reaches the despawn distance
            }
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
    { Debug.Log("Starting Shooting"); // Line for debugging
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
            if (summoningEffect != null)
            {
                Vector3 Position = transform.position; // Store the position where the enemy will split
                Instantiate(summoningEffect, Position, Quaternion.identity);
            }
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

/////////////////////////ChargerEnmey///////////////////////
 // Check conditions for dashing
    private IEnumerator CheckDashConditions()
    {
        while (true)
        {
            // Check if the player is within dash distance
            if (Vector2.Distance(transform.position, player.position) <= dashDistance && canDash)
            {
                // Start the dash coroutine
                StartCoroutine(Dash());
            }
            yield return null;
        }
    }

    // Dash coroutine
    protected IEnumerator Dash()
    {
        isCharging = true;
        canDash = false; // Disable dashing until cooldown

        // Stop movement and trigger dash animation
        animator.SetBool("Dash", true);
        PauseMovement(true);

        // Wait for pause duration before dashing
        yield return new WaitForSeconds(0.4f);

        Vector3 targetPosition = player.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceRemaining = dashDistance;

        while (distanceRemaining > 0)
        {
            // Move towards the player at dash speed
            transform.position += direction * dashSpeed * Time.deltaTime;
            distanceRemaining -= dashSpeed * Time.deltaTime;
            yield return null;
        }

        // Stop dash animation and resume movement
        animator.SetBool("Dash", false);
        isCharging = false;
        PauseMovement(false);

        // Wait for dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Enable dashing again
    }

/////////////////////////SplittingEnemy///////////////////////
    IEnumerator SpawnEnemiesWithDelay()
    {
        //Debug.Log("Starting Splittting"); // Line for debugging
        yield return new WaitForSeconds(.5f);
           
        Vector3 currentPosition = transform.position;
        for (int i = 0; i < numberOfSplits; i++)
        {
            Vector3 offset = Random.insideUnitSphere.normalized * spawnOffsetDistance;
            Vector3 spawnPosition = currentPosition + offset;
            //Debug.Log("Spawn Position: " + spawnPosition); // Print spawn position for debugging
            Instantiate(enemyData.enemySplitPrefab, spawnPosition, Quaternion.identity);
            
        }
    }

/////////////////////////ExplodingEnemy///////////////////////
    IEnumerator ExplodeCoroutine()
    {
        while (true)
        {
            yield return null;

            if (!isExploding && Vector2.Distance(transform.position, player.position) <= explosionStartDistance)
            {
                isExploding = true;
                
                // Increase movement speed and move pattern
                currentMoveSpeed += movementSpeedIncrease;
                MoveInSpiralPattern();

                // Flash color
                StartCoroutine(FlashColor());

                // Wait for flash duration
                yield return new WaitForSeconds(explosionDuration);

                // Explode
                Explode();
                
                // Destroy the enemy
                Destroy(gameObject);
            }
        }
    }

    IEnumerator FlashColor()
    {
        float originalFlashDuration = flashDuration;
        Color originalColor = sr.color;
        Color currentColor = originalColor;
        Color targetColor = flashColor;
        float timeInterval = 0.5f; // Initial time interval between color changes
        float speedUpInterval = 2f; // Time interval after which the flashing speeds up
        float timeElapsed = 0f;

        while (true)
        {
            // Alternate between the two colors
            if (currentColor == originalColor)
                currentColor = flashColor;
            else
                currentColor = originalColor;

            // Apply the current color
            sr.color = currentColor;

            // Decrease wait time between color changes gradually to speed up the flashing
            yield return new WaitForSeconds(timeInterval);

            timeElapsed += timeInterval;

            // Speed up the flashing after a certain interval
            if (timeElapsed >= speedUpInterval)
            {
                timeInterval /= 2.4f; // Reduce the time interval by half to speed up flashing
                timeElapsed = 0f; // Reset the time elapsed
            }
        }
    }

    void Explode()
    {
        if (explodingEffect != null)
        {
            Vector3 Position = transform.position; // Store the position where the enemy will split
            Instantiate(explodingEffect, Position, Quaternion.identity);
        }
        // Deal damage to nearby objects within explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStats>().TakeDamage(explosionDamage);
            }
        }
        Destroy(gameObject);
    }

    void MoveInSpiralPattern()
    {
        // Calculate direction towards the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Move towards the player in a spiral pattern
        transform.position += directionToPlayer * spiralRadiusIncreaseRate * Time.deltaTime;

        // Rotate towards the player
        transform.RotateAround(player.position, Vector3.forward, Time.deltaTime * 30f);
    }
}
