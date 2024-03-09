using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to be attached to all Weapon prefabs. The Weapon prefab works together with the WeaponData
/// ScriptableObjects to manage and run the behaviours of all weapons in the game.
/// </summary>
public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public Projectile projectilePrefab;
        public GameObject burnPrefab; // Prefab for the burn effect.
        public GameObject icePrefab; // Prefab for the burn effect.

        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        [Header("Burn Damage")]
        [Tooltip("Chance to apply burn damage upon hitting a target.")]
        [Range(0f, 1f)]
        public float burnChance; // The chance to deal burn damage.

        [Tooltip("Minimum amount of burn damage per tick.")]
        public float minBurnDamage; // The minimum amount of burn damage per tick.

        [Tooltip("Maximum amount of burn damage per tick.")]
        public float maxBurnDamage; // The maximum amount of burn damage per tick.

        [Tooltip("Duration of the burn damage in seconds.")]
        public float burnDuration; // The duration of the burn damage.

        [Tooltip("Rate at which burn damage is applied per second.")]
        public float burnTickRate; // The rate at which burn damage is applied.

        [Tooltip("Delay before the burn damage starts.")]
        public float burnDelay; // Delay before burn damage starts.

        [Header("Critical Hit")]
        [Tooltip("Chance to deal a critical hit (percentage from 0 to 100%).")]
        [Range(0f, 1f)]
        public float critChance;

        [Tooltip("Minimum crit damage percentage of base weapon damage.")]
        [Range(0f, 1f)]
        public float minCritDamagePercent;

        [Tooltip("Maximum crit damage percentage of base weapon damage.")]
        [Range(0f, 1f)]
        public float maxCritDamagePercent;
        private GameObject burnEffectInstance;

        [Header("Ice Effect")]
        [Tooltip("Chance to apply ice effect upon hitting a target.")]
        [Range(0f, 1f)]
        public float iceChance; // The chance to apply ice effect.

        [Tooltip("Minimum slow percentage applied to the enemy's movement speed.")]
        [Range(0f, 1f)]
        public float minSlowMoveSpeedPercent; // The minimum slow percentage applied to the enemy's movement speed.

        [Tooltip("Maximum slow percentage applied to the enemy's movement speed.")]
        [Range(0f, 1f)]
        public float maxSlowMoveSpeedPercent; // The maximum slow percentage applied to the enemy's movement speed.

        [Tooltip("Duration of the ice effect in seconds.")]
        public float iceDuration; // The duration of the ice effect.

        [Tooltip("Delay before the ice effect starts.")]
        public float iceDelay; // Delay before the ice effect starts.

        private GameObject iceEffectInstance;


        // Allows us to use the + operator to add 2 Stats together.
        // Very important later when we want to increase our weapon stats.
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;

            // Add burn damage related values
            result.burnChance = s1.burnChance + s2.burnChance;
            result.minBurnDamage = s1.minBurnDamage + s2.minBurnDamage;
            result.maxBurnDamage = s1.maxBurnDamage + s2.maxBurnDamage;
            result.burnDuration = Mathf.Max(s1.burnDuration, s2.burnDuration); // Take the maximum burn duration.
            result.burnTickRate = Mathf.Min(s1.burnTickRate, s2.burnTickRate); // Take the minimum tick rate.
        result.burnDelay = Mathf.Max(s1.burnDelay, s2.burnDelay); // Take the maximum delay.

         // Add critical hit related values
            result.critChance = Mathf.Clamp01(s1.critChance + s2.critChance);
            result.minCritDamagePercent = Mathf.Clamp01(s1.minCritDamagePercent + s2.minCritDamagePercent);
            result.maxCritDamagePercent = Mathf.Clamp01(s1.maxCritDamagePercent + s2.maxCritDamagePercent);
            return result;
            }

        // Get damage dealt.
        // Get damage dealt.
        public float GetDamage()
        {
            float totalDamage = damage + Random.Range(0, damageVariance);
            if (Random.value <= burnChance)
            {
                // Apply burn damage over time.
                totalDamage += CalculateBurnDamage();
            }
            if (Random.value <= critChance)
            {
                // Apply critical hit damage multiplier.
                float critMultiplier = Random.Range(minCritDamagePercent, maxCritDamagePercent);
                totalDamage *= (1 + critMultiplier); // Adjust total damage by crit multiplier.
            }
            return totalDamage;
        }

        // Get ice effect applied.
        public float GetIceEffect()
        {
        if (Random.value <= iceChance)
            {
            return Random.Range(minSlowMoveSpeedPercent, maxSlowMoveSpeedPercent);
            }
            return 0f;
}

    // Method to instantiate burn effect prefab on the enemy.
        public void InstantiateBurnEffect(Transform parent)
        {
            if (burnEffectInstance == null)
            {
                // Instantiate burn effect prefab and attach it to the parent.
                burnEffectInstance = GameObject.Instantiate(burnPrefab, parent.position, Quaternion.identity);
                burnEffectInstance.transform.parent = parent;
            }
        }

        // Method to remove burn effect prefab from the enemy.
        public void RemoveBurnEffect()
        {
            if (burnEffectInstance != null)
            {
                // Destroy burn effect prefab.
                GameObject.Destroy(burnEffectInstance);
                burnEffectInstance = null;
            }
        }

        private float CalculateBurnDamage()
        {
        // Calculate total burn damage over the duration.
            return Random.Range(minBurnDamage, maxBurnDamage) * (burnDuration / burnTickRate);
        }

         // Method to instantiate ice effect prefab on the enemy.
        public void InstantiateIceEffect(Transform parent)
        {
            if (iceEffectInstance == null)
            {
                // Instantiate ice effect prefab and attach it to the parent.
                iceEffectInstance = GameObject.Instantiate(icePrefab, parent.position, Quaternion.identity);
                iceEffectInstance.transform.parent = parent;
            }
        }

        public void RemoveIceEffect()
        {
        if (iceEffectInstance != null)
            {
                // Destroy ice effect prefab.
                GameObject.Destroy(iceEffectInstance);
                iceEffectInstance = null;
            }
        }
    }

    

    protected Stats currentStats;

    public WeaponData data;

    protected float currentCooldown;

    protected PlayerMovement movement; // Reference to the player's movement.

    // For dynamically created weapons, call initialise to set everything up.
    public virtual void Initialise(WeaponData data)
    {

        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        currentCooldown = currentStats.cooldown;
    }

    protected virtual void Awake()
    {
        // Assign the stats early, as it will be used by other scripts later on.
        if(data) currentStats = data.baseStats;
    }

    protected virtual void Start()
    {
        // Don't initialise the weapon if the weapon data is not assigned.
        if(data)
        {
            Initialise(data);
        }
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) //Once the cooldown becomes 0, attack
        {
            Attack(currentStats.number);
        }
    }



    // Levels up the weapon by 1, and calculates the corresponding stats.
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // Prevent level up if we are already at max level.
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to Level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // Otherwise, add stats of the next level to our weapon.
        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }

    // Lets us check whether this weapon can attack at this current moment.
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    // Performs an attack with the weapon.
    // Returns true if the attack was successful.
    // This doesn't do anything. We have to override this at the child class to add a behaviour.
    protected virtual bool Attack(int attackCount = 1)
    {
        if(CanAttack()) {
	    currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }

    // Gets the amount of damage that the weapon is supposed to deal.
    // Factoring in the weapon's stats (including damage variance),
    // as well as the character's Might stat.
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.CurrentMight;
    }

    // For retrieving the weapon's stats.
    public virtual Stats GetStats() { return currentStats; }
}