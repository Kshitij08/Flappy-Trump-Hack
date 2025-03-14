using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondHands : MonoBehaviour
{
    public float speed = 2f; // Movement speed to the left

    // Audio components
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Prevent multiple triggers
    private bool isCollided = false;

    // Cached references
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Start()
    {
        // Cache components for better performance
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Assign NFT material based on the current level
        int nftLevel = GameManager.Instance.web3Manager.diamondHandsNFTCurrentLevel - 1;
        spriteRenderer.material = GameManager.Instance.nftMaterialArrayList[nftLevel];
    }

    void Update()
    {
        // Move the object left towards -12 on the x-axis
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // Destroy if out of bounds and has not collided
        if (transform.position.x <= -12f && !isCollided)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with the player
        if (other.CompareTag("Player") && !isCollided)
        {
            isCollided = true;

            // Retrieve the player's controller and apply Diamond Hands effect
            if (other.TryGetComponent(out PlayerController player))
            {
                float resetTime = 5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.diamondHandsNFTCurrentLevel - 1];

                player.isDiamondHands = true;
                StartCoroutine(GameManager.Instance.playerController.ResetDiamondHands(resetTime));

                // Play sound effect
                audioSource.PlayOneShot(audioClip);

                // Disable visuals and collider
                spriteRenderer.enabled = false;
                boxCollider.enabled = false;

                // Increment NFT count
                GameManager.Instance.web3Manager.diamondHandsNFTCount++;

                // Destroy the object after the reset time + 1 second
                Destroy(gameObject, resetTime + 1);
            }
        }
    }
}
