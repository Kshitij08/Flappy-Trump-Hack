using System.Collections;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    // Public power-up prefab variables to be assigned in the Inspector.
    public GameObject mrPresidentGO;
    public GameObject liquidityInjectionGO;
    public GameObject pepeBoostGO;
    public GameObject mAGAHatGO;
    public GameObject stableGainsShieldGO;
    public GameObject pumpAndDumpGO;
    public GameObject diamondHandsGO;
    public GameObject airdropGO;

    // Flag to control whether power-ups can be spawned.
    public bool canSapwn = true;

    // Start is called before the first frame update.
    void Start()
    {
        // Optionally, start the spawn routine coroutine if spawning is enabled.
        // Uncomment the following line to enable continuous spawning.
        // StartCoroutine(SpawnRoutine());
    }

    // Coroutine that spawns power-ups at random intervals.
    public IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for a random duration between 20 and 30 seconds.
            float waitTime = Random.Range(20f, 30f);
            yield return new WaitForSeconds(waitTime);

            // Randomly pick one of the eight spawn functions.
            int randomIndex = Random.Range(0, 8);
            switch (randomIndex)
            {
                case 0:
                    SpawnMrPresident();
                    break;
                case 1:
                    SpawnLiquidityInjection();
                    break;
                case 2:
                    SpawnPepeBoost();
                    break;
                case 3:
                    SpawnMAGAHat();
                    break;
                case 4:
                    SpawnStableGainsShield();
                    break;
                case 5:
                    SpawnPumpAndDump();
                    break;
                case 6:
                    SpawnDiamondHands();
                    break;
                case 7:
                    SpawnAirdrop();
                    break;
            }
        }
    }

    // Helper method to spawn a power-up at a random horizontal position.
    // For most power-ups, x is fixed at 12, and y is a random value (rounded to half increments).
    private void SpawnAtRandomHorizontalPosition(GameObject powerup)
    {
        float randomY = Mathf.Round(Random.Range(0f, 10f)) / 2f;
        Vector3 spawnPosition = new Vector3(12f, randomY, 0f);
        Instantiate(powerup, spawnPosition, Quaternion.identity);
    }

    // Spawns the Mr. President power-up.
    void SpawnMrPresident()
    {
        SpawnAtRandomHorizontalPosition(mrPresidentGO);
    }

    // Spawns the Liquidity Injection power-up.
    void SpawnLiquidityInjection()
    {
        SpawnAtRandomHorizontalPosition(liquidityInjectionGO);
    }

    // Spawns the Pepe Boost power-up.
    void SpawnPepeBoost()
    {
        SpawnAtRandomHorizontalPosition(pepeBoostGO);
    }

    // Spawns the MAGA Hat power-up.
    void SpawnMAGAHat()
    {
        SpawnAtRandomHorizontalPosition(mAGAHatGO);
    }

    // Spawns the Stable Gains Shield power-up.
    void SpawnStableGainsShield()
    {
        SpawnAtRandomHorizontalPosition(stableGainsShieldGO);
    }

    // Spawns the Pump and Dump power-up.
    void SpawnPumpAndDump()
    {
        SpawnAtRandomHorizontalPosition(pumpAndDumpGO);
    }

    // Spawns the Diamond Hands power-up.
    void SpawnDiamondHands()
    {
        SpawnAtRandomHorizontalPosition(diamondHandsGO);
    }

    // Spawns the Airdrop power-up at a different spawn location.
    // Here, the x position is randomized between 2 and 18 (rounded to half increments), and y is fixed at 7.5.
    void SpawnAirdrop()
    {
        float randomX = Mathf.Round(Random.Range(2f, 18f)) / 2f;
        Vector3 spawnPosition = new Vector3(randomX, 7.5f, 0f);
        Instantiate(airdropGO, spawnPosition, Quaternion.identity);
    }
}
