using UnityEngine;

public class SpinningWeapon : Weapon
{
    // Number of projectiles to spawn in a circle around the player
    public int numberOfProjectiles = 6;

    // Angle between each projectile
    private float angleBetweenProjectiles;

    // Radius of the circle around the player
    public float circleRadius = 2f;

    // Current angle for spinning
    private float currentAngle = 0f;

    public override void Initialise(WeaponData data)
    {
        base.Initialise(data);
        // Calculate the angle between each projectile based on the number of projectiles
        angleBetweenProjectiles = 360f / numberOfProjectiles;
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!CanAttack()) return false;

        // Calculate the position of the spawn point away from the player
        Vector2 spawnPosition = CalculateSpawnPosition();

        // Spawn projectiles evenly around the player
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Calculate the angle for each projectile
            float angle = i * angleBetweenProjectiles + currentAngle;

            // Spawn a projectile at the calculated position and angle
            SpawnProjectile(spawnPosition, angle);
        }

        currentAngle += angleBetweenProjectiles; // Update the angle for the next spawn

        ActivateCooldown(true);
        return true;
    }

    // Calculate the position of the spawn point away from the player
    private Vector2 CalculateSpawnPosition()
    {
        // Adjust the spawn position based on the distance away from the player
        // For simplicity, let's spawn it above the player for now
        return owner.transform.position + Vector3.up * circleRadius;
    }

    // Spawn a projectile at the given position and angle
    private void SpawnProjectile(Vector2 spawnPosition, float angle)
    {
        // Calculate the spawn position for the projectile based on the angle
        Vector2 offset = Quaternion.Euler(0, 0, angle) * Vector2.right;
        Vector2 projectileSpawnPosition = spawnPosition + offset;

        // Spawn the projectile at the calculated position
        Projectile prefab = Instantiate(currentStats.projectilePrefab, projectileSpawnPosition, Quaternion.identity);
        prefab.weapon = this;
        prefab.owner = owner;
    }
}
