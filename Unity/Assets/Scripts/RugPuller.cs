using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RugPuller : MonoBehaviour
{
    // Movement speed for the RugPuller.
    public float speed = 1f;

    // Tracks the elapsed time.
    public float time;

    // Ghost image settings:
    // Interval between spawning ghost images.
    public float ghostSpawnInterval = 0.1f;
    // Duration for each ghost image to fade out.
    public float ghostFadeTime = 0.5f;
    // Initial color (with transparency) for the ghost image.
    public Color ghostColor = new Color(1f, 1f, 1f, 0.5f);

    // Cached SpriteRenderer component.
    private SpriteRenderer spriteRenderer;
    // Reference to the BoxCollider2D component.
    public BoxCollider2D boxCollider2D;

    // Called before the first frame update.
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Begin the ghost image spawning coroutine.
        StartCoroutine(SpawnGhosts());
    }

    // Called once per frame.
    void Update()
    {
        // Update elapsed time.
        time += Time.deltaTime;

        // Move the RugPuller leftward toward x = -12.
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        // If the RugPuller moves off-screen, destroy it after 1 second.
        if (transform.position.x <= -12f)
        {
            Destroy(gameObject, 1f);
        }
    }

    // Coroutine that continuously spawns ghost images at fixed intervals.
    IEnumerator SpawnGhosts()
    {
        while (true)
        {
            SpawnGhost();
            yield return new WaitForSeconds(ghostSpawnInterval);
        }
    }

    // Spawns a ghost image of the current RugPuller.
    void SpawnGhost()
    {
        // Create a new GameObject for the ghost.
        GameObject ghost = new GameObject("Ghost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        // Destroy the ghost after the fade duration.
        Destroy(ghost, ghostFadeTime);

        // Add a SpriteRenderer to the ghost and copy the main sprite and sorting settings.
        SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();
        ghostSR.sprite = spriteRenderer.sprite;
        ghostSR.sortingLayerID = spriteRenderer.sortingLayerID;
        ghostSR.sortingOrder = spriteRenderer.sortingOrder - 1;  // Place the ghost behind the main sprite.
        ghostSR.color = ghostColor;  // Set the initial ghost color.

        // Start the fade-out coroutine for the ghost.
        StartCoroutine(FadeAndDestroy(ghostSR));
    }

    // Coroutine that gradually fades the ghost image's alpha to 0.
    IEnumerator FadeAndDestroy(SpriteRenderer ghostSR)
    {
        float elapsed = 0f;
        Color startColor = ghostSR.color;
        while (elapsed < ghostFadeTime)
        {
            if (ghostSR != null)
            {
                elapsed += Time.deltaTime;
                // Interpolate the alpha from its start value down to 0.
                float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / ghostFadeTime);
                ghostSR.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }
            yield return null;
        }
    }

    // Handles collision with other colliders.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Player".
        if (other.CompareTag("Player"))
        {
            // Retrieve the PlayerController and inflict damage.
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            // Temporarily disable this object's collider to prevent multiple hits.
            StartCoroutine(DisableCollider());
        }
    }

    // Coroutine to disable the collider for 2 seconds and then re-enable it.
    IEnumerator DisableCollider()
    {
        boxCollider2D.enabled = false;
        yield return new WaitForSeconds(2f);
        boxCollider2D.enabled = true;
    }
}
