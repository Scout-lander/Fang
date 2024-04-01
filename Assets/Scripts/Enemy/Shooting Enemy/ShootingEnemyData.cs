using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShootingEnemyData : MonoBehaviour
{
    public ShootingEnemyScriptableObject SenemyData;

    // Current stats
    public float currentMoveSpeed;
    public float currentHealth;
    public float currentDamage; // Collision damage
    public float shootingDamage; // Shooting damage
    public float maxShootingDistance = 10f; // Maximum shooting distance
    public float stoppingDistance = 5f; // Distance at which enemy stops moving
    public float despawnDistance = 20f;

    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1); // What the color of the damage flash should be.
    public float damageFlashDuration = 0.2f; // How long the flash should last.
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade.
    Color originalColor;
    SpriteRenderer sr;
    ShootingEnemyMovement movement;
    private bool canShoot = true;
    private Coroutine shootingCoroutine;

    void Awake()
    {
        // Assign the variables
        currentMoveSpeed = SenemyData.MoveSpeed;
        currentHealth = SenemyData.MaxHealth;
        currentDamage = SenemyData.CollisionDamage; // Assign collision damage
        shootingDamage = SenemyData.ShootingDamage; // Assign shooting damage
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>()?.transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<ShootingEnemyMovement>();
        // Start shooting coroutine
        shootingCoroutine = StartCoroutine(ShootCoroutine());
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(SenemyData.ShootingCooldown);

            if (canShoot && Vector2.Distance(transform.position, player.position) <= maxShootingDistance)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (player == null)
            return;

        Vector3 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(SenemyData.ProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * SenemyData.ProjectileSpeed;

        // Set damage of the projectile
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage((int)shootingDamage); // Convert shootingDamage to an int
        }

        canShoot = false;
        StartCoroutine(ResetShoot());
    }

    IEnumerator ResetShoot()
    {
        yield return new WaitForSecondsRealtime(SenemyData.ShootingCooldown);
        canShoot = true;
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // Create the text popup when enemy takes damage.
        if (dmg > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

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

    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSecondsRealtime(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        StopCoroutine(shootingCoroutine); // Stop the shooting coroutine
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player?.TakeDamage(currentDamage); // Apply collision damage if the player is not null
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
}
