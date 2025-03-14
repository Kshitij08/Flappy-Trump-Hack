using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed = 0.5f;

    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(12f, transform.position.y, 0f), speed * Time.deltaTime);

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
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
