using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableGainsShield : MonoBehaviour
{
    // Movement speed of the power-up.
    public float speed = 2f;

    // Audio components for playing collision sound.
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Flag to ensure the collision is processed only once.
    private bool isCoillided = false;

    // Cached references for better performance.
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    // Initialization.
    private void Start()
    {
        // Cache the SpriteRenderer and BoxCollider2D components.
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Set the power-up's material based on the current NFT level.
        // The material is chosen from the GameManager's nftMaterialArrayList using the current shield NFT level.
        spriteRenderer.material = GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.uSDCShieldNFTCurrentLevel - 1];
    }

    // Update is called once per frame.
    void Update()
    {
        // Move the power-up leftward toward x = -12 while preserving its y and z positions.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // If the power-up moves off-screen and hasn't been collided, destroy it.
        if (transform.position.x <= -12f && !isCoillided)
        {
            Destroy(gameObject);
        }
    }

    // Handles collision with other colliders.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            // Mark the power-up as collided to avoid multiple triggers.
            isCoillided = true;

            // Calculate the shield effect duration using the NFT multiplier.
            float resetTime = 5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.uSDCShieldNFTCurrentLevel - 1];

            // Activate the shield effect on the player.
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.isStableGainShield = true;
            }
            // Start the coroutine to reset the shield effect after the duration.
            StartCoroutine(GameManager.Instance.playerController.ResetStableGainShield(resetTime));

            // Play the collision sound effect.
            audioSource.PlayOneShot(audioClip);

            // Hide the power-up's visual representation and disable its collider.
            spriteRenderer.enabled = false;
            boxCollider.enabled = false;

            // Increment the usage count for this NFT shield.
            GameManager.Instance.web3Manager.uSDCShieldNFTCount += 1;

            // Destroy the power-up after the reset duration plus an extra second to allow all effects to finish.
            Destroy(gameObject, resetTime + 1);
        }
    }
}
