using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float slowdownDuration = 1.5f;
    public float reverseSpeed = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(SlowAndReverse());
    }

    IEnumerator SlowAndReverse()
    {
        yield return new WaitForSeconds(slowdownDuration);
        rb.velocity = -rb.velocity.normalized * reverseSpeed;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {

            if (IsWithinCameraBounds(other.GetComponent<SpriteRenderer>()))
            {
                Destroy(other.gameObject); // Destroy enemy
                Destroy(gameObject); // Destroy fireball
            }

        }


        if (other.CompareTag("Player"))
        {
        
            if(rb.velocity.x <= 0)
            {
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(1); // Reduce health
                }
                Destroy(gameObject); // Destroy fireball
            }
            

        }

    }

    private bool IsWithinCameraBounds(SpriteRenderer spriteRenderer)
    {
        // Get the camera's world bounds
        Vector3 minScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Get the object's bounds
        Bounds spriteBounds = spriteRenderer.bounds;

        // Check if the sprite is inside the camera's bounds
        return spriteBounds.min.x < maxScreenBounds.x && spriteBounds.max.x > minScreenBounds.x &&
               spriteBounds.min.y < maxScreenBounds.y && spriteBounds.max.y > minScreenBounds.y;
    }

}

