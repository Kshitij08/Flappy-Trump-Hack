using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum defining different enemy types
public enum EnemyType
{
    Obstacle,    // Spawns on the right, can use patterns if quantity > 1
    FUDMonster,  // Spawns on the left, can use patterns if quantity > 1
    SECGary,     // Spawns on the left, requires unique Y-coordinates in patterns
    RugPuller,   // Spawns on the right, same pattern as Obstacles
    Melania,     // Unique spawn behavior
    BitcoinFly,  // Unique spawn behavior
    Elon         // Unique spawn behavior
}

// Defines enemy spawn events, including type, quantity, interval, and start time
[System.Serializable]
public class SpawnEvent
{
    public EnemyType enemyType;  // Type of enemy to spawn
    public int quantity;         // Number of enemies to spawn
    public float spawnInterval;  // Time interval between each spawn
    public float spawnTime;      // Time at which this event starts
}

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] obstacleArrayGO;
    public GameObject fudMonsterGO;
    public GameObject secGaryGO;
    public GameObject rugPullerGO;
    public GameObject melaniaGO;
    public GameObject bitcoinFlyGO;
    public GameObject elonGO;

    [Header("Overlap Settings")]
    public LayerMask enemyLayer;         // Layer to check for enemy overlap
    public float enemySpawnRadius = 0.5f; // Radius to prevent overlapping spawns

    public bool canSapwn = true; // Controls whether enemies can spawn
    public float localTimer = 0f; // Tracks elapsed time since game start

    [Header("Spawn Events")]
    public SpawnEvent[] spawnEvents; // Array of predefined spawn events

    void Start()
    {
        // Start processing spawn events
        StartCoroutine(ProcessSpawnEvents());
    }

    void Update()
    {
        // Increment game timer
        localTimer += Time.deltaTime;
    }

    /// <summary>
    /// Coroutine to process spawn events based on their scheduled times.
    /// </summary>
    public IEnumerator ProcessSpawnEvents()
    {
        foreach (SpawnEvent ev in spawnEvents)
        {
            // Wait until the scheduled spawn time is reached
            yield return new WaitUntil(() => localTimer >= ev.spawnTime);

            // If the enemy type supports pattern spawning and has multiple entities, use patterns
            if ((ev.enemyType == EnemyType.Obstacle ||
                 ev.enemyType == EnemyType.FUDMonster ||
                 ev.enemyType == EnemyType.SECGary ||
                 ev.enemyType == EnemyType.RugPuller) && ev.quantity > 1)
            {
                yield return StartCoroutine(SpawnEnemyPattern(ev.enemyType, ev.quantity, ev.spawnInterval));
            }
            else
            {
                // Spawn enemies normally when not using patterns
                for (int i = 0; i < ev.quantity; i++)
                {
                    SpawnEnemy(ev.enemyType);
                    yield return new WaitForSeconds(ev.spawnInterval);
                }
            }
        }
    }

    /// <summary>
    /// Spawns a single enemy based on its type.
    /// </summary>
    void SpawnEnemy(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Obstacle: SpawnObstacles(); break;
            case EnemyType.FUDMonster: SpawnFUDMonster(); break;
            case EnemyType.SECGary: SpawnSECGary(); break;
            case EnemyType.RugPuller: SpawnRugPuller(); break;
            case EnemyType.Melania: SpawnMelania(); break;
            case EnemyType.BitcoinFly: SpawnBitcoinFly(); break;
            case EnemyType.Elon: SpawnElon(); break;
            default:
                Debug.LogWarning($"Unknown enemy type: {enemyType}");
                break;
        }
    }

    /// <summary>
    /// Coroutine to spawn enemies in patterns.
    /// </summary>
    IEnumerator SpawnEnemyPattern(EnemyType enemyType, int quantity, float interval)
    {
        Vector2[][] patternSet = GetPatternSet(quantity);
        if (patternSet == null)
        {
            SpawnEnemy(enemyType);
            yield break;
        }

        Vector2[] chosenPattern = patternSet[Random.Range(0, patternSet.Length)];

        // Ensure SECGary has unique Y-coordinates
        if (enemyType == EnemyType.SECGary)
        {
            int attempt = 0, maxAttempts = 10;
            while (!AllUniqueY(chosenPattern) && attempt < maxAttempts)
            {
                chosenPattern = patternSet[Random.Range(0, patternSet.Length)];
                attempt++;
            }
        }

        // Determine the origin based on spawn side
        Vector3 origin = (enemyType == EnemyType.FUDMonster || enemyType == EnemyType.SECGary)
            ? new Vector3(-13f, -4f, 0f)  // Left side spawn
            : new Vector3(13f, -4f, 0f);  // Right side spawn

        foreach (Vector2 offset in chosenPattern)
        {
            Vector3 spawnPosition = origin + new Vector3(offset.x, offset.y, 0f);

            // Ensure no overlap when spawning
            int attempts = 0, maxAttempts = 10;
            while (Physics2D.OverlapCircle(spawnPosition, enemySpawnRadius, enemyLayer) != null && attempts < maxAttempts)
            {
                spawnPosition.y = Random.Range(-4f, 3.5f);
                attempts++;
            }

            Instantiate(GetEnemyPrefab(enemyType), spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>
    /// Retrieves the appropriate pattern set for the given quantity.
    /// </summary>
    Vector2[][] GetPatternSet(int quantity)
    {
        return quantity switch
        {
            2 => patterns2,
            3 => patterns3,
            4 => patterns4,
            5 => patterns5,
            _ => null
        };
    }

    /// <summary>
    /// Checks if all Y-coordinates in the pattern are unique.
    /// </summary>
    bool AllUniqueY(Vector2[] pattern)
    {
        HashSet<float> yValues = new HashSet<float>();
        foreach (Vector2 v in pattern)
        {
            if (!yValues.Add(v.y)) return false;
        }
        return true;
    }

    /// <summary>
    /// Returns the corresponding enemy prefab based on the type.
    /// </summary>
    GameObject GetEnemyPrefab(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Obstacle => obstacleArrayGO[Random.Range(0, obstacleArrayGO.Length)],
            EnemyType.RugPuller => rugPullerGO,
            EnemyType.FUDMonster => fudMonsterGO,
            EnemyType.SECGary => secGaryGO,
            _ => null
        };
    }

    /// <summary>
    /// Helper function to get a random spawn position for enemies.
    /// </summary>
    Vector3 GetRandomSpawnPosition(float xPos)
    {
        return new Vector3(xPos, Random.Range(-4f, 3.5f), 0f);
    }

    // Spawn functions for different enemy types
    void SpawnObstacles() => Instantiate(obstacleArrayGO[Random.Range(0, obstacleArrayGO.Length)], GetRandomSpawnPosition(12f), Quaternion.identity);
    void SpawnFUDMonster() => Instantiate(fudMonsterGO, GetRandomSpawnPosition(-13f), Quaternion.identity);
    void SpawnSECGary() => Instantiate(secGaryGO, GetRandomSpawnPosition(-13f), Quaternion.identity);
    void SpawnRugPuller() => Instantiate(rugPullerGO, GetRandomSpawnPosition(12f), Quaternion.identity);
    void SpawnMelania() => Instantiate(melaniaGO, GetRandomSpawnPosition(Random.Range(4f, 18f)), Quaternion.identity);
    void SpawnBitcoinFly() => Instantiate(bitcoinFlyGO, GetRandomSpawnPosition(Random.Range(4f, 18f)), Quaternion.identity);
    void SpawnElon() => Instantiate(elonGO, GetRandomSpawnPosition(Random.Range(3f, 17f)), Quaternion.identity);

    // Predefined spawn patterns for different enemy quantities
    private Vector2[][] patterns2 = { new Vector2[] { new Vector2(0, 0), new Vector2(0, 2) } };
    private Vector2[][] patterns3 = { new Vector2[] { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0, 4) } };
    private Vector2[][] patterns4 = { new Vector2[] { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0, 4), new Vector2(0, 6) } };
    private Vector2[][] patterns5 = { new Vector2[] { new Vector2(0, 0), new Vector2(0, 2), new Vector2(0, 4), new Vector2(2, 1), new Vector2(2, 3) } };
}
