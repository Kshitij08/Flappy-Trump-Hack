using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SECGary : MonoBehaviour
{
    // Movement speed for the enemy.
    public float speed = 2f;

    // Prefab of the bullet to be fired.
    public GameObject bulletPrefab;
    // Optional spawn point for bullets; if not set, enemy's position is used.
    public Transform bulletSpawnPoint;

    // Random interval range (in seconds) between consecutive bullet shots.
    public float minFireInterval = 1f;
    public float maxFireInterval = 3f;

    // Bullet movement settings.
    public float bulletSpeed = 5f;
    // Direction in which the bullet will be fired (normalized internally).
    public Vector2 bulletDirection = Vector2.right;

    // Delay (in seconds) before enabling the enemy's collider.
    public float delay = 0.5f;

    // Cached reference to the BoxCollider2D component.
    private BoxCollider2D boxCollider2D;

    // Timer tracking elapsed time.
    public float time;

    // (Optional) Particle effect GameObject, could be used for visual feedback.
    public GameObject particleEffectGO;

    // Start is called before the first frame update.
    void Start()
    {
        // Set the enemy's material based on the current NFT level from GameManager.
        GetComponent<SpriteRenderer>().material =
            GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.garyNFTCurrentLevel - 1];

        // Cache the BoxCollider2D component.
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            // Disable the collider initially.
            boxCollider2D.enabled = false;
            // Enable the collider after a delay.
            StartCoroutine(EnableCollider());
        }
        else
        {
            Debug.LogWarning("BoxCollider2D component not found on " + gameObject.name);
        }

        // Start the coroutine that fires bullets at random intervals.
        StartCoroutine(FireBulletRoutine());
    }

    // Update is called once per frame.
    private void Update()
    {
        // Update the timer.
        time += Time.deltaTime;

        // Move the enemy leftward toward x = -9 while preserving its y and z positions.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-9f, transform.position.y, 0f), speed * Time.deltaTime);
    }

    // Coroutine that repeatedly fires bullets at random intervals.
    IEnumerator FireBulletRoutine()
    {
        while (true)
        {
            // Wait for a random duration between the specified minimum and maximum intervals.
            float waitTime = Random.Range(minFireInterval, maxFireInterval);
            yield return new WaitForSeconds(waitTime);

            // Determine the spawn position: use the bulletSpawnPoint if assigned; otherwise, use the enemy's position.
            Vector3 spawnPosition = bulletSpawnPoint != null ? bulletSpawnPoint.position : transform.position;

            // Instantiate the bullet prefab at the spawn position with no rotation.
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // If the bullet has a Rigidbody2D, set its velocity based on the given direction and speed.
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = bulletDirection.normalized * bulletSpeed;
            }
        }
    }

    // Coroutine to enable the collider after a short delay.
    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D.enabled = true;
    }
}
