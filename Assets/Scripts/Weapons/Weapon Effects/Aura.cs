using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An aura is a damage-over-time effect that applies to a specific area in timed intervals.
/// It is used to give the functionality of Garlic, and it can also be used to spawn holy
/// water effects as well.
/// </summary>
public class Aura : WeaponEffect
{

    protected Rigidbody2D rb;

    protected int piercing;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>();

    // Update is called once per frame
    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        // Loop through every target affected by the aura, and reduce the cooldown
        // of the aura for it. If the cooldown reaches 0, deal damage to it.
        foreach(KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if(pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    // If the target is marked for removal, remove it.
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    // Reset the cooldown and deal damage.
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }


protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }

        // Prevent the area from being 0, as it hides the projectile.
        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x), 
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // Set how much piercing this object has.
        piercing = stats.piercing;

        // Destroy the projectile after its lifespan expires.
        if(stats.lifespan > 0) Destroy(gameObject, stats.lifespan);

        // If the projectile is auto-aiming, automatically find a suitable enemy.
    }

    // If the projectile is homing, it will automatically find a suitable target
    // to move towards.
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; // We need to determine where to aim.

        // Find all enemies on the screen.
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        // Select a random enemy (if there is at least 1).
        // Otherwise, pick a random angle.
        if(targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }
        
        // Point the projectile towards where we are aiming at.
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    // Update is called once per frame




    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out EnemyStats es))
        {
            // If the target is not yet affected by this aura, add it
            // to our list of affected targets.
            if(!affectedTargets.ContainsKey(es))
            {
                // Always starts with an interval of 0, so that it will get
                // damaged in the next Update() tick.
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(es))
                {
                    targetsToUnaffect.Remove(es);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent(out EnemyStats es))
        {
            // Do not directly remove the target upon leaving,
            // because we still have to track their cooldowns.
            if(affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}