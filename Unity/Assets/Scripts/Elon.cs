using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elon : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f; // Speed of movement
    public Vector2 minBoundary = new Vector2(1.5f, 1.5f);
    public Vector2 maxBoundary = new Vector2(8.5f, 3.5f);

    [Header("Timing Settings")]
    public float minMoveTime = 2f, maxMoveTime = 4f; // Movement duration
    public float minIdleTime = 2f, maxIdleTime = 3f; // Idle duration

    private Vector3 targetPosition; // Next movement target
    private Quaternion initialRotation; // Stores initial rotation
    private enum DroneState { Moving, Idle }
    private DroneState currentState;

    // Delay before enabling the collider (in seconds)
    public float delay = 0.5f;
    private BoxCollider2D boxCollider2D;

    public float time; // Tracks elapsed time
    public int health = 3;
    public GameObject[] healthGO; // Health UI elements

    // Audio Components
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Particle Effect
    public GameObject particleEffectGO;

    // Cached player reference
    private PlayerController playerController;

    void Start()
    {
        // Assign NFT material based on the current level
        int nftLevel = GameManager.Instance.web3Manager.elonNFTCurrentLevel - 1;
        GetComponent<SpriteRenderer>().material = GameManager.Instance.nftMaterialArrayList[nftLevel];

        // Cache the BoxCollider2D component
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false; // Disable initially
            StartCoroutine(EnableCollider());
        }
        else
        {
            Debug.LogWarning($"BoxCollider2D component not found on {gameObject.name}");
        }

        // Store the initial rotation to maintain throughout movement
        initialRotation = transform.rotation;
        currentState = DroneState.Moving; // Start in moving state
        PickNewTarget(); // Set initial target
        StartCoroutine(DroneStateMachine()); // Start state machine

        // Find the player and assign the "isElon" effect
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerController.isElon = true;
        }
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D.enabled = true;
    }

    IEnumerator DroneStateMachine()
    {
        while (true)
        {
            if (currentState == DroneState.Moving)
            {
                float moveDuration = Random.Range(minMoveTime, maxMoveTime);
                float moveTimer = 0f;

                while (moveTimer < moveDuration)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                    // If near target, pick a new one
                    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                        PickNewTarget();

                    moveTimer += Time.deltaTime;
                    yield return null;
                }

                currentState = DroneState.Idle; // Transition to idle
            }
            else if (currentState == DroneState.Idle)
            {
                float idleDuration = Random.Range(minIdleTime, maxIdleTime);
                float idleTimer = 0f;
                Vector3 idlePosition = transform.position; // Hold position

                while (idleTimer < idleDuration)
                {
                    transform.position = idlePosition; // Stay in place
                    idleTimer += Time.deltaTime;
                    yield return null;
                }

                PickNewTarget();
                currentState = DroneState.Moving;
            }

            transform.rotation = initialRotation; // Maintain original rotation
            yield return null;
        }
    }

    // Picks a new random target position within the boundaries
    void PickNewTarget()
    {
        float randomX = Random.Range(minBoundary.x, maxBoundary.x);
        float randomY = Random.Range(minBoundary.y, maxBoundary.y);
        targetPosition = new Vector3(randomX, randomY, transform.position.z);
    }

    public void TakeDamage()
    {
        if (health > 0)
        {
            health--;
            int index = Mathf.Clamp(health, 0, 2);
            healthGO[index].GetComponent<SpriteRenderer>().color = new Color(0.68f, 0.42f, 0.4f, 1f);

            audioSource.PlayOneShot(audioClip);
        }

        if (health == 0)
        {
            // Update player score
            int score = GameManager.Instance.isBullMarket ? 25 : 125;
            GameManager.Instance.UpdateScore(score * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.elonNFTCurrentLevel - 1]);

            // Disable "isElon" effect for the player
            if (playerController != null)
            {
                playerController.isElon = false;
            }

            // Play effects and destroy the object
            audioSource.PlayOneShot(audioClip);
            Destroy(transform.GetChild(0).gameObject);
            GetComponent<SpriteRenderer>().enabled = false;
            boxCollider2D.enabled = false;
            particleEffectGO.SetActive(true);

            // Increment NFT count
            GameManager.Instance.web3Manager.elonNFTCount++;

            Destroy(gameObject, 1f);
        }
    }
}
