using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airdrop : MonoBehaviour
{
    // Defines the target region from which a random destination will be chosen.
    public Vector2 targetMin = new Vector2(-14f, -6f);
    public Vector2 targetMax = new Vector2(-6f, -6f);

    // Movement speed of the airdrop in units per second.
    public float moveSpeed = 1f;

    // The target position where the airdrop will move.
    private Vector3 targetPosition;

    // Audio-related components for playing sound effects.
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Cached references to improve performance.
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    void Start()
    {
        // Cache frequently used components.
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Assign appropriate material based on NFT level.
        int nftLevel = GameManager.Instance.web3Manager.airdropNFTCurrentLevel - 1;
        spriteRenderer.material = GameManager.Instance.nftMaterialArrayList[nftLevel];

        // Select a random target position within the specified region.
        float targetX = Random.Range(targetMin.x, targetMax.x);
        float targetY = Random.Range(targetMin.y, targetMax.y);
        targetPosition = new Vector3(targetX, targetY, transform.position.z);
    }

    void Update()
    {
        // Move towards the target position at a fixed speed.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Destroy the airdrop if it goes out of bounds.
        if (transform.position.x <= -12f || transform.position.y <= -12f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the airdrop collides with the player.
        if (other.CompareTag("Player"))
        {
            // Get the NFT multiplier and score based on market condition.
            float nftMultiplier = GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.airdropNFTCurrentLevel - 1];
            int scoreToAdd = GameManager.Instance.isBullMarket ? 50 : 250;

            // Update player score using the appropriate multiplier.
            GameManager.Instance.UpdateScore(scoreToAdd * nftMultiplier);

            // Play the airdrop pickup sound.
            audioSource.PlayOneShot(audioClip);

            // Hide the airdrop visually and disable its collider.
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;

            // Increment airdrop count in Web3 manager.
            GameManager.Instance.web3Manager.airdropNFTCount++;

            // Destroy the game object after a delay to allow sound to play.
            Destroy(gameObject, 1f);
        }
    }
}
