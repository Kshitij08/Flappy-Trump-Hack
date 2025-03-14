using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    // Speed at which the obstacle moves left.
    public float speed = 2f;

    // Delay (in seconds) before enabling the collider after the game starts.
    public float delay = 0.5f;

    // Reference to the PolygonCollider2D attached to this obstacle.
    private PolygonCollider2D polygonCollider2D;

    // Tracks elapsed time since the obstacle is active.
    public float time;

    // Audio source and clip for playing a sound on collision.
    public AudioSource audioSource;
    public AudioClip audioClip;

    // GameObject for the particle effect to be activated on collision.
    public GameObject particleEffectGO;

    // Start is called before the first frame update.
    private void Start()
    {
        // Set the material based on the object's name.
        // If the object name contains "NGMI", apply the corresponding NGMI NFT material.
        if (gameObject.name.Contains("NGMI"))
        {
            gameObject.GetComponent<SpriteRenderer>().material =
                GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.nGMINFTCurrentLevel - 1];
        }
        // If the object name contains "Paper", apply the corresponding Paper Hands NFT material.
        else if (gameObject.name.Contains("Paper"))
        {
            gameObject.GetComponent<SpriteRenderer>().material =
                GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.paperHandsNFTCurrentLevel - 1];
        }

        // Get the PolygonCollider2D component attached to this game object.
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        if (polygonCollider2D != null)
        {
            // Disable the collider initially.
            polygonCollider2D.enabled = false;
            // Start the coroutine to enable the collider after a delay.
            StartCoroutine(EnableCollider());
        }
        else
        {
            // Warn if the PolygonCollider2D component is missing.
            Debug.LogWarning("PolygonCollider2D component not found on " + gameObject.name);
        }
    }

    // Update is called once per frame.
    private void Update()
    {
        // Increment elapsed time.
        time += Time.deltaTime;

        // Move the obstacle left toward x = -12 while maintaining its current y and z positions.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // If the obstacle has moved out of bounds, destroy it.
        if (transform.position.x <= -12f)
        {
            Destroy(gameObject);
        }
    }

    // Called when another collider enters the obstacle's trigger collider.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            // Get the player's controller component and inflict damage.
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1); // Reduce player's health by 1.
            }

            // Play the collision sound effect.
            audioSource.PlayOneShot(audioClip);

            // Destroy the first child game object (assumed to be a visual element of the obstacle).
            Destroy(transform.GetChild(0).gameObject);

            // Activate particle effects to show visual feedback.
            particleEffectGO.SetActive(true);

            // Hide the obstacle visually and disable its collider to prevent further collisions.
            GetComponent<SpriteRenderer>().enabled = false;
            polygonCollider2D.enabled = false;

            // Destroy this obstacle after a short delay (allowing the sound and particle effects to play).
            Destroy(gameObject, 1f);
        }
    }

    // Coroutine to enable the collider after a specified delay.
    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        polygonCollider2D.enabled = true;
    }
}
