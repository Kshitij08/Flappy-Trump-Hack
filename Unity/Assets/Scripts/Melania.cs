using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melania : MonoBehaviour
{
    // --- Movement Timing Parameters ---
    [Header("Timing")]
    public float minMoveDuration = 2f;  // Minimum duration Melania moves
    public float maxMoveDuration = 4f;  // Maximum duration Melania moves
    public float minStopDuration = 2f;  // Minimum duration Melania stops
    public float maxStopDuration = 3f;  // Maximum duration Melania stops

    // --- Movement and Rotation Settings ---
    [Header("Movement")]
    public float moveSpeed = 5f;         // Speed at which Melania moves
    public float rotationSpeed = 180f;   // Rotation speed while moving

    // --- Movement Boundaries ---
    [Header("Boundaries")]
    public float minX = -8f;             // Left boundary limit
    public float maxX = 9f;              // Right boundary limit
    public float minY = -3.75f;          // Bottom boundary limit
    public float maxY = 3.75f;           // Top boundary limit

    // --- Internal State Variables ---
    private Vector3 targetPosition;      // Stores the next movement target position
    private Quaternion originalRotation; // Stores the original rotation
    private enum State { Moving, Stopped } // Enum for movement states
    private State currentState;          // Tracks the current movement state

    // --- Collider Management ---
    public float delay = 0f;             // Delay before enabling the collider
    private BoxCollider2D boxCollider2D; // Reference to the BoxCollider2D

    // --- Timer & Health Properties ---
    public float time;                   // Tracks the elapsed time
    public int health = 2;                // Health value
    public GameObject[] healthGO;         // UI representation of health

    // --- Audio & Effects ---
    public AudioSource audioSource;       // Audio source component
    public AudioClip audioClip;           // Sound clip to play on impact
    public GameObject particleEffectGO;   // Particle effect when destroyed

    void Start()
    {
        // Assign the NFT material based on the current NFT level
        gameObject.GetComponent<SpriteRenderer>().material =
            GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.melaniaNFTCurrentLevel - 1];

        // Get and disable the BoxCollider2D at start
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false;
            StartCoroutine(EnableCollider());
        }
        else
        {
            Debug.LogWarning("BoxCollider2D component not found on " + gameObject.name);
        }

        // Store original rotation and initialize movement state
        originalRotation = transform.rotation;
        currentState = State.Moving;

        // Set initial movement target
        ChooseNewTarget();

        // Start movement loop
        StartCoroutine(StateLoop());

        // Assign player effect if found
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerController>().isMelania = true;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    // Coroutine to alternate between moving and stopping
    IEnumerator StateLoop()
    {
        while (true)
        {
            // ----- MOVING STATE -----
            float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);
            float moveTimer = 0f;

            while (moveTimer < moveDuration)
            {
                // Move towards the target position
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Rotate while moving
                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

                // If close to the target, pick a new one
                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    ChooseNewTarget();
                }

                moveTimer += Time.deltaTime;
                yield return null;
            }

            // ----- STOPPING STATE -----
            float stopDuration = Random.Range(minStopDuration, maxStopDuration);
            float stopTimer = 0f;
            Quaternion currentRotation = transform.rotation; // Store rotation before stopping
            Vector3 stopPosition = transform.position; // Store position before stopping

            while (stopTimer < stopDuration)
            {
                transform.position = stopPosition; // Keep position fixed
                transform.rotation = Quaternion.Slerp(currentRotation, originalRotation, 2f * stopTimer / stopDuration); // Gradually reset rotation
                stopTimer += Time.deltaTime;
                yield return null;
            }

            transform.rotation = originalRotation; // Ensure perfect reset
            ChooseNewTarget(); // Set new movement target
            currentState = State.Moving; // Resume movement
        }
    }

    // Picks a new target position within boundaries
    void ChooseNewTarget()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        targetPosition = new Vector3(randomX, randomY, transform.position.z);
    }

    // Handles health reduction and destruction
    public void TakeDamage()
    {
        if (health == 2)
        {
            health -= 1;
            healthGO[1].GetComponent<SpriteRenderer>().color = new Color(0.68f, 0.42f, 0.4f, 1f);
            audioSource.PlayOneShot(audioClip);
        }
        else if (health == 1)
        {
            health -= 1;
            healthGO[0].GetComponent<SpriteRenderer>().color = new Color(0.68f, 0.42f, 0.4f, 1f);

            // Calculate score based on market condition
            int scoreMultiplier = GameManager.Instance.isBullMarket ? 20 : 100;
            GameManager.Instance.UpdateScore(scoreMultiplier *
                GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.melaniaNFTCurrentLevel - 1]);

            // Disable effect on player if found
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.GetComponent<PlayerController>().isMelania = false;
            }

            audioSource.PlayOneShot(audioClip);

            // Disable sprite and collider
            Destroy(transform.GetChild(0).gameObject);
            GetComponent<SpriteRenderer>().enabled = false;
            boxCollider2D.enabled = false;

            // Play particle effect
            particleEffectGO.SetActive(true);

            // Increase NFT count
            GameManager.Instance.web3Manager.melaniaNFTCount += 1;

            // Destroy the object after a delay
            Destroy(gameObject, 1f);
        }
    }

    // Enables the collider after a delay
    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D.enabled = true;
    }
}
