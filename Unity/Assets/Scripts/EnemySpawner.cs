using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnInfo
    {
        public string name; // Debugging aid
        public GameObject entity; // Prefab reference
        public float initialDelay; // Delay before first spawn
        public float spawnInterval; // Time between spawns
        public Vector2 xRange; // X-position range
        public Vector2 yRange; // Y-position range
    }

    [Header("Entities to Spawn")]
    public SpawnInfo[] spawnEntities;

    void Start()
    {
        // Start repeating spawn for each entity
        foreach (SpawnInfo spawn in spawnEntities)
        {
            InvokeRepeating(spawn.name, spawn.initialDelay, spawn.spawnInterval);
        }
    }

    void Update()
    {
        // Debugging shortcuts using number keys (1-7)
        for (int i = 0; i < spawnEntities.Length; i++)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1 + i))
            {
                SpawnEntity(spawnEntities[i]);
            }
        }
    }

    // Centralized spawn function
    void SpawnEntity(SpawnInfo spawn)
    {
        Vector3 spawnPosition = new Vector3(
            GetRandomPosition(spawn.xRange),
            GetRandomPosition(spawn.yRange),
            0f
        );

        Instantiate(spawn.entity, spawnPosition, Quaternion.identity);
    }

    // Helper function to get a randomized position within the given range
    float GetRandomPosition(Vector2 range)
    {
        return Mathf.Round(Random.Range(range.x, range.y)) / 2f;
    }
}
