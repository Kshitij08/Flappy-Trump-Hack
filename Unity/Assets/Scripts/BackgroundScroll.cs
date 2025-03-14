using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    // Speed at which the background moves to the left.
    public float scrollSpeed = 2.0f;

    // The width of the background sprite (set in Start).
    private float spriteWidth;

    // Cached reference to the SpriteRenderer.
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Cache the SpriteRenderer component.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the SpriteRenderer exists before accessing its bounds.
        if (spriteRenderer != null)
        {
            spriteWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            Debug.LogError($"SpriteRenderer not found on {gameObject.name}. Disabling script.");
            enabled = false; // Disable the script to prevent errors.
        }
    }

    void Update()
    {
        // Move the background left at a constant speed.
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // If the background has completely moved off-screen, reposition it to the right.
        if (transform.position.x < -spriteWidth)
        {
            transform.position = new Vector3(transform.position.x + spriteWidth * 3,
                                             transform.position.y,
                                             transform.position.z);
        }
    }
}
