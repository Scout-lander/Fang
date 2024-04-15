using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;  // The number of enemies of this type to spawn
        public GameObject enemyPrefab;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;    // A list of groups of enemies to spawn in this wave
        public float startDelay; // The delay before this wave starts
        public float spawnRadius; // The radius of the circular area in which enemies will spawn for this wave
    }

    [Header("Spawn Attributes")]
    public List<Transform> spawnPoints; // The list of spawn points for enemies
    public LayerMask obstacleMask; // Layer mask for obstacles to prevent overlapping

    public List<Wave> waves; // A list of all the waves in the game
    public int currentWaveIndex; // The index of the current wave [Remember, a list starts from 0]

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWaveEnemies(waves[currentWaveIndex]));
        }
        else
        {
            //Debug.LogWarning("All waves have been completed!");
        }
    }

    IEnumerator SpawnWaveEnemies(Wave wave)
    {
        yield return new WaitForSeconds(wave.startDelay);

        Transform chosenSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        foreach (var enemyGroup in wave.enemyGroups)
        {
            for (int i = 0; i < enemyGroup.enemyCount; i++)
            {
                // Calculate a random position within the spawn radius
                Vector2 randomCircle = Random.insideUnitCircle * wave.spawnRadius;
                Vector3 spawnPosition = chosenSpawnPoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

                // Adjust the spawn position to ensure it's not obstructed
                if (IsPositionClear(spawnPosition))
                {
                    // If the position is clear, instantiate the enemy
                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
                }
            }
        }

        currentWaveIndex++;
        StartWave();
    }

    bool IsPositionClear(Vector3 position)
    {
        return !Physics.CheckSphere(position, 1f, obstacleMask);
    }
}
