using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for spawning obstacle game objects at set intervals.
public class ObstacleSpawner : MonoBehaviour
{
    // Array holding different obstacle prefabs to spawn.
    public GameObject[] obstacleArray;

    // Initial delay (in seconds) before the first obstacle is spawned.
    public float initialDelay = 1f;

    // Time interval (in seconds) between subsequent obstacle spawns.
    public float spawnInterval = 3.5f;

    // Start is called before the first frame update.
    void Start()
    {
        // Schedule the SpawnEnemy method to be called repeatedly.
        // It will start after 'initialDelay' seconds and then repeat every 'spawnInterval' seconds.
        InvokeRepeating("SpawnEnemy", initialDelay, spawnInterval);
    }

    // Method that spawns a random obstacle from the obstacleArray.
    void SpawnEnemy()
    {
        // Calculate a random spawn position:
        // x is fixed at 12 (off-screen to the right),
        // y is a random value (rounded and divided by 2 for discrete vertical positions),
        // z is fixed at 0.
        Vector3 spawnPosition = new Vector3(12f, (Mathf.Round(Random.Range(-4f, 7f))) / 2f, 0f);

        // Select a random index for the obstacleArray.
        int i = Random.Range(0, obstacleArray.Length);

        // Instantiate the chosen obstacle at the computed spawn position with no rotation.
        Instantiate(obstacleArray[i], spawnPosition, Quaternion.identity);
    }
}
