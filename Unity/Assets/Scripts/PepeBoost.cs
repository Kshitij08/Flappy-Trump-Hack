using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepeBoost : MonoBehaviour
{
    // Public variables controlling movement and audio settings.
    public float speed = 2f;                   // Movement speed of the PepeBoost power-up.
    public AudioSource audioSource;            // Audio source for playing sound effects.
    public AudioClip audioClip;                // Sound effect clip for the collision event.

    // Private flag to ensure the collision is processed only once.
    private bool isCoillided = false;

    // Private cached references for performance optimization.
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update.
    private void Start()
    {
        // Cache components for performance.
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set the material for this power-up based on the current PepeBoost NFT level.
        // The material is fetched from GameManager's nftMaterialArrayList using the current level index.
        spriteRenderer.material = GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.pepeBoostNFTCurrentLevel - 1];
    }

    // Update is called once per frame.
    void Update()
    {
        // Move the power-up leftwards toward x = -12 while keeping its current y and z positions.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // If the power-up moves off-screen (x <= -12) and hasn't been collided with, destroy it.
        if (transform.position.x <= -12f && !isCoillided)
        {
            Destroy(gameObject);
        }
    }

    // Called when another collider enters the trigger collider attached to this power-up.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Player".
        if (other.CompareTag("Player"))
        {
            isCoillided = true; // Mark as collided to avoid duplicate processing.

            // Calculate the duration of the power-up effect.
            // The reset time is 5 seconds multiplied by the NFT multiplier for the current level.
            float resetTime = 5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.pepeBoostNFTCurrentLevel - 1];

            // Activate the PepeBoost effect on the player.
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.isPepeBoost = true;
            }

            // Start the coroutine to reset the player's PepeBoost state after the calculated reset time.
            StartCoroutine(GameManager.Instance.playerController.ResetPepeBoost(resetTime));

            // Play the collision sound effect.
            audioSource.PlayOneShot(audioClip);

            // Disable the visual representation and collider of the power-up.
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;

            // Increment the usage count of the PepeBoost NFT.
            GameManager.Instance.web3Manager.pepeBoostNFTCount += 1;

            // Destroy the power-up object after a delay (resetTime + 1 seconds) to allow audio playback.
            Destroy(gameObject, resetTime + 1);
        }
    }
}
