using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrumpCoin : MonoBehaviour
{
    // Duration before the coin slows down and reverses direction.
    public float slowdownDuration = 1.5f;
    // Speed at which the coin reverses its movement.
    public float reverseSpeed = 5f;

    // Cached reference to the Rigidbody2D component.
    private Rigidbody2D rb;

    // World-space boundaries of the camera.
    Vector3 minScreenBounds;
    Vector3 maxScreenBounds;

    // Determines if the coin is allowed to reverse its velocity.
    public bool canReverse = true;

    // Audio components for damage feedback.
    public AudioSource audioSource;
    public AudioClip enemyDamageAudioClip; // Played when an enemy is damaged.
    public AudioClip playerDamageAudioClip;  // Played when the player is damaged.

    // Called before the first frame update.
    void Start()
    {
        // Cache the Rigidbody2D component.
        rb = GetComponent<Rigidbody2D>();
        // Begin the coroutine to slow down and reverse the coin.
        StartCoroutine(SlowAndReverse());

        // Calculate the camera's world bounds for later collision checks.
        minScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        maxScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
    }

    // Update is called once per frame.
    void Update()
    {
        // Destroy the coin if it moves outside horizontal bounds (-12 or 12).
        if (transform.position.x <= -12f || transform.position.x >= 12f)
        {
            Destroy(gameObject);
        }
    }

    // Coroutine to slow down the coin and then reverse its velocity.
    IEnumerator SlowAndReverse()
    {
        if (canReverse)
        {
            // Wait for the defined slowdown duration.
            yield return new WaitForSeconds(slowdownDuration);
            // Reverse the velocity while maintaining the set reverse speed.
            rb.velocity = -rb.velocity.normalized * reverseSpeed;
        }
        else
        {
            yield return null;
        }
    }

    // Handles collision with other objects.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Process collision with objects tagged as "Enemy".
        if (other.CompareTag("Enemy"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Destroy the enemy and this coin.
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }

        // Process collision with objects tagged as "Obstacle".
        if (other.CompareTag("Obstacle"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Update player score based on market mode and obstacle type.
                if (GameManager.Instance.isBullMarket)
                {
                    if (other.gameObject.name.Contains("NGMI"))
                    {
                        GameManager.Instance.UpdateScore(5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.nGMINFTCurrentLevel - 1]);
                    }
                    else if (other.gameObject.name.Contains("Paper"))
                    {
                        GameManager.Instance.UpdateScore(5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.paperHandsNFTCurrentLevel - 1]);
                    }
                }
                else // Bear market mode.
                {
                    if (other.gameObject.name.Contains("NGMI"))
                    {
                        GameManager.Instance.UpdateScore(25 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.nGMINFTCurrentLevel - 1]);
                    }
                    else if (other.gameObject.name.Contains("Paper"))
                    {
                        GameManager.Instance.UpdateScore(25 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.paperHandsNFTCurrentLevel - 1]);
                    }
                }

                // Play enemy damage sound.
                audioSource.PlayOneShot(enemyDamageAudioClip);

                // Disable obstacle's sprite and its PolygonCollider2D.
                other.GetComponent<SpriteRenderer>().enabled = false;
                other.GetComponent<PolygonCollider2D>().enabled = false;
                // Activate the obstacle's particle effect.
                other.GetComponent<ObstacleScript>().particleEffectGO.SetActive(true);
                // Destroy the obstacle after a short delay.
                Destroy(other.gameObject, 1f);
                // Destroy the first child of the obstacle (if it exists).
                Destroy(other.transform.GetChild(0).gameObject);

                // Disable coin's own sprite and collider to prevent further collisions.
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;

                // Increment NFT usage counters based on obstacle type.
                if (other.gameObject.name.Contains("NGMI"))
                {
                    GameManager.Instance.web3Manager.nGMINFTCount += 1;
                }
                else if (other.gameObject.name.Contains("Paper"))
                {
                    GameManager.Instance.web3Manager.paperHandsNFTCount += 1;
                }

                // Destroy this coin after a delay.
                Destroy(gameObject, 1f);
            }
        }

        // Process collision with objects tagged as "FUDMonster".
        if (other.CompareTag("FUDMonster"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Update score based on market mode.
                if (GameManager.Instance.isBullMarket)
                {
                    GameManager.Instance.UpdateScore(10 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.fUDNFTCurrentLevel - 1]);
                }
                else
                {
                    GameManager.Instance.UpdateScore(50 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.fUDNFTCurrentLevel - 1]);
                }

                audioSource.PlayOneShot(enemyDamageAudioClip);

                // Disable the FUDMonster's sprite and collider.
                other.GetComponent<SpriteRenderer>().enabled = false;
                other.GetComponent<BoxCollider2D>().enabled = false;
                // Activate its particle effect.
                other.GetComponent<FUDMonster>().particleEffectGO.SetActive(true);
                // Destroy the monster after a delay.
                Destroy(other.gameObject, 1f);
                // Destroy its first child.
                Destroy(other.transform.GetChild(0).gameObject);

                // Disable coin visuals and collider.
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;

                // Increment FUD NFT count.
                GameManager.Instance.web3Manager.fUDNFTCount += 1;

                // Destroy the coin.
                Destroy(gameObject, 1f);
            }
        }

        // Process collision with objects tagged as "SECGary".
        if (other.CompareTag("SECGary"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Update score based on market mode.
                if (GameManager.Instance.isBullMarket)
                {
                    GameManager.Instance.UpdateScore(15 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.garyNFTCurrentLevel - 1]);
                }
                else
                {
                    GameManager.Instance.UpdateScore(75 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.garyNFTCurrentLevel - 1]);
                }

                audioSource.PlayOneShot(enemyDamageAudioClip);

                // Disable SECGary's sprite and collider.
                other.GetComponent<SpriteRenderer>().enabled = false;
                other.GetComponent<BoxCollider2D>().enabled = false;
                // Activate particle effect.
                other.GetComponent<SECGary>().particleEffectGO.SetActive(true);
                // Destroy SECGary after a delay.
                Destroy(other.gameObject, 1f);
                // Destroy its first child.
                Destroy(other.transform.GetChild(0).gameObject);

                // Disable coin visuals and collider.
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;

                // Increment Gary NFT count.
                GameManager.Instance.web3Manager.garyNFTCount += 1;

                // Destroy this coin.
                Destroy(gameObject, 1f);
            }
        }

        // Process collision with objects tagged as "SECBullet".
        if (other.CompareTag("SECBullet"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Disable the SECBullet's visuals and collider.
                other.GetComponent<SpriteRenderer>().enabled = false;
                other.GetComponent<BoxCollider2D>().enabled = false;
                // Destroy the SECBullet after a delay.
                Destroy(other.gameObject, 1f);

                // Disable coin visuals and collider.
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                // Destroy the coin.
                Destroy(gameObject, 1f);
            }
        }

        // Process collision with objects tagged as "BitcoinFly".
        if (other.CompareTag("BitcoinFly"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Instruct the BitcoinFly to take damage.
                other.GetComponent<BitcoinFlyController>().TakeDamage();
                // Destroy this coin.
                Destroy(gameObject);
            }
        }

        // Process collision with objects tagged as "Melania".
        if (other.CompareTag("Melania"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Instruct Melania to take damage.
                other.GetComponent<Melania>().TakeDamage();
                // Destroy this coin.
                Destroy(gameObject);
            }
        }

        // Process collision with objects tagged as "Elon".
        if (other.CompareTag("Elon"))
        {
            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                // Instruct Elon to take damage.
                other.GetComponent<Elon>().TakeDamage();
                // Destroy this coin.
                Destroy(gameObject);
            }
        }

        // Process collision with objects tagged as "Player".
        if (other.CompareTag("Player"))
        {
            // Only affect the player if the game is not in Bull Market mode and the coin is moving left.
            if (!GameManager.Instance.isBullMarket)
            {
                if (rb.velocity.x <= 0)
                {
                    PlayerController player = other.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        // Play player damage sound and apply damage.
                        audioSource.PlayOneShot(playerDamageAudioClip);
                        player.TakeDamage(1);
                    }
                    // Destroy the coin.
                    Destroy(gameObject);
                }
            }
        }
    }

    // Checks if the sprite is within the camera's view bounds.
    private bool IsWithinCameraBounds(SpriteRenderer spriteRenderer)
    {
        // Get the sprite's bounds in world space.
        Bounds spriteBounds = spriteRenderer.bounds;

        // Verify that the sprite overlaps the camera's horizontal and vertical extents.
        return spriteBounds.min.x < maxScreenBounds.x && spriteBounds.max.x > minScreenBounds.x &&
               spriteBounds.min.y < maxScreenBounds.y && spriteBounds.max.y > minScreenBounds.y;
    }
}
