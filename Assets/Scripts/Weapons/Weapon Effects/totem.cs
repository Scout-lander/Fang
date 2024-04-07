using UnityEngine;

public class TotemEffect : WeaponEffect
{
    public GameObject totemPrefab; // Reference to the totem prefab to be spawned
    public float areaOfEffectRadius = 3f; // Adjust the area of effect radius as needed

    private GameObject spawnedTotem; // Reference to the spawned totem object

    // Update is called once per frame
    void Update()
    {
        if (spawnedTotem == null)
        {
            SpawnTotem();
        }
    }

    void SpawnTotem()
    {
        // Spawn the totem behind the player
        Vector3 spawnPosition = transform.position - transform.forward * 2f; // Adjust this value as needed
        spawnedTotem = Instantiate(totemPrefab, spawnPosition, Quaternion.identity);

        // Apply area damage around the totem
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, areaOfEffectRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out EnemyStats EnemyStats))
            {
                // Deal damage to enemies within the area of effect
                EnemyStats.TakeDamage(GetDamage(), spawnPosition);
            }
        }
    }
}