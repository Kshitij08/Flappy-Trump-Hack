using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAGAHat : MonoBehaviour
{
    // Movement speed of the object
    public float speed = 2f;

    // Audio components for collision sound
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Flag to track if the object has already collided with the player
    private bool isCollided = false;

    void Start()
    {
        // Assigns the appropriate NFT material to the object based on the player's NFT level
        gameObject.GetComponent<SpriteRenderer>().material =
            GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.mAGAHatNFTCurrentLevel - 1];
    }

    void Update()
    {
        // Move the object toward the left side of the screen
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // Destroy the object once it reaches -12f on the x-axis if it hasn't collided
        if (transform.position.x <= -12f && !isCollided)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object collides with the player
        if (other.CompareTag("Player"))
        {
            isCollided = true;

            // Calculate the reset duration based on the NFT level multiplier
            float resetTime = 5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.mAGAHatNFTCurrentLevel - 1];

            // Apply MAGA Hat effect to the player
            other.GetComponent<PlayerController>().isMAGAHat = true;
            StartCoroutine(GameManager.Instance.playerController.ResetMAGAHat(resetTime));

            // Play collision sound
            audioSource.PlayOneShot(audioClip);

            // Hide the sprite and disable collision after pickup
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            // Increment the NFT count for MAGA Hat
            GameManager.Instance.web3Manager.mAGAHatNFTCount += 1;

            // Destroy the object after the effect duration
            Destroy(gameObject, resetTime + 1);
        }
    }
}
