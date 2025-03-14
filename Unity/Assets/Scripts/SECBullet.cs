using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SECBullet : MonoBehaviour
{
    // Start is called before the first frame update.
    // No initialization is needed for SECBullet.
    void Start()
    {
    }

    // Update is called once per frame.
    // This method checks if the bullet has moved off-screen.
    void Update()
    {
        // If the bullet's x-position is beyond 12, it is off-screen and should be destroyed.
        if (transform.position.x >= 12f)
        {
            Destroy(gameObject);
        }
    }

    // This method is called when another collider enters this bullet's trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player.
        if (other.CompareTag("Player"))
        {
            // Get the PlayerController component from the colliding object.
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Reduce player's health by 1.
                player.TakeDamage(1);
            }
            // Destroy the bullet after it hits the player.
            Destroy(gameObject);
        }
    }
}
