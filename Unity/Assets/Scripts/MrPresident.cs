using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrPresident : MonoBehaviour
{
    // Movement Settings: Controls the speed of the object.
    [Header("Movement Settings")]
    public float speed = 2f; // Speed at which the object moves.

    // Tags to Destroy: List of object tags to search for and destroy upon collision.
    [Header("Tags to Destroy")]
    public string[] tagsToDestroy = new string[]
    {
        "BitcoinFly", "Elon", "FUDMonster", "Melania", "Obstacle",
        "RugPuller", "SECBullet", "SECGary", "TrumpCoin"
    };

    // Audio Settings: Holds reference to the audio source and the sound clip for collision.
    [Header("Audio Settings")]
    public AudioSource audioSource;  // Audio source component for playing sounds.
    public AudioClip audioClip;      // Collision sound effect.

    // Flag to ensure collision events are handled only once.
    private bool isCollided = false;

    // Start is called before the first frame update.
    private void Start()
    {
        // Set the material for this object's sprite renderer based on the current NFT level.
        // The material is chosen from an array in GameManager using the current NFT level index.
        gameObject.GetComponent<SpriteRenderer>().material =
            GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.nukeEmNFTCurrentLevel - 1];
    }

    // Update is called once per frame.
    private void Update()
    {
        // Move the object leftward towards x = -12, maintaining its current y and z position.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // If the object goes out of bounds (x <= -12) and hasn't collided, destroy it.
        if (transform.position.x <= -12f && !isCollided)
        {
            Destroy(gameObject);
        }
    }

    // Called when another collider enters this object's trigger collider.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Player".
        if (other.CompareTag("Player"))
        {
            isCollided = true; // Prevent further collision handling.
            // On collision with the player, destroy designated enemy objects and obstacles.
            DestroyEnemiesAndObstacles(other);
        }
    }

    /// <summary>
    /// Destroys all game objects with the specified tags, resets certain player states,
    /// plays a collision sound, hides this object visually, and then destroys it after a delay.
    /// </summary>
    /// <param name="other">The player's collider that triggered the collision.</param>
    public void DestroyEnemiesAndObstacles(Collider2D other)
    {
        // Loop through each tag in the tagsToDestroy array.
        foreach (string tag in tagsToDestroy)
        {
            // Find all game objects with the current tag.
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            // Destroy each found object.
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }

        // Reset specific player power-up states upon collision.
        other.GetComponent<PlayerController>().isElon = false;
        other.GetComponent<PlayerController>().isMelania = false;

        // Play the collision sound effect.
        audioSource.PlayOneShot(audioClip);

        // Hide the object's sprite and disable its collider to remove it from play visually.
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        // Increment the NFT usage count in the GameManager.
        GameManager.Instance.web3Manager.nukeEmNFTCount += 1;

        // Destroy this object after a short delay to allow the sound effect to play.
        Destroy(gameObject, 1f);
    }
}
