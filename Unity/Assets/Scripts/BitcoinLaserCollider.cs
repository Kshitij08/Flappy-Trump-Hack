using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitcoinLaserCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is the player.
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerController component and apply damage if found.
            if (other.TryGetComponent(out PlayerController player))
            {
                player.TakeDamage(5); // Reduce health
            }
        }
    }
}
